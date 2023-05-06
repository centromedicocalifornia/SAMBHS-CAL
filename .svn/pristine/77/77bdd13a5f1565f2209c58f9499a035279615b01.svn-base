<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM004.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM004" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" >
        <x:PageManager ID="PageManager1" runat="server" AutoSizePanelID="Panel1" />
        <x:Panel ID="Panel1" runat="server" ShowBorder="True" ShowHeader="True" Title="Administración de Empresas" EnableBackgroundColor="true"
            Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="3 7 12 5">
            <Items>
                <x:GroupPanel runat="server" AutoHeight="True" Title="Búsqueda / Filtro" ID="GroupPanel1" AutoWidth="true" BoxFlex="1" Height="70">
                    <Items>
                        <x:SimpleForm ID="Form5" ShowBorder="False" EnableBackgroundColor="True" ShowHeader="False" runat="server">
                            <Items>
                                <x:Panel ID="Panel133" ShowHeader="false" CssClass="x-form-item datecontainer" ShowBorder="false" EnableBackgroundColor="true"
                                    Layout="Column" runat="server">
                                    <Items>
                                        <x:Label ID="Label1" Width="50px" runat="server" CssClass="inline" ShowLabel="false" Text="Empresa:" />
                                        <x:TextBox ID="txtNodeFilter" Label="Empresa" runat="server" CssClass="mrightmayus" />
                                        <x:Button ID="btnFilter" Text="Filtrar" Icon="Find" IconAlign="Left" runat="server"
                                            AjaxLoadingType="Mask" CssClass="inline" OnClick="btnFilter_Click">
                                        </x:Button>
                                        <x:Button ID="btnMigrarTablas" Text="Migrar Tablas" Icon="DatabaseCopy" IconAlign="Left" runat="server"
                                            AjaxLoadingType="Mask" CssClass="inline">
                                        </x:Button>
                                    </Items>
                                </x:Panel>
                            </Items>
                        </x:SimpleForm>
                    </Items>
                </x:GroupPanel>
                <x:Grid ID="grdData" ShowBorder="true" ShowHeader="false" Title="Administración de Empresas" runat="server"
                    PageSize="15" EnableRowNumber="True" AllowPaging="true" OnPageIndexChange="grdData_PageIndexChange"
                    IsDatabasePaging="true" EnableRowNumberPaging="true" AutoHeight="true" RowNumberWidth="40" AjaxLoadingType="Mask"
                    EnableMouseOverColor="true" ShowGridHeader="true" BoxFlex="2" BoxMargin="5" OnRowCommand="grdData_RowCommand"
                    DataKeyNames="i_NodeId,v_RUC" EnableTextSelection="true" EnableAlternateRowColor="true" OnPreDataBound="grdData_PreDataBound" EnableAjaxLoading="true">
                    <Toolbars>
                        <x:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <x:Button ID="btnNew" Text="Nueva Empresa" Icon="Add" runat="server">
                                </x:Button>
                            </Items>
                        </x:Toolbar>
                    </Toolbars>
                    <Columns>
                        <x:WindowField ColumnID="myWindowField" Width="25px" WindowID="winEdit" HeaderText=""
                            Icon="Pencil" ToolTip="Editar Empresa" DataTextFormatString="{0}"
                            DataIFrameUrlFields="i_NodeId,v_RUC" DataIFrameUrlFormatString="FRM004A.aspx?Mode=Edit&nodeId={0}&RUC={1}"
                            DataWindowTitleField="v_RazonSocial" DataWindowTitleFormatString="Editar Empresa: {0}" />

                        <x:WindowField ColumnID="myWindowField1" Width="25px" WindowID="winEdit1" HeaderText=""
                            Icon="House" ToolTip="Asignar Almacenes a la Empresa." DataTextFormatString="{0}"
                            DataIFrameUrlFields="i_NodeId,v_RazonSocial" DataIFrameUrlFormatString="FRM004B.aspx?nodeId={0}&nodeName={1}"
                            DataWindowTitleField="v_RazonSocial" DataWindowTitleFormatString="Asignar Almacenes a la Empresa: {0}" />

                        <x:LinkButtonField TextAlign="Center" ConfirmText="¿Está seguro de Generarle una Base de Datos a esta Empresa?" Icon="DatabaseAdd" ConfirmTarget="Top"
                            ColumnID="lbfAction3" Width="25px" ToolTip="Generar Base de Datos" CommandName="DBGenerate" AjaxLoadingType="Mask"  DataTextFormatString="adsd" />

                        <x:LinkButtonField TextAlign="Center" ConfirmText="¿Está seguro de descargar una copia a la Base de Datos de esta Empresa?" Icon="DatabaseSave" ConfirmTarget="Top"
                            ColumnID="lbfAction2" Width="25px" ToolTip="Descargar una Copia de la Base de Datos" CommandName="DbBackUP" AjaxLoadingType="Mask"  />

                        <x:WindowField ColumnID="myWindowField3" Width="25px" WindowID="winRestore" HeaderText=""
                            Icon="DatabaseWrench" ToolTip="Restaurar Base de Datos de la Empresa" DataTextFormatString="{0}"
                            DataIFrameUrlFields="v_RUC" DataIFrameUrlFormatString="FRM004R.aspx?RUC={0}"
                            DataWindowTitleField="v_RazonSocial" DataWindowTitleFormatString="Restaurar Base de Datos de la Empresa: {0}" />

                        <x:LinkButtonField TextAlign="Center" ConfirmText="Está seguro de eliminar el item seleccionado?" Icon="Delete" ConfirmTarget="Top"
                            ColumnID="lbfAction4" Width="25px" ToolTip="Eliminar Empresa" CommandName="DeleteAction" />

                        <x:BoundField Width="50px" DataField="i_NodeId" DataFormatString="{0:000}" HeaderText="Nro." />
                        <x:BoundField Width="200px" DataField="v_RazonSocial" DataFormatString="{0}" HeaderText="Razón Social" />
                        <x:BoundField Width="100px" DataField="v_RUC" DataFormatString="{0}" HeaderText="RUC" />
                        <x:BoundField Width="150px" DataField="v_Direccion" DataFormatString="{0}" HeaderText="Dirección" />
                        <x:BoundField Width="100px" DataField="v_InsertUser" DataFormatString="{0}" HeaderText="Usuario Crea." />
                        <x:BoundField Width="100px" DataField="d_InsertDate" DataFormatString="{0}" HeaderText="Fecha Crea. " />
                        <x:BoundField Width="60px" DataField="v_UpdateUser" DataFormatString="{0}" HeaderText="Usuario Act." />
                        <x:BoundField Width="100px" DataField="d_UpdateDate" DataFormatString="{0}" HeaderText="Fecha Act." />
                    </Columns>
                </x:Grid>
            </Items>
        </x:Panel>
        <x:HiddenField ID="hfRefresh" runat="server" />

        <x:Window ID="winEdit" Title="Nueva Empresa" Popup="false" EnableIFrame="true" runat="server" Icon="Pencil"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="true"
            Target="Top" OnClose="winEdit_Close" IsModal="True" Width="500px" Height="203px" >
        </x:Window>

        <x:Window ID="winEdit1" Title="Nuevo Organización" Popup="false" EnableIFrame="true" runat="server" Icon="House"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="true"
            Target="Top" IsModal="True" Width="800px" Height="500px">
        </x:Window>

         <x:Window ID="winRestore" Title="Nuevo Rol" Popup="false" EnableIFrame="true" runat="server" Icon="DatabaseWrench"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="true"
            Target="Top" OnClose="winEdit_Close" IsModal="True" Width="470px" Height="165px" >
        </x:Window>  

        <x:Window ID="WinMigracion" Title="Migrar Datos de Empresa a Empresa" Popup="false" EnableIFrame="true" runat="server" Icon="DatabaseCopy"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="about:blank" EnableMaximize="false" EnableResize="true"
            Target="Top" OnClose="winEdit_Close" IsModal="True" Width="625px" Height="240px" >
        </x:Window> 
    </form>
</body></html>
