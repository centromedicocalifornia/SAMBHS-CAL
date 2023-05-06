using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class TicketBL
    {
        public List<PlanList> TienePlan(string protocolId, string unidadProd)
        {
            try
            {
                using(var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from [dbo].[plan] where v_ProtocoloId = '" + protocolId +
                                "' and v_IdUnidadProductiva = '" + unidadProd + "'";

                    var List = cnx.Query<PlanList>(query).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }


}
