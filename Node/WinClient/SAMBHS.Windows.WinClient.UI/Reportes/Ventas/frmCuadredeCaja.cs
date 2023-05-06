﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using System.Threading;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmCuadredeCaja : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init & Load
        public frmCuadredeCaja(string arg)
        {
            InitializeComponent();
        }
        private void frmCuadredeCaja_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();

            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", new DatahierarchyBL().GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboEstablecimiento.Enabled = Globals.ClientSession.UsuarioEsContable == 1 ? true : false;
            rbModelo1.Checked = true;
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
        }
        #endregion

        #region Methods
        private void CargarReporte(string Periodo, DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdAlmacen, string FechaHoraImpresion, bool Export, int? rolId = -1, int systemUserId = -1, int tipoServicioId = -1)
        {
            var objOperationResult = new OperationResult();
            if (rbModelo2.Checked)
            {
                var rp = new crCuadredeCaja();

                var bl = new VentaBL();
                var aptitudeCertificate = bl.ReporteCuadreCaja(int.Parse(cboEstablecimiento.Value.ToString()), FechaRegistroIni, FechaRegistroFin, IdAlmacen, rolId, systemUserId);
                if (tipoServicioId != -1)
                {
                 aptitudeCertificate =    aptitudeCertificate.FindAll(p => p.i_ClienteEsAgente == tipoServicioId);
                }
                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                //var aptitudeCertificate3 = bl.ReporteCuadreCajaResumen(0, FechaRegistroIni, FechaRegistroFin, IdAlmacen);
                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);

                dt.TableName = "dsCuadredeCaja";
                dt2.TableName = "dsEmpresa";

                var workRow1 = dt.NewRow();

                workRow1[0] = new string('─', 10);
                workRow1[1] = "";
                workRow1[2] = 0;
                workRow1[3] = 0;
                workRow1[4] = 0;
                dt.Rows.Add(workRow1);
                var gps = aptitudeCertificate.GroupBy(r => r.CondicionPago);
                foreach (var gp in gps)
                {
                    var workRow = dt.NewRow();
                    workRow[0] = "*****TOTAL*****";
                    workRow[1] = gp.Key;
                    workRow[2] = gp.Sum(r => r.TotalS);
                    workRow[3] = gp.Sum(r => r.TotalD);
                    workRow[4] = gp.Sum(r => r.TotalGS);
                    dt.Rows.Add(workRow);
                }
                workRow1 = dt.NewRow();
                workRow1[0] = new string('─', 10);
                workRow1[1] = "";
                workRow1[2] = 0;
                workRow1[3] = 0;
                workRow1[4] = 0;
                dt.Rows.Add(workRow1);
                dt.TableName = "dsCuadredeCaja";
                dt2.TableName = "dsEmpresa";

                using (var ds1 = new DataSet())
                {
                    ds1.Tables.AddRange(new[] { dt, dt2 });
                    rp.SetDataSource(ds1);
                }
                rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
                rp.SetParameterValue("IdMoneda", 1);
                rp.SetParameterValue("FechaRegistroIni", FechaRegistroIni);
                rp.SetParameterValue("FechaRegistroFin", FechaRegistroFin);
                rp.SetParameterValue("DineroCaja", double.Parse(TxtDineroCaja.Text));
                rp.SetParameterValue("DepositoDiaant", double.Parse(TxtDepositoDiaant.Text));
                rp.SetParameterValue("Nrooperacion", TxtNrooperacion.Text.Trim());
                rp.SetParameterValue("Total", aptitudeCertificate.Sum(r => r.TotalS));
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
            else
            {
                var rp = new crCuadreCajaModeloUno();
                var bl = new VentaBL();
                var ds1 = new DataSet();
                var dt = new DataTable();

                //var rowTitleEgreso = dt.NewRow();
                //rowTitleEgreso[0] = "EGRESOS";
                //dt.Rows.Add(rowTitleEgreso);


                var result = bl.ReporteCuadreCajaModeloUno(int.Parse(cboEstablecimiento.Value.ToString()), FechaRegistroIni, FechaRegistroFin, IdAlmacen, rolId, systemUserId);

                var aptitudeCertificate = bl.ReporteCuadreCaja(int.Parse(cboEstablecimiento.Value.ToString()), FechaRegistroIni, FechaRegistroFin, IdAlmacen, rolId, systemUserId);
               
                if (tipoServicioId != -1)
                {
                    aptitudeCertificate = aptitudeCertificate.FindAll(p => p.i_ClienteEsAgente == tipoServicioId);
                    result = result.FindAll(p => p.i_ClienteEsAgente == tipoServicioId);
                }

                dt = Utils.ConvertToDatatable(result);
                var workRow1 = dt.NewRow();

                //workRow1[0] = new string('─', 10);
                //workRow1[1] = "";
                //workRow1[2] = 0;
                //workRow1[3] = 0;
                //workRow1[4] = 0;
                //dt.Rows.Add(workRow1);
                var gps = aptitudeCertificate.GroupBy(r => r.CondicionPago);
                foreach (var gp in gps)
                {
                    var workRow = dt.NewRow();
                    workRow[0] = "";
                    workRow[1] = "*****TOTAL*****";
                    workRow[2] = gp.Key;
                    workRow[3] = gp.Sum(r => r.TotalS);
                    workRow[4] = gp.Sum(r => r.TotalD);
                    workRow[5] = gp.Sum(r => r.TotalGS);
                    dt.Rows.Add(workRow);
                }
                //workRow1 = dt.NewRow();
                //workRow1[0] = new string('─', 10);
                //workRow1[1] = "";
                //workRow1[2] = 0;
                //workRow1[3] = 0;
                //workRow1[4] = 0;
                //dt.Rows.Add(workRow1);
                dt.TableName = "dtCuadreCaja";
                ds1.Tables.Add(dt);
                rp.SetDataSource(ds1);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                #region ...

                //OcultarMostrarBuscar(true);
                //Cursor.Current = Cursors.WaitCursor;

                //List<CuadreCajaModeloAlternativo> aptitudeCertificate = null;

                //Task.Factory.StartNew(() =>
                //aptitudeCertificate = new VentaBL().ReporteCuadreCajaModelo1(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), DateTime.Parse(dtpFechaInicio.Text), DateTime.Parse(dtpFechaFin.Text + " 23:59"), -1, "-1", "GrupoLLave,IdTipoDocumento,NumeroDocumento,IdCobranza", int.Parse(cboAlmacen.Value.ToString()), rolId, systemUserId), _cts.Token)
                //.ContinueWith(t =>
                //{
                //    if (_cts.IsCancellationRequested) return;
                //    OcultarMostrarBuscar(false);
                //    Cursor.Current = Cursors.Default;
                //    if (objOperationResult.Success == 0)
                //    {
                //        UltraMessageBox.Show("Ocurrió un Error al generar Cuadre de Caja", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        return;
                //    }
                //    var dt = Utils.ConvertToDatatable(aptitudeCertificate);


                //    if (Export)
                //    {
                //        #region Headers

                //        var columnas = new[]
                //    {
                //        "Fecha", "TipoDocumento", "NumeroDocumento", "Cliente", "NroDocCliente", "CondicionPago",
                //        "Vendedor", "Total", "Moneda" , "MontoCobrado","DocumentoCobranza","FormasPago","Deuda"
                //    };


                //        var heads = new ExcelHeader[]
                //    {
                //        "FECHA", "DOC", "NÚMERO", "CLIENTE", "RUC CLIENTE", "CONDICIÓN", "VENDEDOR",
                //        "TOTAL DOC.", "MONEDA" ,"MONTO COBRADO/MONTO PAGADO","DOC. COBRANZA/ DOC. PAGO","FORMA PAGO","DEUDA"
                //    };

                //        #endregion

                //        var excel = new ExcelReport(dt) { Headers = heads };
                //        excel.AutoSizeColumns(1, 20, 5, 20, 50, 15, 15, 15, 10, 15, 15, 15, 15);
                //        excel.SetTitle("CUADRE  DE CAJA ");
                //        var fechaMostrar = dtpFechaInicio.Value == dtpFechaFin.Value ? "DEL " + dtpFechaInicio.Value.Day + " DE " +
                //              dtpFechaInicio.Value.ToString("MMMM").ToUpper() + " DEL " + Globals.ClientSession.i_Periodo
                //            : "DEL " + dtpFechaInicio.Value.Day.ToString("00") + " DE " +
                //              dtpFechaInicio.Value.ToString("MMMM").ToUpper() + " AL " + dtpFechaFin.Value.Day +
                //              " DE " + dtpFechaFin.Value.ToString("MMMM").ToUpper() + " DEL " +
                //              Globals.ClientSession.i_Periodo;
                //        excel[2].Cells[4].Value = fechaMostrar;
                //        excel.SetHeaders();
                //        var last = new int[0];
                //        var group = 0;
                //        excel.EndSection += excel_EndSection;
                //        excel.EndSectionGroup += (sender, e) =>
                //        {
                //            var obj = (ExcelReport)sender;
                //            if (e.EndSections.Length == 0) return;
                //            if (group == 0)
                //            {
                //                obj.SetFormulas(7, "TOTAL", "=" + string.Join("+", e.EndSections.Select(i => "I" + i).ToArray()), "=J" + e.EndSections.FirstOrDefault(), "=" + string.Join("+", e.EndSections.Select(i => "k" + i).ToArray()));
                //                obj.CurrentPosition++;
                //            }
                //            last = e.EndSections;
                //            group++;
                //        };
                //        var filtros = new[] { "GrupoLlave", "Grupollave2" };
                //        excel.SetData(ref objOperationResult, columnas, filtros);
                //        InsertTable(excel, last);
                //        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                //        excel.Generate(path);
                //        System.Diagnostics.Process.Start(path);
                //    }
                //    else
                //    {



                //        var ds1 = new DataSet();
                //        var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                //        dt.TableName = "dsCuadreCajaModeloAlternativo";
                //        ds1.Tables.Add(dt);
                //        var rp = new crCuadreCajaModeloAlternativo();
                //        rp.SetDataSource(ds1);
                //        rp.SetParameterValue("FechaHoraImpresion", DateTime.Now);
                //        rp.SetParameterValue("Moneda", "MONEDA DEL REPORTE : " + cboMoneda.Text.Trim());
                //        rp.SetParameterValue("Fecha", "DEL " + dtpFechaInicio.Value.Day.ToString("00") + "/" + dtpFechaInicio.Value.Month.ToString("00") + "/" + dtpFechaInicio.Value.Year.ToString() + " AL " + dtpFechaFin.Value.Day.ToString("00") + "/" + dtpFechaFin.Value.Month.ToString("00") + "/" + dtpFechaFin.Value.Year.ToString());
                //        rp.SetParameterValue("NombreEmpresaPropietaria", aptitudeCertificate2.First().NombreEmpresaPropietaria);
                //        rp.SetParameterValue("RucEmpresaPropietaria", "R.U.C. : " + aptitudeCertificate2.First().RucEmpresaPropietaria);
                //        rp.SetParameterValue("NumeroRegistros", aptitudeCertificate.Count);
                //        rp.SetParameterValue("MonedaTotales", int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles ? "S" : "D");
                //        rp.SetParameterValue("DineroCaja", string.IsNullOrEmpty(TxtDineroCaja.Text) ? 0 : decimal.Parse(TxtDineroCaja.Text));


                //        decimal TotalMontoCobradoSoles = aptitudeCertificate.Any() ? aptitudeCertificate.Where(l => l.MonedaCobranza == "S" && l.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").ToList().Sum(o => o.MontoCobrado) : 0;
                //        decimal TotalMontoCobradoDolares = aptitudeCertificate.Any() ? aptitudeCertificate.Where(l => l.MonedaCobranza == "D" && l.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").ToList().Sum(o => o.MontoCobrado) : 0;
                //        decimal TotalMontoPagadoSoles = aptitudeCertificate.Any() ? aptitudeCertificate.Where(l => l.MonedaCobranza == "S" && l.GrupoLLave == "4.-DOCUMENTOS DE COMPRA REGISTRADOS").ToList().Sum(o => o.MontoCobrado) : 0;
                //        decimal TotalMontoPagadoDolares = aptitudeCertificate.Any() ? aptitudeCertificate.Where(l => l.MonedaCobranza == "D" && l.GrupoLLave == "4.-DOCUMENTOS DE COMPRA REGISTRADOS").ToList().Sum(o => o.MontoCobrado) : 0;

                //        rp.SetParameterValue("EfectivoSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("EFEC") && o.MonedaCobranza != null && o.MonedaCobranza == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("TotalMontoCobradoSoles", TotalMontoCobradoSoles);
                //        rp.SetParameterValue("TotalMontoCobradoDolares", TotalMontoCobradoDolares);
                //        //rp.SetParameterValue("TotalTodosEmitidosSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(l => l.MonedaCobranza == "S" && l.GrupoLLave =="1.- DOCUMENTOS DE VENTA EMITIDOS").ToList().Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("TotalMontoPagadoSoles", TotalMontoPagadoSoles);
                //        rp.SetParameterValue("TotalMontoPagadoDolares", TotalMontoPagadoDolares);
                //        rp.SetParameterValue("EfectivoDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("EFEC") && o.MonedaCobranza == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS" && o.MonedaCobranza != null).Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("VisaSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("VISA") && o.MonedaCobranza != null && o.MonedaCobranza == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("VisaDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("VISA") && o.MonedaCobranza != null && o.MonedaCobranza == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("NcrSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("NOTA") && o.MonedaCobranza != null && o.MonedaCobranza == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("NcrDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("NOTA") && o.MonedaCobranza != null && o.MonedaCobranza == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("ValeSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("VALE") && o.MonedaCobranza != null && o.MonedaCobranza == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("ValeDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("VALE") && o.MonedaCobranza != null && o.MonedaCobranza == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("MastercardSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("MASTER") && o.MonedaCobranza != null && o.MonedaCobranza == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("MastercardDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("MASTER") && o.MonedaCobranza != null && o.MonedaCobranza == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("DepositoSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("DEP") && o.MonedaCobranza != null && o.MonedaCobranza == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);
                //        rp.SetParameterValue("DepositoDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.FormasPago != null && o.FormasPago.Contains("DEP") && o.MonedaCobranza != null && o.MonedaCobranza == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.MontoCobrado) : 0);

                //        rp.SetParameterValue("TotalCreditoSoles", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.CondicionPago != null && o.CondicionPago.Contains("DITO") && o.Moneda != null && o.Moneda == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.Total) : 0);
                //        rp.SetParameterValue("TotalCreditoDolares", aptitudeCertificate.Any() ? aptitudeCertificate.Where(o => o.CondicionPago != null && o.CondicionPago.Contains("DITO") && o.Moneda != null && o.Moneda == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").Sum(o => o.Total) : 0);
                //        var VentasDelDia = aptitudeCertificate.Where(o => o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS").GroupBy(o => o.IdVenta).Select(group => group.Last()).ToList(); //.Where(o => o.Moneda != null && o.Moneda == "S" && o.GrupoLLave != null && o.IdTipoDocumento != 7).ToList().OrderBy(o => o.IdTipoDocumento).ThenBy (o=>o.NumeroDocumento).ToList();
                //        //var yyy = gg.Where(o => o.IdVenta == "N001-ZQ000008192").ToList();
                //        rp.SetParameterValue("TotalVentaSoles", aptitudeCertificate.Any() ? VentasDelDia.Where(o => o.Moneda != null && o.Moneda == "S" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS" && o.IdTipoDocumento != 7 && o.Fecha == dtpFechaInicio.Text).Sum(o => o.Total) : 0);
                //        rp.SetParameterValue("TotalVentaDolares", aptitudeCertificate.Any() ? VentasDelDia.Where(o => o.Moneda != null && o.Moneda == "D" && o.GrupoLLave != null && o.GrupoLLave == "1.- DOCUMENTOS DE VENTA EMITIDOS" && o.IdTipoDocumento != 7 && o.Fecha == dtpFechaInicio.Text).Sum(o => o.Total) : 0);
                //        rp.SetParameterValue("LetrasTotalVentas", "TOTAL DE VENTAS DEL " + dtpFechaInicio.Text + " : ");

                //        rp.SetParameterValue("EfectivoATenerSoles", TotalMontoCobradoSoles - TotalMontoPagadoSoles);
                //        rp.SetParameterValue("EfectivoATenerDolares", TotalMontoCobradoDolares - TotalMontoPagadoDolares);
                //        crystalReportViewer1.ReportSource = rp;
                //        crystalReportViewer1.Show();
                //        crystalReportViewer1.Zoom(110);
                //    }
                //}, TaskScheduler.FromCurrentSynchronizationContext());
                #endregion


            }
        }

        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            if (e.StartPosition == e.EndPosition) return;
            var obj = (ExcelReport)sender;
            obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["NombreDocumento"];
            obj.SetFormulas(7, "SUBTOTAL", string.Format("=SUM(I{0}:I{1})", e.StartPosition + 1, e.EndPosition), "=J" + e.EndPosition, string.Format("=SUM(K{0}:K{1})", e.StartPosition + 1, e.EndPosition));
            obj.CurrentPosition++;
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : "") + @"Cuadre de Caja";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }

        private void VisualizarReporte(bool export)
        {
            try
            {
                if (rbModelo2.Checked)
                {
                    if (uvDatos.Validate(true, false).IsValid)
                    {
                    }
                }
                else if (rbModelo1.Checked)
                {
                    if (!ValidarModelo1.Validate(true, false).IsValid)
                        return;
                }

                CargarReporte(Globals.ClientSession.i_Periodo.ToString(), DateTime.Parse(dtpFechaInicio.Text), DateTime.Parse(dtpFechaFin.Text + " 23:59"), int.Parse(cboAlmacen.Value.ToString()), chkHoraimpresion.Checked ? "1" : "0", export, Globals.ClientSession.i_RoleId, Globals.ClientSession.i_SystemUserId,int.Parse(cboTipoServicio.Value.ToString()));
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertTable(ExcelReport rp, IList<int> gr)
        {
            rp.CurrentPosition++;
            var row = rp[rp.CurrentPosition++];
            row.Cells[1].Value = "TOTAL DOCUMENTOS DE VENTA EMITIDOS :";
            row.Cells[5].Value = "TOTAL MONTO COBRADO :";
            if (gr.Count > 0)
            {
                row.Cells[4].ApplyFormula("=I" + gr[0]);
                row.Cells[7].ApplyFormula("=I" + gr[0]);
            }
            row = rp[rp.CurrentPosition++];
            row.Cells[1].Value = "TOTAL DOCUMENTOS DE VENTA CRÉDITO :";
            row.Cells[5].Value = "TOTAL MONTO PAGADO :";
            if (gr.Count > 1)
            {
                row.Cells[4].ApplyFormula("=I" + (gr[1] - 3));
                if (gr.Count > 2)
                    row.Cells[7].ApplyFormula("=K" + (gr[2] - 3));
            }
            row = rp[rp.CurrentPosition];
            row.Cells[1].Value = "TOTAL DOCUMENTOS DE COMPRA REGISTRADOS :";
            row.Cells[5].Value = "TOTAL ENVIO EFECTIVO  : *:";
            if (gr.Count > 2)
            {
                row.Cells[4].ApplyFormula("=I" + (gr[2] - 3));
                decimal caja;
                decimal.TryParse(TxtDineroCaja.Text ?? "0", out caja);
                row.Cells[7].ApplyFormula(string.Format("={0:N} + $H{1}-$H{2}", caja, rp.CurrentPosition - 1, rp.CurrentPosition));
            }
            rp[rp.CurrentPosition + 1].Cells[1].Value = "TOTAL EN EFECTIVO = DINERO EN CAJA +(TOTAL MONTO COBRADO - TOTAL MONTO PAGADO)";
        }

        #endregion

        #region Events

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            VisualizarReporte(false);
        }

        private void TxtNumeric_Validating(object sender, CancelEventArgs e)
        {
            var obj = (Infragistics.Win.UltraWinEditors.UltraTextEditor)sender;
            double salida;
            if (obj.Text == string.Empty || !double.TryParse(obj.Text, out salida))
            {
                obj.Text = "0.0";
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            VisualizarReporte(true);
        }

        #endregion

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var tck = new Ablimatex.TicketCuadreCaja(DateTime.Parse(dtpFechaInicio.Text),
                DateTime.Parse(dtpFechaFin.Text + " 23:59"), int.Parse(cboEstablecimiento.Value.ToString()), int.Parse(cboAlmacen.Value.ToString()), TxtDineroCaja.Text, TxtDepositoDiaant.Text, TxtNrooperacion.Text);
            tck.Print();
        }

        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            var objEstablecimientoBl = new EstablecimientoBL();
            var AlmacenesEstablecimiento = new List<KeyValueDTO>();
            string _whereAlmacenesConcatenados = string.Empty;
            string _almacenesConcatenados = string.Empty;
            if (cboEstablecimiento.Value == null || cboEstablecimiento.Value.ToString() == "-1") return;
            AlmacenesEstablecimiento = objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            //if (AlmacenesEstablecimiento.Count > 0)
            //{
            //    foreach (var item in AlmacenesEstablecimiento)
            //    {
            //        _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
            //        _almacenesConcatenados = _almacenesConcatenados + item.Value1 + ", ";
            //    }
            //    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
            //    _almacenesConcatenados = _almacenesConcatenados.Substring(0, _almacenesConcatenados.Length - 2);
            //    _whereAlmacenesTemporales = _whereAlmacenesConcatenados;
            //}
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", AlmacenesEstablecimiento, DropDownListAction.All);
            cboAlmacen.SelectedIndex = 1;
        }

    }
}