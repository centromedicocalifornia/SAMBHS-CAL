using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Requerimientos.NBS;
using Infragistics.Win.UltraWinGrid;
using System.Threading;
using System.Threading.Tasks;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmBandejaOrdenTrabajo : Form
    {
        public string _strFilterExpression = null;
        OrdenTrabajoBL _objOrdenTrabajoBL = new OrdenTrabajoBL();
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        public string v_Id = "";
        public string v_IdCliente = "";
       
        public frmBandejaOrdenTrabajo(string P)
        {
            InitializeComponent();
        }

        private void frmBandejaOrdenTrabajo_Load(object sender, EventArgs e)
        {
           // BackColor= Color.White;
            this.BackColor = new GlobalFormColors().FormColor;
           // Text = @"Buscar Órdenes de Trabajo";
           //ultraGrid1.DataSource = new List<DatosGrilla>
           // {
           //     new DatosGrilla{ Cliente = "QUIROZ COSME EDUARDO", Fecha = DateTime.Today, Importe = 78, OrdenTrabajo = "00004858", Responsable = "[USUARIO DEL SISTEMA]", Procesar = true},
           //     new DatosGrilla{ Cliente = "ÁVILA MARTINEZ ALICIA", Fecha = DateTime.Today, Importe = 250.50, OrdenTrabajo = "00004859", Responsable = "[USUARIO DEL SISTEMA]", Procesar = false},
           //     new DatosGrilla{ Cliente = "MERCHAN COSME ALBERTO", Fecha = DateTime.Today, Importe = 177.25, OrdenTrabajo = "00004860", Responsable = "[USUARIO DEL SISTEMA]", Procesar = false}
           // };
        }

        private class DatosGrilla
        {
            public DateTime Fecha { get; set; }
            public string Cliente { get; set; }
            public string OrdenTrabajo { get; set; }
            public string Responsable { get; set; }
            public Double Importe { get; set; }
            public bool Procesar { get; set; }
        }

   

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //frmOrdenTrabajo frm = new frmOrdenTrabajo("Nuevo", "");
            //frm.Show();
            SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmOrdenTrabajo frm = new frmOrdenTrabajo("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion(frm.v_IdOrdenTrabajo);
            };

            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtCliente.Text))
           {
               Filters.Add("v_IdCliente==\""+v_IdCliente+"\"");

           }
        
           _strFilterExpression = null;
           if (Filters.Count > 0)
           {
               foreach (string item in Filters)
               {
                   _strFilterExpression = _strFilterExpression + item + " && ";
               }
               _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
           }
           BindGrid();
        }

        private void BindGrid()
        {
            var objData = GetData("v_NroOrdenTrabajo ASC", _strFilterExpression);
            grdData.DataSource = objData;
            grdData.DataSource = objData;
            if (objData != null)
            {
                if (objData.Count > 0)
                {

                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = true;
                }
                else
                {
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
              //  if (Globals.ClientSession.UsuarioEsContable == 0) CierraOperacionesContables();
            }
        }
        private List<nbs_ordentrabajoDto > GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
             var _objData = _objOrdenTrabajoBL.ListarBusquedaOrdenTrabajo(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"));

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
              return _objData;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;
           
            frmOrdenTrabajo frm = new frmOrdenTrabajo("Edicion", grdData.ActiveRow.Cells["v_IdOrdenTrabajo"].Value.ToString());
             v_Id = grdData.ActiveRow.Cells["v_IdOrdenTrabajo"].Value.ToString();
             frm.Show();
            BindGrid();
            MantenerSeleccion(v_Id);
            btnEditar.Enabled = true;
        }


        private void MantenerSeleccion(string ValorSeleccionado)
        {
            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdOrdenTrabajo"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (btnEditar.Enabled) btnEditar_Click(sender, e);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Pedidos";
            UltraGridExcelExporter ultraGridExcelExporter1 = new UltraGridExcelExporter();
            SaveFileDialog sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            };

            if (sf.ShowDialog() != DialogResult.OK) return;
            btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;

            _tarea = Task.Factory.StartNew(() => { ultraGridExcelExporter1.Export(grdData, sf.FileName); }, cts.Token)
                                 .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void ActualizarLabel(string Texto)
        {
            lblDocumentoExportado.Text = Texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
              grdData.ActiveRow.Activation == Activation.Disabled) return;

            OperationResult _objOperationResult = new OperationResult();
            string NumeroFormato = _objOrdenTrabajoBL.BuscarNumeroFormatoUnico( grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdOrdenTrabajo"].Value.ToString());
            if (NumeroFormato != string.Empty)
            {
                UltraMessageBox.Show("No se puede Eliminar,Orden Trabajo está siendo utilizada en Formato Unico de Facturación :  " + NumeroFormato, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (UltraMessageBox.Show("¿Está seguro de Eliminar este pedido de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
              
                _objOrdenTrabajoBL.EliminarOrdenTrabajo(ref _objOperationResult, grdData.ActiveRow.Cells["v_IdOrdenTrabajo"].Value.ToString(), Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "btnBuscarCliente")
            {
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                frm.ShowDialog();
                if (frm._IdCliente != null)
                {
                    txtCliente.Text = frm._CodigoCliente + " " + frm._RazonSocial;
                    v_IdCliente = frm._IdCliente;
                    txtCliente.ButtonsRight["btnEliminar"].Enabled = true;
                }
                else
                {
                    txtCliente.ButtonsRight["btnEliminar"].Enabled = false;
                }

            }
            else
            {
                v_IdCliente = null;
                txtCliente.Clear();
            }
        }

        private void ultraGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

      
    }
}
