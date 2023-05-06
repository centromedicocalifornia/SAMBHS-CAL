using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{    
    public partial class frmOrdenesTrabajoPendientes : Form
    {
        public List<nbs_formatounicofacturaciondetalleDto> OrdenTrabajoDetalle
        {
            get { return _ordenTrabajoDetalle; }
        }

        private List<nbs_formatounicofacturaciondetalleDto> _ordenTrabajoDetalle = new List<nbs_formatounicofacturaciondetalleDto>();

        public frmOrdenesTrabajoPendientes(string idCliente)
        {
            InitializeComponent();
            ObtenerData(idCliente);
            OperationResult objOperationResult = new OperationResult();
            var cliente = new ClienteBL().ObtenerCliente(ref objOperationResult, idCliente);
            Text = @"Ordenes de trabajo pendientes de: " +
                   (cliente.v_ApePaterno + " " + cliente.v_ApeMaterno + " " + cliente.v_PrimerNombre + " " +
                    cliente.v_RazonSocial).Trim();
        }

        private void frmOrdenesTrabajoPendientes_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        void ObtenerData(string idCliente)
        {
            try
            {
                var objOperationResult = new OperationResult();
                var ds = FormatoUnicoFacturacionBl.BuscarOrdenesTrabajoPendientes(ref objOperationResult, idCliente);
                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ultraGrid1.DataSource = ds;
                if (ultraGrid1.Rows.Any()) ultraGrid1.Rows.ToList().ForEach(fila => fila.Cells["_Check"].Value = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraGrid1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var ots = ultraGrid1.Rows.Where(row => Convert.ToBoolean(row.Cells["_Check"].Value.ToString()))
                                .Select(p => p.Cells["v_IdOrdenTrabajo"].Value.ToString()).ToList();

            var objOperationResult = new OperationResult();
            _ordenTrabajoDetalle = FormatoUnicoFacturacionBl.DevolverDetallePendientesOT(ref objOperationResult, ots);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSeleccionarTodos_Click(object sender, EventArgs e)
        {
            if (ultraGrid1.Rows.Any()) ultraGrid1.Rows.ToList().ForEach(fila => fila.Cells["_Check"].Value = true);
        }
    }
}
