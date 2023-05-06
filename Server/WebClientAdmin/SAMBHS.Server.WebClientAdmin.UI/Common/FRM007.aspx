<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM007.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM007" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
      <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" runat="server" AutoSizePanelID="Panel1" />
        <x:Panel ID="Panel1" runat="server" ShowBorder="True" ShowHeader="True" Title="Administración de Roles" EnableBackgroundColor="true"
            Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="3 7 12 5" >
            <Items>
                <x:GroupPanel runat="server" AutoHeight="True" Title="Búsqueda / Filtro" ID="GroupPanel1" AutoWidth="true" BoxFlex="1" Height="70">
                    <Items>
                        <x:SimpleForm ID="Form5" ShowBorder="False" EnableBackgroundColor="True" ShowHeader="False" runat="server">
                            <Items>
                                <x:Panel ID="Panel133" ShowHeader="false" CssClass="x-form-item datecontainer" ShowBorder="false" EnableBackgroundColor="true"
                                    Layout="Column" runat="server">
                                    <Items>
                                        <x:Label ID="Label1" Width="50px" runat="server" CssClass="inline" ShowLabel="false" Text="Rol:" />
                                        <x:TextBox ID="txtRoleFilter" Label="Rol" runat="server" CssClass="mrightmayus" />
                                        <x:Button ID="btnFilter" Text="Filtrar" Icon="Find" IconAlign="Left" runat="server"
                                            AjaxLoadingType="Mask" CssClass="inline" OnClick="btnFilter_Click">
                                        </x:Button>
                                    </Items>
                                </x:Panel>
                            </Items>
                        </x:SimpleForm>
                    </Items>
                </x:GroupPanel>
                <x:Grid ID="grdData" ShowBorder="true" ShowHeader="false" Title="Administración de Roles" runat="server"
                    PageSize="15" EnableRowNumber="True" AllowPaging="true" OnPageIndexChange="grdData_PageIndexChange"
                    IsDatabasePaging="true" EnableRowNumberPaging="true" AutoHeight="true" RowNumberWidth="40" AjaxLoadingType="Default"
                    EnableMouseOverColor="true" ShowGridHeader="true" BoxFlex="2" BoxMargin="5" OnRowCommand="grdData_RowCommand"
                    DataKeyNames="i_RoleId" EnableTextSelection="true" EnableAlternateRowColor="true">
                    <Toolbars>
                        <x:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <x:Button ID="btnNew" Text="Nuevo Rol" Icon="Add" runat="server">
                                </x:Button>
                            </Items>
                        </x:Toolbar>
                    </Toolbars>
                    <Columns>
                        <x:WindowField ColumnID="myWindowField" Width="25px" WindowID="winEdit" HeaderText=""
                            Icon="Pencil" ToolTip="Editar Rol" DataTextFormatString="{0}"
                            DataIFrameUrlFields="i_RoleId" DataIFrameUrlFormatString="FRM007A.aspx?Mode=Edit&roleId={0}"
                            DataWindowTitleField="v_Name" DataWindowTitleFormatString="Editar Rol: {0}" />
                        <x:WindowField ColumnID="myWindowField1" Width="30px" WindowID="winEditPermission" HeaderText=""
                            Icon="LockKey" ToolTip="Asignar / Desasignar Permisos al Rol." DataTextFormatString="{0}"
                            DataIFrameUrlFields="i_RoleId" DataIFrameUrlFormatString="FRM007B.aspx?Mode=New&roleId={0}"
                            DataWindowTitleField="v_Name" DataWindowTitleFormatString="Asignar / Desasignar Permisos al Rol : {0} " />

                        <x:LinkButtonField TextAlign="Center" ConfirmText="Está seguro de eliminar el item seleccionado?" Icon="Delete"  ConfirmTarget="Top"
                            ColumnID="lbfAction2" Width="25px" ToolTip="Eliminar Rol" CommandName="DeleteAction" />
                        <x:BoundField Width="60px" DataField="i_RoleId" DataFormatString="{0}" HeaderText="Rol Id" />
                        <x:BoundField Width="200px" DataField="v_Name" DataFormatString="{0}" HeaderText="Rol" />         
                        <x:BoundField Width="100px" DataField="v_InsertUser" DataFormatString="{0}" HeaderText="Usuario Crea." />
                        <x:BoundField Width="160px" DataField="d_InsertDate" DataFormatString="{0}" HeaderText="Fecha Crea. " />
                        <x:BoundField Width="100px" DataField="v_UpdateUser" DataFormatString="{0}" HeaderText="Usuario Act." />
                        <x:BoundField Width="160px" DataField="d_UpdateDate" DataFormatString="{0}" HeaderText="Fecha Act." />
                    </Columns>
                </x:Grid>
            </Items>
        </x:Panel>
        <x:HiddenField ID="hfRefresh" runat="server" />

        <x:Window ID="winEdit" Title="Nuevo Rol" Popup="false" EnableIFrame="true" runat="server" Icon="Pencil"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="true"
            Target="Top" OnClose="winEdit_Close" IsModal="True" Width="450px" Height="150px" >
        </x:Window>      

        <x:Window ID="winEditPermission" Title="Nuevo Permiso" Popup="false" EnableIFrame="true" runat="server" Icon="LockKey"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="true"
            Target="Top" OnClose="winEditPermission_Close" IsModal="True" Width="600px" Height="550px">
        </x:Window>

    </form>
</body>
</html>
