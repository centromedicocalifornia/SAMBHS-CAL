using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Windows.WinClient.UI;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmAddRecipe : Form
    {
        private Receta _recetaDto = new Receta();
        private OperationResult _pobjOperationResult;
        private int _actionForm;
        private RecetaBL _objRecetaBl = new RecetaBL();

        private string _idDiagnosticRepository;
        private string _recipeId;
        private string idUnidadProductiva;
        private string _protocolId;
        private string _serviceId;
        private string _LineId;
        private int _userId;
        private string _UserName;

        public frmAddRecipe(int actionForm, string idDiagnosticRepository, string recipeId, string protocolId, string serviceId, string LineId, string UserName,int idMedico)
        {
            InitializeComponent();
            _UserName = UserName;
            _userId = idMedico;
            _recipeId = recipeId;
            _idDiagnosticRepository = idDiagnosticRepository;
            _pobjOperationResult = new OperationResult();
            _objRecetaBl = new RecetaBL();
            _actionForm = actionForm;
            _protocolId = protocolId;
            if (actionForm == (int) Enums.ActionForm.Add)
            {
                Text = "Agregar Nueva Receta";
            } 
            else
            {
                Text = "Editar Receta";
            }

            _serviceId = serviceId;

            _LineId = LineId;
        }

        private int GetUserId_(string UserName)
        {
            int userId = 0;
            ConexionSigesoft conexion = new ConexionSigesoft();
            conexion.opensigesoft();
            string sql = "";
            SqlCommand comando = new SqlCommand(sql, conexion.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                userId = (int) lector.GetValue(0);
            }

            return userId;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void Cargar(string idDiagnostic, string recipeId)
        {
            try
            {
                _pobjOperationResult = new OperationResult();
                switch (_actionForm)
                {
                    case (int)Enums.ActionForm.Add:
                        _recetaDto = new Receta();
                        _recetaDto.v_DiagnosticRepositoryId = idDiagnostic;
                        break;

                    case (int)Enums.ActionForm.Edit:
                        _recetaDto = _objRecetaBl.GetRecipeById(ref _pobjOperationResult, recipeId);
                        if (_pobjOperationResult.Success == 0)
                        {
                            MessageBox.Show(_pobjOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        txtMedicamento.Text = _recetaDto.NombreMedicamento;
                        txtMedicamento.Tag = _recetaDto.v_IdProductoDetalle;
                        txtCantidad.Text = (_recetaDto.d_Cantidad ?? 0m).ToString(CultureInfo.InvariantCulture);
                        txtDuracion.Text = _recetaDto.v_Duracion.Trim();
                        txtPosologia.Text = _recetaDto.v_Posologia.Trim();
                        idUnidadProductiva = _recetaDto.v_IdUnidadProductiva;
                        txtUnidadProductiva.Text = _recetaDto.v_IdUnidadProductiva;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Cargar()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmAddRecipe_Load(object sender, EventArgs e)
        {
            Cargar(_idDiagnosticRepository, _recipeId);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            TicketBL oTicketBL = new TicketBL();
            try
            {
                _pobjOperationResult = new OperationResult();
                if (!uvDatos.Validate(true, false).IsValid) return;

                if (txtMedicamento.Tag == null)
                {
                    MessageBox.Show(@"Por favor seleccione un medicamento", @"Error de validación", MessageBoxButtons.OK);
                    txtMedicamento.Focus();
                    return;
                }

                decimal d;
                _recetaDto.d_Cantidad = decimal.TryParse(txtCantidad.Text, out d) ? d : 0;
                _recetaDto.v_Duracion = txtDuracion.Text.Trim();
                _recetaDto.v_Posologia = txtPosologia.Text.Trim();
                _recetaDto.t_FechaFin = dtpFechaFin.Value;
                _recetaDto.v_IdProductoDetalle = txtMedicamento.Tag.ToString();
                _recetaDto.v_IdUnidadProductiva = idUnidadProductiva;
                _recetaDto.v_ServiceId = _serviceId;
                _recetaDto.v_DiagnosticRepositoryId = _idDiagnosticRepository;
                _recetaDto.i_MedicoId = _userId;
                _recetaDto.v_ReceiptId = _recipeId;
                _recetaDto.v_MedicoName = _UserName;
                var tienePlan = false;
                var resultplan = oTicketBL.TienePlan(_protocolId, txtUnidadProductiva.Text);
                if (resultplan.Count > 0) tienePlan = true;
                else tienePlan = false;

                if (tienePlan)
                {
                    if (resultplan[0].i_EsCoaseguro == 1)
                    {
                        #region Conexion SIGESOFT verificar la unidad productiva del componente
                        ConexionSigesoft conectasam = new ConexionSigesoft();
                        conectasam.opensigesoft();
                        //var cadena1 = "select PL.d_ImporteCo " +
                        //              "from [dbo].[plan] PL " +
                        //              "inner join protocol PR on PL.v_ProtocoloId=PR.v_ProtocolId " +
                        //              "inner join servicecomponent SC on PL.v_IdUnidadProductiva=SC.v_IdUnidadProductiva " +
                        //              "inner join diagnosticrepository DR on DR.v_ComponentId=SC.v_ComponentId " +
                        //              "where PR.v_ProtocolId='" + _protocolId + "' and SC.v_ServiceId='" + _serviceId + "' ";

                        var cadena1 = @" Select DISTINCT pl.d_ImporteCo 
                                        from service s
                                        join protocol p on s.v_ProtocolId = p.v_ProtocolId
                                        join servicecomponent sc on s.v_ServiceId = sc.v_ServiceId
                                        join [dbo].[plan] pl on p.v_ProtocolId = pl.v_ProtocoloId " +
                                      "where p.v_ProtocolId='" + _protocolId + "' and s.v_ServiceId='" + _serviceId + "' and pl.v_IdUnidadProductiva = '" + txtUnidadProductiva.Text + "' and sc.i_IsRequiredId = 1";
                        SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                        SqlDataReader lector = comando.ExecuteReader();
                        string ImporteCo = "";
                        bool lectorleido = false;
                        while (lector.Read())
                        {
                            ImporteCo = lector.GetValue(0).ToString();
                            lectorleido = true;
                        }
                        if (lectorleido == false)
                        {
                            MessageBox.Show(@"El consultorio no tiene Plan de Seguros", @"Error de validación", MessageBoxButtons.OK);
                            return;
                        }
                        lector.Close();
                        conectasam.closesigesoft();
                        #endregion
                        _recetaDto.d_SaldoPaciente = (decimal.Parse(ImporteCo) / 100) * (decimal.Parse(txtNuevoPrecio.Text) * _recetaDto.d_Cantidad);
                        _recetaDto.d_SaldoAseguradora = (decimal.Parse(txtNuevoPrecio.Text) * _recetaDto.d_Cantidad) - _recetaDto.d_SaldoPaciente;
                    }

                }
                else
                {

                    _recetaDto.d_SaldoPaciente = decimal.Parse(txtPrecio.Text) * _recetaDto.d_Cantidad;
                    //_recetaDto.d_SaldoAseguradora = 0;
                }

                //ConexionSigesoft conectasam_ = new ConexionSigesoft();
                //conectasam_.opensigesoft();
                //var cadena_ = "update protocol set " +
                //             "v_AseguradoraOrganizationId='" + _aseguradoraId + "' " +
                //             "where v_ProtocolId='" + _protocolId + "'";
                //SqlCommand comando_ = new SqlCommand(cadena_, connection: conectasam_.conectarsigesoft);
                //var lector_ = comando_.ExecuteReader();
                //lector_.Close();
                //conectasam_.closesigesoft();

                _objRecetaBl.AddUpdateRecipe(ref _pobjOperationResult, _recetaDto);

                if (_pobjOperationResult.Success == 0)
                {
                    MessageBox.Show(_pobjOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"btnGuardar_Click()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetIdCabecera()
        {
            throw new NotImplementedException();
        }

        private void txtCantidad_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!string.IsNullOrWhiteSpace(txtCantidad.Text))
                txtCantidad.Text = decimal.TryParse(txtCantidad.Text.Trim(), out d) ? d.ToString(CultureInfo.InvariantCulture) : "0";
        }

        private void txtMedicamento_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            TicketBL oTicketBL = new TicketBL();
            var f = new frmSearchMedicamento();
            var result = f.ShowDialog();
            if (result == DialogResult.OK)
            {
                var medicamento = f.MedicamentoSeleccionado;
                if (medicamento == null) return;
                txtMedicamento.Text = medicamento.NombreCompleto;
                txtMedicamento.Tag = medicamento.IdProductoDetalle;
                idUnidadProductiva = medicamento.IdLinea;
                txtUnidadProductiva.Text = medicamento.IdLinea;
                txtPrecio.Text = medicamento.PrecioVenta.ToString();
                var tienePlan = false;
                var resultplan = oTicketBL.TienePlan(_protocolId, txtUnidadProductiva.Text);
                if (resultplan.Count > 0) tienePlan = true;
                else tienePlan = false;

                if (tienePlan)
                {
                    if (resultplan[0].i_EsCoaseguro == 1)
                    {
                        #region Conexion SAM
                        ConexionSigesoft conectasam = new ConexionSigesoft();
                        conectasam.opensigesoft();
                        #endregion
                        var cadena1 = "select PR.r_MedicineDiscount, PR.r_DiscountExam, OO.v_Name, PR.v_CustomerOrganizationId from Organization OO inner join protocol PR On PR.v_AseguradoraOrganizationId = OO.v_OrganizationId where PR.v_ProtocolId ='" + _protocolId + "'";
                        SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                        SqlDataReader lector = comando.ExecuteReader();
                        string eps = "";
                        string GananciaInsumo = "";
                        while (lector.Read())
                        {
                            eps = lector.GetValue(0).ToString();
                            GananciaInsumo = lector.GetValue(1).ToString();
                        }
                        lector.Close();
                        conectasam.closesigesoft();

                        if (medicamento.i_Insumo == 0)
                        {
                            //calculo nuevo precio
                            txtPPS.Text = medicamento.d_PrecioMayorista.ToString();
                            txtDctoEPS.Text = eps;
                            decimal nuevoPrecio = decimal.Parse(txtPPS.Text) -
                                                  ((decimal.Parse(eps) * decimal.Parse(txtPPS.Text)) / 100);
                            txtNuevoPrecio.Text = nuevoPrecio.ToString();
                        }
                        else
                        {
                            txtPPS.Text = "Precio Costo";
                            txtDctoEPS.Text = "Aumento %";
                            if (GananciaInsumo != "0")
                            {
                                txtPPS.Text = medicamento.d_PrecioCosto.ToString();
                                txtDctoEPS.Text = GananciaInsumo;
                                decimal nuevoPrecio = decimal.Round((decimal.Parse(txtPPS.Text) +
                                                                     ((decimal.Parse(GananciaInsumo) *
                                                                       decimal.Parse(txtPPS.Text)) / 100)) *
                                                                    decimal.Parse("1.18"), 2);
                                txtNuevoPrecio.Text = nuevoPrecio.ToString();
                            }
                        }

                    }

                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }       

    }
}
