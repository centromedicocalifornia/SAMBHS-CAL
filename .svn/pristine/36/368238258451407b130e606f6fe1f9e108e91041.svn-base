using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigracionReciboHonorarios : Form
    {
        List<recibohonorarioDto> ListaReciboHonorarios = new List<recibohonorarioDto>();
        List<importaciondetallefobDto> ListaDetalleFOB = new List<importaciondetallefobDto>();
        List<importaciondetalleproductoDto> ListaDetalleProducto = new List<importaciondetalleproductoDto>();
        List<importaciondetallegastosDto> ListaDetalleGastos = new List<importaciondetallegastosDto>();
        DiarioBL _objDiarioBL = new DiarioBL();

        List<recibohonorariodetalleDto> _TempDetalle_AgregarDto = new List<recibohonorariodetalleDto>();
        ReciboHonorarioBL _objReciboHonorariosBL = new ReciboHonorarioBL();
        public frmMigracionReciboHonorarios(string p)
        {
            InitializeComponent();
        }



        private void cboImportarCabecera_Click(object sender, EventArgs e)
        {

            if (grdCabecera.Rows.Count() > 0)
            {
                grdCabecera.Selected.Rows.AddRange((UltraGridRow[])grdCabecera.Rows.All);
                grdCabecera.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName = "";
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Archivos Excel (*.*)| *.*";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;
                }
                else
                {
                    return;
                }

                using (new LoadingClass.PleaseWait(this.Location, "Importando Cabeceras..."))
                {
                    Workbook workbook = Workbook.Load(sFileName);
                    Worksheet worksheet = workbook.Worksheets[0];

                    this.DataSourceCabecera.Reset();

                    bool isHeaderRow = true;
                    foreach (WorksheetRow row in worksheet.Rows)
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                string columnKey = cell.GetText();

                                UltraDataColumn column = this.DataSourceCabecera.Band.Columns.Add(columnKey);

                                switch (columnKey)
                                {
                                    default:
                                        column.DataType = typeof(string);
                                        break;
                                }
                            }

                            isHeaderRow = false;
                        }
                        else
                        {
                            List<object> rowData = new List<object>();
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                rowData.Add(cell.Value);
                            }

                            this.DataSourceCabecera.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdCabecera.SetDataBinding(this.DataSourceCabecera, "Band 0");

            lblContadorFilasCabecera.Text = string.Format("Se encontraron {0} registros", grdCabecera.Rows.Count());

        }






        private void btnImportarDetalles_Click(object sender, EventArgs e)
        {

            if (grdDetalles.Rows.Count() > 0)
            {
                grdDetalles.Selected.Rows.AddRange((UltraGridRow[])grdDetalles.Rows.All);
                grdDetalles.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName = "";
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Archivos Excel (*.*)| *.*";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;
                }
                else
                {
                    return;
                }

                using (new LoadingClass.PleaseWait(this.Location, "Importando Detalles..."))
                {
                    Workbook workbook = Workbook.Load(sFileName);
                    Worksheet worksheet = workbook.Worksheets[0];


                    this.DataSourceGastos.Reset();

                    bool isHeaderRow = true;
                    foreach (WorksheetRow row in worksheet.Rows)
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                string columnKey = cell.GetText();


                                UltraDataColumn column = this.DataSourceGastos.Band.Columns.Add(columnKey);

                                switch (columnKey)
                                {
                                    default:
                                        column.DataType = typeof(string);
                                        break;
                                }
                            }

                            isHeaderRow = false;
                        }
                        else
                        {
                            List<object> rowData = new List<object>();
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                rowData.Add(cell.Value);
                            }

                            this.DataSourceGastos.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }


            grdDetalles.SetDataBinding(this.DataSourceGastos, "Band 0");
            lblContadorFilasDetalles.Text = string.Format("Se encontraron {0} registros", grdDetalles.Rows.Count());


        }

        private void cboFormatoFechas_Click(object sender, EventArgs e)
        {

            if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (grdCabecera.Rows.Count() > 0)
                {
                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcom_3"].Value != null && p.Cells["fcom_3"].Value.ToString() != "  -   -" && p.Cells["fcom_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcom_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcom_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }



                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcome_3"].Value != null && p.Cells["fcome_3"].Value.ToString() != "  -   -" && p.Cells["fcome_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcome_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcome_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }

                }

            }

            var resp = UltraMessageBox.Show("Proceso Terminado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnGuardar.Enabled = true;



        }
        private bool ValidarCuentasGeneracionLibro()
        {

            string CuartaCategoria = int.Parse(((int)Concepto.CuartaCategoria).ToString()) < 10 ? ("0" + ((int)Concepto.CuartaCategoria).ToString()).Trim() : (((int)Concepto.CuartaCategoria).ToString()).Trim();
            string PorPagarSoles = int.Parse(((int)Concepto.PorPagarSoles).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarSoles).ToString()).Trim() : (((int)Concepto.PorPagarSoles).ToString()).Trim();
            string PorPagarDolares = int.Parse(((int)Concepto.PorPagarDolares).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarDolares).ToString()).Trim() : (((int)Concepto.PorPagarDolares).ToString()).Trim();


            if (!_objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(CuartaCategoria))
            {
                UltraMessageBox.Show("Concepto Imp. 4ta. Categoria(Código 06) en  Administracion Conceptos no es correcto para poder generar el  Libro Diario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            if (!_objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(PorPagarSoles))
            {
                UltraMessageBox.Show("Concepto Por Pagar (Código 08) en  Administracion Conceptos no es correcto para poder generar el  Libro Diario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }

            if (!_objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(PorPagarDolares))
            {
                UltraMessageBox.Show("Concepto Por Pagar M.E. (Código 09) en  Administracion Conceptos no es correcto  para poder generar el  Libro Diario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }


            return true;

        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            
            
            
            
            
            
            AdministracionConceptosBL _objAdministracionConceptosBL = new AdministracionConceptosBL();
            OperationResult pobjOperationResult = new OperationResult();
            if (ValidarCuentasGeneracionLibro())
            {
                string CuartaCategoria = int.Parse(((int)Concepto.CuartaCategoria).ToString()) < 10 ? ("0" + ((int)Concepto.CuartaCategoria).ToString()).Trim() : (((int)Concepto.CuartaCategoria).ToString()).Trim();
                string PorPagarSoles = int.Parse(((int)Concepto.PorPagarSoles).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarSoles).ToString()).Trim() : (((int)Concepto.PorPagarSoles).ToString()).Trim();
                string PorPagarDolares = int.Parse(((int)Concepto.PorPagarDolares).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarDolares).ToString()).Trim() : (((int)Concepto.PorPagarDolares).ToString()).Trim();

                var CuartaCategoriaCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, CuartaCategoria).v_CuentaPVenta;
                var PorPagarSolesCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref  pobjOperationResult, PorPagarSoles).v_CuentaPVenta;
                var PorPagarDolaresCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, PorPagarDolares).v_CuentaPVenta;

                string Mensaje = "Los asientos de los recibos por honorarios se generarán con las sgtes. cuentas :\n  " + "Cuarta Categoria :" + CuartaCategoriaCuenta + "\n" + " Por Pagar Soles : " + PorPagarSolesCuenta + "\n" + " Por Pagar Dolares :" + PorPagarDolaresCuenta + "\n";
                if (UltraMessageBox.Show(Mensaje + "¿Desea Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                ClienteBL _objClienteBL = new ClienteBL();
                ProductoBL _objProductoBL = new ProductoBL();
                DatahierarchyBL _objDatahierachyBL = new DatahierarchyBL();
                recibohonorarioDto _recibohonorarioDto = new recibohonorarioDto();

                DocumentoBL _objDocumentosBL = new DocumentoBL();

                if (grdCabecera.Rows.Count() == 0)
                {
                    UltraMessageBox.Show("Por favor importe los excel con las cabeceras ...");
                }
                else
                {
                    List<KeyValueDTO> Proveedores = _objClienteBL.DevuelveProveedores();
                    List<string> ProveedoresNoEncontrados = new List<string>();
                    List<string> CuentasNoEncontradas = new List<string>();


                    try
                    {
                        #region Procesar Cabeceras



                        List<UltraGridRow> RecibosHonorariosCabeceras = grdCabecera.Rows.AsParallel()

                                                                .OrderBy(x => x.Cells["ncom_3"].Value.ToString()).ToList();

                        foreach (UltraGridRow Fila in RecibosHonorariosCabeceras)
                        {


                            _recibohonorarioDto = new recibohonorarioDto();

                            try
                            {

                                DateTime FechaRegistro = DateTime.Parse(Fila.Cells["fcom_3"].Value.ToString());

                                _recibohonorarioDto.v_Periodo = FechaRegistro.Year.ToString("0000");
                                _recibohonorarioDto.v_Mes = FechaRegistro.Month.ToString("00");
                                // ImportacionesDto.v_Correlativo = Fila.Cells["cncom_3"].Value.ToString().Substring(2, 8);

                                _recibohonorarioDto.i_IdTipoDocumento = Fila.Cells["tidoc_3"].Value != null ? int.Parse(Fila.Cells["tidoc_3"].Value.ToString()) : -1;
                                _recibohonorarioDto.v_SerieDocumento = Fila.Cells["sfactu_3"].Value.ToString();
                                _recibohonorarioDto.v_CorrelativoDocumento = Fila.Cells["nfactu_3"].Value.ToString();
                                _recibohonorarioDto.i_IdIgv = 1;
                                _recibohonorarioDto.t_FechaEmision = Fila.Cells["fcome_3"].Value == null || Fila.Cells["fcome_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcome_3"].Value.ToString());
                                _recibohonorarioDto.t_FechaRegistro = Fila.Cells["fcom_3"].Value == null || Fila.Cells["fcom_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcome_3"].Value.ToString());

                                _recibohonorarioDto.d_TipoCambio = decimal.Parse(Fila.Cells["tcam_3"].Value.ToString());  //   txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                                _recibohonorarioDto.i_IdMoneda = int.Parse(Fila.Cells["mone_3"].Value.ToString());
                                _recibohonorarioDto.i_IdEstado = int.Parse(Fila.Cells["flag_3"].Value.ToString());
                                _recibohonorarioDto.v_Glosa = Fila.Cells["gloa_3"].Value.ToString();
                                _recibohonorarioDto.d_Importe = _recibohonorarioDto.i_IdMoneda == 1 ? decimal.Parse(Fila.Cells["imp1_3"].Value.ToString()) : decimal.Parse(Fila.Cells["imp2_3"].Value.ToString());
                                _recibohonorarioDto.i_RentaCuartaCategoria = int.Parse(Fila.Cells["ren4ta_3"].Value.ToString()) == 2 ? 0 : 1;
                                _recibohonorarioDto.d_RentaCuartaCategoria = _recibohonorarioDto.i_IdMoneda == 1 ? decimal.Parse(Fila.Cells["igv1_3"].Value.ToString()) : decimal.Parse(Fila.Cells["igv2_3"].Value.ToString());
                                _recibohonorarioDto.d_PorPagar = _recibohonorarioDto.i_IdMoneda == 1 ? decimal.Parse(Fila.Cells["tot1_3"].Value.ToString()) : decimal.Parse(Fila.Cells["tot2_3"].Value.ToString());
                                _recibohonorarioDto.d_TotalDebe = _recibohonorarioDto.d_Importe.Value;
                                _recibohonorarioDto.d_TotalHaber = _recibohonorarioDto.d_RentaCuartaCategoria.Value + _recibohonorarioDto.d_PorPagar.Value;
                                _recibohonorarioDto.d_Diferencia = _recibohonorarioDto.d_TotalDebe.Value - _recibohonorarioDto.d_TotalHaber.Value;
                                _recibohonorarioDto.i_PorcentajeCuartaCategoria = _recibohonorarioDto.i_RentaCuartaCategoria == 1 ? 2 : -1;
                                _recibohonorarioDto.i_IdDocumentoReferencia = -1;
                                _recibohonorarioDto.v_SerieDocumentoRef = "";
                                _recibohonorarioDto.v_CorrelativoDocumentoRef = "";






                                string CodCliente1 = Fila.Cells["fich_3"].Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(CodCliente1))
                                {
                                    KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodCliente1).FirstOrDefault();

                                    if (Cliente != null)
                                    {
                                        _recibohonorarioDto.v_IdProveedor = Cliente.Id;
                                        //_recibohonorarioDto.v_IdProveedor = frm._IdProveedor;

                                    }
                                    else
                                    {

                                        ProveedoresNoEncontrados.Add(CodCliente1);
                                    }
                                }
                                else
                                {
                                    ProveedoresNoEncontrados.Add(CodCliente1);
                                }
                                _recibohonorarioDto.KeyRegistro = Fila.Cells["ncom_3"].Value.ToString();
                                ListaReciboHonorarios.Add(_recibohonorarioDto);


                            }
                            catch (Exception ex)
                            {
                                UltraMessageBox.Show(ex.Message);
                                break;
                            }
                        }

                        #endregion

                        #region Procesar Detalles
                        //List<UltraGridRow> DetallesFob = grdDetallesFob.Rows.Where(p => !string.IsNullOrEmpty(p.Cells["deta_35"].Value.ToString()) && !string.IsNullOrEmpty(p.Cells["tdoc_35"].Value.ToString()) && !string.IsNullOrEmpty(p.Cells["sdoc_35"].Value.ToString()) && !string.IsNullOrEmpty(p.Cells["ndoc_35"].Value.ToString())).ToList();
                        List<UltraGridRow> DetallesRecibosHonorarios = grdDetalles.Rows.ToList();

                        foreach (UltraGridRow Fila in DetallesRecibosHonorarios)
                        {
                            recibohonorariodetalleDto _objReciboDetalle = new recibohonorariodetalleDto();

                            _objReciboDetalle.v_IdReciboHonorario = null;


                            string Cuenta = Fila.Cells["cuen_3a"].Value.ToString().Trim();

                            if (!string.IsNullOrEmpty(Cuenta))
                            {
                                if (!_objReciboHonorariosBL.ExistenciaCuentaImputable(Cuenta))
                                {


                                    CuentasNoEncontradas.Add(Cuenta);
                                }
                                else
                                {


                                    _objReciboDetalle.v_NroCuenta = Cuenta;
                                }
                            }
                            else
                            {
                                CuentasNoEncontradas.Add(Cuenta);
                            }

                            _objReciboDetalle = new recibohonorariodetalleDto();

                            _objReciboDetalle.v_IdReciboHonorario = _recibohonorarioDto.v_IdReciboHonorario;
                            _objReciboDetalle.v_NroCuenta = Fila.Cells["cuen_3a"].Value == null ? null : Fila.Cells["cuen_3a"].Value.ToString();
                            _objReciboDetalle.d_ImporteSoles = Fila.Cells["imp1_3a"].Value == null ? 0 : decimal.Parse(Fila.Cells["imp1_3a"].Value.ToString());
                            _objReciboDetalle.d_ImporteDolares = Fila.Cells["imp2_3a"].Value == null ? 0 : decimal.Parse(Fila.Cells["imp2_3a"].Value.ToString());
                            _objReciboDetalle.i_CCosto = Fila.Cells["ccos_3a"].Value == null ? "" : Fila.Cells["ccos_3a"].Value.ToString();
                            _objReciboDetalle.KeyRegistro = Fila.Cells["ncom_3a"].Value.ToString();
                            _TempDetalle_AgregarDto.Add(_objReciboDetalle);



                        }

                        #endregion



                        if (CuentasNoEncontradas.Count() != 0)
                        {
                            string cuenta = "";
                            foreach (var item in CuentasNoEncontradas)
                            {
                                cuenta = cuenta + " " + item;

                            }
                            var resp = UltraMessageBox.Show(string.Format("Hay {0} cuentas no registradas, Agregue estos productos antes de continuar", CuentasNoEncontradas.Distinct().Count() + " " + cuenta), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            var productos = string.Join(", ", CuentasNoEncontradas);
                            Clipboard.SetText(productos);
                            // if (resp == DialogResult.No  ) 
                            return;

                        }
                        else if (ProveedoresNoEncontrados.Count() != 0)
                        {
                            string prov = "";
                            foreach (var item in ProveedoresNoEncontrados)
                            {
                                prov = prov + " " + item;

                            }

                            var resp = UltraMessageBox.Show(string.Format("Hay proveedores no registrados,Agregue estos proveedores antes de continuar :" + prov), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            var productos = string.Join(", ", ProveedoresNoEncontrados);
                            Clipboard.SetText(productos);
                            // if (resp == DialogResult.No) 
                            return;

                        }
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                        bwkProceso.RunWorkerAsync();
                        //  #endregion
                    }

                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                        return;
                    }
                }
            }


        }

        private void bwkProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                OperationResult pobjOperationResult = new OperationResult();
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    #region Elimina Diarios

                    var ReciboHonorarioEliminar = _objReciboHonorariosBL.ObtenerListadoReciboHonorariosProcesoMigracion(ref  pobjOperationResult, Globals.ClientSession.i_Periodo.ToString());


                    var ListaDiariosEliminar = _objDiarioBL.ObtenerListadoDiarioTipoDocumentoyReferencia(ref  pobjOperationResult, Globals.ClientSession.i_Periodo.ToString(), (int)TiposDocumentos.LibroReciboHonorario, null);
                    int CantidadHonorarios = ListaReciboHonorarios.Count();
                    int cantidadHonorariosEliminar = ReciboHonorarioEliminar == null ? 0 : ReciboHonorarioEliminar.Count();
                    int CantidadDiariosEliminar = ListaDiariosEliminar == null ? 0 : ListaDiariosEliminar.Count();
                    Globals.ProgressbarStatus.i_TotalProgress = CantidadHonorarios + CantidadDiariosEliminar + cantidadHonorariosEliminar;

                    if (ReciboHonorarioEliminar != null)
                    {
                        foreach (var recibo in ReciboHonorarioEliminar)
                        {
                            _objReciboHonorariosBL.EliminarReciboHonorarios(ref pobjOperationResult, recibo.Value2, Globals.ClientSession.GetAsList());
                            Globals.ProgressbarStatus.i_Progress++;
                        }
                    }
                    if (ListaDiariosEliminar != null)
                    {
                        foreach (var diario in ListaDiariosEliminar)
                        {
                            _objDiarioBL.EliminarDiario(ref pobjOperationResult, diario.Value2, Globals.ClientSession.GetAsList());
                            Globals.ProgressbarStatus.i_Progress++;
                        }
                    }
                    #endregion


                    #region Empieza el guardado de las importaciones filtradas




                    foreach (recibohonorarioDto ReciboH in ListaReciboHonorarios.AsParallel())
                    {

                        OperationResult objOperationResult = new OperationResult();
                        List<recibohonorariodetalleDto> DetallesRecibo = _TempDetalle_AgregarDto.Where(p => p.KeyRegistro == ReciboH.KeyRegistro).ToList();
                        // Totales Pie Formulario
                        var _ListadoReciboHCambioFecha = _objReciboHonorariosBL.ObtenerListadoReciboHonorarios(ref objOperationResult, ReciboH.v_Periodo, ReciboH.v_Mes);
                        int MaxMovimiento = _ListadoReciboHCambioFecha.Count() > 0 ? int.Parse(_ListadoReciboHCambioFecha[_ListadoReciboHCambioFecha.Count() - 1].Value1.ToString()) : 0;
                        MaxMovimiento++;
                        ReciboH.v_Correlativo = MaxMovimiento.ToString("00000000");
                        string newIdReciboHonorario = _objReciboHonorariosBL.InsertarReciboHonorarios(ref objOperationResult, ReciboH, Globals.ClientSession.GetAsList(), DetallesRecibo);
                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Ocurrió un error al Migrar Recibos Honorarios" + ReciboH.KeyRegistro, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Globals.ProgressbarStatus.b_Cancelado = true;
                            return;
                        }
                        Globals.ProgressbarStatus.i_Progress++;
                    }

                    ts.Complete();

                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }
            UltraMessageBox.Show("Importación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    #endregion
        }

        private void frmMigracionReciboHonorarios_Load(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;
        }
    }
}
