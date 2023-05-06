using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmRecetaMedica : Form
    {
        private string _serviceId;
        private OperationResult _pobjOperationResult = new OperationResult();
        private RecetaBL _objRecetaBl = new RecetaBL();
        private List<DiagnosticRepositoryList> _listDiagnosticRepositoryLists = new List<DiagnosticRepositoryList>();
        private string _protocolId;
        private string ReceiptId;

        public frmRecetaMedica(List<DiagnosticRepositoryList> ListaDX, string serviceId, string protocolId, string v_ReceiptId)
        {
            if (ListaDX == null)
            {
                ListaDX = new List<DiagnosticRepositoryList>();
            }
            _serviceId = serviceId;
            _protocolId = protocolId;
            _listDiagnosticRepositoryLists = ListaDX;
            ReceiptId = v_ReceiptId;
            InitializeComponent();
        }

        private void GetData(List<DiagnosticRepositoryList> ListaDX)
        {

            try
            {
                ListaDX.ForEach(l => l.RecipeDetail = new List<Receta>());
                //var data = _objRecetaBl.GetHierarchycalData(ref _pobjOperationResult, ListaDX);
                var data = _objRecetaBl.GetHierarchycalData_(ref _pobjOperationResult, ReceiptId, ListaDX);
                //if (data.Any())
                //{
                    var previousIndex = grdTotalDiagnosticos.ActiveRow != null ? grdTotalDiagnosticos.ActiveRow.Index : 0;
                    grdTotalDiagnosticos.DataSource = data;
                    grdTotalDiagnosticos.Rows.Refresh(RefreshRow.ReloadData);
                    grdTotalDiagnosticos.Rows[previousIndex].Activate();
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"GetData()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MedicinaReceta(string serviceId)
        {
            var data = _objRecetaBl.GetHierarchycalData(ref _pobjOperationResult, _listDiagnosticRepositoryLists);

            if (data.Any())
            {
                var previousIndex = grdTotalDiagnosticos.ActiveRow != null ? grdTotalDiagnosticos.ActiveRow.Index : 0;
                grdTotalDiagnosticos.DataSource = data;
                grdTotalDiagnosticos.Rows.Refresh(RefreshRow.ReloadData);
                grdTotalDiagnosticos.Rows[previousIndex].Activate();
            }
        }

        private void frmRecetaMedica_Load(object sender, EventArgs e)
        {
            GetData(_listDiagnosticRepositoryLists);
            lblRecetamEd.Text = "Receta N°: " + ReceiptId;
            #region Conexion SIGESOFT verificar la unidad productiva del componente
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 = "select re.i_MedicoId " +
                          "from receipHeader re " +
                          "where re.v_ReceipId='" + ReceiptId + "' ";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            string medicotratante = "";
            while (lector.Read())
            {
                medicotratante = lector.GetValue(0).ToString();
                break;
            }
            lector.Close();
            conectasam.closesigesoft();
            #endregion

            if (medicotratante == "" || medicotratante == null)
            {
                cboMedicoTratante.Enabled = true;
                cboMedicoTratante.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);
                cboMedicoTratante.SelectedValue = "-1";
            }
            else
            {
                cboMedicoTratante.Enabled = true;
                cboMedicoTratante.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);
                cboMedicoTratante.SelectedValue = medicotratante;
            }
            
        }

        private void grdTotalDiagnosticos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                if (e.Cell == null || e.Cell.Row.Cells["v_DiagnosticRepositoryId"].Value == null) return;
                var diagnosticRepositoryId = e.Cell.Row.Cells["v_DiagnosticRepositoryId"].Value.ToString();
                #region Conexion SIGESOFT verificar la unidad productiva del componente
                ConexionSigesoft conectasam = new ConexionSigesoft();
                conectasam.opensigesoft();
                var cadena1 = "select CP.v_IdUnidadProductiva " +
                              "from diagnosticrepository DR " +
                              "inner join component CP on DR.v_ComponentId=CP.v_ComponentId " +
                              "where DR.v_DiagnosticRepositoryId='" + diagnosticRepositoryId + "' ";
                SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                SqlDataReader lector = comando.ExecuteReader();
                string LineId = "";
                while (lector.Read())
                {
                    LineId = lector.GetValue(0).ToString();
                }
                lector.Close();
                conectasam.closesigesoft();
                #endregion
                switch (e.Cell.Column.Key)
                {
                    case "_AddRecipe":
                        {
                            var f = new frmAddRecipe((int)Enums.ActionForm.Add, diagnosticRepositoryId, ReceiptId, _protocolId, _serviceId, LineId, cboMedicoTratante.Text, int.Parse(cboMedicoTratante.SelectedValue.ToString())) { StartPosition = FormStartPosition.CenterScreen };
                            f.ShowDialog();
                            GetData(_listDiagnosticRepositoryLists);
                        }
                        break;

                    case "_EditRecipe":
                        {
                            var recipeId = int.Parse(e.Cell.Row.Cells["i_IdReceta"].Value.ToString());
                            var f = new frmAddRecipe((int)Enums.ActionForm.Edit, diagnosticRepositoryId, ReceiptId, _protocolId, _serviceId, LineId, cboMedicoTratante.Text, int.Parse(cboMedicoTratante.SelectedValue.ToString())) { StartPosition = FormStartPosition.CenterScreen };
                            f.ShowDialog();
                            GetData(_listDiagnosticRepositoryLists);
                        }
                        break;

                    case "_DeleteRecipe":
                        {
                            _pobjOperationResult = new OperationResult();
                            var recipeId = int.Parse(e.Cell.Row.Cells["i_IdReceta"].Value.ToString());
                            var msg = MessageBox.Show(@"¿Seguro de eliminar esta receta?", @"Confirmación",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (msg == DialogResult.No) return;
                            _objRecetaBl.DeleteRecipe(ref _pobjOperationResult, recipeId);
                            if (_pobjOperationResult.Success == 0)
                            {
                                MessageBox.Show(_pobjOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }

                            GetData(_listDiagnosticRepositoryLists);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"grdTotalDiagnosticos_ClickCellButton()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var f = new frmConfirmarDespacho(_serviceId);
            f.ShowDialog();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var diagnosticRepositoryId = grdTotalDiagnosticos.Selected.Rows[0].Cells["v_DiagnosticRepositoryId"].Value.ToString();

            var recomendaciones = string.Join("\n-", _listDiagnosticRepositoryLists.Where(o => !string.IsNullOrWhiteSpace(o.v_RecomendationsName) && o.v_DiagnosticRepositoryId == diagnosticRepositoryId).Select(p => p.v_RecomendationsName).Distinct()).Trim();
            var restricciones = string.Join("\n-", _listDiagnosticRepositoryLists.Where(o => !string.IsNullOrWhiteSpace(o.v_RestrictionsName) && o.v_DiagnosticRepositoryId == diagnosticRepositoryId).Select(p => p.v_RestrictionsName).Distinct()).Trim();
            if (grdTotalDiagnosticos.Selected.Rows.Count == 0)
            {

                return;
            }
            else
            {

                var f = new frmReporteReceta(_serviceId, recomendaciones, restricciones, diagnosticRepositoryId);
                f.ShowDialog();
            }
        }

        private void btnDespachado_Click(object sender, EventArgs e)
        {
            int medico = int.Parse(cboMedicoTratante.SelectedValue.ToString());
            string medicoNombre = cboMedicoTratante.Text;
            if (medico == -1)
            {
                MessageBox.Show("Seleccione el médico tratante para finalizar la receta.",
                    "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                #region Conexion SAM
                ConexionSigesoft conectasam = new ConexionSigesoft();
                conectasam.opensigesoft();
                #endregion
                var cadena1 = "update receta set i_Lleva = 1, v_MedicoTratante = " + medico + " where v_ReceipId='" + ReceiptId + "'";
                SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                SqlDataReader lector = comando.ExecuteReader();
                lector.Close();

                var cadena2 = "update receipHeader set i_MedicoId = " + medico + ", v_MedicoName = '" + medicoNombre + "' where v_ReceipId='" + ReceiptId + "'";
                SqlCommand comando2 = new SqlCommand(cadena2, connection: conectasam.conectarsigesoft);
                SqlDataReader lector2 = comando2.ExecuteReader();
                lector.Close();

                UltraMessageBox.Show("Se ha despachado correctamente.", "Información");
                GetData(_listDiagnosticRepositoryLists);
                cboMedicoTratante.Enabled = false;
                return;
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        
    }
}
