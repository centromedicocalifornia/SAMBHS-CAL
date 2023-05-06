using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmReprocesarSaldosContables : Form
    {
        string pstrOrigen;
        int Periodo, iMes;
        public frmReprocesarSaldosContables(string N)
        {
            InitializeComponent();
        }

        private void frmReprocesarSaldosContables_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            txtPeriodo.MaxValue = int.Parse(DateTime.Now.Year.ToString());
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            LlenarCombo();
            cbMeses.SelectedIndex = 0;
        }

        private void LlenarCombo()
        {
            pstrOrigen = "Load";
            String[] Meses = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            String[] MesesMayuscula = new String[12];
            int i = 0;
            foreach (var s in Meses)
            {
                if (i < 12)
                {
                    try
                    {
                        string a = s.ToUpper();
                        MesesMayuscula[i] = a;
                        i = i + 1;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

            }

            cbMeses.Items.AddRange(MesesMayuscula);
            cbMeses.SelectedText = "--Seleccionar--";
            pstrOrigen = string.Empty;
        }

        private int Mes(string pstrMes)
        {
            switch (pstrMes)
            {
                case "ENERO": return 1;
                case "FEBRERO": return 2;
                case "MARZO": return 3;
                case "ABRIL": return 4;
                case "MAYO": return 5;
                case "JUNIO": return 6;
                case "JULIO": return 7;
                case "AGOSTO": return 8;
                case "SEPTIEMBRE": return 9;
                case "OCTUMBRE": return 10;
                case "NOVIEMBRE": return 11;
                case "DICIEMBRE": return 12;

            }
            return 0;
        }

        private string Mes(int mes)
        {
            switch (mes)
            {
                case 1: return "ENERO";
                case 2: return "FEBRERO";
                case 3: return "MARZO";
                case 4: return "ABRIL";
                case 5: return "MAYO";
                case 6: return "JUNIO";
                case 7: return "JULIO";
                case 8: return "AGOSTO";
                case 9: return "SEPTIEMBRE";
                case 10: return "OCTUMBRE";
                case 11: return "NOVIEMBRE";
                case 12: return "DICIEMBRE";


            }
            return string.Empty;
        }

        private void ultraCheckEditor1_CheckedChanged(object sender, EventArgs e)
        {
            if (ultraCheckEditor1.Checked)
            {
                cbMeses.Enabled = true;
            }
            else
            {
                cbMeses.Enabled = false;
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            {
                bool ConfirmacionAceptada = false;
                if (!ultraCheckEditor1.Checked)
                {
                    if (UltraMessageBox.Show(string.Format("¿Seguro de regenerar los saldos del año {0}?", txtPeriodo.Text), "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Si el proceso falla, por favor contactar a sistemas.") == System.Windows.Forms.DialogResult.Yes)
                    {
                        ConfirmacionAceptada = true;
                    }
                }
                else
                {
                    if (UltraMessageBox.Show(string.Format("¿Seguro de regenerar los saldos del mes de {1} del {0}?", txtPeriodo.Text, cbMeses.Text), "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Si el proceso falla, por favor contactar a sistemas.") == System.Windows.Forms.DialogResult.Yes)
                    {
                        ConfirmacionAceptada = true;
                    }
                }

                if (ConfirmacionAceptada)
                {
                    Periodo = int.Parse(txtPeriodo.Text);
                    iMes = ultraCheckEditor1.Checked ? Mes(cbMeses.Text) : 0;
                    btnGenerar.Enabled = false;
                    Globals.ProgressbarStatus.i_Progress = 1;
                    Globals.ProgressbarStatus.i_TotalProgress = 1;
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    bwProcesoBL.RunWorkerAsync();
                    (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                }
            }
            else
            {
                UltraMessageBox.Show("Hay otro proceso corriendo de fondo, por favor espere que termine.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
            OperationResult objOperationResult = new OperationResult();

            new SaldoContableBL().ReprocesarSaldosContables(ref objOperationResult, Periodo, iMes);

            if (objOperationResult.Success == 0)
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                UltraMessageBox.Show("Saldos Contables Reprocesados Correctamente.");
            }
        }

        private void bwProcesoBL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnGenerar.Enabled = true;
        }
    }
}
