using SAMBHS.Almacen.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRecalcularIngresosImportacion : Form
    {
        int IDAlmacen;
        DiarioBL _objDiarioBL = new DiarioBL();
        public frmRecalcularIngresosImportacion(string p)
        {
            InitializeComponent();
        }

        private void frmRecalcularIngresosImportacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            // Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboAll(ref objOperationResult, null), DropDownListAction.All);
            // cboAlmacen.Value = "-1";
            //IDAlmacen = cboAlmacen.Value == null ? -1 : int.Parse(cboAlmacen.Value.ToString());
        }

        private void btnRecalcularSeparacion_Click(object sender, EventArgs e)
        {

            if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            {
                if (rbRecalcularKardex.Checked)
                {
                    if (UltraMessageBox.Show("¿Seguro de Recalcular Importes Kardex?", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        
                        bwkProcesoBL.RunWorkerAsync();
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                    }
                }
                if (rbRegenerarDiarios.Checked)
                {



                    if (UltraMessageBox.Show("¿Seguro de  Regenerar Diarios?", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        
                        bwkProcesoBL.RunWorkerAsync();
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                    }

                }
            }

        }

        private void bwkProcesoBL_DoWork(object sender, DoWorkEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            MovimientoBL _objMovimientoBL = new MovimientoBL();
            PedidoBL _objPedidoLBL = new PedidoBL();
            ImportacionBL _objImportacionBL = new ImportacionBL();

            if (rbRegenerarDiarios.Checked)
            {

                _objDiarioBL.RegeneraDiariosImportacion(ref objOperationResult);

                if (objOperationResult.Success == 1)
                {
                    
                    UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    
                }
                else
                {
                    //Globals.ProgressbarStatus.b_Cancelado = true;
                    UltraMessageBox.Show("Ocurrió un Error al regenerar Diarios " + " Adicional :\n\n" + objOperationResult.ErrorMessage + "\n\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


                }
            }
            if (rbRecalcularKardex.Checked)
            {
                _objImportacionBL.ActualizaNotadeIngresoDesdeImportacion(ref objOperationResult);
                if (objOperationResult.Success == 1)
                {
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Globals.ProgressbarStatus.b_Cancelado = true;
                    UltraMessageBox.Show("Ocurrió un Error al regenerar Ingresos Almacén  " + " Adicional :\n\n" + objOperationResult.ErrorMessage + "\n\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


                }
            }

        }

        private void cboAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            // IDAlmacen = int.Parse(cboAlmacen.Value.ToString());
        }
    }
}
