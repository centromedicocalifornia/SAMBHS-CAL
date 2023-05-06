using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmDocumentoNotaSalidaAlmacen : Form
    {

        #region Construct
        public frmDocumentoNotaSalidaAlmacen(string idMovimiento)
        {
            InitializeComponent();
            ImpresionDirecto(idMovimiento);
        }
        #endregion

        #region Methods
        private void ImpresionDirecto(string idMovimiento)
        {
            if (idMovimiento != null)
            {
                var rp = new crNotaSalidaAlmacen();
                var empresa = new NodeBL().ReporteEmpresa().FirstOrDefault();
                var aptitudeCertificate = new AlmacenBL().ReporteNotaSalidaAlmacen(idMovimiento, 2);
                using (var ds1 = new DataSet())
                {
                    var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                    dt.TableName = "dsNotaSalidaAlmacen";

                    ds1.Tables.Add(dt);
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("DecimalesCantidad", Globals.ClientSession.i_CantidadDecimales ?? 2);
                    rp.SetParameterValue("DecimalesPrecio", Globals.ClientSession.i_PrecioDecimales ?? 2);

                    if (empresa != null)
                    {
                        rp.SetParameterValue("NombreEmpresa", empresa.NombreEmpresaPropietaria.Trim());
                        rp.SetParameterValue("RucEmpresa", "R.U.C. : " + empresa.RucEmpresaPropietaria.Trim());
                    }
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }
            else
            {
                UltraMessageBox.Show("El documento no se puede imprimir ", "Error de Validacion", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
