<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM008.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM008" %>
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
     <x:Panel ID="Panel1" runat="server"  ShowBorder="True" ShowHeader="True" Title="Administración de Empresa Propietaria"  EnableBackgroundColor="true" Layout="VBox" 
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
                                    <x:Label ID="Label3" Width="52px" runat="server" CssClass="inline" ShowLabel="false" Text="Ruc:" />   
                                    <x:NumberBox ID="txtIdentificationNumber" Width="132px" Label="Ruc" Required="false" CssClass="mrightmayus" runat="server" TabIndex="3" MaxLength="20" NoDecimal="true" NoNegative="true"/>                               
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
            EnableMouseOverColor="true" ShowGridHeader="true"   DataKeyNames="i_OrganizationId" 
            EnableTextSelection="true" EnableAlternateRowColor="true" BoxFlex="2" BoxMargin="5" OnRowCommand="grdData_RowCommand" OnPreRowDataBound="grdData_PreRowDataBound">
                <Toolbars>
                    <x:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <x:Button ID="btnNew" Text="Nueva Empresa Propietaria" Icon="Add" runat="server">
                            </x:Button>
                        </Items>
                    </x:Toolbar>
                </Toolbars>
                <Columns>    
                    <x:WindowField ColumnID="myWindowField" Width="25px" WindowID="winEdit" HeaderText=""
                        Icon="Pencil" ToolTip="Editar Empresa Propietaria" DataTextFormatString="{0}" 
                        DataIFrameUrlFields="i_OrganizationId" DataIFrameUrlFormatString="FRM008A.aspx?Mode=Edit&i_OrganizationId={0}" 
                        DataWindowTitleField="v_Name" DataWindowTitleFormatString="Editar Organización {0}" />
                    
                    <x:LinkButtonField TextAlign="Center" ConfirmText="Está seguro de eliminar el item seleccionado?" Icon="Delete" ConfirmTarget="Top"
                        ColumnID="lbfAction2" Width="25px" ToolTip="Eliminar Empresa Propietaria" CommandName="DeleteAction" />
                    <x:boundfield Width="30px" DataField="i_OrganizationId" DataFormatString="{0}" HeaderText="Id" />
                    <x:boundfield Width="250px" DataField="v_Name" DataFormatString="{0}" HeaderText="Razón social" />
                    <x:boundfield Width="100px" DataField="v_IdentificationNumber" DataFormatString="{0}" HeaderText="Ruc" />
                    <x:boundfield Width="200px" DataField="v_Address" DataFormatString="{0}" HeaderText="Dirección" />
                    <x:boundfield Width="100px" DataField="v_InsertUser" DataFormatString="{0}" HeaderText="Usuario Crea." />
                    <x:boundfield Width="100px" DataField="d_InsertDate" DataFormatString="{0}" HeaderText="Fecha Crea" />
                    <x:boundfield Width="100px" DataField="v_UpdateUser" DataFormatString="{0}" HeaderText="Usuario Act." />
                    <x:boundfield Width="100px" DataField="d_UpdateDate" DataFormatString="{0}" HeaderText="Fecha Act." />
                </Columns>
            </x:Grid>
        </Items>
    </x:Panel>
    
    <x:HiddenField ID="hfRefresh" runat="server" />

    <x:Window ID="winEdit" Title="Nuevo Empresa Propietaria" Popup="false" EnableIFrame="true" runat="server" Icon="UserBrown"
        CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="false" 
        Target="Top" OnClose="winEdit_Close" IsModal="True" Width="450px" Height="280px" >
    </x:Window>
    </form>
</body>
</html>
