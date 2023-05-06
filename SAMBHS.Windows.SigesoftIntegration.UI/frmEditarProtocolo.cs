using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmEditarProtocolo : Form
    {
        private string _protocolId;
        private int _tipoServicioId;
        private int _servicioId;
        private int? _medicoTratanteId;
        private string _serviceId;
        private string _ruc;
        private string protocoloName = "";
        private AgendaBl agendaBl_ = new AgendaBl();
        private EmpresaBl empresaBl_ = new EmpresaBl();
        public frmEditarProtocolo(string protocolId, int tipoServicioId, int servicioId, int? medicoTratanteId, string serviceId, string RUC)
        {
            _ruc = RUC;
            _protocolId = protocolId;
            _tipoServicioId = tipoServicioId;
            _servicioId = servicioId;
            _medicoTratanteId = medicoTratanteId;
            _serviceId = serviceId;
            InitializeComponent();
        }

        private void frmEditarProtocolo_Load(object sender, EventArgs e)
        {
            AgendaBl.LlenarComboMedicoSolicitanteExterno(cboMedicoSolicitanteExterno);
            AgendaBl.LlenarComboEstablecimiento(cboEstablecimiento);
            AgendaBl.LlenarComboVendedorExterno(cboVendedorExterno);


            cboEstablecimiento.SelectedValue = 1;
            cboVendedorExterno.SelectedValue = 1;
            cboMedicoSolicitanteExterno.SelectedValue = 1;


            cboModalidadTrabajo.DataSource = agendaBl_.LlenarModalidadTrabajo(cboModalidadTrabajo);
            cboModalidadTrabajo.SelectedValue = -1;

            cboMarketing.DataSource = agendaBl_.LlenarComboMarketing(cboMarketing);
            cboMarketing.SelectedIndex = 0;

            cboTipoServicio.DataSource = agendaBl_.LlenarComboTipoServicio(cboTipoServicio);
            cboTipoServicio.SelectedIndex = 1;

            cboEmpresaEmpleadora.DataSource = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
            cboEmpresaEmpleadora.SelectedIndex = 1;

            cboEmpresaCliente.DataSource = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaCliente, 9);
            cboEmpresaCliente.SelectedIndex = 1;

            cboEmpresaTrabajo.DataSource = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaTrabajo, 9);
            cboEmpresaTrabajo.SelectedIndex = 1;

            cboMedicoTratante.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);
            cboMedicoTratante.SelectedIndex = 1;

            AgendaBl.LlenarComboPlan(cboPlanAtencion);
            cboTipoServicio.SelectedItem = _tipoServicioId;
            cboServicio.SelectedValue = _servicioId;

            cboProtocolo.SelectedValue = _protocolId;

            cboMedicoTratante.SelectedValue = _medicoTratanteId.ToString();
            txtRucCliente.Text = _ruc;
            var listaCCo = AgendaBl.ObtenerCC();
            txtCCosto.DataSource = listaCCo;
            txtCCosto.DisplayMember = "Costo";
            txtCCosto.ValueMember = "Costo";

            var lista = AgendaBl.ObtenerAreas();
            txtArea.DataSource = lista;
            txtArea.DisplayMember = "Area";
            txtArea.ValueMember = "Area";

            txtArea.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtArea.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtArea.DropDownWidth = 250;
            txtArea.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtArea.DisplayLayout.Bands[0].Columns[1].Width = 250;
            if (!string.IsNullOrEmpty(""))
            {
                txtArea.Value = "";
            }


            txtCCosto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtCCosto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtCCosto.DropDownWidth = 250;
            txtCCosto.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtCCosto.DisplayLayout.Bands[0].Columns[1].Width = 250;
            if (!string.IsNullOrEmpty(""))
            {
                txtCCosto.Value = "";
            }

            protocoloName = cboProtocolo.Text;
        }

        private void cboTipoServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoServicio.SelectedIndex == 0 || cboTipoServicio.SelectedIndex == -1)
            {
                cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, 1000);
                cboServicio.SelectedIndex = 0;
            }
            else
            {
                cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, int.Parse(cboTipoServicio.SelectedValue.ToString()));
                cboServicio.SelectedIndex = 0;
                if (int.Parse(cboTipoServicio.SelectedValue.ToString()) == 9 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 11 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 12 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 13)
                {
                    cboMedicoTratante.Enabled = true;
                }
                else
                {
                    cboMedicoTratante.Enabled = false;
                    cboMedicoTratante.SelectedValue = "-1";
                }
            }
        }

        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboServicio.SelectedIndex == 0 || cboServicio.SelectedIndex == -1)
                cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, 1000, 1000);
            else
                cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));
    
        }

        private void cboProtocolo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProtocolo.SelectedIndex == 0 || cboProtocolo.SelectedIndex == -1)
                LimpiarControlesProtocolo();
            else
                LlenarControlesProtocolo();

            string protocolo;
            //int id;
            //if (!int.TryParse(cboProtocolo.SelectedValue.ToString(), out id)) return;

            if (cboProtocolo.SelectedIndex == -1) return;
            protocolo = cboProtocolo.SelectedValue.ToString();
            if (protocolo != "SAMBHS.Windows.SigesoftIntegration.UI.EsoDto")
            {
                AgendaBl.LlenarComboPlanes(cboPlanAtencion, protocolo);
            }
        }

        private void LimpiarControlesProtocolo()
        {
            cboGeso.SelectedValue = -1;
            cboTipoEso.SelectedValue = -1; ;
            cboEmpresaCliente.SelectedValue = -1;
            cboEmpresaEmpleadora.SelectedValue = -1;
            cboEmpresaTrabajo.SelectedValue = -1;
        }

        private void LlenarControlesProtocolo()
        {
            cboTipoEso.DataSource = new AgendaBl().LlenarComboSystemParametro(cboTipoEso, 118);

            var datosProtocolo = AgendaBl.GetDatosProtocolo(cboProtocolo.SelectedValue.ToString());
            cboGeso.DataSource = new AgendaBl().ObtenerGesoProtocol(cboGeso, datosProtocolo.v_EmployerOrganizationId,
                datosProtocolo.v_EmployerLocationId);
            cboGeso.SelectedIndex = 0;
            cboGeso.SelectedValue = datosProtocolo.v_GroupOccupationId;
            cboTipoEso.SelectedValue = datosProtocolo.i_EsoTypeId.ToString();
            cboEmpresaCliente.SelectedValue = datosProtocolo.EmpresaCliente;
            cboEmpresaEmpleadora.SelectedValue = datosProtocolo.EmpresaEmpleadora;
            cboEmpresaTrabajo.SelectedValue = datosProtocolo.EmpresaTrabajo;

            var datosServico = AgendaBl.GetDatosServicio(_serviceId);


            cboEstablecimiento.SelectedValue = datosServico.Establecimiento;
            cboVendedorExterno.SelectedValue = datosServico.VendedorExterno;
            cboMedicoSolicitanteExterno.SelectedValue = datosServico.MedicoSolicitanteExterno;

            var lista = AgendaBl.ObtenerAreas();
            txtArea.DataSource = lista;
            txtArea.DisplayMember = "Area";
            txtArea.ValueMember = "Area";

            txtArea.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtArea.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtArea.DropDownWidth = 250;
            txtArea.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtArea.DisplayLayout.Bands[0].Columns[1].Width = 250;
            if (!string.IsNullOrEmpty(datosServico.Area))
            {
                txtArea.Value = datosServico.Area;
            }
            if (datosServico.i_ModTrabajo != null && datosServico.i_ModTrabajo != 0)
            {
                cboModalidadTrabajo.SelectedValue = datosServico.i_ModTrabajo;
            }

            if (datosServico.i_ProcedenciaPac_Mkt != null && datosServico.i_ProcedenciaPac_Mkt != -1)
            {
                cboMarketing.SelectedValue = datosServico.i_ProcedenciaPac_Mkt;
            }

            var listaCCosto = AgendaBl.ObtenerCC();
            txtCCosto.DataSource = listaCCosto;
            txtCCosto.DisplayMember = "Costo";
            txtCCosto.ValueMember = "Costo";

            txtCCosto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtCCosto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtCCosto.DropDownWidth = 250;
            txtCCosto.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtCCosto.DisplayLayout.Bands[0].Columns[1].Width = 250;
            if (!string.IsNullOrEmpty(datosServico.CCosto))
            {
                txtCCosto.Value = datosServico.CCosto;
            }

            if (!string.IsNullOrEmpty(datosServico.v_LicenciaConducir))
            {
                txtLicenciadeConducir.Text = datosServico.v_LicenciaConducir;
            }


            //txtCCosto
        }

        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (!txtRucCliente.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                frmEmpresa frm = new frmEmpresa(txtRucCliente.Text.Trim());
                frm.ShowDialog();
                cboEmpresaEmpleadora.DataSource = new EmpresaBl().GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
                cboEmpresaEmpleadora.SelectedValue = frm.orgnizationEmployerId ?? "-1";
            }
        }

        private void btnschedule_Click(object sender, EventArgs e)
        {
            int usuarioactualiza = 11;

            if (Globals.ClientSession.i_SystemUserId == 2034)
            {
                usuarioactualiza = 199;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2035)
            {
                usuarioactualiza = 197;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2044)
            {
                usuarioactualiza = 193;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2046)
            {
                usuarioactualiza = 244;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2047)
            {
                usuarioactualiza = 245;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2041)
            {
                usuarioactualiza = 203;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2040)
            {
                usuarioactualiza = 232;
            }
            else if (Globals.ClientSession.i_SystemUserId == 2038)
            {
                usuarioactualiza = 232;
            }

            ProtocolDto oProtocolDto = new ProtocolDto();
            string comentario = "";
            if (_protocolId != cboProtocolo.SelectedValue.ToString())
            {
                comentario = ProtocoloBl.GetCommentaryUpdateByserviceId(_serviceId);
                comentario += "<FechaActualiza:" + DateTime.Now.ToString() + "|UsuarioActualiza:" + Globals.ClientSession.v_UserName + "|";
                comentario += "Nombre de protocolo :" + protocoloName;

            }

            int establecimiento = -1;
            if (cboEstablecimiento.SelectedValue == null)
            {
                establecimiento = AgendaBl.CreateItemEstablecimiento(cboEstablecimiento.Text);
            }
            else
            {
                establecimiento = int.Parse(cboEstablecimiento.SelectedValue.ToString());
            }

            int vendedorExterno = -1;
            if (cboVendedorExterno.SelectedValue == null)
            {
                vendedorExterno = AgendaBl.CreateItemVendedorExterno(cboVendedorExterno.Text);
            }
            else
            {
                vendedorExterno = int.Parse(cboVendedorExterno.SelectedValue.ToString());
            }

            int medicoExterno = -1;
            if (cboMedicoSolicitanteExterno.SelectedValue == null)
            {
                medicoExterno = AgendaBl.CreateItemMedicoExterno(cboMedicoSolicitanteExterno.Text);
            }
            else
            {
                medicoExterno = int.Parse(cboMedicoSolicitanteExterno.SelectedValue.ToString());
            }

            oProtocolDto.i_MasterServiceTypeId = int.Parse(cboTipoServicio.SelectedValue.ToString());
            oProtocolDto.i_MasterServiceId = int.Parse(cboServicio.SelectedValue.ToString());
            oProtocolDto.v_ProtocolId = cboProtocolo.SelectedValue.ToString();
            oProtocolDto.v_GroupOccupationId = cboGeso.SelectedValue.ToString();
            oProtocolDto.i_EsoTypeId = int.Parse(cboTipoEso.SelectedValue.ToString());

            var Employer = cboEmpresaEmpleadora.SelectedValue.ToString().Split('|');
            var Customer = cboEmpresaCliente.SelectedValue.ToString().Split('|');
            var Working = cboEmpresaTrabajo.SelectedValue.ToString().Split('|');

            oProtocolDto.v_EmployerOrganizationId = Employer[0];
            oProtocolDto.v_EmployerLocationId = Employer[1];
            oProtocolDto.v_CustomerOrganizationId = Customer[0];
            oProtocolDto.v_CustomerLocationId = Customer[1];
            oProtocolDto.v_WorkingOrganizationId = Working[0];
            oProtocolDto.v_WorkingLocationId = Working[1];

            int medico = int.Parse(cboMedicoTratante.SelectedValue.ToString());
            string Area = txtArea.Text;
            int ModalidadTrabajo = int.Parse(cboModalidadTrabajo.SelectedValue.ToString());

            int mkt = int.Parse(cboMarketing.SelectedValue.ToString());


            string Plan = cboPlanAtencion.SelectedValue.ToString();
            string Licencia = txtLicenciadeConducir.Text;
            ProtocoloBl.UpdateServiceComponent(cboProtocolo.SelectedValue.ToString(), oProtocolDto.i_MasterServiceTypeId, medico, _serviceId, txtCCosto.Text, oProtocolDto.i_MasterServiceId, usuarioactualiza, Area, Plan, Licencia, ModalidadTrabajo, mkt, establecimiento, vendedorExterno, medicoExterno);

            DialogResult = DialogResult.OK;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}
