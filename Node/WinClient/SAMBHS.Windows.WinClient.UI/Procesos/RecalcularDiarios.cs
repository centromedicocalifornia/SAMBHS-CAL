using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class RecalcularDiarios : Form
    {
        LiquidacionCompraBL _objLiquidacionCompraBL = new LiquidacionCompraBL();
        public RecalcularDiarios()
        {
            InitializeComponent();
        }

        private void RecalcularDiarios_Load(object sender, EventArgs e)
        {
            lblPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            _objLiquidacionCompraBL.RegenerarDiariosLiquidacionCompras(ref objOperationResult, lblPeriodo.Text.Trim(), DateTime.Parse(dtpFechaRegistroDe.Text + " 23:59"), Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso finalizado satisfactoriamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                UltraMessageBox.Show("Hubo un Error ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            //if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            //{
            //    if (UltraMessageBox.Show("¿Seguro de regenerar Diarios?", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            //    {
            //        Globals.ProgressbarStatus.i_Progress = 1;
            //        Globals.ProgressbarStatus.i_TotalProgress = 1;
            //        Globals.ProgressbarStatus.b_Cancelado = false;
            //        bwkProcesoBL.RunWorkerAsync();
            //        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
            //    }
            //}
        }


        //private void bwkProcesoBL_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    MovimientoBL _objMovimientoBL = new MovimientoBL();
        //    _objMovimientoBL.RecalcularStock(ref objOperationResult, IDAlmacen, Globals.ClientSession.i_Periodo.Value, dtpFechaInicio.Value.Month, dtpFechaInicio.Value.Day);

        //    if (objOperationResult.Success == 1)
        //    {
        //        UltraMessageBox.Show("Proceso terminado correctamente!");
        //    }
        //    else
        //    {
        //        Globals.ProgressbarStatus.b_Cancelado = true;
        //        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}



    }

}
