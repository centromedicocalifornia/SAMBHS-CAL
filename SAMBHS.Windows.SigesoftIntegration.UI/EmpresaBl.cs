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
using SAMBHS.Common.DataModel;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class EmpresaBl
    {
        public string AddOrganization(EmpresaDto pobjDtoEntity)
        {
            try
            {
                var secuentialId = UtilsSigesoft.GetNextSecuentialId(5).SecuentialId;
                var newId = UtilsSigesoft.GetNewId(9, secuentialId, "OO");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "INSERT INTO [dbo].[organization]([v_OrganizationId],[i_OrganizationTypeId],v_IdentificationNumber,v_Name,v_Address,v_PhoneNumber,v_Mail,[i_SectorTypeId],[i_IsDeleted],i_InsertUserId,d_InsertDate)" +
                                "VALUES ('" + newId + "', " + pobjDtoEntity.i_OrganizationTypeId + ", '" + pobjDtoEntity.v_IdentificationNumber + "','" + pobjDtoEntity.v_Name + "','" + pobjDtoEntity.v_Address + "','" + pobjDtoEntity.v_PhoneNumber + "','" + pobjDtoEntity.v_Mail + "'," + pobjDtoEntity.i_SectorTypeId + ", 0, 11,GETDATE())";
                    cnx.Execute(query);

                    return newId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddOrdenReportes(List<OrdenReportes> pobjDtoEntity)
        {
            try
            {
                var secuentialId = UtilsSigesoft.GetNextSecuentialId(210).SecuentialId;
                var newId = UtilsSigesoft.GetNewId(9, secuentialId, "OZ");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    foreach (var item in pobjDtoEntity)
                    {
                        var query = "INSERT INTO [dbo].[ordenreporte](v_OrdenReporteId,v_OrganizationId,v_NombreReporte,v_ComponenteId,i_Orden,v_NombreCrystal,i_NombreCrystalId,[i_IsDeleted],i_InsertUserId,d_InsertDate)" +
                               "VALUES ('" + newId + "', '" + item.v_OrganizationId + "', '" + item.v_NombreReporte + "','" + item.v_ComponenteId + "'," + item.i_Orden + ",'" + item.v_NombreCrystal + "'," + item.i_NombreCrystalId + ", 0, 11,GETDATE())";
                        cnx.Execute(query);
                    }
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string AddLocation(LocationDto pobjDtoEntity)
        {
            try
             {
                var secuentialId = UtilsSigesoft.GetNextSecuentialId(14).SecuentialId;
                var newId = UtilsSigesoft.GetNewId(9, secuentialId, "OL");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "INSERT INTO [dbo].[location]([v_LocationId],[v_OrganizationId],v_Name,[i_IsDeleted],i_InsertUserId,d_InsertDate)" +
                                "VALUES ('" + newId + "', '" + pobjDtoEntity.v_OrganizationId + "', '" + pobjDtoEntity.v_Name + "', 0, 11,GETDATE())";
                    cnx.Execute(query);

                    return newId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddNodeOrganizationLoactionWarehouse(NodeOrganizationLoactionWarehouseList pobjNodeOrgLocationWarehouse,
            List<nodeorganizationlocationwarehouseprofileDto> pobjWarehouseList)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "INSERT INTO [dbo].[nodeorganizationprofile](i_NodeId,v_OrganizationId,[i_IsDeleted],i_InsertUserId,d_InsertDate)" +
                                "VALUES (" + pobjNodeOrgLocationWarehouse.i_NodeId + ", '" + pobjNodeOrgLocationWarehouse.v_OrganizationId + "', 0, 11,GETDATE())";
                    cnx.Execute(query);

                    var query1 = "INSERT INTO [dbo].[nodeorganizationlocationprofile](i_NodeId,v_OrganizationId,v_LocationId,[i_IsDeleted],i_InsertUserId,d_InsertDate)" +
                               "VALUES (" + pobjNodeOrgLocationWarehouse.i_NodeId + ", '" + pobjNodeOrgLocationWarehouse.v_OrganizationId + "','" + pobjNodeOrgLocationWarehouse.v_LocationId + "', 0, 11,GETDATE())";
                    cnx.Execute(query1);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddGroupOccupation( groupoccupationDto pobjDtoEntity)
        {
            try
            {
                var secuentialId = UtilsSigesoft.GetNextSecuentialId(13).SecuentialId;
                var newId = UtilsSigesoft.GetNewId(9, secuentialId, "OG");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "INSERT INTO [dbo].[groupoccupation]([v_GroupOccupationId],[v_LocationId],v_Name,[i_IsDeleted],i_InsertUserId,d_InsertDate)" +
                                "VALUES ('" + newId + "', '" + pobjDtoEntity.v_LocationId + "', '" + pobjDtoEntity.v_Name + "',0, 11,GETDATE())";
                    cnx.Execute(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<OrdenReportes> GetOrdenReportes(string pstrEmpresaPlantillaId)
        { 

            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = @"SELECT v_OrdenReporteId,v_ComponenteId,v_NombreReporte,i_Orden,v_NombreCrystal,i_NombreCrystalId " +
                            "FROM ordenreporte a " +
                            "WHERE ('" + pstrEmpresaPlantillaId + "'  = a.v_OrganizationId ) ";

                var data = cnx.Query<OrdenReportes>(query).ToList();
                return data;
            }
        }

        public List<KeyValueDTO> GetJoinOrganizationAndLocation(ComboBox cbo, int pintNodeId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {

                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<KeyValueDTO> query = (from a in dbContext.getjoinorganizationandlocation_sp(pintNodeId)
                                           select new KeyValueDTO
                                           {
                                               Id = a.Id,
                                               Value1 = a.Value1
                                           }).ToList();

                query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Value1";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 1;


                //if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                
                ////var query = @"SELECT b.v_OrganizationId + '|' +  b.v_LocationId as Id , d.v_Name + '/ '+ 'sede: ' +  d.v_Name as Value1, e.v_Name " +
                ////            "FROM node a " +
                ////            "INNER JOIN nodeorganizationlocationprofile B on  a.i_NodeId =  " + pintNodeId +
                ////            " INNER JOIN nodeorganizationprofile C  on b.i_NodeId =  c.i_NodeId  and b.v_OrganizationId = c.v_OrganizationId" +
                ////            " INNER JOIN organization D on  c.v_OrganizationId = d.v_OrganizationId " +
                ////            " INNER JOIN location E on  b.v_LocationId = E.v_LocationId " +
                ////            " WHERE (" + pintNodeId + "  = a.i_NodeId )  and a.i_IsDeleted = 0 and  b.i_IsDeleted = 0";

                //var query = "EXEC [GetJoinOrganizationAndLocation_SP] " + pintNodeId;
                
                //var data = cnx.Query<KeyValueDTO>(query).ToList();
                //data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                //cbo.DataSource = data;
                //cbo.DisplayMember = "Value1";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 1;
            }
        }

        public List<KeyValueDTO> GetOrganizationFacturacion(ComboBox cbo, int pintNodeId)
        {

            //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            //{
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<KeyValueDTO> query = (from a in dbContext.getorganizationfacturacion_sp(pintNodeId)
                                      select new KeyValueDTO
                                      {
                                          Id = a.Id,
                                          Value1 = a.Value1
                                      }).ToList();

                query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Value1";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 0;

                //if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                ////var query = @"SELECT b.v_OrganizationId as Id , d.v_Name as Value1 " +
                ////            "FROM node a " +
                ////            "INNER JOIN nodeorganizationlocationprofile B on  a.i_NodeId =  " + pintNodeId +
                ////            " INNER JOIN nodeorganizationprofile C  on b.i_NodeId =  c.i_NodeId  and b.v_OrganizationId = c.v_OrganizationId" +
                ////            " INNER JOIN organization D on  c.v_OrganizationId = d.v_OrganizationId " +
                ////            " WHERE (" + pintNodeId + "  = a.i_NodeId )  and a.i_IsDeleted = 0 and  b.i_IsDeleted = 0";

                //var query = "EXEC [GetOrganizationFacturacion_SP] " + pintNodeId;

                //var data = cnx.Query<KeyValueDTO>(query).ToList();
                //data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                //cbo.DataSource = data;
                //cbo.DisplayMember = "Value1";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 0;
            //}
        }

        public static string ObtenerGesoId(string pstrLocationId, string pstrGesoName)
        {
            SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

            groupoccupationDto query = (from a in dbContext.obtenergesoid_sp(pstrLocationId, pstrGesoName)
                                              select new groupoccupationDto
                                       {
                                           v_GroupOccupationId = a.v_GroupOccupationId
                                       }).FirstOrDefault();


            return query.v_GroupOccupationId;

            //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            //{
            //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

            //    var query = @"SELECT v_GroupOccupationId " +
            //                "FROM groupoccupation " +
            //                "WHERE ('" + pstrLocationId + "'  = v_LocationId  and  v_Name = '" + pstrGesoName + "') ";

            //    var data = cnx.Query<groupoccupationDto>(query).ToList().FirstOrDefault();
            //    return data.v_GroupOccupationId;
            //}
        }

        public static void GetOrganizationSeguros(ComboBox cbo, int pintNodeId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = @"SELECT b.v_OrganizationId as Id , d.v_Name as Value1 " +
                            "FROM node a " +
                            "INNER JOIN nodeorganizationlocationprofile B on  a.i_NodeId =  " + pintNodeId +
                            " INNER JOIN nodeorganizationprofile C  on b.i_NodeId =  c.i_NodeId  and b.v_OrganizationId = c.v_OrganizationId" +
                            " INNER JOIN organization D on  c.v_OrganizationId = d.v_OrganizationId " +
                            " WHERE (" + pintNodeId + "  = a.i_NodeId )  and a.i_IsDeleted = 0 and  b.i_IsDeleted = 0 and D.i_OrganizationTypeId=4";

                var data = cnx.Query<KeyValueDTO>(query).ToList();
                data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                cbo.DataSource = data;
                cbo.DisplayMember = "Value1";
                cbo.ValueMember = "Id";
                cbo.SelectedIndex = 0;
            }
        }
    }
}
