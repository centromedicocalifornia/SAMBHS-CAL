using SAMBHS.Common.BE;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI;
using Sigesoft.Node.WinClient.UI;
using System.IO;
using System.Drawing.Imaging;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmPacient : Form
    {

        #region Delarations


        PacientBL _objBL = new PacientBL();

        //------------------------------------------------------------------------------------
        PacientBL _objPacientBL = new PacientBL();
        personCustom objpersonDto;

        private string _fileName;
        private string _filePath;

        string PacientId;
        string Mode;
        string NumberDocument;
        string _personId;

        #endregion

        #region Properties

        public byte[] FingerPrintTemplate { get; set; }

        public byte[] FingerPrintImage { get; set; }

        public byte[] RubricImage { get; set; }

        public string RubricImageText { get; set; }

        #endregion

        public frmPacient(string personId)
        {
            _personId = personId;
            InitializeComponent();
        }
        
        private void frmPacient_Load(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            this.BindGrid();

        }

        private void BindGrid()
        {
            if (_personId != null && _personId != "N")
            {

                OperationResult objOperationResult = new OperationResult();

                var Lista = _objBL.GetPacientsPagedAndFilteredByPErsonId(ref objOperationResult, 0, 99999, _personId);
                grdData.DataSource = Lista;
                if (grdData.Rows.Count > 0)
                {
                    txtFirstLastNameDocNumber.Text = Lista[0].v_DocNumber;
                    grdData.Rows[0].Selected = true;
                }
                _personId = null;
            }
            else
            {
                var objData = GetData(0, null, txtFirstLastNameDocNumber.Text);

                grdData.DataSource = objData;
                if (objData != null)
                {
                    lblRecordCount.Text = string.Format("Se encontraron {0} registros.", objData.Count());

                }

                if (grdData.Rows.Count > 0)
                {
                    grdData.Rows[0].Selected = true;
                }
            }
            grdData.DataBind();
        }

        private List<PacientList> GetData(int pintPageIndex, int? pintPageSize, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            string[] Apellidos = pstrFilterExpression.Split(' '); string apMat = ""; string apPat = ""; string nombre = "";
            int x = Apellidos.Count();
            List<PacientList> pacients = null;
            if (x == 1)
            {
                pacients = _objBL.GetPacientsPagedAndFiltered(ref objOperationResult, pintPageIndex, 99999, pstrFilterExpression);
            }
            else if (x == 2)
            {
                apPat = Apellidos[0]; apMat = Apellidos[1];
                pacients = _objBL.GetPacientsPagedAndFiltered_Apellidos(ref objOperationResult, pintPageIndex, 99999, pstrFilterExpression, apPat, apMat);

            }
            else if (x == 3)
            {
                apPat = Apellidos[0]; apMat = Apellidos[1]; nombre = Apellidos[2];
                pacients = _objBL.GetPacientsPagedAndFiltered_Apellidos_Nombre(ref objOperationResult, pintPageIndex, 99999, pstrFilterExpression, apPat, apMat, nombre);

            }
            else if (x >= 4)
            {
                MessageBox.Show("El criterio de búsqueda es de 3 palabras, no admite apellidos compuestos", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //return pacients;
            }



            if (objOperationResult.Success != 1)
            {
                MessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (pacients == null)
            {
                MessageBox.Show("No se encontraron resultados, busque así: ApPaterno ApMaterno Nombre", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pacients = _objBL.GetPacientsPagedAndFiltered(ref objOperationResult, pintPageIndex, 99999, "");
            }

            return pacients;

        }

        private void grdData_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            if (grdData.Selected.Rows.Count == 0)
                return;

            string strPacientId = grdData.Selected.Rows[0].Cells["v_PersonId"].Value.ToString();
            loadData(strPacientId, "");
            btnEditar.Enabled = true;
            btnNuevo.Enabled = true;
        }

        private void loadData(string strPacientId, string pstrMode)
        {

            Mode = pstrMode;
            PacientId = strPacientId;


            OperationResult objOperationResult = new OperationResult();


            PacientList objpacientDto = new PacientList();
            objpacientDto = _objPacientBL.GetPacient(ref objOperationResult, PacientId, null);
            pbPersonImage.Image = UtilsSigesoft.BytesArrayToImage(objpacientDto.b_Photo, pbPersonImage);
            txtDocNumber.Text = objpacientDto.v_DocNumber;
            txtAdress.Text = objpacientDto.v_AdressLocation;
            txtCurrentOccupation.Text = objpacientDto.v_CurrentOccupation;
            txtEmail.Text = objpacientDto.v_Mail;
            txtFechNac.Text = objpacientDto.d_Birthdate.Value.ToShortDateString();
            txtBlood.Text = objpacientDto.GrupoSanguineo + " " + objpacientDto.FactorSanguineo;
        }

        private void LoadFile(string pfilePath)
        {
            Image img = pbPersonImage.Image;

            // Destruyo la posible imagen existente en el control
            //
            if (img != null)
            {
                img.Dispose();
            }

            using (FileStream fs = new FileStream(pfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Image original = Image.FromStream(fs);
                pbPersonImage.Image = original;

            }
        }


        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var frm = new frmCrudPacient("New", "");
            frm.ShowDialog();
            BindGrid();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.Selected.Rows.Count == 0)
            {
                MessageBox.Show("Seleccione una fila para continuar", "VALIDACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            string pacientId = grdData.Selected.Rows[0].Cells["v_PersonId"].Value.ToString();
            var frm = new frmCrudPacient("Edit", pacientId);
            frm.ShowDialog();
            BindGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFirstLastNameDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {            
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.BindGrid();
            }            
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.Rows == null)
            {
                cmPacient.Items["verCambiosToolStripMenuItem"].Enabled = false;
            }
            else if (grdData.Rows.Count == 0)
            {
                cmPacient.Items["verCambiosToolStripMenuItem"].Enabled = false;
            }
            else
            {
                cmPacient.Items["verCambiosToolStripMenuItem"].Enabled = true;
            }
        }

        private void verCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pacientId = grdData.Selected.Rows[0].Cells["v_PersonId"].Value.ToString();
            string commentary = new PacientBL().GetComentaryUpdateByPersonId(pacientId);
            if (commentary == "" || commentary == null)
            {
                MessageBox.Show("Aún no se han realizado cambios.", "AVISO", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            var frm = new frmViewChanges(commentary);
            frm.ShowDialog();
        }

    }
}
