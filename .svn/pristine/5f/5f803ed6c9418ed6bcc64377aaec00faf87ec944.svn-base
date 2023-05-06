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
    public partial class frmMigracionNotasIngreso : Form
    {

        List<movimientoDto> ListaMovimiento = new List<movimientoDto>();
        List<movimientodetalleDto> ListaMovimientoDetalle = new List<movimientodetalleDto>();
        movimientoDto _movimientoDto = new movimientoDto();
        MovimientoBL _objMovimientoBL = new MovimientoBL();

        public frmMigracionNotasIngreso(string p)
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
                }

            }

            var resp = UltraMessageBox.Show("Proceso Terminado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnGuardar.Enabled = true;



        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            AdministracionConceptosBL _objAdministracionConceptosBL = new AdministracionConceptosBL();
            OperationResult pobjOperationResult = new OperationResult();

            _movimientoDto = new movimientoDto();
            ClienteBL _objClienteBL = new ClienteBL();
            ProductoBL _objProductoBL = new ProductoBL();
            DatahierarchyBL _objDatahierachyBL = new DatahierarchyBL();
            DocumentoBL _objDocumentosBL = new DocumentoBL();
            ListaMovimientoDetalle = new List<movimientodetalleDto>();
            ListaMovimiento = new List<movimientoDto>();
            if (grdCabecera.Rows.Count() == 0)
            {
                UltraMessageBox.Show("Por favor importe los excel con las cabeceras ...");
            }
            else
            {
                List<KeyValueDTO> Proveedores = _objClienteBL.DevuelveProveedores();
                List<string> UmNoEncontrados = new List<string>();
                List<string> ProductosNoEncontrados = new List<string>();
                List<KeyValueDTO> Productos = _objProductoBL.DevolverProductos();
                List<KeyValueDTO> UM = _objDatahierachyBL.GetDataHierarchyForComboKeyValueDto(ref pobjOperationResult, 17, "");
                clienteDto _clienteDto = new clienteDto();

                try
                {
                    #region Procesar Cabeceras

                   // _objClienteBL.InsertarProveedorSiNoExiste(ref pobjOperationResult, (string)Constants.RucJorplast);



                    List<UltraGridRow> NotasIngresoCabeceras = grdCabecera.Rows.AsParallel()

                                                            .OrderBy(x => x.Cells["ncom_3"].Value.ToString()).ToList();

                    foreach (UltraGridRow Fila in NotasIngresoCabeceras)
                    {

                        _movimientoDto = new movimientoDto();

                        try
                        {

                            DateTime FechaRegistro = DateTime.Parse(Fila.Cells["fcom_3"].Value.ToString());
                            _movimientoDto.v_Periodo = FechaRegistro.Year.ToString("0000");
                            _movimientoDto.v_Mes = FechaRegistro.Month.ToString("00");
                            _movimientoDto.d_TipoCambio = decimal.Parse(Fila.Cells["tcam_3"].Value.ToString());
                            _movimientoDto.i_IdAlmacenOrigen = int.Parse(Fila.Cells["talm_3"].Value.ToString());
                            _movimientoDto.i_IdMoneda = int.Parse(Fila.Cells["mone_3"].Value.ToString());
                            _movimientoDto.i_IdTipoMotivo = 15;
                            _movimientoDto.t_Fecha = FechaRegistro;
                            _movimientoDto.v_Glosa = Fila.Cells["gloa_3"].Value.ToString();
                            _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;

                            _movimientoDto.i_EsDevolucion = int.Parse(Fila.Cells["adevol_3"].Value.ToString()); // adevol_3chkDevolucion.Checked == true ? 1 : 0;
                            _movimientoDto.i_IdEstablecimiento = int.Parse(Fila.Cells["emp_3"].Value.ToString());

                            //List<string> ProveedoresNoEncontrados = new List<string>();
                            //string CodProveedor = Fila.Cells["fich_3"].Value.ToString().Trim();
                            //KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodProveedor).FirstOrDefault();
                            //if (!string.IsNullOrEmpty(CodProveedor))
                            //{


                            //    if (Cliente != null)
                            //    {

                            //        _movimientoDto.v_IdCliente = Cliente.Id;
                            //    }
                            //    else
                            //    {
                            //        CodProveedor = Constants.RucJorplast;
                            //        Cliente = Proveedores.Where(p => p.Value5.Trim() == CodProveedor).FirstOrDefault();
                            //        if (Cliente != null)
                            //        {
                            //            _movimientoDto.v_IdCliente = Cliente.Id;
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    CodProveedor = Constants.RucJorplast;
                            //    Cliente = Proveedores.Where(p => p.Value5.Trim() == CodProveedor).FirstOrDefault();
                            //    if (Cliente != null)
                            //    {
                            //        _movimientoDto.v_IdCliente = Cliente.Id;
                            //    }

                               
                            //}
                            _movimientoDto.v_IdCliente = null;
                            _movimientoDto.KeyRegistro = Fila.Cells["ncom_3"].Value.ToString();
                            ListaMovimiento.Add(_movimientoDto);


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
                    List<UltraGridRow> MovimientoDetalles = grdDetalles.Rows.ToList();

                    foreach (UltraGridRow FilaDetalles in MovimientoDetalles)
                    {

                        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();

                        _movimientoDto.v_IdMovimiento = null;

                        string CodProducto = FilaDetalles.Cells["linea_3a"].Value.ToString().Trim();

                        KeyValueDTO Producto = Productos.Where(p => p.Value1.Trim() == CodProducto.Trim()).FirstOrDefault();

                        if (!string.IsNullOrEmpty(CodProducto))
                        {
                            if (Producto != null)
                            {

                                _movimientodetalleDto.v_IdProductoDetalle = Producto.Value2;
                            }
                            else
                            {
                                ProductosNoEncontrados.Add(CodProducto);
                            }
                        }
                        else
                        {
                            ProductosNoEncontrados.Add(CodProducto);
                        }

                        string UnidadMedida = FilaDetalles.Cells["unid_3a"].Value.ToString().Trim();

                        //var UnidadMed = UM.Where(x => x.Value1.Trim () == UnidadMedida).FirstOrDefault();
                        //if (UnidadMed != null)
                        //{
                        //    _objImportaciondetalleproductoDto.i_IdUnidadMedida = int.Parse ( UnidadMed.Id);
                        //}
                        //else
                        //{
                        //    UmNoEncontrados.Add(UnidadMedida);
                        //}
                        switch (UnidadMedida)
                        {
                            case "MTS":
                                _movimientodetalleDto.i_IdUnidad = 8;


                                break;
                            case "UND":
                                _movimientodetalleDto.i_IdUnidad = 15;
                                break;

                            case "CJA":
                                _movimientodetalleDto.i_IdUnidad = 19;
                                break;

                            case "SET":
                                _movimientodetalleDto.i_IdUnidad = 21;
                                break;

                            case "HJS":

                                _movimientodetalleDto.i_IdUnidad = 22;
                                break;

                            default:
                                UmNoEncontrados.Add(UnidadMedida);
                                break;
                        }

                        _movimientodetalleDto.v_NroGuiaRemision = null;
                        _movimientodetalleDto.i_IdTipoDocumento = -1;
                        _movimientodetalleDto.v_NumeroDocumento = null;
                        _movimientodetalleDto.d_Cantidad = FilaDetalles.Cells["cant_3a"].Value == null ? 0 : decimal.Parse(FilaDetalles.Cells["cant_3a"].Value.ToString());
                        _movimientodetalleDto.d_CantidadEmpaque = FilaDetalles.Cells["cant_3a"].Value == null ? 0 : decimal.Parse(FilaDetalles.Cells["cant_3a"].Value.ToString());
                        _movimientodetalleDto.d_Precio = 0;  //FilaDetalles.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(FilaDetalles.Cells["d_Precio"].Value.ToString());
                        _movimientodetalleDto.d_Total = _movimientodetalleDto.d_Cantidad * _movimientodetalleDto.d_Precio;
                        _movimientodetalleDto.v_NroPedido = null;
                        _movimientodetalleDto.i_EsProductoFinal = 1; //;
                        _movimientodetalleDto.KeyRegistro = FilaDetalles.Cells["ncom_3a"].Value.ToString();
                        ListaMovimientoDetalle.Add(_movimientodetalleDto);
                        // _TempDetalle_AgregarDto.Add(_objReciboDetalle);
                    }

                    #endregion



                    if (UmNoEncontrados.Count() != 0)
                    {
                        string cuenta = "";
                        foreach (var item in UmNoEncontrados)
                        {
                            cuenta = cuenta + " " + item;

                        }
                        var resp = UltraMessageBox.Show(string.Format("Hay {0} unidad medidas no registradas, Agregue estas unidades antes de continuar", UmNoEncontrados.Distinct().Count() + " " + cuenta), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        var productos = string.Join(", ", UmNoEncontrados);
                        Clipboard.SetText(productos);
                        // if (resp == DialogResult.No  ) 
                        return;

                    }
                    else if (ProductosNoEncontrados.Count() != 0)
                    {
                        string prov = "";
                        foreach (var item in ProductosNoEncontrados)
                        {
                            prov = prov + " " + item;

                        }

                        var resp = UltraMessageBox.Show(string.Format("Hay productos no registrados,Agregue estos productos antes de continuar :" + prov), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        var productos = string.Join(", ", ProductosNoEncontrados);
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

        private void bwkProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                OperationResult pobjOperationResult = new OperationResult();
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    Globals.ProgressbarStatus.i_TotalProgress = ListaMovimiento.Count();


                    #region Empieza el guardado de las importaciones filtradas
                    foreach (movimientoDto Movimiento in ListaMovimiento.AsParallel())
                    {

                        
                        List<movimientodetalleDto> DetallesNotaIngreso = ListaMovimientoDetalle.Where(p => p.KeyRegistro == Movimiento.KeyRegistro).ToList();
                        // Totales Pie Formulario

                        List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                        ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref pobjOperationResult, Movimiento.v_Periodo, Movimiento.v_Mes, (int)TipoDeMovimiento.NotadeIngreso);

                        int MaxMovimiento;
                        MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;
                        MaxMovimiento++;
                        Movimiento.v_Correlativo = MaxMovimiento.ToString("00000000");
                        Movimiento.d_TotalCantidad = DetallesNotaIngreso.Sum(k => k.d_Cantidad); // txtCantidad.Text == "" ? 0 : decimal.Parse(txtCantidad.Text);
                        Movimiento.d_TotalPrecio = DetallesNotaIngreso.Sum(l => l.d_Total);


                        if (Movimiento.KeyRegistro == "16000004")
                        {

                            string h = "!";
                        }
                        string xx = _objMovimientoBL.InsertarMovimiento(ref  pobjOperationResult, Movimiento, Globals.ClientSession.GetAsList(), DetallesNotaIngreso);
                        if (pobjOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Ocurrió un error al Migrar Notas Ingreso" + Movimiento.KeyRegistro, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
