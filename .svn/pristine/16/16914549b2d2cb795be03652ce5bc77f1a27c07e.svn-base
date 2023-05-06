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


namespace SAMBHS.Server.WebClientAdmin.UI.Configuration
{
    public partial class FRM012 : System.Web.UI.Page
    {
        //ConfigurationBL _objConfigurationBL = new ConfigurationBL();
     
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        LoadData();
              
        //    }
        //}

        //private void LoadData()
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    DataHierarchyBL objDataHierarchyBL = new DataHierarchyBL();
        //    SystemParameterBL objSystemParameterBL = new SystemParameterBL();
        //        //Llenado de combos
        //        Utils.Web.LoadDropDownList(ddlOrganizationId, "Value1", "Id", OrganizationBL.GetAllOrganization(ref objOperationResult, ""), DropDownListAction.Select);
        //        Utils.Web.LoadDropDownList(ddlIgvId, "Value1", "Id", objDataHierarchyBL.GetDataHierarchyForComboKeyValueDto(ref objOperationResult, 100, ""), DropDownListAction.Select);
        //        Utils.Web.LoadDropDownList(ddlCurrencyId, "Value1", "Id", objDataHierarchyBL.GetDataHierarchyForComboKeyValueDto(ref objOperationResult, 102, ""), DropDownListAction.Select);
        //        Utils.Web.LoadDropDownList(ddlIsAffectedIgvId, "Value1", "Id", SystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 111, ""), DropDownListAction.Select);
        //        Utils.Web.LoadDropDownList(ddlIncludeIGV, "Value1", "Id", SystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 111, ""), DropDownListAction.Select);
        
        //}

        //protected void ddlNode_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    organizationconfigurationDto objorganizationconfigurationDTO = _objConfigurationBL.GetOrganizationConfigurationId(ref objOperationResult, ddlOrganizationId.SelectedValue.ToString());

        //    if (objorganizationconfigurationDTO != null)
        //    {
        //        ViewState["Mode"] = "Edit";
        //        Session["sobjOrganizationDTO"] = objorganizationconfigurationDTO;

        //        // Informacion del Organization
        //        ddlOrganizationId.SelectedValue = objorganizationconfigurationDTO.i_OrganizationId;
        //        ddlIgvId.SelectedValue = objorganizationconfigurationDTO.i_IgvId.ToString();
        //        ddlCurrencyId.SelectedValue = objorganizationconfigurationDTO.i_CurrencyTypeId.ToString();
        //        txtMaxDecimal.Text = objorganizationconfigurationDTO.i_NumberDecimals.ToString();
        //        ddlIsAffectedIgvId.SelectedValue = objorganizationconfigurationDTO.i_IsAffectedIgvId.ToString();
        //        ddlIncludeIGV.SelectedValue = objorganizationconfigurationDTO.i_IncludesIgvId.ToString();
        //    }
        //    else
        //    {
        //        ViewState["Mode"] = "New";
        //        ddlIgvId.SelectedValue = "-1";
        //        ddlCurrencyId.SelectedValue = "-1";
        //        txtMaxDecimal.Text ="";
        //        ddlIsAffectedIgvId.SelectedValue = "-1";
        //        ddlIncludeIGV.SelectedValue = "-1";
        //    }
        
        //}

        //protected void btnSaveRefresh_Click(object sender, EventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    ConfigurationBL objConfigurationBL = new ConfigurationBL();
        //    if (ViewState["Mode"].ToString() == "New")
        //    {
            

        //        // Datos de Organization
        //        organizationconfigurationDto objorganizationconfigurationDto = new organizationconfigurationDto();
        //        objorganizationconfigurationDto.i_OrganizationId = ddlOrganizationId.SelectedValue;
        //        objorganizationconfigurationDto.i_IgvId = int.Parse(ddlIgvId.SelectedValue.ToString());
        //        objorganizationconfigurationDto.i_CurrencyTypeId = int.Parse(ddlCurrencyId.SelectedValue.ToString());
        //        objorganizationconfigurationDto.i_NumberDecimals = int.Parse(txtMaxDecimal.Text.ToString());
        //        objorganizationconfigurationDto.i_IsAffectedIgvId = int.Parse(ddlIsAffectedIgvId.SelectedValue.ToString());
        //        objorganizationconfigurationDto.i_IncludesIgvId = int.Parse(ddlIncludeIGV.SelectedValue.ToString());
        //        // Graba Organization
        //        objConfigurationBL.AddOrganizationConfiguration(ref objOperationResult, objorganizationconfigurationDto, ((ClientSession)Session["objClientSession"]).GetAsList());

        //        if (objOperationResult.Success != 1)
        //        {
        //            Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
        //        }

        //    }
        //    else if (ViewState["Mode"].ToString() == "Edit")
        //    {
        //        organizationconfigurationDto objorganizationconfigurationDto = Session["sobjOrganizationDTO"] as organizationconfigurationDto;

            
        //        // Datos de Organization
        //        objorganizationconfigurationDto.i_OrganizationId = ddlOrganizationId.SelectedValue;
        //        objorganizationconfigurationDto.i_IgvId = int.Parse(ddlIgvId.SelectedValue.ToString());
        //        objorganizationconfigurationDto.i_CurrencyTypeId = int.Parse(ddlCurrencyId.SelectedValue.ToString());
        //        objorganizationconfigurationDto.i_NumberDecimals = int.Parse(txtMaxDecimal.Text.ToString());
        //        objorganizationconfigurationDto.i_IsAffectedIgvId = int.Parse(ddlIsAffectedIgvId.SelectedValue.ToString());
        //        objorganizationconfigurationDto.i_IncludesIgvId = int.Parse(ddlIncludeIGV.SelectedValue.ToString());
             
        //        // Actualiza Organization
        //        OperationResult objOperationResult1 = new OperationResult();
        //        objConfigurationBL.UpdateOrganizationConfiguration(ref objOperationResult1, objorganizationconfigurationDto, ((ClientSession)Session["objClientSession"]).GetAsList());

        //        if (objOperationResult1.Success != 1)
        //        {
        //            Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
        //        }

        //    }

        //    Alert.Show("Se grabo correctamenre");
        //}

    }
}