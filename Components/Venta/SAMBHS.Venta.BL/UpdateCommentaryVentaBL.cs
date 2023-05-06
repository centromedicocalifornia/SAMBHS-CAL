using SAMBHS.Windows.SigesoftIntegration.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
namespace SAMBHS.Venta.BL
{
    public class VentaCustom
    {
        public string v_CommentaryUpdate { get; set; }
    }
    public class UpdateCommentaryVentaBL
    {
        
        public string GetCommentaryUpdateVentaId(string ventaId)
        {
            using (var cnx = ConnectionHelper.GetNewContasolConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                var query = "select v_CommentaryUpdate from venta where v_IdVenta = '" + ventaId + "'";

                var comentario = cnx.Query<VentaCustom>(query).FirstOrDefault();
                if (comentario.v_CommentaryUpdate == null) return "";

                return comentario.v_CommentaryUpdate;
            }
        }

        public void UpdateCommentaryVenta(string ventaId, string commentary)
        {
            using (var cnx = ConnectionHelper.GetNewContasolConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                var query = "update venta set v_CommentaryUpdate = '" + commentary + "' where v_IdVenta = '" + ventaId + "'";
                cnx.Execute(query);
            }
        }
    }
}
