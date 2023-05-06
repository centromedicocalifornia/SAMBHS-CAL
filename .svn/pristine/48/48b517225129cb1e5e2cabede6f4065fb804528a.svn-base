using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Letras.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;
using CrystalDecisions.CrystalReports.Engine;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoLetraCobrar : Form
    {
        public string IdCanje = string.Empty;
        LetrasBL _objLetrasBL = new LetrasBL();
        public bool FormatoVoucher = false;
        public bool PorCobrar = false;
        public bool ImprimirDniPNatural = false;
        public frmDocumentoLetraCobrar(string sIdCanje, bool formatVoucher,bool letrasCobrar,bool imprimirDniPNatural)
        {
            InitializeComponent();
            IdCanje = sIdCanje;
            FormatoVoucher = formatVoucher;
            PorCobrar = letrasCobrar;
            ImprimirDniPNatural = imprimirDniPNatural; 
            ImpresionLetra(IdCanje);

        }
        private void ImpresionLetra(string IdLetra)
        {
            //OperationResult objOperationResult = new OperationResult();
            //var RucEmpresa = new NodeBL().ReporteEmpresa();
            //ReportDocument rp = new ReportDocument();
            //List<ReporteDocumentoLetra> aptitudeCertificate = new List<ReporteDocumentoLetra>();
            //if (FormatoVoucher)
            //{
            //    aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetraFormatoVoucher(ref objOperationResult, IdLetra);
            //    rp = new Reportes.Ventas.crDocumentoLetrasCobrarFormatoVoucher();
            //}
            //else
            //{
            //    aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetra(ref objOperationResult, IdLetra);
            //     rp = ReportesUtils.DevolverReporte(RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim(), TiposReportes.LetraCobrar);
            //}


            //if (rp == null)
            //{
            //    UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //DataSet ds1 = new DataSet();
            //DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            //dt.TableName = "dsLetrasCobrar";
            //ds1.Tables.Add(dt);

            //rp.SetDataSource(ds1);
            //crystalReportViewer1.ReportSource = rp;
            //crystalReportViewer1.Show();
            //crystalReportViewer1.Zoom(110);
            OperationResult objOperationResult = new OperationResult();
            var RucEmpresa = Globals.ClientSession.v_RucEmpresa;
            ReportDocument rp = new ReportDocument();
            List<ReporteDocumentoLetra> aptitudeCertificate = new List<ReporteDocumentoLetra>();
            List<ReporteDocumentoLetraDetalles> aptitudeCertificate2 = new List<ReporteDocumentoLetraDetalles>();
            if (FormatoVoucher)
            {
                if (PorCobrar)
                {
                aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetraCobrarFormatoVoucher(ref objOperationResult, IdLetra);
                aptitudeCertificate2 = _objLetrasBL.ReporteDocumentoLetraCobrarFormatoVoucherDetalles(ref objOperationResult, IdLetra);
                }
                else
                {
                    aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetraPagarFormatoVoucher(ref objOperationResult, IdLetra);
                    aptitudeCertificate2 = _objLetrasBL.ReporteDocumentoLetraPagarFormatoVoucherDetalles(ref objOperationResult, IdLetra);

                    }
                rp = new Reportes.Ventas.crDocumentoLetrasCobrarFormatoVoucher();
                if (rp == null)
                {
                    UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                //dt.TableName = "dsLetrasCobrar";
                ds1.Tables[0].TableName = "dsLetrasCobrar";
                ds1.Tables[1].TableName = "dsLetrasCobrarDetalle";

                rp.SetDataSource(ds1);
                rp.SetParameterValue("MontoCanjeadoLetras", "TOTAL MONTO CANJEADO : ");
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                crystalReportViewer1.Zoom(110);
            }
            else
            {
                if (PorCobrar)
                {
                    aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetra(ref objOperationResult, IdLetra, ImprimirDniPNatural);
                }
                else
                {
                    aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetraPagar(ref objOperationResult, IdLetra, ImprimirDniPNatural);
                }
                rp = ReportesUtils.DevolverReporte(RucEmpresa.Trim(), TiposReportes.LetraCobrar);
                if (rp == null)
                {
                    UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);


                dt.TableName = "dsLetrasCobrar";
                ds1.Tables.Add(dt);
                rp.SetDataSource(ds1);
               
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                crystalReportViewer1.Zoom(110);

            }




        }

        private void frmDocumentoLetraCobrar_Load(object sender, EventArgs e)
        {
            this.Text = PorCobrar ? "LETRAS POR COBRAR" : "LETRAS POR PAGAR";
        }

    }
}

    

