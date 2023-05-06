<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM012.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Configuration.FRM012" %>
<%@ Register assembly="FineUI" namespace="FineUI" tagprefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" AutoSizePanelID="Panel5" runat="server" />
        <x:Panel ID="Panel5" runat="server" ShowBorder="True" BodyPadding="5px" ShowHeader="False" EnableBackgroundColor="True">
            <Toolbars>
                <x:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <x:Button ID="btnSaveRefresh" Text="Guardar" runat="server" Icon="SystemSave" OnClick="btnSaveRefresh_Click" ValidateForms="Form2,SimpleForm2" 
                            TabIndex="6">
                        </x:Button>                       
                    </Items>
                </x:Toolbar>
            </Toolbars>
            <Items>
                <x:Panel ID="Panel6" AutoHeight="true" EnableBackgroundColor="True"
                    runat="server" BodyPadding="2px" ShowBorder="False" ShowHeader="False">
                    <Items>
                        <x:GroupPanel runat="server" Title="Configuración a nivel de Empresa" ID="GroupPanel4">
                            <Items>
                                <x:Form ID="Form2" runat="server" EnableBackgroundColor="true" ShowBorder="False" ShowHeader="False"
                                    LabelWidth="160px">
                                    <Rows>
                                        <x:FormRow ID="FormRow1" runat="server">
                                            <Items>
                                                  <x:DropDownList ID="ddlOrganizationId" Label="Empresa" Required="false" runat="server" TabIndex="1"
                                                    AutoPostBack="true" OnSelectedIndexChanged="ddlNode_SelectedIndexChanged"
                                                    ShowRedStar="true" CompareType="String" CompareValue="-1" 
                                                    CompareOperator="NotEqual" CompareMessage="Por favor seleccione una Empresa!" />
                                          
                                            </Items>
                                        </x:FormRow>
                                        <x:FormRow ID="FormRow2" runat="server">
                                            <Items>
                                                 <x:DropDownList ID="ddlIgvId" Label="Igv" Required="false" runat="server" TabIndex="2"
                                                    AutoPostBack="true"
                                                    ShowRedStar="true" CompareType="String" CompareValue="-1" 
                                                    CompareOperator="NotEqual" CompareMessage="Por favor seleccione un IGV!" />                                                                                         
                                            </Items>
                                        </x:FormRow>  
                                        <x:FormRow ID="FormRow5" runat="server">
                                            <Items>
                                                   <x:DropDownList ID="ddlCurrencyId" Label="Moneda" Required="false" runat="server" TabIndex="4"
                                                    AutoPostBack="true"
                                                    ShowRedStar="true" CompareType="String" CompareValue="-1" 
                                                    CompareOperator="NotEqual" CompareMessage="Por favor seleccione una Moneda" />                                                                                             
                                            </Items>
                                        </x:FormRow>                                            
                                        <x:FormRow ID="FormRow7" runat="server">
                                            <Items>
                                                <x:NumberBox ID="txtMaxDecimal" Label="Nro. Decimales" Required="true" ShowRedStar="true" runat="server" TabIndex="5"/>
                                            </Items>
                                        </x:FormRow>       
                                        <x:FormRow ID="FormRow8" runat="server">
                                            <Items>
                                                <x:DropDownList ID="ddlIsAffectedIgvId" Label="Afecto IGV" Required="false" runat="server" TabIndex="5"
                                                   AutoPostBack="true"
                                                    ShowRedStar="true" CompareType="String" CompareValue="-1" 
                                                    CompareOperator="NotEqual" CompareMessage="Por favor seleccione una opción" />                                                                                             
                                               </Items>
                                        </x:FormRow>     
                                         <x:FormRow ID="FormRow9" runat="server">
                                            <Items>
                                                <x:DropDownList ID="ddlIncludeIGV" Label="Incluye IGV" Required="false" runat="server" TabIndex="5"
                                                   AutoPostBack="true"
                                                    ShowRedStar="true" CompareType="String" CompareValue="-1" 
                                                    CompareOperator="NotEqual" CompareMessage="Por favor seleccione una opción" />                                                                                             
                                               </Items>
                                        </x:FormRow>     
                                                                                      
                                    </Rows>
                                </x:Form>
                            </Items>
                        </x:GroupPanel>                    
                    </Items>
                </x:Panel>
            </Items>
        </x:Panel>
      
    </form>
</body>
</html>
