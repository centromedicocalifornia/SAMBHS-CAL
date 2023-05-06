using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
//using SAMBHS.Windows.WinClient.UI.Sigesoft;
using Sigesoft.Node.WinClient.UI;
using System.Configuration;
using NetPdf;
using System.IO;

using SAMBHS.Windows.SigesoftIntegration.UI;
//using SAMBHS.Windows.WinClient.UI.Sigesoft;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using NetPdf;
using System.Configuration;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Common.BE.Custom;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmBandejaAgenda : Form
    {
        public frmBandejaAgenda(string value)
        {
            InitializeComponent();
        }

        private string _strServicelId;
        private string _calendarId;
        private DateTime _fechaNacimiento;
        List<string> ListaComponentes = new List<string>();
        int _RowIndexgrdDataCalendar;
        private string _personId;
        private string _nroDoc;
        private string _serviceId;
        private void frmBandejaAgenda_Load(object sender, EventArgs e)
        {
            UltraGridColumn c = grdDataCalendar.DisplayLayout.Bands[0].Columns["b_Seleccionar"];
            c.CellActivation = Activation.AllowEdit;
            c.CellClickAction = CellClickAction.Edit;

            AgendaBl.LlenarComboGetServiceType(ddlServiceTypeId, 119);
            ddlLineStatusId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlLineStatusId, 120);
            ddlLineStatusId.SelectedIndex = 0;
            ddlMasterServiceId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlMasterServiceId, -1);
            ddlMasterServiceId.SelectedIndex = 0;

            ddlNewContinuationId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlNewContinuationId, 121);
            ddlVipId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlVipId, 111);
            ddlVipId.SelectedIndex = 0;
            ddlCalendarStatusId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlCalendarStatusId, 122);
            ddlCalendarStatusId.SelectedIndex = 0;

            ddlServiceTypeId.SelectedValue = "1";
            ddlMasterServiceId.SelectedValue = "2";
            btnHistoriaClinica.Enabled = false;
            btnFilter_Click(sender, e);
        }
        
        private void ddlServiceTypeId_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(ddlServiceTypeId.SelectedValue.ToString(), out id)) return;
            id = int.Parse(ddlServiceTypeId.SelectedValue.ToString());
            AgendaBl.LlenarComboServicios(ddlMasterServiceId, id);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {

            if (ddlServiceTypeId.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show("Por favor seleccionar Tipo de Servicio", "Validación!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //if (ddlMasterServiceId.SelectedValue.ToString() == "-1")
            //{
            //    MessageBox.Show("Por favor seleccionar Servicio", "Validación!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            var oFiltros = new FiltroAgenda
            {
                FechaInicio = dtpDateTimeStar.Value,
                //TipoServicio = int.Parse(ddlServiceTypeId.SelectedValue.ToString()),
                Cola = int.Parse(ddlLineStatusId.SelectedValue.ToString()),
                Paciente = txtPacient.Text,
                FechaFin = dptDateTimeEnd.Value,
                Servicio = int.Parse(ddlMasterServiceId.SelectedValue.ToString()),
                Vip = int.Parse(ddlVipId.SelectedValue.ToString()),
                NroDocumento = txtNroDocument.Text,
                Modalidad = int.Parse(ddlNewContinuationId.SelectedValue.ToString()),
                EstadoCita = int.Parse(ddlCalendarStatusId.SelectedValue.ToString())
                
            };
            var objData = AgendaBl.ObtenerListaAgendados(oFiltros);
            grdDataCalendar.DataSource = objData;

           lblRecordCountCalendar.Text = string.Format("Se encontraron {0} registros.", objData.Count());



        }



        private void grdDataCalendar_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            btnImprimirHojaRuta.Enabled = (ugComponentes.Rows.Count > 0);
            btnEditarTrabajador.Enabled = (ugComponentes.Rows.Count > 0);
            btnCambiarProtocolo.Enabled = (ugComponentes.Rows.Count > 0);
            if (grdDataCalendar.Selected.Rows.Count != 0)
            {
                txtTrabajador.Text = grdDataCalendar.Selected.Rows[0].Cells["v_Pacient"].Value.ToString();
                if (grdDataCalendar.Selected.Rows[0].Cells["v_WorkingOrganizationName"].Value != null)
                    WorkingOrganization.Text = grdDataCalendar.Selected.Rows[0].Cells["v_WorkingOrganizationName"].Value.ToString();
                txtProtocol.Text = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolName"].Value == null ? "" : grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolName"].Value.ToString();
                txtService.Text = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceName"].Value.ToString();
                if (grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value != null)
                {
                    txtTypeESO.Text = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value.ToString() == ConstantsSigesoft.CONSULTAMEDICA ? "" : grdDataCalendar.Selected.Rows[0].Cells["v_EsoTypeName"].Value.ToString();
                }

                //Obtener Foto, huella y firma
                _personId = grdDataCalendar.Selected.Rows[0].Cells["v_PersonId"].Value.ToString();
                var oPerson = AgendaBl.ObtenerImagenesTrabajador(_personId);
                //var fotoTrabajador = grdDataCalendar.Selected.Rows[0].Cells["FotoTrabajador"];
                if (oPerson.FotoTrabajador != null)
                {
                    var foto = oPerson.FotoTrabajador;

                    pbImage.Image = UtilsSigesoft.BytesArrayToImageOficce(foto, pbImage);
                }
                else
                {
                    pbImage.Image = null;
                }

                // Huella y Firma
                //var huellaTrabajador = grdDataCalendar.Selected.Rows[0].Cells["HuellaTrabajador"].Value;
                if (oPerson.HuellaTrabajador == null)
                {
                    txtExisteHuella.Text = "NO REGISTRADO";
                    txtExisteHuella.ForeColor = Color.Red;
                }
                else
                {
                    txtExisteHuella.Text = "REGISTRADO";
                    txtExisteHuella.ForeColor = Color.DarkBlue;
                }

                // Firma
                //var firmaTrabajador = grdDataCalendar.Selected.Rows[0].Cells["FirmaTrabajador"].Value;
                if (oPerson.FirmaTrabajador == null)
                {
                    txtExisteFirma.Text = "NO REGISTRADO";
                    txtExisteFirma.ForeColor = Color.Red;
                }
                else
                {
                    txtExisteFirma.Text = "REGISTRADO";
                    txtExisteFirma.ForeColor = Color.DarkBlue;
                }

                var ListServiceComponent = new List<Categoria>();
                _strServicelId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
                _calendarId = grdDataCalendar.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();
                _fechaNacimiento = (DateTime)grdDataCalendar.Selected.Rows[0].Cells["d_Birthdate"].Value;

                _masterServiceId = int.Parse(grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString());

                _nroDoc = grdDataCalendar.Selected.Rows[0].Cells["v_NumberDocument"].Value.ToString();
                _serviceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
                _protocolId = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value.ToString();
                _dni = grdDataCalendar.Selected.Rows[0].Cells["v_DocNumber"].Value.ToString();

                _medicoTratanteId = -1;// grdDataCalendar.Selected.Rows[0].Cells["i_MedicoTratanteId"].Value == null ? -1 : int.Parse(grdDataCalendar.Selected.Rows[0].Cells["i_MedicoTratanteId"].Value.ToString());
                ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
                ugComponentes.DataSource = ListServiceComponent;

                var ListServiceComponentAMC = AgendaBl.GetServiceComponents_(_strServicelId);

                ListaComponentes = new List<string>();
                foreach (var item in ListServiceComponentAMC)
                {
                    ListaComponentes.Add(item.v_ComponentId);
                }
            }
        }

        private void grdDataCalendar_MouseDown(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            var uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null)return;

            var row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (e.Button != MouseButtons.Right && e.Button != MouseButtons.Left) return;
            if (row == null) return;
            _RowIndexgrdDataCalendar = row.Index;
            grdDataCalendar.Rows[row.Index].Selected = true;
            txtTrabajador.Text = grdDataCalendar.Selected.Rows[0].Cells["v_Pacient"].Value.ToString();
            if (grdDataCalendar.Selected.Rows[0].Cells["v_WorkingOrganizationName"].Value != null)
                WorkingOrganization.Text = grdDataCalendar.Selected.Rows[0].Cells["v_WorkingOrganizationName"].Value.ToString();
            txtProtocol.Text = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolName"].Value == null ? "" : grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolName"].Value.ToString();
            txtService.Text = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceName"].Value.ToString();
            if (grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value != null)
            {
                txtTypeESO.Text = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value.ToString() == ConstantsSigesoft.CONSULTAMEDICA ? "" : grdDataCalendar.Selected.Rows[0].Cells["v_EsoTypeName"].Value.ToString();
            }

            //Obtener Foto, huella y firma
            _personId = grdDataCalendar.Selected.Rows[0].Cells["v_PersonId"].Value.ToString();
            var oPerson = AgendaBl.ObtenerImagenesTrabajador(_personId);
            //var fotoTrabajador = grdDataCalendar.Selected.Rows[0].Cells["FotoTrabajador"];
            if (oPerson.FotoTrabajador != null)
            {
                var foto = oPerson.FotoTrabajador;

                pbImage.Image = UtilsSigesoft.BytesArrayToImageOficce(foto, pbImage);
            }
            else
            {
                pbImage.Image = null;
            }

            // Huella y Firma
            //var huellaTrabajador = grdDataCalendar.Selected.Rows[0].Cells["HuellaTrabajador"].Value;
            if (oPerson.HuellaTrabajador == null)
            {
                txtExisteHuella.Text = "NO REGISTRADO";
                txtExisteHuella.ForeColor = Color.Red;
            }
            else
            {
                txtExisteHuella.Text = "REGISTRADO";
                txtExisteHuella.ForeColor = Color.DarkBlue;
            }

            // Firma
            //var firmaTrabajador = grdDataCalendar.Selected.Rows[0].Cells["FirmaTrabajador"].Value;
            if (oPerson.FirmaTrabajador == null)
            {
                txtExisteFirma.Text = "NO REGISTRADO";
                txtExisteFirma.ForeColor = Color.Red;
            }
            else
            {
                txtExisteFirma.Text = "REGISTRADO";
                txtExisteFirma.ForeColor = Color.DarkBlue;
            }

            var ListServiceComponent = new List<Categoria>();
            _strServicelId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
            _calendarId = grdDataCalendar.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();
            _fechaNacimiento = (DateTime)grdDataCalendar.Selected.Rows[0].Cells["d_Birthdate"].Value;

           
            _nroDoc = grdDataCalendar.Selected.Rows[0].Cells["v_NumberDocument"].Value.ToString();
            ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
            ugComponentes.DataSource = ListServiceComponent;

            var ListServiceComponentAMC = AgendaBl.GetServiceComponents_(_strServicelId);

            ListaComponentes = new List<string>();
            foreach (var item in ListServiceComponentAMC)
            {
                ListaComponentes.Add(item.v_ComponentId);
            }



        }

        private void btnAgregarExamen_Click(object sender, EventArgs e)
        {
            //amc
            var mode = "";
            var masterServiceId = grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
            if (masterServiceId == "10" ||  masterServiceId == "13" || masterServiceId == "14" || masterServiceId == "15" || masterServiceId == "16" ||
                masterServiceId == "17" || masterServiceId == "18" || masterServiceId == "19" || masterServiceId == "20")
            {
                mode = "HOSPI";
            }
            else if (masterServiceId == "12" || masterServiceId == "13")
            {
                mode = "ASEG";
            }
            else
            {
                mode = "EMPRE";
            }
            var frm = new frmAddExam(ListaComponentes, mode, _protocolId, "", _strServicelId, null, "", "", _dni);

            //var frm = new frmAddExam(ListaComponentes,mode,_protocolId, "", "", null) {_serviceId = _strServicelId};
            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.Cancel)
                return;

            var ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
            ugComponentes.DataSource = ListServiceComponent;
        }

        private void btnRemoverEsamen_Click(object sender, EventArgs e)
        {
            if (ugComponentes.Selected.Rows.Count == 0)
                return;

            DialogResult Result = MessageBox.Show("¿Está seguro de eliminar este registro?", "ADVERTENCIA!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (Result == DialogResult.OK)
            {
                var _auxiliaryExams = new List<ServiceComponentList>();
                OperationResult objOperationResult = new OperationResult();

                string v_ServiceComponentId =
                    ugComponentes.Selected.Rows[0].Cells["v_serviceComponentId"].Value.ToString();
                ServiceComponentList auxiliaryExam = new ServiceComponentList();
                auxiliaryExam.v_ServiceComponentId = v_ServiceComponentId;
                _auxiliaryExams.Add(auxiliaryExam);

                AgendaBl.UpdateAdditionalExam(_auxiliaryExams, _strServicelId, (int?) SiNo.NO);
                var ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
                ugComponentes.DataSource = ListServiceComponent;


            }

        }

        private void ugComponentes_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null)
                return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row != null)
            {
                contextMenuStrip2.Items["btnRemoverEsamen"].Enabled = true;
            }
            else
            {
                contextMenuStrip2.Items["btnRemoverEsamen"].Enabled = false;
            }
        }

        private void btnImprimirHojaRuta_Click(object sender, EventArgs e)
        {
            var frm = new Sigesoft.Node.WinClient.UI.Reports.frmRoadMap(_strServicelId, _calendarId,_fechaNacimiento);
            frm.ShowDialog();
        }

        private void btnPerson_Click(object sender, EventArgs e)
        {
            //var frmAgendarTrabajador = new frmAgendarTrabajador("CONTINUE", "","");
            //if (frmAgendarTrabajador.ShowDialog() == DialogResult.OK) btnFilter_Click(sender, e); 
            
            var frmPre = new frmPreCarga(1);
            frmPre.ShowDialog();
        }

        private void grdDataCalendar_ClickCell(object sender, ClickCellEventArgs e)
        {
            if ((e.Cell.Column.Key == "b_Seleccionar"))
            {
                if ((e.Cell.Value.ToString() == "False"))
                {
                    e.Cell.Value = true;

                    //btnFechaEntrega.Enabled = true;
                    //btnAdjuntarArchivo.Enabled = true;
                }
                else
                {
                    e.Cell.Value = false;
                    //btnFechaEntrega.Enabled = false;
                    //btnAdjuntarArchivo.Enabled = false;
                }

            }
        }
        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        private string _protocolId;
        private string _dni;

        private int _masterServiceId;
        private int? _medicoTratanteId;

        private void btnFacturar_Click(object sender, EventArgs e)
        {
            //var _ListaCalendar = new List<string,float>();
            var result = new BindingList<ventadetalleDto>();
           
            foreach (var item in grdDataCalendar.Rows)
            {
                
                if ((bool)item.Cells["b_Seleccionar"].Value)
                {
                    var protocolo = item.Cells["v_ProtocolName"].Value.ToString();
                    var precioTotal = float.Parse(item.Cells["PrecioTotalProtocolo"].Value.ToString());
                    var serviceId = item.Cells["v_ServiceId"].Value.ToString();
                    var empFact = item.Cells["v_OrganizationId"].Value.ToString();
                    var rucEmpFact = item.Cells["RucEmpFact"].Value.ToString();
                    var masterServiceId = grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
                    if (masterServiceId == "12" || masterServiceId == "13")
                    {
                        var listSaldo =  FacturacionServiciosBl.SaldoPacienteAseguradora(serviceId);
                        if (listSaldo != null)
                        {
                            foreach (var itemSaldo in listSaldo)
                            {
                                var cant = 1;
                                var pu = decimal.Parse(itemSaldo.d_SaldoPaciente.ToString());
                                var valorV = Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
                                var igv = Math.Round(pu * 0.18m, 2, MidpointRounding.AwayFromZero);
                                var oventadetalleDto = new ventadetalleDto
                                {
                                    i_Anticipio = 0,
                                    i_IdAlmacen = 1,
                                    i_IdCentroCosto = "0",
                                    i_IdUnidadMedida = 15,
                                    ProductoNombre = itemSaldo.v_Name,
                                    v_DescripcionProducto = itemSaldo.v_Name,
                                    v_IdProductoDetalle = "N001-PE000015780",
                                    v_NroCuenta = string.Empty,
                                    d_PrecioVenta = decimal.Parse(precioTotal.ToString()),
                                    d_Igv = igv,
                                    d_Cantidad = cant,
                                    d_CantidadEmpaque = cant,
                                    d_Precio = pu,
                                    d_Valor = pu,
                                    d_ValorVenta = pu,
                                    d_PrecioImpresion = pu,
                                    v_CodigoInterno = "ATMD01",
                                    Empaque = 1,
                                    UMEmpaque = "UND",
                                    i_EsServicio = 1,
                                    i_IdUnidadMedidaProducto = 15,
                                    v_ServiceId = serviceId,
                                    EmpresaFacturacion = empFact,
                                    RucEmpFacturacion = rucEmpFact,
                                };
                                result.Add(oventadetalleDto);
                            }
                        }
                        else
                        {
                            MessageBox.Show("El paciente no tiene saldo por pagar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        

                    }
                    else
                    {
                        var cant = 1;
                        var pu = decimal.Parse(precioTotal.ToString());
                        var valorV = Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
                        var igv = Math.Round(pu * 0.18m, 2, MidpointRounding.AwayFromZero);
                        var oventadetalleDto = new ventadetalleDto
                        {
                            i_Anticipio = 0,
                            i_IdAlmacen = 1,
                            i_IdCentroCosto = "0",
                            i_IdUnidadMedida = 15,
                            ProductoNombre = protocolo,
                            v_DescripcionProducto = protocolo,
                            v_IdProductoDetalle = "N001-PE000015780",
                            v_NroCuenta = string.Empty,
                            d_PrecioVenta = decimal.Parse(precioTotal.ToString()),
                            d_Igv = igv,
                            d_Cantidad = cant,
                            d_CantidadEmpaque = cant,
                            d_Precio = pu,
                            d_Valor = pu,
                            d_ValorVenta = pu,
                            d_PrecioImpresion = pu,
                            v_CodigoInterno = "ATMD01",
                            Empaque = 1,
                            UMEmpaque = "UND",
                            i_EsServicio = 1,
                            i_IdUnidadMedidaProducto = 15,
                            v_ServiceId = serviceId,
                            EmpresaFacturacion = empFact,
                            RucEmpFacturacion = rucEmpFact,
                        };
                        result.Add(oventadetalleDto);
                    }
                   
                }

                //var oServiceDto = OServiceDto();
                //Task.Factory.StartNew(() =>
                //{
                    
                //}).ContinueWith(t =>
                //{
                //    if (!ListadoVentaDetalle.Any()) return;
                //    DialogResult = DialogResult.OK;
                //    Close();
                //}, TaskScheduler.FromCurrentSynchronizationContext());
            }

            ListadoVentaDetalle = result; 
            DialogResult = DialogResult.OK;
        }

        private void btnEditarTrabajador_Click(object sender, EventArgs e)
        {
            frmEditarTrabajador frm = new frmEditarTrabajador(_nroDoc, _personId, _serviceId);
            frm.ShowDialog();
        }

        private void btnCambiarProtocolo_Click(object sender, EventArgs e)
        {
            var tipoServicioId = -1;
            if (_masterServiceId == 10 || _masterServiceId == 15 || _masterServiceId == 16 || _masterServiceId == 17 || _masterServiceId == 18 || _masterServiceId == 19)
            {
                tipoServicioId = 9;
            }
            else
            {
                tipoServicioId = 2;
            }
            frmEditarProtocolo frm = new frmEditarProtocolo(_protocolId, tipoServicioId, _masterServiceId, _medicoTratanteId,_strServicelId, "");
            frm.ShowDialog();

            btnFilter_Click(sender,e);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            //var frm = new frmReport();
            //frm.ShowDialog();

             GenerateTest();
        }

        private void GenerateTest()
        {
            string ruta = GetApplicationConfigValue("rutaLiquidacion").ToString();

          var path = string.Format("{0}.pdf", Path.Combine(ruta, "Report"));
            ReportPDF.CreateTest(path);
        }

        //private void btnHistoriaClinica_Click(object sender, EventArgs e)
        //{
        //    OperationResult _objOperationResult = new OperationResult();
        //    var doc = grdDataCalendar.Selected.Rows[0].Cells["v_DocDumber"].Value.ToString();
        //    var serviceID = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
        //    var protocolId = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value.ToString();

        //    using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
        //    {
        //        this.Enabled = false;

        //        var MedicalCenter = _serviceBL.GetInfoMedicalCenter();
                
        //        var datosP = _pacientBL.DevolverDatosPaciente(hospser.v_ServiceId);

        //        //var lista = _serviceBL.GetListaLiquidacion(ref _objOperationResult, liquidacionID);

        //        var _DataService = _serviceBL.GetInfoEmpresaLiquidacion(serviceID);

        //        string ruta = GetApplicationConfigValue("rutaLiquidacion").ToString();
               
        //        string fecha = DateTime.Now.ToString().Split('/')[0] + "-" + DateTime.Now.ToString().Split('/')[1] + "-" + DateTime.Now.ToString().Split('/')[2];
        //        string nombre = "Historia Clinica N° " + doc + " - CSL";

        //        //var obtenerInformacionEmpresas = new ServiceBL().ObtenerInformacionEmpresas(serviceID);

        //        Liquidacion_EMO.CreateLiquidacion_EMO(ruta + nombre + ".pdf", MedicalCenter, datosP, _DataService);
        //        this.Enabled = true;
        //    }
        //}

        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }

        private void btnHistoriaClinica_Click(object sender, EventArgs e)
        {
            OperationResult _objOperationResult = new OperationResult();

            var serviceID = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                this.Enabled = false;

                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new ServiceBL().GetDatosTrabajador(serviceID);

                int edad = new ServiceBL().GetAge(datosP.d_Birthdate.Value);
                //var exams = new ServiceBL().GetServiceComponentsReport(serviceID);

                var medicoTratante = new ServiceBL().GetMedicoTratante(serviceID);

                this.Enabled = false;

                string ruta = GetApplicationConfigValue("rutaHistoriaClinica").ToString();

                string fecha = DateTime.Now.ToString().Split('/')[0] + "-" + DateTime.Now.ToString().Split('/')[1] + "-" + DateTime.Now.ToString().Split('/')[2];
                string nombre = "Historia Clinica N° " + serviceID + " - CSL";

                //var obtenerInformacionEmpresas = new ServiceBL().ObtenerInformacionEmpresas(serviceID);
                var añosCompleto = DiferenciaFechas(DateTime.Now, datosP.d_Birthdate.Value);

                Hisoria_Clinica.CreateHistoria_Clinica(ruta + nombre + ".pdf", MedicalCenter, datosP, medicoTratante, edad, añosCompleto);
                this.Enabled = true;
            }
        }
        private string DiferenciaFechas(DateTime newdt, DateTime olddt)
        {
            int anios;
            int meses;
            int dias;
            string str = "";

            anios = (newdt.Year - olddt.Year);
            meses = (newdt.Month - olddt.Month);
            dias = (newdt.Day - olddt.Day);

            if (meses < 0)
            {
                anios -= 1;
                meses += 12;
            }
            if (dias < 0)
            {
                meses -= 1;
                dias += DateTime.DaysInMonth(newdt.Year, newdt.Month);
            }

            if (anios < 0)
            {
                return "La fecha inicial es mayor a la fecha final";
            }
            if (anios > 0)
            {
                if (anios == 1)
                    str = str + anios.ToString() + " año ";
                else
                    str = str + anios.ToString() + " años ";
            }
            if (meses > 0)
            {
                if (meses == 1)
                    str = str + meses.ToString() + " mes y ";
                else
                    str = str + meses.ToString() + " meses y ";
            }
            if (dias > 0)
            {
                if (dias == 1)
                    str = str + dias.ToString() + " día ";
                else
                    str = str + dias.ToString() + " días ";
            }
            return str;
        }

        private void ddlServiceTypeId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlServiceTypeId.SelectedIndex == 2 || ddlServiceTypeId.SelectedIndex == 3)
            {
                btnHistoriaClinica.Enabled = true;
            }
        }

        private void btnCartaSolicitud_Click(object sender, EventArgs e)
        {
            frmAddSolicitudCarta frm = new frmAddSolicitudCarta(_serviceId);
            frm.Show();
        }

        private void verExámenesAdicionalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                var ProtocolId = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value.ToString();

                OperationResult objOperationResult = new OperationResult();
                string ServiceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                List<AdditionalExamCustom> ListAdditionalExams = new AdditionalExamBL().GetAdditionalExamByServiceId(ServiceId);

                List<string> ComponentAdditionalList = new List<string>();
                List<string> ComponentNewService = new List<string>();

                foreach (var obj in ListAdditionalExams)
                {
                    ComponentAdditionalList.Add(obj.ComponentId);
                    if (obj.IsNewService == (int)SiNo.SI)
                    {
                        ComponentNewService.Add(obj.ComponentId);
                    }
                }

                List<Categoria> DataSource = new List<Categoria>();


                foreach (var componentId in ComponentAdditionalList)
                {
                    var ListServiceComponent = AgendaBl.GetAllComponents((int)Enums.TipoBusqueda.ComponentId, componentId);



                    Categoria categoria = DataSource.Find(x => x.i_CategoryId == ListServiceComponent[0].i_CategoryId);
                    if (categoria != null)
                    {
                        List<ComponentDetailList> componentDetail = new List<ComponentDetailList>();
                        componentDetail = ListServiceComponent[0].Componentes;
                        DataSource.Find(x => x.i_CategoryId == ListServiceComponent[0].i_CategoryId).Componentes.AddRange(componentDetail);
                    }
                    else
                    {
                        DataSource.AddRange(ListServiceComponent);
                    }
                }

                //var frm = new frmAddExam(ComponentNewService, "", ProtocolId, _personId, ServiceId, DataSource);
                var frm = new frmAddExam(ComponentNewService, "", _protocolId, _personId, _strServicelId, DataSource, "", "", _dni);

                frm.ShowDialog();

            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void verExamenesAdicionalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                var ProtocolId = grdDataCalendar.Selected.Rows[0].Cells["v_ProtocolId"].Value.ToString();

                OperationResult objOperationResult = new OperationResult();
                string ServiceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                List<AdditionalExamCustom> ListAdditionalExams = new AdditionalExamBL().GetAdditionalExamByServiceId(ServiceId);

                List<string> ComponentAdditionalList = new List<string>();
                List<string> ComponentNewService = new List<string>();

                foreach (var obj in ListAdditionalExams)
                {
                    ComponentAdditionalList.Add(obj.ComponentId);
                    if (obj.IsNewService == (int)SiNo.SI)
                    {
                        ComponentNewService.Add(obj.ComponentId);
                    }
                }

                List<Categoria> DataSource = new List<Categoria>();


                foreach (var componentId in ComponentAdditionalList)
                {
                    var ListServiceComponent = AgendaBl.GetAllComponents((int)Enums.TipoBusqueda.ComponentId, componentId);



                    Categoria categoria = DataSource.Find(x => x.i_CategoryId == ListServiceComponent[0].i_CategoryId);
                    if (categoria != null)
                    {
                        List<ComponentDetailList> componentDetail = new List<ComponentDetailList>();
                        componentDetail = ListServiceComponent[0].Componentes;
                        DataSource.Find(x => x.i_CategoryId == ListServiceComponent[0].i_CategoryId).Componentes.AddRange(componentDetail);
                    }
                    else
                    {
                        DataSource.AddRange(ListServiceComponent);
                    }
                }

                var frm = new frmAddExam(ComponentNewService, "", _protocolId, _personId, _strServicelId, DataSource, "", "", _dni);

                //var frm = new frmAddExam(ComponentNewService, "", ProtocolId, _personId, ServiceId, DataSource);
                frm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ugComponentes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void btnFusionar_Click(object sender, EventArgs e)
        {
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                OperationResult objOperationResult = new OperationResult();
                List<string> Services = new List<string>();
                var personId = "";
                bool personChange = false;
                foreach (var row in grdDataCalendar.Rows)
                {
                    if ((bool)row.Cells["b_Seleccionar"].Value)
                    {
                        var strpersonId = row.Cells["v_PersonId"].Value.ToString();
                        var strServiceId = row.Cells["v_ServiceId"].Value.ToString();
                        var circuitStartDate = row.Cells["d_EntryTimeCM"].Value;
                        Services.Add(strServiceId);
                        if (personId == strpersonId || personChange == false)
                        {
                            personId = strpersonId;
                            personChange = true;
                        }
                        else
                        {
                            MessageBox.Show("Por favor, elija a una misma persona para poder fusionar", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (circuitStartDate == null)
                        {
                            MessageBox.Show("Procure que el paciente inicie el circuito.", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                if (Services.Count > 0)
                {
                    var result = new PacientBL().FusionServices(ref objOperationResult, Services,
                        Globals.ClientSession.GetAsList());
                    if (result == null)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage, "ERROR", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void grdDataCalendar_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if ((int)e.Row.Cells["i_CalendarStatusId"].Value == (int)CalendarStatus.Atendido && (int)e.Row.Cells["i_LineStatusId"].Value == (int)LineStatus.FueraCircuito)
            {
                e.Row.Appearance.BackColor = Color.White;
                e.Row.Appearance.BackColor2 = Color.Gray;
                //Y doy el efecto degradado vertical
                e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }
            else if ((int)e.Row.Cells["i_CalendarStatusId"].Value == (int)CalendarStatus.Cancelado)
            {
                e.Row.Appearance.BackColor = Color.White;
                e.Row.Appearance.BackColor2 = Color.Yellow;
                //Y doy el efecto degradado vertical
                e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }

            if (e.Row.Cells["i_StatusLiquidation"].Value == null)
                return;

            if ((int)e.Row.Cells["i_StatusLiquidation"].Value == 2) //generada
            {
                e.Row.Appearance.BackColor = Color.White;
                e.Row.Appearance.BackColor2 = Color.SkyBlue;
                //Y doy el efecto degradado vertical
                e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }
        }

        //public organizationDto GetInfoMedicalCenter()
        //{
        //    using (SAMBHS.Common.DataModel.db dbContext = new SigesoftEntitiesModel())
        //    {
        //        organizationDto objDtoEntity = null;
        //        var objEntity = (from o in dbContext.organization
        //                         where o.v_OrganizationId == "N009-OO000000052"
        //                         select o).SingleOrDefault();

        //        var other = (from o in dbContext.location
        //                     where o.v_OrganizationId == "N009-OO000000052"
        //                     select o).SingleOrDefault();
        //        objEntity.v_SectorName = other == null ? "" : other.v_Name;

        //        if (objEntity != null)
        //            objDtoEntity = organizationAssembler.ToDTO(objEntity);

        //        return objDtoEntity;
        //    }
        //}
        //private void btnHospitalizacion_Click(object sender, EventArgs e)
        //{
        //    var frmAgendarTrabajador = new frmAgendarTrabajador("HOPITALIZACION");
        //    if (frmAgendarTrabajador.ShowDialog() == DialogResult.OK) btnFilter_Click(sender, e); 
        //}

        //private void btnEmergencia_Click(object sender, EventArgs e)
        //{
        //    var frmAgendarTrabajador = new frmAgendarTrabajador("EMERGENCIA");
        //    if (frmAgendarTrabajador.ShowDialog() == DialogResult.OK) btnFilter_Click(sender, e); 
        //}
    }
}
