using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAMBHS.Server.WebClientAdmin.UI.Common
{
    public partial class FRM014 : System.Web.UI.Page
    {
        SystemParameterBL objSystemParameterBL = new SystemParameterBL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OperationResult objOperationResult = new OperationResult();
                Utils.Web.LoadDropDownList(ddlEmpresaOrigen, "Value1", "Id", objSystemParameterBL.GetNode(ref objOperationResult, ""), DropDownListAction.Select);
                Utils.Web.LoadDropDownList(ddlEmpresaDestino, "Value1", "Id", objSystemParameterBL.GetNode(ref objOperationResult, ""), DropDownListAction.Select);
            }
        }

        protected void btnMigrar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            var ListadoEmpresas = objSystemParameterBL.GetNode(ref objOperationResult, "");
            bool ClienteProveedores, Conceptops, ConfigEmpresa, Destinos, PlanCuentas;
            string RUCOrigen = ListadoEmpresas.Where(p => p.Id == ddlEmpresaOrigen.SelectedValue.ToString()).FirstOrDefault().Value3;
            string RUCDestino = ListadoEmpresas.Where(p => p.Id == ddlEmpresaDestino.SelectedValue.ToString()).FirstOrDefault().Value3;

            #region Realiza la copia de información
            ClienteProveedores = chkClientesProveedores.Checked == true ? true : false;
            Conceptops = chkConceptos.Checked == true ? true : false;
            ConfigEmpresa = chkConfigEmpresa.Checked == true ? true : false;
            Destinos = chkDestinos.Checked == true ? true : false;
            PlanCuentas = chkPlanCuentas.Checked == true ? true : false;

            //new DbConfigBL().MigracionDeTablas(ref objOperationResult, ConfigEmpresa, PlanCuentas, ClienteProveedores, Destinos, Conceptops, RUCOrigen, RUCDestino);
            #endregion
        }
    }
}