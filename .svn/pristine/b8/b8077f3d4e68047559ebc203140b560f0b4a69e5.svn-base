using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using FineUI;
using SAMBHS.Security.BL;


namespace SAMBHS.Server.WebClientAdmin.UI.Common
{
    public partial class FRM007B : System.Web.UI.Page
    {
        SecurityBL _objSecurityBL = new SecurityBL();       
        ApplicationHierarchyBL _objApplicationHierarchyBL = new ApplicationHierarchyBL();
        List<roleprofileDto> _listRoleProfileDtoAdd = new List<roleprofileDto>();
        List<roleprofileDto> _listRoleProfileDtoDelete = new List<roleprofileDto>();
        List<roleprofileDto> _listRoleProfileDtoUpdate = new List<roleprofileDto>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["objlistRoleProfileDtoDelete"] = _listRoleProfileDtoDelete;
                Session["objlistRoleProfileDtoUpdate"] = _listRoleProfileDtoUpdate;
                LoadTreeGlobalPermissions();
                LoadData();
                btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();

            }
        }

        private void LoadData()
        {
            int RoleId = -1;
            if (Request.QueryString["RoleId"] != null)
                RoleId = int.Parse(Request.QueryString["RoleId"].ToString());

            // Verificar si el usuario tiene permisos asignados
            OperationResult objSecurityOperationResult1 = new OperationResult();
            OperationResult objSecurityOperationResult2 = new OperationResult();

            var filterExpression = string.Format("i_RoleId={0}", RoleId);
            int count1 = _objSecurityBL.GetRoleProfileCount(ref objSecurityOperationResult1, filterExpression);


            if (count1 > 0)
                hfMode.Text = "Edit";
            else
                hfMode.Text = "New";

            if (hfMode.Text == "New")
            {
                // Additional logic here.

            }
            else if (hfMode.Text == "Edit")
            {
                // Get the Entity Data
                OperationResult objOperationResult = new OperationResult();
                //var objEntity = _objSecurityBL.GetSystemUser(ref objOperationResult, systemUserId);

                // Cargar permisos Globales
                OperationResult objCommonOperationResultGlobal = new OperationResult();
                var objGlobalAuthorization = _objSecurityBL.GetRoleProfiles(ref objCommonOperationResultGlobal, RoleId);

                //// Marcar (CkeckBox) los permisos en el Tree Contextual
                foreach (var item in objGlobalAuthorization)
                {
                    SearchNode(tvGlobalPermissions.Nodes, item.I_ApplicationHierarchyId, true);
                }

            }
        }

        private void InsertGlobal(FineUI.TreeNode treeNode, int pRoleId)
        {
            if (treeNode.Checked == true)
            {
                var applicationHierarchyId = Convert.ToInt32(treeNode.NodeID);
                var optionText = treeNode.Text;

                roleprofileDto _roleProfileDTO = new roleprofileDto();
                _roleProfileDTO.i_ApplicationHierarchyId = applicationHierarchyId;
                _roleProfileDTO.i_RoleId = pRoleId;
                _roleProfileDTO.i_InsertUserId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;
                _roleProfileDTO.d_InsertDate = DateTime.Now;

                // Cargar lista de permisos a grabar
                _listRoleProfileDtoAdd.Add(_roleProfileDTO);
            }

            foreach (FineUI.TreeNode tn in treeNode.Nodes)
            {
                InsertGlobal(tn, pRoleId);
            }
        }
        
        private void AccessUpdateGlobal(int pRoleId, FineUI.TreeNodeCollection pNodes)
        {
            foreach (FineUI.TreeNode n in pNodes)
            {
                UpdateGlobal(n, pRoleId);
            }
        }

        private void AccessDeleteGlobal(int pRoleId, FineUI.TreeNodeCollection pNodes)
        {
            foreach (FineUI.TreeNode n in pNodes)
            {
                DeleteGlobal(n, pRoleId);
            }
        }

        private void UpdateGlobal(FineUI.TreeNode treeNode, int pRoleId)
        {
            if (treeNode.Checked == true)
            {
                if (treeNode.CommandName != "1")
                {
                    var _applicationHierarchyId = Convert.ToInt32(treeNode.NodeID);
                    var _optionText = treeNode.Text;

                    roleprofileDto _roleProfileDTO = new roleprofileDto();

                    _roleProfileDTO.i_ApplicationHierarchyId = _applicationHierarchyId;
                    _roleProfileDTO.i_RoleId = pRoleId;
                    _roleProfileDTO.i_InsertUserId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;
                    _roleProfileDTO.d_InsertDate = DateTime.Now;

                    // Cargar lista de permisos a actualizar
                    _listRoleProfileDtoUpdate.Add(_roleProfileDTO);
                }
            }

            foreach (FineUI.TreeNode tn in treeNode.Nodes)
            {
                UpdateGlobal(tn, pRoleId);
            }

            Session["objlistRoleProfileDtoUpdate"] = _listRoleProfileDtoUpdate;
        }

        private void DeleteGlobal(FineUI.TreeNode treeNode, int pRoleId)
        {
            if (!treeNode.Checked)
            {
                if (treeNode.CommandName == "1")
                {
                    _listRoleProfileDtoDelete.Add(new roleprofileDto
                    {
                        i_ApplicationHierarchyId = Convert.ToInt32(treeNode.NodeID),
                        i_RoleId = pRoleId,
                    });
                }
            }

            foreach (FineUI.TreeNode tn in treeNode.Nodes)
            {
                DeleteGlobal(tn, pRoleId);
            }

            Session["objlistRoleProfileDtoDelete"] = _listRoleProfileDtoDelete;
        }
        
        private void AccessInsertGlobal(int pSystemUserId, FineUI.TreeNodeCollection pNodes)
        {
            foreach (FineUI.TreeNode n in pNodes)
            {
                InsertGlobal(n, pSystemUserId);
            }

            //Grabar
            OperationResult objOperationResult = new OperationResult();
            _objSecurityBL.AddRoleProfiles(ref objOperationResult,
                                                        _listRoleProfileDtoAdd,
                                                        pSystemUserId,
                                                        ((ClientSession)Session["objClientSession"]).GetAsList(),
                                                        true);

        }

        protected void btnSaveRefresh_Click(object sender, EventArgs e)
        {
            string Mode = Request.QueryString["Mode"].ToString();
            OperationResult objOperationResult1 = new OperationResult();
            //int nodeId = -1;
            //string personId = string.Empty;
            int roleId = -1;

            if (Request.QueryString["roleId"] != null)
                roleId = int.Parse(Request.QueryString["roleId"].ToString());

            if (hfMode.Text == "New")
            {
                // Graba permisos globales
                AccessInsertGlobal(roleId, tvGlobalPermissions.Nodes);

            }
            else if (hfMode.Text == "Edit")
            {
                // Actualiza Permisos globales
                AccessUpdateGlobal(roleId, tvGlobalPermissions.Nodes);
                // Elimina Permisos globales
                AccessDeleteGlobal(roleId, tvGlobalPermissions.Nodes);

                var objlistRoleProfileDtoUpdate = Session["objlistRoleProfileDtoUpdate"] as List<roleprofileDto>;
                var objlistRoleProfileDtoDelete = Session["objlistRoleProfileDtoDelete"] as List<roleprofileDto>;

                OperationResult objOperationResult = new OperationResult();
                _objSecurityBL.UpdateRoleProfiles(ref objOperationResult,
                                                                objlistRoleProfileDtoUpdate,
                                                                objlistRoleProfileDtoDelete,
                                                                ((ClientSession)Session["objClientSession"]).GetAsList());

            }

            // Cerrar página actual y hacer postback en el padre para actualizar
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());

            //Operación con error
            //Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objCommonOperationResult.ExceptionMessage);
            // Se queda en el formulario.
        }

        private void LoadTreeGlobalPermissions()
        {
            OperationResult objOperationResult = new OperationResult();
            var listApplicationHierarchy = _objApplicationHierarchyBL.GetApplicationHierarchyByScopeId(ref objOperationResult, (int)Scope.Global);

            FineUI.TreeNode _nodePrimary = null;

            foreach (var item in listApplicationHierarchy)
            {
                switch (item.i_ParentId.ToString())
                {
                    #region Add Main Nodes

                    case "-1": // 1. Add Main nodes:
                        _nodePrimary = new FineUI.TreeNode();
                        _nodePrimary.AutoPostBack = true;
                        _nodePrimary.EnableCheckBox = true;
                        _nodePrimary.Text = item.v_Description;
                        _nodePrimary.NodeID = item.i_ApplicationHierarchyId.ToString();
                        tvGlobalPermissions.Nodes.Add(_nodePrimary);
                        break;
                    #endregion

                    default: // 2. Add Option nodes:
                        foreach (FineUI.TreeNode tnItem in tvGlobalPermissions.Nodes)
                        {
                            FineUI.TreeNode tnOption = SelectChildrenRecursive(tnItem, item.i_ParentId.ToString());

                            if (tnOption != null)
                            {
                                FineUI.TreeNode _oChildNode = new FineUI.TreeNode();

                                switch ((SAMBHS.Common.Resource.Menu)item.i_ApplicationHierarchyTypeId)
                                {
                                    case SAMBHS.Common.Resource.Menu.AccionPantalla:
                                        _oChildNode.Icon = Icon.ApplicationOsxLightning;
                                        break;

                                    case SAMBHS.Common.Resource.Menu.AgrupadorMenu:
                                        _oChildNode.Icon = Icon.ApplicationViewTile;
                                        break;

                                    case SAMBHS.Common.Resource.Menu.Pantalla:
                                        _oChildNode.Icon = Icon.Application;
                                        break;

                                    case SAMBHS.Common.Resource.Menu.PantallaIndependiente:
                                        _oChildNode.Icon = Icon.ApplicationOsxDouble;
                                        break;
                                }

                                _oChildNode.AutoPostBack = true;
                                _oChildNode.EnableCheckBox = true;
                                _oChildNode.Text = item.v_Description;
                                _oChildNode.NodeID = item.i_ApplicationHierarchyId.ToString();
                                _oChildNode.ToolTip = item.v_Description;
                                tnOption.Nodes.Add(_oChildNode);
                                break;
                            }
                        }
                        break;
                }
            }

            //tvGlobalPermissions.ExpandAllNodes();

        }

        public static FineUI.TreeNode SelectChildrenRecursive(FineUI.TreeNode tn, string searchValue)
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

        protected void tvGlobalPermissions_NodeCheck(object sender, FineUI.TreeCheckEventArgs e)
        {
            if (e.Checked)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    tvGlobalPermissions.CheckAllNodes(e.Node.Nodes);
                }

                if (e.Node.ParentNode != null)
                {
                    if (!e.Node.ParentNode.Checked)
                    {
                        NodeParentsCheck(e.Node);
                        //Tree1.CheckAllNodes(e.Node.ParentNode.ParentNode.Nodes);
                    }

                    if (e.Node.Checked == false)
                    {
                        NodeParentsUnCheck(e.Node.ParentNode);
                    }
                }
            }
            else
            {
                tvGlobalPermissions.UncheckAllNodes(e.Node.Nodes);

                if (e.Node.ParentNode == null) return;

                if (e.Node.Checked == false)
                {
                    NodeParentsUnCheck(e.Node.ParentNode);
                }
            }

        }

        public static void NodeParentsCheck(FineUI.TreeNode pNode)
        {
            FineUI.TreeNode _node = null;
            _node = pNode.ParentNode;
            _node.Checked = pNode.Checked;
            if (_node.ParentNode != null)
            {
                NodeParentsCheck(_node);
            }
        }

        public static void NodeParentsUnCheck(FineUI.TreeNode Node)
        {
            bool _Found = false;
            foreach (FineUI.TreeNode sNode in Node.Nodes)
            {
                if (sNode.Checked)
                {
                    _Found = true;
                    break;
                }
            }
            if (_Found == false)
            {
                Node.Checked = false;
                if (Node.ParentNode != null)
                {
                    NodeParentsUnCheck(Node.ParentNode);
                }
            }
        }

        protected void winEdit_Close(object sender, WindowCloseEventArgs e)
        {

        }

        public void SearchNode(FineUI.TreeNodeCollection pNodes, int ApplicationHierarchyId, bool pStatus)
        {
            //Busca un nodo en el treeview y chekarlo
            foreach (FineUI.TreeNode sNode in pNodes)
            {
                if (sNode.NodeID.Trim() == ApplicationHierarchyId.ToString())
                {
                    sNode.Checked = pStatus;
                    sNode.CommandName = "1";
                    break;
                }
                SearchNode(sNode.Nodes, ApplicationHierarchyId, pStatus);
            }
        }


    }
}