using Infragistics.Win.UltraWinGrid;
using NetPdf;
using SAMBHS.Common.BE;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using Sigesoft.Node.WinClient.UI;
using Sigesoft.Node.WinClient.UI.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System.Data.SqlClient;
using SAMBHS.Windows.SigesoftIntegration.UI;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;

using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaAgenda : Form
    {
        private string _strServicelId;
        private string _calendarId;
        private DateTime _fechaNacimiento;
        List<string> ListaComponentes = new List<string>();
        int _RowIndexgrdDataCalendar;
        private string _personId;
        private string _nroDoc;
        private string _serviceId;
        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        private string _protocolId;
        private string _dni;
        private int _masterServiceId;
        private int? _medicoTratanteId;

        List<DigitalContactCenterDto> ListaGrilla = new List<DigitalContactCenterDto>();

        private DateTime _fecha = new DateTime();
        private DateTime _fechaAgenda = new DateTime();

        List<AgendaDto> agendaGlobal = new List<AgendaDto>();
        List<AgendaDto> agendaGlobalTemp = new List<AgendaDto>();

        int dia;
        int mes;
        int anio;

        private Task _tarea;
        private readonly System.Threading.CancellationTokenSource _cts = new System.Threading.CancellationTokenSource();


        public frmBandejaAgenda(string value)
        {
            InitializeComponent();
        }

        private void frmBandejaAgenda_Load(object sender, EventArgs e)
        {
            UltraGridColumn c = grdDataCalendar.DisplayLayout.Bands[0].Columns["b_Seleccionar"];
            c.CellActivation = Activation.AllowEdit;
            c.CellClickAction = CellClickAction.Edit;
            //ARNOLD NEW STORES
            AgendaBl.LlenarComboGetServiceType(ddlServiceTypeId, 119);
            ddlLineStatusId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlLineStatusId, 120);
            ddlLineStatusId.SelectedIndex = 0;
            ddlMasterServiceId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlMasterServiceId, -1);
            ddlMasterServiceId.SelectedIndex = 0;
            ddlNewContinuationId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlNewContinuationId, 121);
            ddlNewContinuationId.SelectedIndex = 0;
            ddlVipId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlVipId, 111);
            ddlVipId.SelectedIndex = 0;
            ddlCalendarStatusId.DataSource = new AgendaBl().LlenarComboSystemParametro(ddlCalendarStatusId, 122);
            ddlCalendarStatusId.SelectedIndex = 0;

            ddlServiceTypeId.SelectedValue = "1";
            ddlMasterServiceId.SelectedValue = "2";
            btnHistoriaClinica.Enabled = false;
            btnHistOft.Enabled = false;

            dboMedioPago.DataSource = new AgendaBl().LlenarComboSystemParametro(dboMedioPago, 98);
            dboMedioPago.SelectedIndex = 0;

            dboMedioMKT.DataSource = new AgendaBl().LlenarComboSystemParametro(dboMedioMKT, 413);
            dboMedioMKT.SelectedIndex = 0;

            cboEstadoAtencion.DataSource = new AgendaBl().LlenarComboSystemParametro(cboEstadoAtencion, 95);
            cboEstadoAtencion.SelectedIndex = 0;

            cboUserMed.DataSource = new AgendaBl().LlenarComboUsuarios(cboUserMed);
            cboUserMed.SelectedIndex = 0;

            //CALENDARIO

            AgendaBl.LlenarComboProfesion(cboEspecialidadcal);
            AgendaBl.LlenarComboUsuariosN(cboMedicoTratanteCal);
            cboMedicoTratanteCal.SelectedValue = "-1";


            cboEspecialidadcal.SelectedValue = -1;
            _fecha = DateTime.Now;
            ddlMonth.Text = Convert.ToString(_fecha.ToString("MMMM"));
            ddlAnio.Text = _fecha.Year.ToString();


            dia = DateTime.Now.Day;
            mes = DateTime.Now.Month;
            anio = DateTime.Now.Year;
            //LoadCalendarGrid(dia, mes, anio);

            //

            //dboMedioPago.DataSource = new AgendaBl().LlenarComboSystemParametro(dboMedioPago, 98);
            //dboMedioPago.SelectedIndex = 0;
            //btnFilter_Click(sender, e);

            //using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            //{
                this.BindGrid();
            //};

        }

        private void LoadCalendarGrid(int dia, int mes, int anio)
        {
            try
            {
                _fechaAgenda = DateTime.Parse(dia + "/" + mes + "/" + anio);
                if (rbAtenciones.Checked == true)
                {
                    List<AgendaDto> agenda = new List<AgendaDto>();

                    ConexionSigesoft sigesoft = new ConexionSigesoft();
                    sigesoft.opensigesoft();
                    string sql = "exec [dbo].[ListCalendarSambhs] " + anio + ", " + mes + " , " + dia;
                    SqlCommand comando = new SqlCommand(sql, sigesoft.conectarsigesoft);
                    SqlDataReader lector = comando.ExecuteReader();
                    while (lector.Read())
                    {
                        AgendaDto cc = new AgendaDto();
                        cc.v_Pacient = lector.GetValue(0).ToString();
                        cc.v_DocNumber = lector.GetValue(1).ToString();
                        cc.v_Puesto = lector.GetValue(2).ToString();
                        cc.d_DateTimeCalendar = Convert.ToDateTime(lector.GetValue(3).ToString());
                        cc.v_EsoTypeName = lector.GetValue(4).ToString();
                        cc.v_CalendarStatusName = lector.GetValue(5).ToString();
                        cc.v_CreationUser = lector.GetValue(6).ToString();
                        cc.v_ServiceId = lector.GetValue(7).ToString();
                        cc.v_CalendarId = lector.GetValue(8).ToString();
                        //cc.d_Birthdate = Convert.ToDateTime(lector.GetValue(9).ToString());
                        cc.d_BirthdateN = lector.GetValue(9).ToString();

                        cc.v_ProtocolName = lector.GetValue(10).ToString();
                        cc.PrecioTotalProtocolo = float.Parse(lector.GetValue(11).ToString());
                        cc.v_OrganizationId = lector.GetValue(12).ToString();
                        cc.i_MasterServiceId = int.Parse(lector.GetValue(13).ToString());
                        cc.i_NewContinuationId = int.Parse(lector.GetValue(14).ToString());
                        cc.i_ServiceStatusId = int.Parse(lector.GetValue(15).ToString());
                        cc.d_EntryTimeCM_N = lector.GetValue(16).ToString();
                        cc.SERVICIO = lector.GetValue(17).ToString();

                        cc.v_EmployerOrganizationId = lector.GetValue(18).ToString();
                        cc.COMPROBANTE = lector.GetValue(19).ToString();
                        cc.i_IsFac = int.Parse(lector.GetValue(20).ToString());
                        cc.i_CalendarStatusId = int.Parse(lector.GetValue(21).ToString());
                        cc.i_LineStatusId = int.Parse(lector.GetValue(22).ToString());
                        cc.PROTOCOLO = lector.GetValue(23).ToString();


                        agenda.Add(cc);
                    }

                    grdCalendario.DisplayLayout.Bands[0].Columns["v_Pacient"].Header.Caption = "Paciente";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_DocNumber"].Header.Caption = "Documento";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_Puesto"].Header.Caption = "Médico";
                    //grdCalendario.DisplayLayout.Bands[0].Columns["d_DateTimeCalendar"].Hidden = false;
                    grdCalendario.DisplayLayout.Bands[0].Columns["d_DateTimeCalendar"].Header.VisiblePosition = 4;
                    grdCalendario.DisplayLayout.Bands[0].Columns["PROTOCOLO"].Header.VisiblePosition = 5;

                    grdCalendario.DisplayLayout.Bands[0].Columns["d_DateTimeCalendar"].Header.Caption = "Fecha";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_EsoTypeName"].Header.Caption = "Tipo";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_CalendarStatusName"].Header.Caption = "Estado";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_CreationUser"].Header.Caption = "Usuario";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_ServiceId"].Header.Caption = "Servicio";
                    grdCalendario.DisplayLayout.Bands[0].Columns["ESPECIALIDAD"].Header.Caption = "Especialidad";

                    grdCalendario.DataSource = agenda;
                    agendaGlobal = agenda;
                    lblCount.Text = "Se han encontrado: " + agenda.Count + " registros.";
                }
                else if (rbHorarios.Checked == true)
                {
                    List<AgendaDto> agenda = new List<AgendaDto>();
                    ConexionSigesoft sigesoft = new ConexionSigesoft();
                    sigesoft.opensigesoft();
                    //string valCategoria = cboMarketing.SelectedValue.ToString() == "-1" ? "null" : cboMarketing.SelectedValue.ToString();
                    string valCategoria = "null";

                    string sql = "exec [dbo].[ListCalendarHorarioSambhs] " + anio + ", " + mes + " , " + dia + ", " + valCategoria;
                    SqlCommand comando = new SqlCommand(sql, sigesoft.conectarsigesoft);
                    SqlDataReader lector = comando.ExecuteReader();
                    while (lector.Read())
                    {
                        AgendaDto cc = new AgendaDto();
                        cc.v_Pacient = lector.GetValue(0).ToString();
                        cc.v_DocNumber = lector.GetValue(1).ToString();
                        cc.v_Puesto = lector.GetValue(2).ToString();
                        cc.d_DateTimeCalendar = Convert.ToDateTime(lector.GetValue(3).ToString());
                        cc.v_EsoTypeName = lector.GetValue(4).ToString();
                        cc.v_CalendarStatusName = lector.GetValue(5).ToString();
                        cc.v_CreationUser = lector.GetValue(6).ToString();
                        cc.v_ServiceId = lector.GetValue(7).ToString();
                        cc.Medico_ = int.Parse(lector.GetValue(8).ToString());
                        cc.Turno_ = int.Parse(lector.GetValue(9).ToString());
                        cc.Hora_ = int.Parse(lector.GetValue(10).ToString());
                        cc.Grupo_ = int.Parse(lector.GetValue(11).ToString());
                        cc.ESPECIALIDAD = lector.GetValue(12).ToString();

                        cc.v_CalendarId = lector.GetValue(13).ToString();
                        //cc.d_Birthdate = Convert.ToDateTime(lector.GetValue(14).ToString());
                        cc.d_BirthdateN = lector.GetValue(14).ToString();

                        cc.v_ProtocolName = lector.GetValue(15).ToString();
                        cc.PrecioTotalProtocolo = float.Parse(lector.GetValue(16).ToString());
                        cc.v_OrganizationId = lector.GetValue(17).ToString();
                        cc.i_MasterServiceId = int.Parse(lector.GetValue(18).ToString());
                        cc.i_NewContinuationId = int.Parse(lector.GetValue(19).ToString());
                        cc.i_ServiceStatusId = int.Parse(lector.GetValue(20).ToString());
                        cc.d_EntryTimeCM_N = lector.GetValue(21).ToString();
                        cc.SERVICIO = lector.GetValue(22).ToString();

                        cc.v_EmployerOrganizationId = lector.GetValue(23).ToString();
                        cc.COMPROBANTE = lector.GetValue(24).ToString();
                        cc.i_IsFac = int.Parse(lector.GetValue(25).ToString());
                        cc.i_CalendarStatusId = int.Parse(lector.GetValue(26).ToString());
                        cc.i_LineStatusId = int.Parse(lector.GetValue(27).ToString());
                        cc.PROTOCOLO = lector.GetValue(28).ToString();
                        agenda.Add(cc);
                    }

                    grdCalendario.DisplayLayout.Bands[0].Columns["v_Pacient"].Header.Caption = "Médico";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_DocNumber"].Header.Caption = "Turno";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_Puesto"].Header.Caption = "Hora";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_EsoTypeName"].Header.Caption = "Paciente";

                    //grdCalendario.DisplayLayout.Bands[0].Columns["d_DateTimeCalendar"].Hidden = true;
                    grdCalendario.DisplayLayout.Bands[0].Columns["PROTOCOLO"].Header.VisiblePosition = 5;

                    grdCalendario.DisplayLayout.Bands[0].Columns["d_DateTimeCalendar"].Header.Caption = "Fecha";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_CalendarStatusName"].Header.Caption = "Estado";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_CreationUser"].Header.Caption = "Usuario";
                    grdCalendario.DisplayLayout.Bands[0].Columns["v_ServiceId"].Header.Caption = "Servicio";
                    grdCalendario.DisplayLayout.Bands[0].Columns["ESPECIALIDAD"].Header.Caption = "Especialidad";


                    grdCalendario.DisplayLayout.Bands[0].Columns["d_DateTimeCalendar"].Header.VisiblePosition = 14;


                    grdCalendario.DataSource = agenda;
                    agendaGlobal = agenda;
                    lblCount.Text = "Se han encontrado: " + agenda.Count + " registros.";
                }

            }
            catch (Exception ex)
            {
                LoadCalendarGrid(dia, mes, anio);
            }

        }


        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (ddlServiceTypeId.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show("Por favor seleccionar Tipo de Servicio", "Validación!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var oFiltros = new FiltroAgenda();
            oFiltros.FechaInicio = dtpDateTimeStar.Value;
            oFiltros.Cola = int.Parse(ddlLineStatusId.SelectedValue.ToString());
            oFiltros.Paciente = txtPacient.Text;
            oFiltros.FechaFin = dptDateTimeEnd.Value;
            oFiltros.Servicio = int.Parse(ddlMasterServiceId.SelectedValue.ToString());
            oFiltros.Vip = int.Parse(ddlVipId.SelectedValue.ToString());
            oFiltros.NroDocumento = txtNroDocument.Text;
            oFiltros.Modalidad = int.Parse(ddlNewContinuationId.SelectedValue.ToString());
            oFiltros.EstadoCita = int.Parse(ddlCalendarStatusId.SelectedValue.ToString());
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                var objData = AgendaBl.ObtenerListaAgendados(oFiltros);
                grdDataCalendar.DataSource = objData;
                lblRecordCountCalendar.Text = string.Format("Se encontraron {0} registros.", objData.Count());
            };
            

        }


        private void grdDataCalendar_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            btnImprimirHojaRuta.Enabled = (ugComponentes.Rows.Count > 0);
            btnImprimirHojaRutaA4.Enabled = (ugComponentes.Rows.Count > 0);

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
                _medicoTratanteId = -1;
                _dni = grdDataCalendar.Selected.Rows[0].Cells["v_DocNumber"].Value.ToString();
                ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
                ugComponentes.DataSource = ListServiceComponent;

                var ListServiceComponentAMC = AgendaBl.GetServiceComponents_(_strServicelId);

                ListaComponentes = new List<string>();
                foreach (var item in ListServiceComponentAMC)
                {
                    ListaComponentes.Add(item.v_ComponentId);
                }

                //ugComponentes.DataSource = ListaComponentes;
            }
        }



        //public static string UpdateService(ServiceDto serviceDto, string serviceId)
        //{
        //    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
        //    {
        //        var query = "UPDATE service SET" +
        //            " v_OrganizationId = " + "'" + serviceDto.OrganizationId + "', v_ComentaryUpdate = '" + serviceDto.CommentaryUpdate + "' " +
        //            " WHERE v_ServiceId = '" + serviceId + "'";
        //        cnx.Execute(query);

        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = (SqlConnection)ConnectionHelper.GetNewSigesoftConnection;

        //        //SqlCommand com = new SqlCommand("UPDATE service SET b_PersonImage = @PersonImage, b_FingerPrintTemplate = @FingerPrintTemplate, b_FingerPrintImage = @FingerPrintImage, b_RubricImage = @RubricImage, t_RubricImageText = @RubricImageText  WHERE v_PersonId = '" + personId + "'", cmd.Connection);

        //        cmd.Connection.Open();
        //        //cmd.ExecuteNonQuery();
        //        return serviceId;


        //    }
        //}
        private void grdDataCalendar_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {
            if ((e.Cell.Column.Key == "b_Seleccionar"))
            {
                if ((e.Cell.Value.ToString() == "False"))
                {
                    e.Cell.Value = true;
                    e.Cell.Row.Selected = true;

                   
                    string servicio = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
                    //DataGridViewRow row = grdDataCalendar.Rows[e.RowIndex];

                    

                    var masterServiceId = grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();

                    
                    if (true)
                    {
                        if (masterServiceId == "12")
                        {
                            DateTime fecha_Atencion = DateTime.Parse(grdDataCalendar.Selected.Rows[0].Cells["d_DateTimeCalendar"].Value.ToString());
                            DateTime fecha_Nueva = fecha_Atencion.AddDays(65);

                            if (DateTime.Now >= fecha_Nueva)
                            {
                                #region Conexion SAM
                                ConexionSigesoft conectasam = new ConexionSigesoft();
                                conectasam.opensigesoft();
                                #endregion

                                var cadena1 =
                                        "select SR.v_ServiceId, SR.v_ProtocolId, SR.v_PersonId " +
                                        "from service SR " +
                                        " where SR.v_ServiceId = '" + servicio + "' and (SR.i_IsFac <> 2 or SR.v_ComprobantePago <> NULL)";

                                SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                                SqlDataReader lector = comando.ExecuteReader();

                                if (lector.HasRows == false)
                                {
                                    string paciente = grdDataCalendar.Selected.Rows[0].Cells["Nombres"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApePaterno"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApeMaterno"].Value.ToString();
                                    string comprobante = "";

                                    if (grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value != null)
                                    {
                                        comprobante = "Liquidación N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value.ToString();
                                    }
                                    else if (grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value != null)
                                    {
                                        comprobante = "Comprobantes N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value.ToString();
                                    }
                                    else
                                    {
                                        comprobante = "- - -";
                                    }
                                    MessageBox.Show("Paciente " + paciente + " ya tiene comprobante(s) de pago.\nContactese con el administrador para liberar.\nN° Servicio " + servicio + "\n" + comprobante, " ¡ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    btnFacturar.Enabled = false;
                                    return;
                                }
                            }
                            else
                            {

                                #region Conexion SAM
                                ConexionSigesoft conectasam = new ConexionSigesoft();
                                conectasam.opensigesoft();
                                #endregion

                                var cadena1 =
                                        "select SR.v_ServiceId, SR.v_ProtocolId, SR.v_PersonId " +
                                        "from service SR " +
                                        " where SR.v_ServiceId = '" + servicio + "' and (SR.i_IsFac <> 2 or SR.v_ComprobantePago <> NULL)";

                                SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                                SqlDataReader lector = comando.ExecuteReader();

                                if (lector.HasRows == false)
                                {
                                    string paciente = grdDataCalendar.Selected.Rows[0].Cells["Nombres"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApePaterno"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApeMaterno"].Value.ToString();
                                    string comprobante = "";

                                    if (grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value != null)
                                    {
                                        comprobante = "Liquidación N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value.ToString();
                                    }
                                    else if (grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value != null)
                                    {
                                        comprobante = "Comprobantes N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value.ToString();
                                    }
                                    else
                                    {
                                        comprobante = "- - -";
                                    }
                                    MessageBox.Show("Paciente " + paciente + " tiene comprobante(s) de pago.\nN° Servicio " + servicio + "\n" + comprobante, " ¡ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    btnFacturar.Enabled = true;
                                    return;
                                }

                                btnFacturar.Enabled = true;
                            }
                        }
                        else if (masterServiceId == "13")
                        {
                            DateTime fecha_Atencion = DateTime.Parse(grdDataCalendar.Selected.Rows[0].Cells["d_DateTimeCalendar"].Value.ToString());
                            DateTime fecha_Nueva = fecha_Atencion.AddDays(30);

                            if (DateTime.Now >= fecha_Nueva)
                            {
                                #region Conexion SAM
                                ConexionSigesoft conectasam = new ConexionSigesoft();
                                conectasam.opensigesoft();
                                #endregion

                                var cadena1 =
                                        "select SR.v_ServiceId, SR.v_ProtocolId, SR.v_PersonId " +
                                        "from service SR " +
                                        " where SR.v_ServiceId = '" + servicio + "' and (SR.i_IsFac <> 2 or SR.v_ComprobantePago <> NULL)";

                                SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                                SqlDataReader lector = comando.ExecuteReader();

                                if (lector.HasRows == false)
                                {
                                    string paciente = grdDataCalendar.Selected.Rows[0].Cells["Nombres"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApePaterno"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApeMaterno"].Value.ToString();
                                    string comprobante = "";

                                    if (grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value != null)
                                    {
                                        comprobante = "Liquidación N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value.ToString();
                                    }
                                    else if (grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value != null)
                                    {
                                        comprobante = "Comprobantes N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value.ToString();
                                    }
                                    else
                                    {
                                        comprobante = "- - -";
                                    }
                                    MessageBox.Show("Paciente " + paciente + " ya tiene comprobante(s) de pago.\nContactese con el administrador para liberar.\nN° Servicio " + servicio + "\n" + comprobante, " ¡ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    btnFacturar.Enabled = false;
                                    return;
                                }
                            }
                            else
                            {
                                btnFacturar.Enabled = true;
                            }
                        }
                        else
                        {
                            #region Conexion SAM
                            ConexionSigesoft conectasam = new ConexionSigesoft();
                            conectasam.opensigesoft();
                            #endregion

                            var cadena1 =
                                    "select SR.v_ServiceId, SR.v_ProtocolId, SR.v_PersonId " +
                                    "from service SR " +
                                    " where SR.v_ServiceId = '" + servicio + "' and (SR.i_IsFac <> 2 or SR.v_ComprobantePago <> NULL)";

                            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                            SqlDataReader lector = comando.ExecuteReader();

                            if (lector.HasRows == false)
                            {
                                string paciente = grdDataCalendar.Selected.Rows[0].Cells["Nombres"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApePaterno"].Value.ToString() + " " + grdDataCalendar.Selected.Rows[0].Cells["ApeMaterno"].Value.ToString();
                                string comprobante = "";

                                if (grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value != null)
                                {
                                    comprobante = "Liquidación N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_NroLiquidacion"].Value.ToString();
                                }
                                else if (grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value != null)
                                {
                                    comprobante = "Comprobante N° : " + grdDataCalendar.Selected.Rows[0].Cells["v_ComprobantePago"].Value.ToString();
                                }
                                else
                                {
                                    comprobante = "- - -";
                                }
                                MessageBox.Show("Paciente " + paciente + " ya tiene comprobante.\nContactese con el administrador para liberar.\nN° Servicio " + servicio + "\n" + comprobante, " ¡ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                btnFacturar.Enabled = false;
                                return;
                            }
                            else
                            {
                                btnFacturar.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    e.Cell.Value = false;
                }

            }
        }

        private void btnPerson_Click(object sender, EventArgs e)
        {
            
            var frmPre = new frmPreCarga(1);
            frmPre.ShowDialog();

            btnFilter_Click(sender, e);
        }

        private void btnFacturar_Click(object sender, EventArgs e)
        {

            UltraGridBand band = this.grdDataCalendar.DisplayLayout.Bands[0];
            List<string> servicios_ = new List<string>();
            //foreach (UltraGridRow row in band.GetRowEnumerator(GridRowType.DataRow))
            //{
            //    if ((bool)row.Cells["Selected"].Value)
            //    {
                    
            //    }
            //}
            //if (servicios.Count > 0)
            //{
            //    #region Conexion SAM
            //    ConexionSigesoft conectasam = new ConexionSigesoft();
            //    conectasam.opensigesoft();
            //    #endregion
            //    foreach (var item in servicios)
            //    {
            //        var cadena1 = "select * from service as p where v_ServiceId='" + item + "'";
            //        SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            //        SqlDataReader lector = comando.ExecuteReader();
            //        lector.Close();
            //    }
            //}


            var result = new BindingList<ventadetalleDto>();
            var servicios = new List<string>();

            foreach (var item in grdDataCalendar.Rows)
            {

                if ((bool)item.Cells["b_Seleccionar"].Value)
                {
                    var empFact = "";
                    var protocolo = item.Cells["v_ProtocolName"].Value.ToString();
                    float precioTotal = ObtenerPrecioServicio(_serviceId); //float.Parse(item.Cells["PrecioTotalProtocolo"].Value.ToString());
                    var serviceId = item.Cells["v_ServiceId"].Value.ToString();
                    if (item.Cells["v_OrganizationId"].Value != null)
                    {
                        empFact = item.Cells["v_OrganizationId"].Value.ToString();
                    }

                    var rucEmpFact = item.Cells["RucEmpFact"].Value.ToString();
                    var masterServiceId = grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
                    if (masterServiceId == "12" || masterServiceId == "13")
                    {
                        var listSaldo = FacturacionServiciosBl.SaldoPacienteAseguradora(serviceId);
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
                                servicios.Add(serviceId);
                            }
                        }
                        else
                        {
                            MessageBox.Show("El paciente no tiene saldo por pagar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                    }
                    else if (masterServiceId == "2" || masterServiceId == "3")
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
                        servicios.Add(serviceId);
                        //this.Close();
                    }
                    else 
                    {

                        DialogResult Result = MessageBox.Show("¿Desea venta detallada?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (Result == System.Windows.Forms.DialogResult.Yes)
                        {
                            var listSaldo = FacturacionServiciosBl.DetalleVenta(serviceId);
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
                                    servicios.Add(serviceId);
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
                            servicios.Add(serviceId);
                            //this.Close();
                        }
                    }

                    

                }
            }


            
            ListadoVentaDetalle = result;
            DialogResult = DialogResult.OK;

            frmRegistroVentaRapida frm = new frmRegistroVentaRapida("Nuevo", "", servicios);
            frm.ListadoVentaDetalle = ListadoVentaDetalle;
            frm.ShowDialog();
        }

        private float ObtenerPrecioServicio(string _serviceId)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 =
                "select ISNULL(SUM(r_Price),0) from servicecomponent " +
                "where v_ServiceId='"+_serviceId+"' and i_IsDeleted=0 and i_IsRequiredId=1";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            float total = 0;
            while (lector.Read())
            {
                total = Convert.ToSingle(lector.GetValue(0).ToString());
            }

            return total;
        }

        private void btnEditarTrabajador_Click(object sender, EventArgs e)
        {
            frmEditarTrabajador frm = new frmEditarTrabajador(_nroDoc, _personId, _serviceId);
            frm.ShowDialog();
        }

        private void btnCambiarProtocolo_Click(object sender, EventArgs e)
        {
            string rucEmpFact = "";
            
            if ((bool)grdDataCalendar.Selected.Rows[0].Cells["b_Seleccionar"].Value)
            {
                rucEmpFact = grdDataCalendar.Selected.Rows[0].Cells["RucEmpFact"].Value.ToString();
            }
            
            
            var tipoServicioId = -1;
            // aqui hay que agregar todos los tipos de master service particular o ocupacional en el else tienen q entrar los ocupcionales
            if (_masterServiceId == 10 || _masterServiceId == 15 || 
                _masterServiceId == 16 || _masterServiceId == 17 ||
                _masterServiceId == 18 || _masterServiceId == 19 || 
                _masterServiceId == 29)
            {
                tipoServicioId = 9;
            }
            else if (_masterServiceId == 12 || _masterServiceId == 13)
            {
                tipoServicioId = 11;
            }
            else
            {
                tipoServicioId = 2;
            }
            frmEditarProtocolo frm = new frmEditarProtocolo(_protocolId, tipoServicioId, _masterServiceId, _medicoTratanteId, _strServicelId, rucEmpFact);
            frm.ShowDialog();

            btnFilter_Click(sender, e);
        }

        private void btnCartaSolicitud_Click(object sender, EventArgs e)
        {
            frmAddSolicitudCarta frm = new frmAddSolicitudCarta(_serviceId);
            frm.Show();
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

                var medicoTratante = new ServiceBL().GetMedicoTratante(serviceID);

                this.Enabled = false;

                string ruta = GetApplicationConfigValue("rutaHistoriaClinica").ToString();

                string fecha = DateTime.Now.ToString().Split('/')[0] + "-" + DateTime.Now.ToString().Split('/')[1] + "-" + DateTime.Now.ToString().Split('/')[2];
                string nombre = "Historia Clinica N° " + serviceID + " - CSL";
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

        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            GenerateTest();
        }
        private void GenerateTest()
        {
            string ruta = GetApplicationConfigValue("rutaLiquidacion").ToString();

            var path = string.Format("{0}.pdf", Path.Combine(ruta, "Report"));
            ReportPDF.CreateTest(path);
        }

        private void btnImprimirHojaRuta_Click(object sender, EventArgs e)
        {
            var frm = new frmRoadMap(_strServicelId, _calendarId, _fechaNacimiento);
            frm.ShowDialog();
        }

        private void ddlServiceTypeId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlServiceTypeId.SelectedIndex == 2 || ddlServiceTypeId.SelectedIndex == 3)
            {
                btnHistoriaClinica.Enabled = true;
                btnHistOft.Enabled = true;
            }
            else
            {
                btnHistoriaClinica.Enabled = false;
                btnHistOft.Enabled = false;
            }
        }

        private void ddlServiceTypeId_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(ddlServiceTypeId.SelectedValue.ToString(), out id)) return;
            id = int.Parse(ddlServiceTypeId.SelectedValue.ToString());
            AgendaBl.LlenarComboServicios(ddlMasterServiceId, id);
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

                AgendaBl.UpdateAdditionalExam(_auxiliaryExams, _strServicelId, (int?)SiNo.NO);
                var ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
                ugComponentes.DataSource = ListServiceComponent;


            }
        }


        private void btnEditarMedicos_Click(object sender, EventArgs e)
        {
            var serviceComponentId = ugComponentes.Selected.Rows[0].Cells["v_ServiceComponentId"].Value.ToString();
        }

        private void btnAgregarExamen_Click(object sender, EventArgs e)
        {
            //amc
            var mode = "";
            var masterServiceId = grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
            if (masterServiceId == "10" || masterServiceId == "13" || masterServiceId == "14" || masterServiceId == "15" || masterServiceId == "16" ||
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
            //var frm = new frmAddExam(ListaComponentes, mode, _protocolId, "", "", null) { _serviceId = _strServicelId };

            var frm = new frmAddExam(ListaComponentes, mode, _protocolId, "", "", null, "", "", _dni) { _serviceId = _strServicelId };

            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.Cancel)
                return;

            var ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
            ugComponentes.DataSource = ListServiceComponent;
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
                if (ComponentNewService == null)
                {
                    MessageBox.Show("No se encontraron exámenes adicionales", "AVISO", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                if (ComponentNewService.Count == 0)
                {
                    MessageBox.Show("No se encontraron exámenes adicionales", "AVISO", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                var frm = new frmAddExam(ComponentNewService, "", _protocolId, _personId, ServiceId, DataSource, "", "", _dni);

                //var frm = new frmAddExam(ComponentNewService, "", ProtocolId, _personId, ServiceId, DataSource);
                frm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void grdDataCalendar_MouseDown(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            var uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

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

        private void ugComponentes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

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


        private void button1_Click(object sender, EventArgs e)
        {
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
            if (Services.Count <= 1)
            {
                MessageBox.Show("Seleccione 2 a más servicios.", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                OperationResult objOperationResult = new OperationResult();

                if (Services.Count > 1)
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

        private void verCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string commentaryService = ProtocoloBl.GetCommentaryUpdateByserviceId(_strServicelId);
            string commentaryPerson = new PacientBL().GetComentaryUpdateByPersonId(_personId);
            string comentary = "";
            if ((commentaryService == null || commentaryService == "") && (commentaryPerson == null || commentaryPerson == ""))
            {
                MessageBox.Show("Aún no se han realizado cambios.", "AVISO", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (commentaryService != null) comentary += commentaryService;
            if (commentaryPerson != null) comentary += commentaryPerson;

            var frm = new frmViewChanges(comentary);
            frm.ShowDialog();
        }

        private void grdDataCalendar_AfterCardsScroll(object sender, AfterCardsScrollEventArgs e)
        {

        }

        private void grdDataCalendar_AfterRowActivate(object sender, EventArgs e)
        {

        }

        private void grdDataCalendar_CellChange(object sender, CellEventArgs e)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(e.Cell.Column.Key, "b_Seleccionar"))
            {
                //do something special when the checkbox value is changed
            }
        }

        private void btnAsistencial_Click(object sender, EventArgs e)
        {
            var frm = new frmRoadMapAsist(_strServicelId, _calendarId, _fechaNacimiento);
            frm.ShowDialog();
        }

        private void btnHistOft_Click(object sender, EventArgs e)
        {
            OperationResult _objOperationResult = new OperationResult();

            var serviceID = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                this.Enabled = false;

                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new ServiceBL().GetDatosTrabajador(serviceID);

                int edad = new ServiceBL().GetAge(datosP.d_Birthdate.Value);

                var medicoTratante = new ServiceBL().GetMedicoTratante(serviceID);

                this.Enabled = false;

                string ruta = GetApplicationConfigValue("rutaHistoriaClinica").ToString();

                string fecha = DateTime.Now.ToString().Split('/')[0] + "-" + DateTime.Now.ToString().Split('/')[1] + "-" + DateTime.Now.ToString().Split('/')[2];
                string nombre = "OFTALMOLOGIA N° " + serviceID + " - PS";

                Historia_Oftalmologia.CreateHistoria_Oftalmologica(ruta + nombre + ".pdf", MedicalCenter, datosP, medicoTratante, edad);
                this.Enabled = true;
            }
        }

        private void frmBandejaAgenda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void txtPacient_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void dtpDateTimeStar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void dptDateTimeEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void ddlServiceTypeId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void ddlMasterServiceId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void btnEditarMedicos_Click_1(object sender, EventArgs e)
        {
            var serviceComponentId = ugComponentes.Selected.Rows[0].Cells["v_ServiceComponentId"].Value.ToString();

            var frmServiceComponent = new frmEditMedic(serviceComponentId, 1, 1);
            frmServiceComponent.ShowDialog();

        }

        private void grdDataCalendar_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if ((int)e.Row.Cells["i_AptitudeStatusId"].Value != 1)
            {
                e.Row.Appearance.BackColor = Color.GreenYellow;
                e.Row.Appearance.BackColor2 = Color.White;
                //Y doy el efecto degradado vertical
                e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }
            //else 
            //{
            //    e.Row.Appearance.BackColor = Color.Gold;
            //    e.Row.Appearance.BackColor2 = Color.White;
            //    Y doy el efecto degradado vertical
            //    e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            //}

            if ((int)e.Row.Cells["i_CalendarStatusId"].Value == (int)CalendarStatus.Cancelado)
            {
                e.Row.Appearance.BackColor = Color.Yellow;
                e.Row.Appearance.BackColor2 = Color.White;
                //Y doy el efecto degradado vertical            i_AptitudeStatusId
                e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }
            
            else if ((int)e.Row.Cells["i_LineStatusId"].Value == (int)LineStatus.FueraCircuito)
            {
                e.Row.Appearance.BackColor = Color.Gray;
                e.Row.Appearance.BackColor2 = Color.White;
                //Y doy el efecto degradado vertical
                e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            }

            
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void cancelarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("¿Está seguro de CANCELAR este registro?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                string strCalendarId = grdDataCalendar.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();
                var serviceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                var agenda = AgendaBl.CancelarCita(strCalendarId, Globals.ClientSession.i_SystemUserId, serviceId);
                if (agenda)
                {
                    btnFilter_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void regresarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var serviceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

            AgendaBl.UpdateCalendar(serviceId, 1, 0);

            btnFilter_Click(sender, e);
        }

        private void editarPrecioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int usuario = Globals.ClientSession.i_SystemUserId;
            var validacion = new AgendaBl().PermisoUsuario(usuario);
            if (validacion == "AUTORIZADO")
            {
                var _ServiceComponentId = ugComponentes.Selected.Rows[0].Cells["v_ServiceComponentId"].Value.ToString();
                var _ServiceComponentName = ugComponentes.Selected.Rows[0].Cells["v_ComponentName"].Value.ToString();

                var frmEditarPrecio = new FormPrecioComponente_Seg_(_ServiceComponentName, _ServiceComponentId);
                frmEditarPrecio.ShowDialog();
            }
            else
            {
                MessageBox.Show("NO TIENE PERMISOS PARA EDITAR PRECIOS, COMUNÍQUESE CON EL ADMINISTRADOR.", "AVISO", MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void cmChanges_Opening(object sender, CancelEventArgs e)
        {

        }

        private void comprobanteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int usuario = Globals.ClientSession.i_SystemUserId;
            var validacion = new AgendaBl().PermisoUsuario(usuario);
            if (validacion == "AUTORIZADO")
            {
                var serviceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
                var paciente = grdDataCalendar.Selected.Rows[0].Cells["v_Pacient"].Value.ToString();

                var frmPre = new frmComprobante(serviceId, paciente);
                frmPre.ShowDialog();

                btnFilter_Click(sender, e);
            }
            else
            {
                MessageBox.Show("NO TIENE PERMISOS PARA MODIFICAR PAGOS, COMUNÍQUESE CON EL ADMINISTRADOR.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnOcupacional_Click(object sender, EventArgs e)
        {
            //1 ocupacional

            var frmAgendar = new frmAgendarTrabajador("", "", "", 1, "");

            frmAgendar.ShowDialog();
            btnFilter_Click(sender, e);
        }


        private void btnParticular_Click(object sender, EventArgs e)
        {
            //2 particular
            //frmAgendaParticular frmAgendar = new frmAgendaParticular("", "", "N009-OO000000052", 2, "", "", null, "");
            frmAgendaParticular frmAgendar = new frmAgendaParticular("", "", "N009-OO000000052", 2, "", "", null, 0, "", "", "", "", "", "", DateTime.Now,"");

            //frmAgendar.StartPosition
            frmAgendar.ShowDialog();
            btnFilter_Click(sender, e);
        }

        private void btnSeguros_Click(object sender, EventArgs e)
        {
            frmPreCarga frmAgendar = new frmPreCarga(2);

            frmAgendar.ShowDialog();
            btnFilter_Click(sender, e);
        }

        private void iniciarcircuitoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime FechaAgenda = DateTime.Parse(grdDataCalendar.Selected.Rows[0].Cells["d_DateTimeCalendar"].Value.ToString());
            if (FechaAgenda.Date != DateTime.Now.Date)
            {
                MessageBox.Show("No se permite Iniciar Circuito con una fecha que no sea la actual.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult Result = MessageBox.Show("¿Está seguro de INICIAR CIRCUITO este registro?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                string strCalendarId = grdDataCalendar.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();
                var modality = grdDataCalendar.Selected.Rows[0].Cells["i_NewContinuationId"].Value;
                int serviceStatusId = int.Parse(grdDataCalendar.Selected.Rows[0].Cells["i_ServiceStatusId"].Value.ToString());
                var ok = CalendarBL.CircuitStart(strCalendarId, DateTime.Now, Globals.ClientSession.i_SystemUserId, (int)modality, serviceStatusId);

                if (ok)
                {
                    _strServicelId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                    btnFilter_Click(sender, e);
                                                                                                            
                    var ListServiceComponent = AgendaBl.GetAllComponentsByService(_strServicelId);
                    ugComponentes.DataSource = ListServiceComponent;

                    grdDataCalendar.Rows[_RowIndexgrdDataCalendar].Selected = true;
                    MessageBox.Show("Circuito iniciado, paciente disponible para su atención", " ¡ INFORMACIÓN !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", " ¡ ERROR !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
        }

        private void btnMTC_Click(object sender, EventArgs e)
        {
            //4 mtc
            //frmAgendaParticular frmAgendar = new frmAgendaParticular("", "", "", 4, "", "", null,"");
            frmAgendaParticular frmAgendar = new frmAgendaParticular("", "", "", 4, "", "", null, 0, "", "", "", "", "", "", DateTime.Now,"");


            //frmAgendar.StartPosition
            frmAgendar.ShowDialog();
            btnFilter_Click(sender, e);
        }

        private void observacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var serviceId = grdDataCalendar.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
            var observaciones = grdDataCalendar.Selected.Rows[0].Cells["v_ObservacionesAdicionales"].Value.ToString();

            var frmPre = new frmObservaciones(serviceId, observaciones);
            frmPre.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            string Paciente = grdDataCalendar.Selected.Rows[0].Cells["ApePaterno"].Value.ToString() +" "+
                              grdDataCalendar.Selected.Rows[0].Cells["ApeMaterno"].Value.ToString() + " " +
                              grdDataCalendar.Selected.Rows[0].Cells["Nombres"].Value.ToString(); 
            
            Paciente = Paciente.Replace(" ", "%20");
            string numero = grdDataCalendar.Selected.Rows[0].Cells["v_TelephoneNumber"].Value.ToString();
            string urlCompleta = "";
            string urlws = GetApplicationConfigValue("Urlwhatsapp").ToString();
            string msws = GetApplicationConfigValue("Mensajewhatsapp").ToString();
            if (numero.Length > 8)
            {
                urlCompleta = urlws + numero + "?text=" + Paciente + "%20" + msws;
                ProcessStartInfo sInfo = new ProcessStartInfo(urlCompleta);
                Process.Start(sInfo);
            }
            else
            {
                return;
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string PersonId = grdDataCalendar.Selected.Rows[0].Cells["v_PersonId"].Value.ToString();

            if (string.IsNullOrEmpty(PersonId))
            {
                MessageBox.Show("Tiene que grabar al paciente antes de agendar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frmVacunas = new frmVacunasCovid19(PersonId);
            frmVacunas.ShowDialog();
        }

        private void verexamenesAuxiliaresConsultorioItem_Click(object sender, EventArgs e)
        {
            try
            {

                OperationResult objOperationResult = new OperationResult();


                var mode = "";
                var masterServiceId = grdDataCalendar.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
                if (masterServiceId == "10" || masterServiceId == "13" || masterServiceId == "14" || masterServiceId == "15" || masterServiceId == "16" ||
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
                    var ListServiceComponent = new ServiceBL().GetAllComponents(ref objOperationResult, 5, componentId);

                    
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
                if (DataSource != null)
                {
                    if (DataSource.Count > 0)
                    {
                        var frm = new frmAddExam(ComponentNewService, mode, _protocolId, "", ServiceId, DataSource, "", "", _dni);
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron exámenes adicionales para este paciente.", "AVISO",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                }



            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                this.BindGrid();
            };
        }

        private void BindGrid()
        {
            var objData = GetData(0, null, "");
            ListaGrilla = objData;
            grdDataDigitalContactCenter.DataSource = objData;
            
            lblRecordCountCalendar.Text = string.Format("Se encontraron {0} registros.", objData.Count());
        }

        private List<DigitalContactCenterDto> GetData(int pintPageIndex, int? pintPageSize, string pstrSortExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            DateTime? pdatBeginDate = FechaInicioPreRegistro.Value.Date;
            DateTime? pdatEndDate = FechaFinPreRegistro.Value.Date.AddDays(1);

          
            List<DigitalContactCenterDto> _objData = new List<DigitalContactCenterDto>();

            _objData = DigitalContactCenterBl.ObtenerDigitalContactCenterLista(pdatBeginDate, pdatEndDate);

            #region filtros
            if (checkDataTodos.Checked == true)
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.ELIMINADO == 1).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }
            else
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.ELIMINADO == 0).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }

            if (dboMedioPago.SelectedIndex != 0)
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.METODO_PAGO_ID == Convert.ToInt32(dboMedioPago.SelectedValue)).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }

            if (dboMedioMKT.SelectedIndex != 0)
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.MEDIO_MKT_ID == Convert.ToInt32(dboMedioMKT.SelectedValue)).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }

            if (cboEstadoAtencion.SelectedIndex != 0)
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.ESTADO_DCC_ID == Convert.ToInt32(cboEstadoAtencion.SelectedValue)).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }

            if (cboUserMed.SelectedIndex != 0 && cboUserMed.SelectedIndex != -1)
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.ID_INSERT_USER == Convert.ToInt32(cboUserMed.SelectedValue)).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }

            if (txtPacient.Text != string.Empty)
            {
                List<DigitalContactCenterDto> Data = _objData.Where(p => p.DOC.Contains(txtPacient.Text) ||
                                                                      p.NOMBRES.Contains(txtPacient.Text) ||
                                                                      p.AP_PATERNO.Contains(txtPacient.Text) ||
                                                                      p.AP_MATERNO.Contains(txtPacient.Text)).ToList();
                _objData = new List<DigitalContactCenterDto>(Data);
            }
            #endregion


            return _objData;

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //2 particular
            string _Documento = grdDataDigitalContactCenter.Selected.Rows[0].Cells["DOC"].Value.ToString();
            string _ProtocoloId = grdDataDigitalContactCenter.Selected.Rows[0].Cells["PROTOCOL_ID"].Value.ToString();
            DateTime? _FechaCita = (DateTime?)grdDataDigitalContactCenter.Selected.Rows[0].Cells["FECHA_CITA"].Value;
            string idccEditar = grdDataDigitalContactCenter.Selected.Rows[0].Cells["ID_DCC"].Value.ToString();


            //frmAgendaParticular frmAgendar = new frmAgendaParticular("DIGITAL", _Documento, "N009-OO000000052", 2, "", _ProtocoloId, _FechaCita, idccEditar);

            frmAgendaParticular frmAgendar = new frmAgendaParticular("DIGITAL", _Documento, "N009-OO000000052", 2, "", "", "", 0, "", _ProtocoloId, "", "", "", "", _FechaCita, idccEditar);

            //frmAgendar.StartPosition
            frmAgendar.ShowDialog();
            //btnFilter_Click(sender, e);
            button2_Click(sender, e);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                string _EstadoDigital = grdDataDigitalContactCenter.Selected.Rows[0].Cells["ESTADO_DCC_ID"].Value.ToString();
                string _ComentarioDigital = grdDataDigitalContactCenter.Selected.Rows[0].Cells["COMENTARIOS"].Value == null ? "" : grdDataDigitalContactCenter.Selected.Rows[0].Cells["COMENTARIOS"].Value.ToString();
                string idccEditar = grdDataDigitalContactCenter.Selected.Rows[0].Cells["ID_DCC"].Value.ToString();

                frmEditarEstadoDigitalContact frm = new frmEditarEstadoDigitalContact(_EstadoDigital, _ComentarioDigital, "EDITAR", idccEditar, Int32.Parse(Globals.ClientSession.GetAsList()[2]));
                frm.ShowDialog();

                button2_Click(sender, e);
            }
            catch
            {
                MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            


        }

        private void FechaInicioPreRegistro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void FechaFinPreRegistro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void dboMedioPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void dboMedioMKT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void cboEstadoAtencion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void cboUserMed_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void checkDataTodos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void ddlLineStatusId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void ddlVipId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void ddlNewContinuationId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void ddlCalendarStatusId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void txtNroDocument_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(null, null);
            }
        }

        private void cboUserMed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click(null, null);
            }
        }

        private void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            int mes = ObtenerMes(ddlMonth.Text);
            int anio = ddlAnio.Text == "" ? DateTime.Now.Year : Convert.ToInt32(ddlAnio.Text);
            DateTime fecha = new DateTime(anio, mes, 1);

            List<System.Windows.Forms.Button> botones = ObtenerBotones();

            PintarMes(fecha, botones);
        }

        private void ddlAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            int mes = ObtenerMes(ddlMonth.Text);
            int anio = ddlAnio.Text == "" ? DateTime.Now.Year : Convert.ToInt32(ddlAnio.Text);
            DateTime fecha = new DateTime(anio, mes, 1);

            List<System.Windows.Forms.Button> botones = ObtenerBotones();

            PintarMes(fecha, botones);
        }

        private void PintarMes(DateTime fecha, List<System.Windows.Forms.Button> botones)
        {
            try
            {
                string s_mes = Convert.ToString(fecha.ToString("MMMM"));

                int mes = ObtenerMesNro(s_mes);
                DateTime d_primerDia = new DateTime(fecha.Year, fecha.Month, 1);
                string s_primerDia = Convert.ToString(d_primerDia.ToString("dddd"));
                bool find = false;
                bool start = false;

                s_primerDia = Regex.Replace(s_primerDia, @"[^0-9A-Za-z]", "", RegexOptions.None);

                int dia = 1;
                foreach (var item in botones)
                {
                    if (!find) { find = PrimeraSemana(item, s_primerDia); }

                    if (find && !start)
                    {
                        item.Text = dia.ToString();
                        dia++;
                        start = true;
                        item.Visible = true;
                    }
                    else if (find && start && dia <= mes)
                    {
                        item.Text = dia.ToString();
                        dia++;
                        item.Visible = true;
                    }
                    else
                    {
                        item.Text = "";
                        item.Visible = false;
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private bool PrimeraSemana(System.Windows.Forms.Button btn, string dd)
        {
            if (btn.Name.Contains(dd)) { return true; }
            return false;
        }

        private List<System.Windows.Forms.Button> ObtenerBotones()
        {
            List<System.Windows.Forms.Button> botones = new List<System.Windows.Forms.Button>();
            botones.Add(btn_dia_1_lunes); botones.Add(btn_dia_1_martes); botones.Add(btn_dia_1_mircoles); botones.Add(btn_dia_1_jueves); botones.Add(btn_dia_1_viernes); botones.Add(btn_dia_1_sbado); botones.Add(btn_dia_1_domingo);
            botones.Add(btn_dia_2_lunes); botones.Add(btn_dia_2_martes); botones.Add(btn_dia_2_mircoles); botones.Add(btn_dia_2_jueves); botones.Add(btn_dia_2_viernes); botones.Add(btn_dia_2_sbado); botones.Add(btn_dia_2_domingo);
            botones.Add(btn_dia_3_lunes); botones.Add(btn_dia_3_martes); botones.Add(btn_dia_3_mircoles); botones.Add(btn_dia_3_jueves); botones.Add(btn_dia_3_viernes); botones.Add(btn_dia_3_sbado); botones.Add(btn_dia_3_domingo);
            botones.Add(btn_dia_4_lunes); botones.Add(btn_dia_4_martes); botones.Add(btn_dia_4_mircoles); botones.Add(btn_dia_4_jueves); botones.Add(btn_dia_4_viernes); botones.Add(btn_dia_4_sbado); botones.Add(btn_dia_4_domingo);
            botones.Add(btn_dia_5_lunes); botones.Add(btn_dia_5_martes); botones.Add(btn_dia_5_mircoles); botones.Add(btn_dia_5_jueves); botones.Add(btn_dia_5_viernes); botones.Add(btn_dia_5_sbado); botones.Add(btn_dia_5_domingo);
            botones.Add(btn_dia_6_lunes); botones.Add(btn_dia_6_martes);
            return botones;
        }

        private int ObtenerMes(string comboMes)
        {
            int mes = 0;
            switch (ddlMonth.Text)
            {
                case "Enero": { mes = 1; }
                    break;
                case "Febrero": { mes = 2; }
                    break;
                case "Marzo": { mes = 3; }
                    break;
                case "Abril": { mes = 4; }
                    break;
                case "Mayo": { mes = 5; }
                    break;
                case "Junio": { mes = 6; }
                    break;
                case "Julio": { mes = 7; }
                    break;
                case "Agosto": { mes = 8; }
                    break;
                case "Setiembre": { mes = 9; }
                    break;
                case "Octubre": { mes = 10; }
                    break;
                case "Noviembre": { mes = 11; }
                    break;
                case "Diciembre": { mes = 12; }
                    break;
            }
            return mes;
        }

        private int ObtenerMesNro(string s_mes)
        {
            int mes = 0;
            if (s_mes == "Enero" || s_mes == "Marzo" || s_mes == "Mayo" || s_mes == "Julio" ||
                s_mes == "Agosto" || s_mes == "Octubre" || s_mes == "Diciembre") { mes = 31; }
            else if (s_mes == "Abril" || s_mes == "Junio" || s_mes == "Setiembre" ||
                     s_mes == "Noviembre") { mes = 30; }
            else { mes = 28; }

            return mes;
        }

        private void btn_dia_1_lunes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_lunes);
        }

        private void btn_dia_1_martes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_martes);
        }

        private void btn_dia_1_mircoles_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_mircoles);
        }

        private void btn_dia_1_jueves_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_jueves);
        }

        private void btn_dia_1_viernes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_viernes);
        }

        private void btn_dia_1_sbado_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_sbado);
        }

        private void btn_dia_1_domingo_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_1_domingo);
        }

        private void btn_dia_2_lunes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_lunes);
        }

        private void btn_dia_2_martes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_martes);
        }

        private void btn_dia_2_mircoles_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_mircoles);
        }

        private void btn_dia_2_jueves_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_jueves);
        }

        private void btn_dia_2_viernes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_viernes);
        }

        private void btn_dia_2_sbado_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_sbado);
        }

        private void btn_dia_2_domingo_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_2_domingo);
        }

        private void btn_dia_3_lunes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_lunes);
        }

        private void btn_dia_3_martes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_martes);
        }

        private void btn_dia_3_mircoles_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_mircoles);
        }

        private void btn_dia_3_jueves_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_jueves);
        }

        private void btn_dia_3_viernes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_viernes);
        }

        private void btn_dia_3_sbado_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_sbado);
        }

        private void btn_dia_3_domingo_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_3_domingo);
        }

        private void btn_dia_4_lunes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_lunes);
        }







        private void chkOrdenAtencion_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOrdenAtencion.Checked == true)
            {
                List<AgendaDto> list = agendaGlobal.ToList();
                List<AgendaDto> listnew = new List<AgendaDto>();

                listnew = new List<AgendaDto>(list.OrderBy(p => p.d_DateTimeCalendar));

                grdCalendario.DataSource = listnew;

                if (listnew != null)
                {
                    lblCount.Text = string.Format("Se encontraron {0} registros.", listnew.Count());

                }

            }
            else
            {
                grdCalendario.DataSource = agendaGlobal;
                if (grdCalendario != null)
                {
                    lblCount.Text = string.Format("Se encontraron {0} registros.", agendaGlobal.Count());

                }
            }
        }

        private void btn_dia_4_martes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_martes);
        }

        private void btn_dia_4_mircoles_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_mircoles);
        }

        private void btn_dia_4_jueves_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_jueves);
        }

        private void btn_dia_4_viernes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_viernes);
        }

        private void btn_dia_4_sbado_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_sbado);
        }

        private void btn_dia_4_domingo_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_4_domingo);
        }

        private void btn_dia_5_lunes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_lunes);
        }

        private void btn_dia_5_martes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_martes);
        }

        private void btn_dia_5_mircoles_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_mircoles);
        }

        private void btn_dia_5_jueves_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_jueves);
        }

        private void btn_dia_5_viernes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_viernes);
        }

        private void btn_dia_5_sbado_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_sbado);
        }

        private void btn_dia_5_domingo_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_5_domingo);
        }

        private void btn_dia_6_lunes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_6_lunes);
        }

        private void btn_dia_6_martes_Click(object sender, EventArgs e)
        {
            chkOrdenAtencion_CheckedChanged(sender, e);

            CargarGrillaCalendar(btn_dia_6_martes);
        }

        private void CargarGrillaCalendar(System.Windows.Forms.Button btn)
        {
            dia = Convert.ToInt32(btn.Text);
            mes = ObtenerMes(ddlMonth.Text);
            anio = Convert.ToInt32(ddlAnio.Text);
            LoadCalendarGrid(dia, mes, anio);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            frmProfessional frm = new frmProfessional("");
            frm.ShowDialog();
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            try
            { 
                if (rbAtenciones.Checked == true)
                {
                    var frmAgendar = new frmAgendaParticular("", "", "", 2, "", null, null, 0, "", "", "", "", "", "", DateTime.Now,"");
                    frmAgendar.ShowDialog();
                    LoadCalendarGrid(dia, mes, anio);
                    //btnFilter_Click(sender, e);
                }
                else
                {
                    if (grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString() == "NA")
                    {
                        int tipoAtencion = 2;
                        string _modo = "New";
                        string _dni = "";

                        var Medico = grdCalendario.Selected.Rows[0].Cells["Medico_"].Value.ToString();
                        var Turno = grdCalendario.Selected.Rows[0].Cells["Turno_"].Value.ToString();
                        var Hora = grdCalendario.Selected.Rows[0].Cells["Hora_"].Value.ToString();
                        var Grupo = grdCalendario.Selected.Rows[0].Cells["Grupo_"].Value.ToString();

                        frmAgendaParticular frmAgendar = new frmAgendaParticular(_modo, _dni, "", tipoAtencion, "", null, null, 0, "", "", Medico, Turno, Hora, Grupo, _fechaAgenda,"");
                        frmAgendar.ShowDialog();
                        LoadCalendarGrid(dia, mes, anio);

                    }
                }
            }
            catch (Exception exception)
            {
            }
        }

        private void btnConfigDcto_Click(object sender, EventArgs e)
        {
            //frmConfigDescuentos frm = new frmConfigDescuentos();
            //frm.ShowDialog();
        }

        private void btnSolicitud_Click(object sender, EventArgs e)
        {

        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            DateTime FechaAgenda = DateTime.Parse(grdCalendario.Selected.Rows[0].Cells["d_DateTimeCalendar"].Value.ToString());
            if (FechaAgenda.Date != DateTime.Now.Date)
            {
                MessageBox.Show("No se permite Iniciar Circuito con una fecha que no sea la actual.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult Result = MessageBox.Show("¿Está seguro de INICIAR CIRCUITO este registro?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                string strCalendarId = grdCalendario.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();
                var modality = grdCalendario.Selected.Rows[0].Cells["i_NewContinuationId"].Value;
                int serviceStatusId = int.Parse(grdCalendario.Selected.Rows[0].Cells["i_ServiceStatusId"].Value.ToString());
                var ok = CalendarBL.CircuitStart(strCalendarId, DateTime.Now, Globals.ClientSession.i_SystemUserId, (int)modality, serviceStatusId);

                if (ok)
                {
                    _strServicelId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                    LoadCalendarGrid(dia, mes, anio);

                    MessageBox.Show("Circuito iniciado, paciente disponible para su atención", " ¡ INFORMACIÓN !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", " ¡ ERROR !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
        }

        private void btnReagendar_Click(object sender, EventArgs e)
        {
            string serviceId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();


            AgendaBl.UpdateCalendar(serviceId, 1, 0);

            LoadCalendarGrid(dia, mes, anio);
        }

        private void btnCancelarAtencion_Click(object sender, EventArgs e)
        {
            string statudCalendar = grdCalendario.Selected.Rows[0].Cells["v_CalendarStatusName"].Value.ToString();


            if (statudCalendar != "ATENDIDO")
            {
                var serviceId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                string strCalendarId = grdCalendario.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();

                var OK = AgendaBl.CancelarCita(strCalendarId, Globals.ClientSession.i_SystemUserId, serviceId);

                //var OK = CalendarBL.CancelAtx(strCalendarId_, Globals.ClientSession.i_SystemUserId);
                if (OK)
                {
                    LoadCalendarGrid(dia, mes, anio);
                }
                else
                {
                    MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                frmValidacion validacion = new frmValidacion();
                validacion.ShowDialog();

                if (validacion.Respuesta == "ACEPTADO")
                {
                    var v_Service = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                    #region Conexion SAM

                    ConexionSigesoft conectasam = new ConexionSigesoft();
                    conectasam.opensigesoft();


                    #endregion

                    var cadena1 = "update service set " +
                                  "v_ComentaryUpdate = CONCAT('" + validacion.Usuario + " - " + DateTime.Now +
                                  " | ', v_ComentaryUpdate)" +
                                  "where v_ServiceId='" + v_Service + "' ";

                    SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                    SqlDataReader lector = comando.ExecuteReader();
                    lector.Close();

                    var serviceId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                    string strCalendarId = grdCalendario.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();


                    var OK = AgendaBl.CancelarCita(strCalendarId, Globals.ClientSession.i_SystemUserId, serviceId);
                    //string strCalendarId_ = grdCalendario.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();

                    //var OK = CalendarBL.CancelAtx(strCalendarId_, Globals.ClientSession.i_SystemUserId);
                    if (OK)
                    {
                        LoadCalendarGrid(dia, mes, anio);
                    }
                    else
                    {
                        MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No puede ser cancelado, SERVICIO en CIRCUITO sin la AUTORIZACION del Administrador.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }   
        }

        private void button7_Click(object sender, EventArgs e)
        {
            UltraGridBand band = this.grdDataCalendar.DisplayLayout.Bands[0];
            List<string> servicios_ = new List<string>();
         
            var result = new BindingList<ventadetalleDto>();
            var servicios = new List<string>();

            //foreach (var item in grdDataCalendar.Rows)
            //{

            //    if ((bool)item.Cells["b_Seleccionar"].Value)
            //    {
                    var empFact = "";
                    //var protocolo = item.Cells["v_ProtocolName"].Value.ToString();
                    var protocolo = grdCalendario.Selected.Rows[0].Cells["v_ProtocolName"].Value.ToString();
                    var serviceId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

                    float precioTotal = ObtenerPrecioServicio(serviceId); //float.Parse(item.Cells["PrecioTotalProtocolo"].Value.ToString());
                    //var serviceId = item.Cells["v_ServiceId"].Value.ToString();

                    //if (item.Cells["v_OrganizationId"].Value != null)
                    //{
                    //    empFact = item.Cells["v_OrganizationId"].Value.ToString();
                    //}

                    if (grdCalendario.Selected.Rows[0].Cells["v_OrganizationId"].Value != null)
                    {
                        empFact = grdCalendario.Selected.Rows[0].Cells["v_OrganizationId"].Value.ToString();
                    }

                    //var rucEmpFact = item.Cells["RucEmpFact"].Value.ToString();
                    var rucEmpFact = "";//item.Cells["RucEmpFact"].Value.ToString();

                    var masterServiceId = grdCalendario.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
                    if (masterServiceId == "12" || masterServiceId == "13")
                    {
                        var listSaldo = FacturacionServiciosBl.SaldoPacienteAseguradora(serviceId);
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
                                servicios.Add(serviceId);
                            }
                        }
                        else
                        {
                            MessageBox.Show("El paciente no tiene saldo por pagar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                    }
                    else if (masterServiceId == "2" || masterServiceId == "3")
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
                        servicios.Add(serviceId);
                        //this.Close();
                    }
                    else
                    {

                        DialogResult Result = MessageBox.Show("¿Desea venta detallada?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (Result == System.Windows.Forms.DialogResult.Yes)
                        {
                            var listSaldo = FacturacionServiciosBl.DetalleVenta(serviceId);
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
                                    servicios.Add(serviceId);
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
                            servicios.Add(serviceId);
                        }
                    }



            //    }
            //}



            ListadoVentaDetalle = result;
            DialogResult = DialogResult.OK;

            frmRegistroVentaRapida frm = new frmRegistroVentaRapida("Nuevo", "", servicios);
            frm.ListadoVentaDetalle = ListadoVentaDetalle;
            frm.ShowDialog();
            //string protocolo = null;

            //////////////////////////////////// OLD
            
            //UltraGridBand band = this.grdDataCalendar.DisplayLayout.Bands[0];
            //List<string> servicios_ = new List<string>();

            //var result = new BindingList<ventadetalleDto>();
            //var servicios = new List<string>();

            ////foreach (var item in grdCalendario.Rows)
            ////{

            ////if ((bool)item.Cells["b_Seleccionar"].Value)
            ////{
            //var empFact = "";
            //protocolo = grdCalendario.Selected.Rows[0].Cells["v_ProtocolName"].Value.ToString();

            //var precioTotal = float.Parse(grdCalendario.Selected.Rows[0].Cells["PrecioTotalProtocolo"].Value.ToString());


            //var serviceId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();

            //if (grdCalendario.Selected.Rows[0].Cells["v_OrganizationId"].Value != null)
            //{
            //    empFact = grdCalendario.Selected.Rows[0].Cells["v_OrganizationId"].Value.ToString();
            //}

            //var rucEmpFact = "";//item.Cells["RucEmpFact"].Value.ToString();
            //var masterServiceId = grdCalendario.Selected.Rows[0].Cells["i_MasterServiceId"].Value.ToString();
            //if (masterServiceId == "12" || masterServiceId == "13")
            //{
            //    var listSaldo = FacturacionServiciosBl.SaldoPacienteAseguradora(serviceId);
            //    if (listSaldo != null)
            //    {
            //        foreach (var itemSaldo in listSaldo)
            //        {
            //            var cant = 1;
            //            var pu = decimal.Parse(itemSaldo.d_SaldoPaciente.ToString());
            //            var valorV = Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
            //            var valorBase = valorV / 1.18m;
            //            var igv = Math.Round(valorV - valorBase, 2, MidpointRounding.AwayFromZero);  //Math.Round(pu * 0.18m, 2, MidpointRounding.AwayFromZero);
            //            var oventadetalleDto = new ventadetalleDto
            //            {

            //                i_Anticipio = 0,
            //                i_IdAlmacen = 1,
            //                i_IdCentroCosto = "0",
            //                i_IdUnidadMedida = 15,
            //                ProductoNombre = itemSaldo.v_Name,
            //                v_DescripcionProducto = itemSaldo.v_Name,
            //                v_IdProductoDetalle = "N002-PE000000001",
            //                v_NroCuenta = string.Empty,
            //                d_PrecioVenta = decimal.Parse(valorV.ToString()),
            //                d_Igv = igv,
            //                d_Cantidad = cant,
            //                d_CantidadEmpaque = cant,
            //                d_Precio = pu,
            //                d_Valor = pu,
            //                d_ValorVenta = valorBase,
            //                d_PrecioImpresion = pu,
            //                v_CodigoInterno = "ATMD01",
            //                Empaque = 1,
            //                UMEmpaque = "UND",
            //                i_EsServicio = 1,
            //                i_IdUnidadMedidaProducto = 15,
            //                v_ServiceId = serviceId,
            //                EmpresaFacturacion = empFact,
            //                RucEmpFacturacion = rucEmpFact,

            //            };
            //            result.Add(oventadetalleDto);
            //            servicios.Add(serviceId);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("El paciente no tiene saldo por pagar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }
            //}
            //else
            //{
            //    var cant = 1;
            //    //var pu = decimal.Parse(precioTotal.ToString());
            //    float precioSaldo = FacturacionServiciosBl.ExamenesSinFacturar(serviceId);

            //    var pu = decimal.Parse(precioSaldo.ToString());
            //    var valorV = Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
            //    var valorBase = valorV / 1.18m;
            //    var igv = Math.Round(valorV - valorBase, 2, MidpointRounding.AwayFromZero);
            //    var oventadetalleDto = new ventadetalleDto
            //    {

            //        i_Anticipio = 0,
            //        i_IdAlmacen = 1,
            //        i_IdCentroCosto = "0",
            //        i_IdUnidadMedida = 15,
            //        ProductoNombre = protocolo,
            //        v_DescripcionProducto = protocolo,
            //        v_IdProductoDetalle = "N002-PE000000001",
            //        v_NroCuenta = string.Empty,
            //        d_PrecioVenta = decimal.Parse(precioTotal.ToString()),
            //        d_Igv = igv,
            //        d_Cantidad = cant,
            //        d_CantidadEmpaque = cant,
            //        d_Precio = pu,
            //        d_Valor = pu,
            //        d_ValorVenta = valorBase,
            //        d_PrecioImpresion = pu,
            //        v_CodigoInterno = "ATMD01",
            //        Empaque = 1,
            //        UMEmpaque = "UND",
            //        i_EsServicio = 1,
            //        i_IdUnidadMedidaProducto = 15,
            //        v_ServiceId = serviceId,
            //        EmpresaFacturacion = empFact,
            //        RucEmpFacturacion = rucEmpFact,
            //    };
            //    result.Add(oventadetalleDto);
            //    servicios.Add(serviceId);
            //}

            ////}
            ////}



            //ListadoVentaDetalle = result;
            //DialogResult = DialogResult.OK;
            ////frmTipoVisualizacionVenta formTipo = new frmTipoVisualizacionVenta(protocolo);
            ////formTipo.ShowDialog();
            ////if (formTipo.consolidado == -1)
            ////{
            ////    return;
            ////}
            ////else if (formTipo.consolidado == (int)SiNo.SI)
            ////{
            ////    frmRegistroVentaRapida frm = new frmRegistroVentaRapida("Agenda", "", servicios);
            ////    frm.ListadoVentaDetalle = ListadoVentaDetalle;
            ////    frm.ShowDialog();
            ////}
            ////else
            ////{
            //    result = new BindingList<ventadetalleDto>();

            //    servicios = new List<string>();
            //    //foreach (var item in grdDataCalendar.Rows)
            //    //{

            //    //if ((bool)item.Cells["b_Seleccionar"].Value)
            //    //{
            //    //var empFact = "";
            //    var servicioId = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
            //    if (grdCalendario.Selected.Rows[0].Cells["v_OrganizationId"].Value != null)
            //    {
            //        empFact = grdCalendario.Selected.Rows[0].Cells["v_OrganizationId"].Value.ToString();
            //    }

            //    //var rucEmpFact = "";
            //    var ListServiceComponent = new ServiceBL().GetServiceComponents(servicioId);
            //    foreach (var servicecomp in ListServiceComponent)
            //    {

            //        var cant = 1;
            //        var pu = decimal.Parse(servicecomp.r_Price.ToString());
            //        var valorV = Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
            //        var valorBase = valorV / 1.18m;
            //        var igv = Math.Round(valorV - valorBase, 2, MidpointRounding.AwayFromZero);
            //        var oventadetalleDto = new ventadetalleDto
            //        {

            //            i_Anticipio = 0,
            //            i_IdAlmacen = 1,
            //            i_IdCentroCosto = "0",
            //            i_IdUnidadMedida = 15,
            //            ProductoNombre = servicecomp.v_ComponentName == "HISTORIA CLINICA CSL" ? protocolo : servicecomp.v_ComponentName,
            //            v_DescripcionProducto = servicecomp.v_ComponentName == "HISTORIA CLINICA CSL" ? protocolo : servicecomp.v_ComponentName,
            //            v_IdProductoDetalle = "N002-PE000000001",
            //            v_NroCuenta = string.Empty,
            //            d_PrecioVenta = decimal.Parse(servicecomp.r_Price.ToString()),
            //            d_Igv = igv,
            //            d_Cantidad = cant,
            //            d_CantidadEmpaque = cant,
            //            d_Precio = pu,
            //            d_Valor = pu,
            //            d_ValorVenta = valorBase,
            //            d_PrecioImpresion = pu,
            //            v_CodigoInterno = "ATMD01",
            //            Empaque = 1,
            //            UMEmpaque = "UND",
            //            i_EsServicio = 1,
            //            i_IdUnidadMedidaProducto = 15,
            //            v_ServiceId = serviceId,
            //            EmpresaFacturacion = empFact,
            //            RucEmpFacturacion = rucEmpFact,
            //        };
            //        result.Add(oventadetalleDto);

            //    //}
            //    servicios.Add(servicioId);
            //    //}
            //    //}

            //    ListadoVentaDetalle = result;
            //    DialogResult = DialogResult.OK;

            //    frmRegistroVentaRapida frm = new frmRegistroVentaRapida("Agenda", "", servicios);
            //    frm.ListadoVentaDetalle = ListadoVentaDetalle;
            //    frm.ShowDialog();
            //}
        }

        private void btnTicketCalendar_Click(object sender, EventArgs e)
        {
            var Servicio = grdCalendario.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
            var Calendar = grdCalendario.Selected.Rows[0].Cells["v_CalendarId"].Value.ToString();
            var Cumpleaños = DateTime.Parse(grdCalendario.Selected.Rows[0].Cells["d_BirthdateN"].Value.ToString());

            var frm = new frmRoadMapAsist(Servicio, Calendar, Cumpleaños);
            frm.ShowDialog();
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdDataCalendar.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Listado de Atenciones California";

            using (var sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            })
            {
                if (sf.ShowDialog() != DialogResult.OK) return;
                var filename = sf.FileName;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;

                _tarea = Task.Factory.StartNew(() =>
                {
                    using (var ultraGridExcelExporter1 = new UltraGridExcelExporter())
                        ultraGridExcelExporter1.Export(grdDataCalendar, filename);
                }, _cts.Token)
                    .ContinueWith(t => ActualizarLabel("TERMINADO"),
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
            //btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var frm = new frmRoadMapA4(_strServicelId, _calendarId, _fechaNacimiento);
            frm.ShowDialog();
        }


        
    }
}
