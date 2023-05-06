using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Compra.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmRegistroImportaciones : Form
    {
        public string idImportacion;
        public int MonedaImpresion;
        public frmRegistroImportaciones(string pstrIdImportacion,int monImpresion)
        {
            InitializeComponent();
            idImportacion = pstrIdImportacion;
            MonedaImpresion = monImpresion;
        }



        private void frmRegistroImportaciones_Load(object sender, EventArgs e)
        {
            
            
            
            ReportDocument rp = new ReportDocument();
            ImportacionBL objImportacionBL = new ImportacionBL();
            OperationResult objOperationResult = new OperationResult();
            DataSet ds = new DataSet();
            var _ocrPaymentVoucher = new Reportes.Compras.crRegistroImportaciones();
            rp = _ocrPaymentVoucher;
            var fullPathApp = Application.StartupPath;
            var aptitudeCertificate = objImportacionBL.ReporteDocumentoImportaciones(ref objOperationResult, idImportacion,MonedaImpresion);
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            else
            {

                var Empresa = new NodeBL().ReporteEmpresa();
                dt.TableName = "dsImportaciones";
                ds.Tables.Add(dt);
                rp.SetDataSource(ds);

                rp.SetParameterValue("CantidadDecimalC", (int)Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue("NombreEmpresa",Empresa.FirstOrDefault ().NombreEmpresaPropietaria.Trim ());
                rp.SetParameterValue("RucEmpresa",Empresa.FirstOrDefault ().RucEmpresaPropietaria .Trim ());
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }

        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
