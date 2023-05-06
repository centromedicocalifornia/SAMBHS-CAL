<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM009.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM009" %>
<%@ Register assembly="FineUI" namespace="FineUI" tagprefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
      <form id="form1" runat="server">      
    <x:Pagemanager ID="PageManager1" runat="server" AutoSizePanelID="Panel1"/>
     <x:Panel ID="Panel1" runat="server"  ShowBorder="True" ShowHeader="True" Title="Administración de Almacén"  EnableBackgroundColor="true" Layout="VBox" 
            BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="3 7 12 5" >
        <Items>
            <x:GroupPanel runat="server" Title="Búsqueda / Filtro" ID="GroupPanel1" AutoWidth="true" BoxFlex="1" Height="70" >                
                <Items>
                     <x:SimpleForm  ID="frmFiltro" ShowBorder="False" EnableBackgroundColor="True" ShowHeader="False" runat="server">
                        <Items>
                            <x:Panel ID="Panel2" ShowHeader="false" CssClass="x-form-item" ShowBorder="false" EnableBackgroundColor="true" Layout="Column" runat="server">
                                <Items>  
                                    <x:Label ID="Label4" Width="110px" runat="server" CssClass="inline" ShowLabel="false" Text="Razón Social:" /> 
                                    <x:TextBox ID="txtName" Width="205px" Label="Razón Social" runat="server"  CssClass="mrightmayus"></x:TextBox>                                   
                                    <x:Button ID="btnFilter" Text="Filtrar" Icon="Find" IconAlign="Left" runat="server" AjaxLoadingType="Mask" CssClass="inline" OnClick="btnFilter_Click" ></x:Button>                                       
                                </Items>
                            </x:Panel>
                        </Items>
                    </x:SimpleForm>
                </Items>
            </x:GroupPanel>           
            
            <x:Grid ID="grdData" ShowBorder="true" ShowHeader="false" runat="server" 
            PageSize="15" EnableRowNumber="True" AllowPaging="true" OnPageIndexChange="grdData_PageIndexChange"
            IsDatabasePaging="true" EnableRowNumberPaging="true" AutoHeight="true" RowNumberWidth="40" AjaxLoadingType="Default"
            EnableMouseOverColor="true" ShowGridHeader="true"   DataKeyNames="i_WarehouseId" 
            EnableTextSelection="true" EnableAlternateRowColor="true" BoxFlex="2" BoxMargin="5" OnRowCommand="grdData_RowCommand" OnPreRowDataBound="grdData_PreRowDataBound">
                <Toolbars>
                    <x:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <x:Button ID="btnNew" Text="Nuevo Almacén" Icon="Add" runat="server">
                            </x:Button>
                        </Items>
                    </x:Toolbar>
                </Toolbars>
                <Columns>    
                    <x:WindowField ColumnID="myWindowField" Width="25px" WindowID="winEdit" HeaderText=""
                        Icon="Pencil" ToolTip="Editar Almacén" DataTextFormatString="{0}" 
                        DataIFrameUrlFields="i_WarehouseId" DataIFrameUrlFormatString="FRM009A.aspx?Mode=Edit&i_WarehouseId={0}" 
                        DataWindowTitleField="v_Name" DataWindowTitleFormatString="Editar Almacén {0}" />
                    <x:LinkButtonField TextAlign="Center" ConfirmText="Está seguro de eliminar el item seleccionado?" Icon="Delete" ConfirmTarget="Top"
                        ColumnID="lbfAction2" Width="25px" ToolTip="Eliminar Almacén" CommandName="DeleteAction" />
                    <x:boundfield Width="250px" DataField="v_Name" DataFormatString="{0}" HeaderText="Nombre" />
                    <x:boundfield Width="250px" DataField="v_Address" DataFormatString="{0}" HeaderText="Dirección" />
                    <x:boundfield Width="100px" DataField="v_PhoneNumber" DataFormatString="{0}" HeaderText="Teléfono" />
                    <x:boundfield Width="100px" DataField="v_InsertUser" DataFormatString="{0}" HeaderText="Usuario Crea." />
                    <x:boundfield Width="100px" DataField="d_InsertDate" DataFormatString="{0}" HeaderText="Fecha Crea" />
                    <x:boundfield Width="100px" DataField="v_UpdateUser" DataFormatString="{0}" HeaderText="Usuario Act." />
                    <x:boundfield Width="100px" DataField="d_UpdateDate" DataFormatString="{0}" HeaderText="Fecha Act." />
                </Columns>
            </x:Grid>
        </Items>
    </x:Panel>
    
    <x:HiddenField ID="hfRefresh" runat="server" />

    <x:Window ID="winEdit" Title="Nuevo Almacén" Popup="false" EnableIFrame="true" runat="server" Icon="UserBrown"
        CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="false" 
        Target="Top" OnClose="winEdit_Close" IsModal="True" Width="400px" Height="250px" >
    </x:Window>
    </form>
</body>
</html>
