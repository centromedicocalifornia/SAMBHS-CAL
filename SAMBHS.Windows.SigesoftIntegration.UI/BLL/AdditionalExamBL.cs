using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class AdditionalExamBL
    {
        public List<AdditionalExamCustom> GetAdditionalExamByServiceId(string serviceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    List<AdditionalExamCustom> Lista = new List<AdditionalExamCustom>();
                    var query = "SELECT * FROM additionalexam addex " +
                                "WHERE addex.i_IsDeleted = 0 and addex.v_ServiceId = '" + serviceId + "' and addex.i_IsProcessed = " + 0 + "";

                    var data = cnx.Query<AdditionalExamDto>(query).ToList();
                    foreach (var obj in data)
                    {
                        AdditionalExamCustom objAdd = new AdditionalExamCustom();
                        objAdd.AdditionalExamId = obj.v_AdditionalExamId;
                        objAdd.Commentary = obj.v_Commentary;
                        objAdd.ComponentId = obj.v_ComponentId;
                        objAdd.ServiceId = obj.v_ServiceId;
                        objAdd.IsNewService = obj.i_IsNewService;
                        objAdd.IsProcessed = obj.i_IsProcessed;

                        Lista.Add(objAdd);
                    }
                    return Lista;
                }
            }
            catch (Exception e)
            {
                return null;
            }
            
        }


        public void UpdateAdditionalExamByComponentIdAndServiceId(string componentId, string serviceId, int userId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var update = "UPDATE additionalexam " +
                                 " SET i_IsNewService = 0, i_IsProcessed = 1, i_IsDeleted = 0, d_UpdateDate = '" + DateTime.Now + "' , i_UpdateUserId = " + userId + " " +
                                 " WHERE i_IsDeleted = 0 and v_ComponentId = '" + componentId + "' and v_ServiceId = '" + serviceId + "' and i_IsProcessed = " + 0 + "";

                    cnx.Execute(update);



                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
