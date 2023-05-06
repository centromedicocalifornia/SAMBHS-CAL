using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRecipeManager : Form
    {
        private string ServiceId;
        private string PacienteName;
        private string ProtocolId;
        string recetaId;
        RecipeManagerDAL recipeManagerDal = new RecipeManagerDAL();
        AgendaBl agbl_ = new AgendaBl();
        public frmRecipeManager(string v_ServiceId, string v_Paciente, string v_ProtocolId)
        {
            ServiceId = v_ServiceId;
            PacienteName = v_Paciente;
            ProtocolId = v_ProtocolId;
            InitializeComponent();
        }

        private void frmRecipeManager_Load(object sender, EventArgs e)
        {
            this.Text = "Manager de recetas. Paciente: "+PacienteName;
            this.BindGrid();
        }
        private void BindGrid()
        {
            List<Receta> recetas = new List<Receta>();
            recetas = recipeManagerDal.GetRecetas_1(ServiceId);

            recetas = recetas.OrderBy(p => p.v_ReceiptId).ToList();

            grdRecetas.DataSource = recetas;
            lblRecetas.Text = string.Format("Se encontraron {0} registros.", recetas.Count());
        }

       

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            var ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, ServiceId);
            
            if (ListDiseases.Count == 0)
            {
                var DialogResult =  MessageBox.Show("El paciente no cuenta con diagnosticos, se procederá a agregar un diagnostico genérico.", "AVISO", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (DialogResult == DialogResult.OK)
                {
                    string diaxrepositoryiD = new ServiceBL().AddGenericDiseasesByServiceId(ServiceId, Globals.ClientSession.GetAsList());
                    ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, ServiceId);
                    
                    int secuentialId = Utilidades.GetNextSecuentialId(9, 600);
                    recetaId = Utilidades.GetNewId(9, secuentialId, "RC");

                    new ServiceBL().AddRecipeNEwsByServiceId(recetaId, ServiceId, 0, 0, Globals.ClientSession.GetAsList(), diaxrepositoryiD);

                    this.BindGrid();
                }
                else
                {
                    return;
                }
            }
            else
            {
                var DialogResult = MessageBox.Show("¿Desea agregar nueva receta con Dx genérico?", "AVISO", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (DialogResult == DialogResult.OK)
                {
                    string diaxrepositoryiD = new ServiceBL().AddGenericDiseasesByServiceId(ServiceId, Globals.ClientSession.GetAsList());
                    ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, ServiceId);

                    int secuentialId = Utilidades.GetNextSecuentialId(9, 600);
                    recetaId = Utilidades.GetNewId(9, secuentialId, "RC");

                    new ServiceBL().AddRecipeNEwsByServiceId(recetaId, ServiceId, 0, 0, Globals.ClientSession.GetAsList(), diaxrepositoryiD);

                    this.BindGrid();
                }
                else
                {
                    return;
                }
            }
            frmRecetaMedica frm = new frmRecetaMedica(ListDiseases, ServiceId, ProtocolId, recetaId);
            frm.ShowDialog();
        }

        private void grdRecetas_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            var serviceId = grdRecetas.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
            var recetaId = grdRecetas.Selected.Rows[0].Cells["v_ReceiptId"].Value.ToString();
            var ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, serviceId);
            if (ListDiseases == null)
            {
                MessageBox.Show("Sucedió un error, por favor vuelva a intentarlo.", "ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (ListDiseases.Count == 0)
            {
                var DialogResult = MessageBox.Show("El paciente no cuenta con diagnosticos, se procederá a agregar un diagnostico genérico.", "AVISO", MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Information);
                if (DialogResult == DialogResult.OK)
                {
                    new ServiceBL().AddGenericDiseasesByServiceId(serviceId, Globals.ClientSession.GetAsList());
                    ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, serviceId);
                }
                else
                {
                    return;
                }

            }
            var frm = new frmRecetaMedica(ListDiseases, serviceId, ProtocolId, recetaId);
            frm.ShowDialog();
        }

        private void grdRecetas_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            try
            {
                string recetaId = "";
                if (grdRecetas.Selected.Rows.Count == 1)
                {
                    recetaId = grdRecetas.Selected.Rows[0].Cells["v_ReceiptId"].Value.ToString();
                }
                if (recetaId != "- -- -")
                {
                    OperationResult objOperationResult = new OperationResult();
                    var data = new RecetaBL().Detallerecetas(ref objOperationResult, recetaId);

                    ultraGrid1.DataSource = data;

                    lblCantidadDeMedicinas.Text = string.Format("Se encontraron {0} registros.", data.Count());
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message, @"GetData()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            var serviceId = grdRecetas.Selected.Rows[0].Cells["v_ServiceId"].Value.ToString();
            var recetaId = grdRecetas.Selected.Rows[0].Cells["v_ReceiptId"].Value.ToString();
            var ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, serviceId);
            if (ListDiseases == null)
            {
                MessageBox.Show("Sucedió un error, por favor vuelva a intentarlo.", "ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (ListDiseases.Count == 0)
            {
                var DialogResult = MessageBox.Show("El paciente no cuenta con diagnosticos, se procederá a agregar un diagnostico genérico.", "AVISO", MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Information);
                if (DialogResult == DialogResult.OK)
                {
                    new ServiceBL().AddGenericDiseasesByServiceId(serviceId, Globals.ClientSession.GetAsList());
                    ListDiseases = new ServiceBL().GetServiceComponentDisgnosticsByServiceId(ref objOperationResult, serviceId);
                }
                else
                {
                    return;
                }

            }
            var frm = new frmRecetaMedica(ListDiseases, serviceId, ProtocolId, recetaId);
            frm.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var recetaId = grdRecetas.Selected.Rows[0].Cells["v_ReceiptId"].Value.ToString();
            var DialogResult = MessageBox.Show("¿Desea eliminar la receta " + recetaId + "?", "AVISO", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
            if (DialogResult == DialogResult.OK)
            {
                #region Conexion SAM
                ConexionSigesoft conectasam = new ConexionSigesoft();
                conectasam.opensigesoft();
                #endregion
                var cadena1 = "update receipHeader set i_IsDeleted = 1 where v_ReceipId='" + recetaId + "'";
                SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                SqlDataReader lector = comando.ExecuteReader();
                lector.Close();

                UltraMessageBox.Show("Se ha eliminado correctamente.", "Información");
                this.BindGrid();
            }
            else
            {
                return;
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
