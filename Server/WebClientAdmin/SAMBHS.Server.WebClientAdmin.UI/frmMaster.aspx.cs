using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAMBHS.Server.WebClientAdmin.UI
{
    public partial class frmMaster : System.Web.UI.Page
    {
        SecurityBL _objSecurityBL = new SecurityBL();
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            //treeMenu.DataSource = XmlDataSource1;
            //treeMenu.DataBind();
        }

        private void LoadData()
        {
            #region Variables Session

            var objClientSession = (ClientSession)Session["objClientSession"];

            // Actualizar variable de sesión
            Session["objClientSession"] = objClientSession;

            int systemUserId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;
            int currentExecutionNodeId = ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId;
            string systemUserName = ((ClientSession)Session["objClientSession"]).v_UserName;
            int? personId = ((ClientSession)Session["objClientSession"]).i_PersonId;
            int RoleId = ((ClientSession)Session["objClientSession"]).i_RoleId;

            #endregion

            lblDescripcion.Text = string.Format("Bienvenido(a): {0} / ", systemUserName);
            LinkButton1.OnClientClick = winEdit.GetShowReference(string.Format("Security/FRM006A.aspx?Mode=Edit&systemUserId={0}&personId={1}", systemUserId, personId));

            LoadTreeMenu(currentExecutionNodeId, RoleId, systemUserName);
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect(FormsAuthentication.LoginUrl, false);
            Response.End();
        }

        private void LoadTreeMenu(int pintNodeId, int pintRoleId, string pstrUserName)
        {
            // Cargar permisos contextuales / Globales
            OperationResult objOperationResult2 = new OperationResult();
            var objAuthorizationList = _objSecurityBL.GetAuthorizationWeb(ref objOperationResult2, pintRoleId);

            treeMenu.Nodes.Clear();

            FineUI.TreeNode nodePrimary = null;

            foreach (var item in objAuthorizationList)
            {
                switch (item.I_ParentId.ToString())
                {
                    #region Add Main Nodes

                    case "9": // 1. Add Main nodes:
                        nodePrimary = new FineUI.TreeNode();
                        nodePrimary.Text = item.V_Description;
                        nodePrimary.NodeID = item.I_ApplicationHierarchyId.ToString();
                        nodePrimary.NavigateUrl = item.V_Form;
                        treeMenu.Nodes.Add(nodePrimary);
                        break;
                    #endregion

                    default: // 2. Add Option nodes:
                        foreach (FineUI.TreeNode tnItem in treeMenu.Nodes)
                        {
                            FineUI.TreeNode tnOption = SelectChildrenRecursive(tnItem, item.I_ParentId.ToString());

                            if (tnOption != null)
                            {
                                FineUI.TreeNode childNode = new FineUI.TreeNode();
                                childNode.Text = item.V_Description;
                                childNode.NodeID = item.I_ApplicationHierarchyId.ToString();
                                childNode.ToolTip = item.V_Description;
                                childNode.NavigateUrl = item.V_Form;
                                childNode.Target = "main";
                                tnOption.Nodes.Add(childNode);
                                break;
                            }
                        }
                        break;
                }
            }

            treeMenu.ExpandAllNodes();

        }
        
        private FineUI.TreeNode SelectChildrenRecursive(FineUI.TreeNode tn, string searchValue)
        {
            if (tn.NodeID == searchValue)
            {
                return tn;
            }
            else
            {
                //tn.Collapse();
            }
            if (tn.Nodes.Count > 0)
            {
                FineUI.TreeNode objNode = new FineUI.TreeNode();

                foreach (FineUI.TreeNode tnC in tn.Nodes)
                {
                    objNode = SelectChildrenRecursive(tnC, searchValue);
                    if (objNode != null) return objNode;
                }
            }
            return null;
        }
    }
}