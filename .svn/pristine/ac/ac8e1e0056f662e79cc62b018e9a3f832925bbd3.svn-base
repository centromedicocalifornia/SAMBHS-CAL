using SAMBHS.Almacen.BL;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using SAMBHS.Compra.BL;
using SAMBHS.Venta.BL;

using System.Globalization;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRecalculoStock : Form
    {
        int IDAlmacen;
        private readonly ReprocesarSalidasWorker _workerVentas = new ReprocesarSalidasWorker();
        private readonly ReprocesarIngresosWorker _workerCompras = new ReprocesarIngresosWorker();
        public List<string> TempFiltroArticulos = new List<string>();

        public frmRecalculoStock(string N)
        {
            InitializeComponent();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (UltraMessageBox.Show("¿Seguro de recalcular los stocks?", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                bwkProcesoBL.RunWorkerAsync();
                btnRecalcularStock.Enabled = false;
                chkAplicarFiltro.Enabled = false;
                btnBuscarFiltro.Enabled = false;
            }
        }

        private void frmRecalculoStock_Load(object sender, EventArgs e)
        {
            this.BackColor = ultraGroupBox1.BackColor = ultraGroupBox2.BackColor = new GlobalFormColors().FormColor;
            label1.Text = lblPeriodo.Text = string.Format("Periodo: {0}", Globals.ClientSession.i_Periodo);
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadDropDownList(cboAlmacen, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboAll(ref objOperationResult, null), DropDownListAction.All);
            IDAlmacen = int.Parse(cboAlmacen.SelectedValue.ToString());
            var formParent = (this.ParentForm as frmMaster);
            if (formParent != null) btnRecalcularStock.Enabled = !formParent.IsBussy();
            cboTipo.SelectedIndex = 0;
            lblPeriodoTransferencias.Text = string.Format("Periodo: {0}", Globals.ClientSession.i_Periodo);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenTransferencias, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboAll(ref objOperationResult, null), DropDownListAction.Select);
            LlenarComboMeses();
            cboAlmacenTransferencias.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString();
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenSeparacion, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboAll(ref objOperationResult, null), DropDownListAction.Select);
            cboAlmacenSeparacion.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString();
            lblPeriodoSeparacion.Text ="Periodo : "+ Globals.ClientSession.i_Periodo.ToString();
        }

        void LlenarComboMeses()
        {
        }

        private void bwkProcesoBL_DoWork(object sender, DoWorkEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            new StockBl().RecalcularStock(ref objOperationResult, Globals.ClientSession.i_Periodo.ToString(), IDAlmacen, true);
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!");
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDAlmacen = int.Parse(cboAlmacen.SelectedValue.ToString());
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var msg =
                MessageBox.Show(
                    @"¿Seguro de Continuar? Este proceso se ejecuta sólo en casos de contingencia y su ejecución demanda parar los procesos de producción durante su ejecución.",
                    @"Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (msg == DialogResult.No) return;

            pictureBox1.Image = Resource.loadingfinal1;
            chkSeguirConRecalculo.Enabled = false;
            ultraButton2.Enabled = false;
            btnRecalcularStock.Enabled = false;
            cboTipo.Enabled = false;
            chkAplicarFiltro.Enabled = false;
            btnBuscarFiltro.Enabled = false;
            var st = (byte)cboTipo.SelectedItem.DataValue;
            if (st == 0 || st == 1)
            {
                _workerCompras.Periodo = Globals.ClientSession.i_Periodo.ToString();
                _workerCompras.FiltroProductos = TempFiltroArticulos;
                _workerCompras.OnErrorEvent += workerCompras_OnErrorEvent;
                if (st == 0)
                    _workerCompras.OnFinalizadoEvent += workerCompras_OnFinalizadoEvent;
                else
                    _workerCompras.OnFinalizadoEvent += WorkerVentasOnFinalizadoEvent;
                    _workerCompras.OnProcesarEvent += workerCompras_OnProcesarEvent;
            }
            if (st == 0 || st == 2)
            {
                _workerVentas.OnErrorEvent += WorkerVentasOnErrorEvent;
                _workerVentas.OnProcesarEvent += WorkerVentasOnProcesarEvent;
                _workerVentas.Periodo = Globals.ClientSession.i_Periodo.ToString();
                _workerVentas.FiltroProductos = TempFiltroArticulos;
                _workerVentas.OnFinalizadoEvent += WorkerVentasOnFinalizadoEvent;
            }

            if (st == 0 || st == 1)
                _workerCompras.Comenzar();
            else
                _workerVentas.Comenzar();
        }

        void workerCompras_OnProcesarEvent(string pstrEstadoProceso)
        {
            if (!IsDisposed)
            {
                Invoke((MethodInvoker)(() => lblEstado.Text = pstrEstadoProceso));
            }
        }

        void workerCompras_OnFinalizadoEvent()
        {
            _workerVentas.Comenzar();
        }

        void workerCompras_OnErrorEvent(OperationResult objOperationResult)
        {
            pictureBox1.Image = Resource.cancel;
            lblEstado.Text = @"Error en el proceso!";
            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void WorkerVentasOnErrorEvent(OperationResult objOperationResult)
        {
            pictureBox1.Image = Resource.cancel;
            lblEstado.Text = @"Error en el proceso!";
            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void WorkerVentasOnProcesarEvent(string pstrEstadoProceso)
        {
            if (!IsDisposed)
            {
                Invoke((MethodInvoker)(() => lblEstado.Text = pstrEstadoProceso));
            }
        }

        private void WorkerVentasOnFinalizadoEvent()
        {
            if (!IsDisposed)
            {
                Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Image = Resource.accept;
                    lblEstado.Text = @"Todos los Procesos han sido Finalizados!";
                    if (chkSeguirConRecalculo.Checked)
                    {
                        bwkProcesoBL.RunWorkerAsync();
                        btnRecalcularStock.Enabled = false;
                        lblEstado.Text += @"...Recalculando Stock";
                    }
                    else
                        btnRecalcularStock.Enabled = true;

                });
            }
        }

        private void chkAplicarFiltro_CheckedChanged(object sender, EventArgs e)
        {
            btnBuscarFiltro.Enabled = chkAplicarFiltro.Checked;
            if (!chkAplicarFiltro.Checked)
                TempFiltroArticulos = new List<string>();
        }

        private void btnBuscarFiltro_Click(object sender, EventArgs e)
        {
            var f = new frmRecalculoStockFiltro(TempFiltroArticulos);
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                TempFiltroArticulos = f.FiltroNuevo;
            }
        }

        private void btnRegenerarTransferencias_Click(object sender, EventArgs e)
        {
            if (ValidarTransferencias.Validate(true, false).IsValid)
            {

                AlmacenBL _objAlmacenBL = new AlmacenBL();
                OperationResult objOperationResult = new OperationResult();
                var NombreAlmacen = _objAlmacenBL.ObtenerAlmacen(ref objOperationResult, int.Parse(cboAlmacenTransferencias.Value.ToString())).v_Nombre;
                if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
                {
                    if (UltraMessageBox.Show("¿Seguro de Actualizar las transferencias de  " + NombreAlmacen, "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Globals.ProgressbarStatus.i_Progress = 1;
                        Globals.ProgressbarStatus.i_TotalProgress = 1;
                        Globals.ProgressbarStatus.b_Cancelado = false;


                        bwkProcesoTransferencias.RunWorkerAsync();
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                    }
                }
            }
        }





        private void bwkProcesoTransferencias_DoWork(object sender, DoWorkEventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            MovimientoBL _objMovimientoBL = new MovimientoBL();
            string Error = "";
            _objMovimientoBL.RegenerarTransferencias(ref objOperationResult, ref  Error, int.Parse(cboAlmacenTransferencias.Value.ToString()), int.Parse(cboMesesTransferencia.Value.ToString()), chkAplicarMes.Checked);

            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al regenerar Transferencias  en " + Error + " Adicional :\n\n" + objOperationResult.ErrorMessage + "\n\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void chkAplicarMes_CheckedChanged(object sender, EventArgs e)
        {
            cboMesesTransferencia.Enabled = chkAplicarMes.Checked;

        }

        private void btnRegenerarSeparacion_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            new SeparacionBl().RecalcularSeparacion(ref objOperationResult, Globals.ClientSession.i_Periodo.ToString(), int.Parse(cboAlmacenSeparacion.Value.ToString()), true);
            //  _objPedidoLBL.RecalcularSeparacionProductoAlmacen(ref objOperationResult, int.Parse(cboAlmacenSeparacion.Value.ToString()), Globals.ClientSession.GetAsList(), Globals.ClientSession.i_Periodo.ToString());
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al realizar recálculo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        //private void bwkProcesoSeparacion_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    MovimientoBL _objMovimientoBL = new MovimientoBL();
        //    PedidoBL _objPedidoLBL = new PedidoBL();

        //    new SeparacionBl().RecalcularSeparacion(ref objOperationResult,Globals.ClientSession.i_Periodo.ToString (), int.Parse ( cboAlmacenSeparacion.Value.ToString ()));
        //  //  _objPedidoLBL.RecalcularSeparacionProductoAlmacen(ref objOperationResult, int.Parse(cboAlmacenSeparacion.Value.ToString()), Globals.ClientSession.GetAsList(), Globals.ClientSession.i_Periodo.ToString());
        //    if (objOperationResult.Success == 1)
        //    {
        //        UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        //Globals.ProgressbarStatus.b_Cancelado = true;
        //    }
        //    else
        //    {
        //        //Globals.ProgressbarStatus.b_Cancelado = true;
        //        UltraMessageBox.Show("Ocurrió un Error al realizar recálculo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void btnDesactivarProductos_Click(object sender, EventArgs e)
        {
            if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            {
                if (UltraMessageBox.Show("¿Seguro de Desactivar los Productos?", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.ProgressbarStatus.i_Progress = 1;
                    Globals.ProgressbarStatus.i_TotalProgress = 1;
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    bwkDesactivarProductos.RunWorkerAsync();
                    (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                }
            }
        }


        private void bwkProcesoDesactivarProductos_DoWork(object sender, DoWorkEventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            MovimientoBL _objMovimientoBL = new MovimientoBL();
            string Error = "";
            new AlmacenBL().DesactivarProductosAlmacen(ref objOperationResult, DateTime.Parse(dtpFechaRegistroDesactivarProductos.Text + " 23:59"), Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al regenerar Transferencias  en " + Error + " Adicional :\n\n" + objOperationResult.ErrorMessage + "\n\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void ultraTabControl1_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {

        }
    }
}
