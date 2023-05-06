<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frmMaster.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.frmMaster" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>SISTEMA TISN - MÓDULO DE SEGURIDAD</title>
    <style>
        .header .title {
            background-image: url(../images/logo/logo2.gif);
            background-position: left 2px;
            background-repeat: no-repeat;
            padding-left: 0px;
            margin-top: 5px;
            margin-left: 5px;
        }

            .header .title a {
                color:white;
                font-weight: bold;
                font-size: 25px;
                text-decoration: none;
                font-family:Arial;
            }

        .header .version {
            position: absolute;
            right: 200px;
            top: 8px;
        }

            .header .version a {
                color: #fff;
                font-weight: bold;
                font-size: 18px;
            }

        .intro {
            font-family: arial,tahoma,helvetica,sans-serif;
            font-size: 13px;
            line-height: 1.5em;
        }

        .htoolbar {
            background-image: url("../images/logo/logo2.gif");
            background-position: center center;
            background-repeat: no-repeat;
        }

        #logo {
            position: absolute;
            bottom: 0px;
            right: 0px;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            opacity: 0.8;
            z-index: 100000;
        }
        .White
        {
            font-weight: bold;
            color: white;
        }
       
    </style>
      <script language="javascript" type="text/javascript">
          javascript: window.history.forward(1);
    </script> 
</head>
<body>
    <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" AutoSizePanelID="RegionPanel1" runat="server"></x:PageManager>
        <x:RegionPanel ID="RegionPanel1" ShowBorder="false" runat="server">
            <Regions>

                <x:Region ID="Region1" ShowHeader="false" EnableIFrame="true" IFrameUrl="#"
                    IFrameName="main" Margins="0 0 0 0" Position="Center" runat="server">
                </x:Region>


                <x:Region ID="Region2" Split="true" Width="200px" Margins="0 0 0 0" ShowHeader="true" Title="Opciones" EnableCollapse="true"
                    Layout="Fit" Position="Left" runat="server" EnableSplitTip="true" EnableLargeHeader="true" Icon="Outline">
                    <Items>
                        <x:Tree runat="server" EnableArrows="true" ShowBorder="false" ShowHeader="false" EnableCollapse="false" Expanded="true"
                            AutoScroll="true" ID="treeMenu">
                        </x:Tree>
                    </Items>
                </x:Region>

                <x:Region ID="Region3" Margins="0 0 0 0" Height="60px" ShowBorder="false" ShowHeader="false"
                    Position="Top" Layout="Fit" runat="server">
                    <Items>
                        <x:ContentPanel ShowBorder="false" CssClass="header" ShowHeader="false" BodyStyle="background-color:#000000"
                            ID="ContentPanel1" runat="server">
                            <table>
                                <tr>
                                    <td class="title">
                                        <x:HyperLink ID="HyperLink3" NavigateUrl="~/frmMaster.aspx" runat="server" Text="TISN - MÓDULO DE SEGURIDAD">
                                        </x:HyperLink>
                                    </td>
                                                             
                                </tr>
                                <tr>
                                    <td style="color: white; position: absolute; right: 157px; top: 30px;">
                                        <x:Label ID="lblDescripcion" runat="server" Text="Bienvenido:"></x:Label>
                                    </td>
                                    <td style="position: absolute; right: 93px; top: 30px;">
                                        <asp:LinkButton ID="LinkButton1" CssClass="White" runat="server">(Mi cuenta)</asp:LinkButton>
                                    </td>
                                    <td style="position: absolute; right: 17px; top: 30px;">
                                        <asp:LinkButton ID="btnClose" CssClass="White" runat="server" OnClick="btnClose_Click">Cerrar sesión</asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </x:ContentPanel>
                    </Items>
                </x:Region>

            </Regions>
        </x:RegionPanel>

        <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/Menu.xml"></asp:XmlDataSource>

        <x:Window ID="winEdit" Title="Mi cuenta" Popup="false" EnableIFrame="true" runat="server" Icon="UserBrown"
            CloseAction="HidePostBack" EnableConfirmOnClose="true" IFrameUrl="#" EnableMaximize="false" EnableResize="true"
            Target="Top" IsModal="True" Width="750px" Height="570px">
        </x:Window>

    </form>
</body>
</html>
