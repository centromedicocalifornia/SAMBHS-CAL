using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigrarSaldosContables_Administrativos : Form
    {
        public frmMigrarSaldosContables_Administrativos(string N)
        {
            InitializeComponent();
        }

        private void frmMigrarSaldosContables_Administrativos_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            Utils.Windows.MostrarOcultarFiltrosGrilla(ultraGrid1);
        }
        
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            Consultar();
        }

        private void Consultar()
        {
            try
            {
                var objOperationResult = new OperationResult();
                var result =
                    new SaldoContableBL().ObtenerComparacionEntreSaldosContablesYAdministrativos(ref objOperationResult,
                        Globals.ClientSession.i_Periodo.ToString());

                if (objOperationResult.Success == 1)
                    ultraGrid1.DataSource = result;
                else
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                        "Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en el método Consultar()");
            }
           
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            if (!ultraGrid1.Rows.Any()) return;
            var resp = MessageBox.Show("¿Seguro de Aplicar?, Los cambios no se pueden deshacer.", "Advertencia",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.No) return;

            ultraButton2.Enabled = false;
            var objOperationResult = new OperationResult();
            var listaAplicar = new List<ComparacionSaldoContableAdministrativo>();
            ultraGrid1.Rows.ToList().ForEach(f => listaAplicar.Add((ComparacionSaldoContableAdministrativo) f.ListObject));
            if (!listaAplicar.Any()) return;
            new SaldoContableBL().IgualarSaldosContablesAdministrativos(ref objOperationResult, listaAplicar, Globals.ClientSession.i_Periodo.ToString());
            ultraButton2.Enabled = true;
            if (objOperationResult.Success == 0)
            {
                var message = new StringBuilder();
                message.AppendLine(objOperationResult.ErrorMessage);
                if (!string.IsNullOrWhiteSpace(objOperationResult.ExceptionMessage))
                    message.AppendLine(objOperationResult.ExceptionMessage);
                message.AppendLine(objOperationResult.AdditionalInformation);

                MessageBox.Show(message.ToString(), "Error en la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Operación completada con éxito!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
