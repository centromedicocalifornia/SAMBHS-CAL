using System;
using System.Data;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmDocumentoTransferencia : Form
    {
        #region Contruct
        public frmDocumentoTransferencia(string idMovimiento)
        {
            InitializeComponent();
            ImprimirDocumento(idMovimiento);     
        }
        #endregion

        #region Methods
        private void ImprimirDocumento(string pstrIdMovimiento)
        {
            try
            {
                var rp = new crTransferenciaAlmacen();
                var aptitudeCertificate = new AlmacenBL().ReporteTransferenciaAlmacen(pstrIdMovimiento);
                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                var ds1 = new DataSet();
                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);
                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteTransferenciaAlmacen";
                ds1.Tables[1].TableName = "dsEmpresa";
                rp.SetDataSource(ds1);

                var crParameterDiscreteValue = new ParameterDiscreteValue
                {
                    Value = Globals.ClientSession.i_CantidadDecimales ?? 2
                };
                var crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                var crParameterFieldDefinition = crParameterFieldDefinitions["DecimalesCantidad"];
                var crParameterValues = crParameterFieldDefinition.CurrentValues;
                crParameterValues.Clear();
                crParameterValues.Add(crParameterDiscreteValue);
                crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                crParameterDiscreteValue = new ParameterDiscreteValue
                {
                    Value = Globals.ClientSession.i_PrecioDecimales ?? 2
                };
                crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                crParameterFieldDefinition = crParameterFieldDefinitions["DecimalesPrecio"];
                crParameterValues = crParameterFieldDefinition.CurrentValues;
                crParameterValues.Clear();
                crParameterValues.Add(crParameterDiscreteValue);
                crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }catch (Exception)
            {
                UltraMessageBox.Show("Ocurrió un Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);       
            }
        }
        #endregion
    }
}
