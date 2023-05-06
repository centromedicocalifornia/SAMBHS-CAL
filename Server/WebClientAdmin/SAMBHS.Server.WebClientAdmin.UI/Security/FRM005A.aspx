<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM005A.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Security.FRM005A" %>
<%@ Register assembly="FineUI" namespace="FineUI" tagprefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
     <form id="form1" runat="server">
     <x:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <x:Panel ID="Panel1" runat="server" Layout="Fit" ShowBorder="False" ShowHeader="false" BodyPadding="5px" EnableBackgroundColor="true">
        <Toolbars>
            <x:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <x:Button ID="btnSaveRefresh" Text="Guardar y Cerrar" runat="server" Icon="SystemSave" OnClick="btnSaveRefresh_Click" ValidateForms="SimpleForm1" TabIndex="7">
                    </x:Button>
                    <x:Button ID="btnClose" EnablePostBack="false" Text="Cancelar y Cerrar" runat="server" Icon="SystemClose" TabIndex="8">
                    </x:Button>
                </Items>
            </x:Toolbar>
        </Toolbars>
        <Items>
            
            <x:Panel ID="Panel2" Layout="Fit" runat="server" ShowBorder="false" ShowHeader="false" >
                <Items>
                    <x:SimpleForm ID="SimpleForm1" ShowBorder="false" ShowHeader="false" EnableBackgroundColor="true"
                        AutoScroll="true" BodyPadding="5px" runat="server" EnableCollapse="True">
                        <Items>
                            <x:Form ID="Form2" runat="server" EnableBackgroundColor="true" ShowBorder="False" ShowHeader="False"
                                LabelWidth="100px">
                                <Rows>
                                    <x:FormRow ID="FormRow1" runat="server">
                                        <Items>
                                            <x:DropDownList ID="ddlApplicationHierarchyTypeId" runat="server" Width="150" Label="Type" ShowRedStar="true" CompareValue="-1" CompareOperator="NotEqual" Resizable="True" TabIndex="0"></x:DropDownList>
                                            <x:TextBox ID="txtLevel" Label="Order" runat="server" TabIndex="4" ShowRedStar="true" CompareValue="" CompareOperator="NotEqual"/>
                                        </Items>
                                    </x:FormRow>
                                </Rows>
                            </x:Form>

                            <x:DropDownList ID="ddlTypeFormId" runat="server" Label="Aplicación" ShowRedStar="true" CompareValue="-1" CompareOperator="NotEqual" Resizable="True" TabIndex="1"></x:DropDownList>
                            <x:DropDownList ID="ddlScopeId" runat="server" Width="150" Label="Scope" Resizable="True" TabIndex="2"></x:DropDownList>
                            <x:TextBox ID="txtDescription" Label="Description" runat="server" Required="true" ShowRedStar="true" TabIndex="3" />
                            <x:TextBox ID="txtForm" Label="Form" runat="server" TabIndex="4" />
                            <x:TextBox ID="txtCode" Label="Code" runat="server" TabIndex="5" />
                            <x:DropDownList ID="ddlBusinessRule" runat="server" Width="390" Label="Regla Nego." Resizable="True" TabIndex="6"></x:DropDownList>
                            <x:DropDownList ID="ddlParentId" runat="server" Width="390" Label="Parent" Resizable="True" TabIndex="7" EnableSimulateTree="true" AutoPostBack="true"></x:DropDownList>

                        </Items>
                    </x:SimpleForm>
                </Items>
            </x:Panel>
        </Items>
    </x:Panel>
    </form>
</body>
</html>
