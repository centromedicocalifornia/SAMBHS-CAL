using System;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Planilla.BL;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarTrabajador : Form
    {
        public trabajadorconsultaDto TrabajadorconsultaDto {
            get { return _trabajadorconsultaDto; }
        }

        public frmBuscarTrabajador()
        {
            InitializeComponent();
        }

        private trabajadorconsultaDto _trabajadorconsultaDto;

        private void frmBuscarTrabajador_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var dsGrilla = TrabajadorBl.ObtenerTrabajadoresParaBusqueda(ref objOperationResult);
           // var dsGrilla = new ClienteBL.BuscarAnexo(ref objOperationResult,"T");

            //var dsGrilla = new ClienteBL.ObtenerListadoTrabajadores(ref objOperationResult, null , null );
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            grdData.DataSource = dsGrilla;
            lblElementosEncontrados.Text = string.Format("Se encontraron {0} elementos", grdData.Rows.Count);
        }

        private void txtBuscarNombre_KeyDown(object sender, KeyEventArgs e)
        {
            var cantidad = Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim());
            lblElementosEncontrados.Text = string.Format("Se encontraron {0} elementos", cantidad);
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var fila = grdData.ActiveRow;
            _trabajadorconsultaDto = (trabajadorconsultaDto)fila.ListObject;
            Close();
        }
    }
}
