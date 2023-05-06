using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmVistaPreviaOrdenCompra : Form
    {
        List<ReporteOrdenCompraDto> aptitudeCertificate = new List<ReporteOrdenCompraDto>();
        string id = string.Empty;
        string EstadoIGV = string.Empty;
        int IdMoneda;
        string TotalLetras, TotalEnNumero;

        public frmVistaPreviaOrdenCompra(string IdOrdenCompra, string _EstadoIgv, int _IdMoneda, string _TotalNumero)
        {
            InitializeComponent();
            id = IdOrdenCompra;
            EstadoIGV = _EstadoIgv;
            IdMoneda = _IdMoneda;
            TotalEnNumero = _TotalNumero;
           
        }

        private void frmVistaPreviaOrdenCompra_Load(object sender, EventArgs e)
        {
            OrdenCompraBL objOrdenCompraBL = new OrdenCompraBL();
            OperationResult objOperationResult = new OperationResult();
           
            
            Cursor.Current = Cursors.WaitCursor;

            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            {
                aptitudeCertificate = objOrdenCompraBL.ReporteOrdenCompra(ref objOperationResult, id);
            }
            Cursor.Current = Cursors.Default;

            if (objOperationResult.Success == 0)
            {
                if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            if (IdMoneda == (int)Currency.Soles)
            {
                TotalLetras = "SON : " + Utils.ConvertirenLetras(TotalEnNumero).ToUpper() + " SOLES " + "S.E.U.O.";
            }
            if (IdMoneda == (int)Currency.Dolares)
            {
                TotalLetras = "SON : " + Utils.ConvertirenLetras(TotalEnNumero).ToUpper() + " DÓLARES AMERICANOS " + "S.E.U.O.";
            }
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            var rp = ReportesUtils.DevolverReporte(Globals.ClientSession.v_RucEmpresa, TiposReportes.OrdenCompra);

            if (rp == null)
            {
                MessageBox.Show(@"Este reporte no esta configurado para ésta empresa aún...", @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            DataSet ds1 = new DataSet();

            DataTable dt = Utils.ConvertToDatatable(aptitudeCertificate);
            var DatosAlmacen = new AlmacenBL().ObtenerDatosEmpresa(Globals.ClientSession.i_IdAlmacenPredeterminado.Value, Globals.ClientSession.i_IdEstablecimiento.Value);
            string DatosEmpresa = DatosAlmacen.Count() > 0 ? "Dirección : " + DatosAlmacen[0] : "";
           string  TelefonoEmpresa = DatosAlmacen.Count() > 0 ? "Teléfonos : " + DatosAlmacen[1] : "";
           string  OtrosDatosEmpresa = DatosAlmacen.Count() > 0 ? "E-Mail :  " + DatosAlmacen[2] : "";
           var fieldsName = new List<string>();
            ds1.Tables.Add(dt);
            ds1.Tables[0].TableName = "dsOrdenCompra";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria);
            rp.SetParameterValue("RucEmpresa", "R.U.C. : "+aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria);
            rp.SetParameterValue("DatosEmpresa", DatosEmpresa);
            rp.SetParameterValue("MontoLetras", TotalLetras);
            rp.SetParameterValue("EstadoIGV", EstadoIGV);

            foreach (ParameterField item in rp.ParameterFields)
                                    fieldsName.Add(item.Name);
            if (fieldsName.Contains("TelefonoEmpresa"))
                rp.SetParameterValue("TelefonoEmpresa",
                    TelefonoEmpresa);
            if (fieldsName.Contains("Email"))
                rp.SetParameterValue("Email",
                    OtrosDatosEmpresa);

            crystalReportViewer1.ReportSource = rp;
        }
    }
}
