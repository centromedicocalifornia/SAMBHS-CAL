using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMBHS.Security.BL;
using SAMBHS.Common.Resource;
using FineUI;
using System.Web.Security;

namespace SAMBHS.Server.WebClientAdmin.UI
{
    public partial class frmLogin : System.Web.UI.Page
    {
        SecurityBL _objSecurityBL = new SecurityBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Autenticación de usuario
            OperationResult objOperationResult1 = new OperationResult();
            var objSystemUser = _objSecurityBL.ValidateSystemUser(ref objOperationResult1,
                                                                    1,
                                                                    txtUserName.Text.Trim(),
                                                                    Utils.Encrypt(txtPassword.Text.Trim()));

            if (objSystemUser != null)
            {
                ClientSession clientSession = new ClientSession();
                clientSession.i_SystemUserId = objSystemUser.i_SystemUserId;
                clientSession.v_UserName = objSystemUser.v_UserName;
                clientSession.i_CurrentExecutionNodeId = 1;
                clientSession.i_CurrentOrganizationId = 0;
                clientSession.i_PersonId = objSystemUser.i_PersonId;
                clientSession.i_RoleId = (int)objSystemUser.i_RoleId;

                Session["objClientSession"] = clientSession;

                FormsAuthentication.RedirectFromLoginPage(objSystemUser.v_UserName, true);

            }
            else
            {
                Alert.ShowInTop(objOperationResult1.AdditionalInformation);
            }
        }
    }
}