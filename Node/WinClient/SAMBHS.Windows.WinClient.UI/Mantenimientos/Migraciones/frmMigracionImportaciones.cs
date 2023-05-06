using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Contabilidad.BL;
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
    public partial class frmMigracionImportaciones : Form
    {
        List<importacionDto> ListaImportaciones = new List<importacionDto>();
        List<importaciondetallefobDto> ListaDetalleFOB = new List<importaciondetallefobDto>();
        List<importaciondetalleproductoDto> ListaDetalleProducto = new List<importaciondetalleproductoDto>();
        List<importaciondetallegastosDto> ListaDetalleGastos = new List<importaciondetallegastosDto>();
        ImportacionBL _objImportacionBL = new ImportacionBL();

        public frmMigracionImportaciones(string p)
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

            //foreach (UltraGridCell cell in grdCabecera.Rows[0].Cells)
            //{
            //    string ColumnKey = cell.Column.Key;
            //    if (ColumnKey != "Nuevo")
            //    {
            //        cboCabTipoDoc.Items.Add(ColumnKey);
            //        cboCabSerie.Items.Add(ColumnKey);
            //        cboCabCorrelativo.Items.Add(ColumnKey);
            //        cboCabVia.Items.Add(ColumnKey);
            //        cboCabDestino.Items.Add(ColumnKey);
            //        cboCabFechaRegistro.Items.Add(ColumnKey);
            //        cboFobFechaEmision.Items.Add(ColumnKey);
            //        cboCabTipoCambio.Items.Add(ColumnKey);
            //        cboCabBL.Items.Add(ColumnKey);
            //        cboCabMoneda.Items.Add(ColumnKey);
            //        cboCabEstado.Items.Add(ColumnKey);
            //        cboCabOrden.Items.Add(ColumnKey);
            //        cboCabVencimiento.Items.Add(ColumnKey);
            //        cboCabEnt11.Items.Add(ColumnKey);
            //        cboCabEnt12.Items.Add(ColumnKey);
            //        cboCabFechaLlegada.Items.Add(ColumnKey);
            //        cboCabAgencia.Items.Add(ColumnKey);
            //        cboCabTerminal.Items.Add(ColumnKey);
            //        cboCabOrden.Items.Add(ColumnKey);
            //        cboCabVencimiento.Items.Add(ColumnKey);
            //        cboCabEnt11.Items.Add(ColumnKey);
            //        cboCabEnt12.Items.Add(ColumnKey);
            //        cboCabFechaLlegada.Items.Add(ColumnKey);
            //        cboCabAgencia.Items.Add(ColumnKey);
            //        cboCabTerminal.Items.Add(ColumnKey);
            //        cboCabFechaArrivo.Items.Add(ColumnKey);
            //        cboCabEnt21.Items.Add(ColumnKey);
            //        cboCabEnt22.Items.Add(ColumnKey);

            //        cboCabUtilidad.Items.Add(ColumnKey);
            //        cboCabAlmacen.Items.Add(ColumnKey);

            //        cboCabEstablecimiento.Items.Add(ColumnKey);
            //        cboCabIgv.Items.Add(ColumnKey);

            //        cboCabUtilidad.Items.Add(ColumnKey);

            //    }
            //}
        }



        private void btnImportarFOB_Click(object sender, EventArgs e)
        {
            if (grdDetallesFob.Rows.Count() > 0)
            {
                grdDetallesFob.Selected.Rows.AddRange((UltraGridRow[])grdDetallesFob.Rows.All);
                grdDetallesFob.DeleteSelectedRows(false);
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


                                UltraDataColumn column = this.DataSourceFOB.Band.Columns.Add(columnKey);

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

                            this.DataSourceFOB.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdDetallesFob.SetDataBinding(this.DataSourceFOB, "Band 0");


            lblContadorFilasFob.Text = string.Format("Se encontraron {0} registros", grdDetallesFob.Rows.Count());

            //foreach (UltraGridCell cell in grdDetallesFob.Rows[0].Cells)
            //{
            //    string ColumnKey = cell.Column.Key;
            //    if (ColumnKey != "Nuevo")
            //    {
            //        cboFobProveedor.Items.Add(ColumnKey);
            //        cboFobTipoDoc.Items.Add(ColumnKey);
            //        cboFobSerieDoc.Items.Add(ColumnKey);
            //        cboFobNumeroDoc.Items.Add(ColumnKey);
            //        cboFobFechaEmision.Items.Add(ColumnKey);
            //        cboFobTipoCambio.Items.Add(ColumnKey);
            //        cboFobValorFob.Items.Add(ColumnKey);
            //        cboFobPedido.Items.Add(ColumnKey);

            //    }
            //}
        }

        private void btnImportarProductos_Click(object sender, EventArgs e)
        {
            if (grdDetallesProductos.Rows.Count() > 0)
            {
                grdDetallesProductos.Selected.Rows.AddRange((UltraGridRow[])grdDetallesProductos.Rows.All);
                grdDetallesProductos.DeleteSelectedRows(false);
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

                    this.DataSourceProductos.Reset();

                    bool isHeaderRow = true;
                    foreach (WorksheetRow row in worksheet.Rows)
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                string columnKey = cell.GetText();


                                UltraDataColumn column = this.DataSourceProductos.Band.Columns.Add(columnKey);

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

                            this.DataSourceProductos.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdDetallesProductos.SetDataBinding(this.DataSourceProductos, "Band 0");
            lblContadorFilasProductos.Text = string.Format("Se encontraron {0} registros", grdDetallesFob.Rows.Count());

            //foreach (UltraGridCell cell in grdDetallesProductos.Rows[0].Cells)
            //{
            //    string ColumnKey = cell.Column.Key;
            //    if (ColumnKey != "Nuevo")
            //    {
            //        cboProdProducto.Items.Add(ColumnKey);
            //        cboProdCantidad.Items.Add(ColumnKey);
            //        cboProdUm.Items.Add(ColumnKey);
            //        cboProdPrecioUnitario.Items.Add(ColumnKey);
            //        cboProdValorFob.Items.Add(ColumnKey);
            //        cboProdFlete.Items.Add(ColumnKey);
            //        cboProdSeguro.Items.Add(ColumnKey);
            //        cboProdAdvalorem.Items.Add(ColumnKey);
            //        cboProdIgv.Items.Add(ColumnKey);
            //        cboProdOtrosGastos.Items.Add(ColumnKey);
            //        cboProdTotal.Items.Add(ColumnKey);
            //        cboProdCostoUnitario.Items.Add(ColumnKey);
            //        cboProdPrecioVenta.Items.Add(ColumnKey);
            //        cboProdFactura.Items.Add(ColumnKey);
            //        cboProdProveedor.Items.Add(ColumnKey);
            //        cboProdPedido.Items.Add(ColumnKey);



            //    }
            //}
        }

        private void btnImportarGastos_Click(object sender, EventArgs e)
        {

            if (grdDetallesGastos.Rows.Count() > 0)
            {
                grdDetallesGastos.Selected.Rows.AddRange((UltraGridRow[])grdDetallesGastos.Rows.All);
                grdDetallesGastos.DeleteSelectedRows(false);
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


            grdDetallesGastos.SetDataBinding(this.DataSourceGastos, "Band 0");
            lblContadorFilasGastos.Text = string.Format("Se encontraron {0} registros", grdDetallesGastos.Rows.Count());

            //foreach (UltraGridCell cell in grdDetallesGastos.Rows[0].Cells)
            //{
            //    string ColumnKey = cell.Column.Key;
            //    if (ColumnKey != "Nuevo")
            //    {
            //        cboGastosCod.Items.Add(ColumnKey);
            //        cboGastosCuenta.Items.Add(ColumnKey);
            //        cboGastosTipoDoc.Items.Add(ColumnKey);
            //        cboGastosSerieDoc.Items.Add(ColumnKey);
            //        cboGastosCorrelativoDoc.Items.Add(ColumnKey);
            //        cboGastosFechaEmision.Items.Add(ColumnKey);
            //        cboGastosDetalle.Items.Add(ColumnKey);
            //        cboGastosTipoCambio.Items.Add(ColumnKey);
            //        cboGastosMoneda.Items.Add(ColumnKey);
            //        cboGastosValorVenta.Items.Add(ColumnKey);
            //        cboGastosVNoAfecto.Items.Add(ColumnKey);
            //        cboGastosIgv.Items.Add(ColumnKey);
            //        cboGastosTotalSoles.Items.Add(ColumnKey);
            //        cboGastosTotalDolares.Items.Add(ColumnKey);
            //        cboGastosCentroCosto.Items.Add(ColumnKey);
            //        cboGastosGlosa.Items.Add(ColumnKey);
            //        cboGastosTipoDocRef.Items.Add(ColumnKey);
            //        cboGastosSerieRef.Items.Add(ColumnKey);
            //        cboGastosCorrelativoRef.Items.Add(ColumnKey);

            //    }
            //}
        }

        private void cboFormatoFechas_Click(object sender, EventArgs e)
        {

            if (grdCabecera.Rows.Count() > 0)
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {


                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fecemi_3"].Value != null && p.Cells["fecemi_3"].Value.ToString() != "  -   -" && p.Cells["fecemi_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fecemi_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fecemi_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }



                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fecreg_3"].Value != null && p.Cells["fecreg_3"].Value.ToString() != "  -   -" && p.Cells["fecreg_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fecreg_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fecreg_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }





                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcomreg_3"].Value != null && p.Cells["fcomreg_3"].Value.ToString() != "  -   -" && p.Cells["fcomreg_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcomreg_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcomreg_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }

                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcomfle_3"].Value != null && p.Cells["fcomfle_3"].Value.ToString() != "  -   -" && p.Cells["fcomfle_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcomfle_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcomfle_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }


                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcompse_3"].Value != null && p.Cells["fcompse_3"].Value.ToString() != "  -   -" && p.Cells["fcompse_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcompse_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcompse_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }


                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcomadv_3"].Value != null && p.Cells["fcomadv_3"].Value.ToString() != "  -   -" && p.Cells["fcomadv_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcomadv_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcomadv_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }

                    try
                    {
                        var xx = grdCabecera.Rows.Where(p => p.Cells["fcomigv_3"].Value != null && p.Cells["fcomigv_3"].Value.ToString() != "  -   -" && p.Cells["fcomigv_3"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcomigv_3"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcomigv_3"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }

                }
            }


            if (grdDetallesFob.Rows.Count() > 0)
            {

                try
                {

                    var xx = grdDetallesFob.Rows.Where(p => p.Cells["femi_35"].Value != null && p.Cells["femi_35"].Value.ToString() != "  -   -" && p.Cells["femi_35"].Value.ToString().Trim().Length == 5).ToList();

                    xx.ForEach(o => o.Cells["femi_35"].Value = DateTime.FromOADate(double.Parse(o.Cells["femi_35"].Value.ToString())));
                }
                catch (Exception ex)
                {
                    UltraMessageBox.Show(ex.Message);
                }

            }



            if (grdDetallesGastos.Rows.Count() > 0)
            {
                if (cboGastosFechaEmision.Text != "--Seleccionar--")
                {

                    try
                    {

                        var xx = grdDetallesGastos.Rows.Where(p => p.Cells["fcom_31"].Value != null && p.Cells["fcom_31"].Value.ToString() != "  -   -" && p.Cells["fcom_31"].Value.ToString().Trim().Length == 5).ToList();

                        xx.ForEach(o => o.Cells["fcom_31"].Value = DateTime.FromOADate(double.Parse(o.Cells["fcom_31"].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }



            }


            var resp = UltraMessageBox.Show("Proceso Terminado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);



        }

        private bool ValidarCuentasGeneracionLibroDiario()
        {

            string ConceptoFlete = ((int)Concepto.Flete).ToString();
            string ConceptoProveedorFlete = ((int)Concepto.ProveedoresFlete).ToString();

            if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoFlete) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorFlete)) { }
            else
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Flete", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string ConceptoFob = ((int)Concepto.ValorFob).ToString();
            string ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();

            if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoFob) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedoresFob)) { }
            else
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  FOB", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string ConceptoSeguro = ((int)Concepto.Seguro).ToString();
            string ConceptoProveedorSeguro = ((int)Concepto.ProveedoresSeguro).ToString();

            if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoSeguro) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorSeguro)) { }
            else
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Seguro", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string ConceptoAdValorem = ((int)Concepto.AdValorem).ToString();
            string ConceptoProveedorAdvalorem = ((int)Concepto.ProveedoresAdvalorem).ToString();

            if ((_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoAdValorem) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorAdvalorem))) { }
            else
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  AdValorem", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string ConceptoIgv = ((int)Concepto.Igv).ToString();
            string ConceptoProveedorIgv = ((int)Concepto.ProveedorIgv).ToString();


            if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoIgv) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorIgv)) { }
            else
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Igv", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string Percepcion = ((int)Concepto.Percepcion).ToString();
            string ConceptoProveedorPercepcion = ((int)Concepto.ProveedorPercepcion).ToString();

            if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(Percepcion) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorPercepcion)) { }
            else
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Percepción", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;



        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {

            ClienteBL _objClienteBL = new ClienteBL();
            ProductoBL _objProductoBL = new ProductoBL();
            DatahierarchyBL _objDatahierachyBL = new DatahierarchyBL();
            importacionDto ImportacionesDto = new importacionDto();
            OperationResult pobjOperationResult = new OperationResult();
            DocumentoBL _objDocumentosBL = new DocumentoBL();
            AdministracionConceptosBL _objAdministracionConceptosBL = new AdministracionConceptosBL();
            if (Validar.Validate(true, false).IsValid)
            {


                if (ValidarCuentasGeneracionLibroDiario())
                {


                    if (grdCabecera.Rows.Count() == 0)
                    {
                        UltraMessageBox.Show("Por favor importe los excel con las cabeceras ...");
                    }
                    else
                    {



                        string ConceptoFlete = ((int)Concepto.Flete).ToString();
                        string ConceptoProveedorFlete = ((int)Concepto.ProveedoresFlete).ToString();

                        string ConceptoFob = ((int)Concepto.ValorFob).ToString();
                        string ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();


                        string ConceptoSeguro = ((int)Concepto.Seguro).ToString();
                        string ConceptoProveedorSeguro = ((int)Concepto.ProveedoresSeguro).ToString();

                        string ConceptoAdValorem = ((int)Concepto.AdValorem).ToString();
                        string ConceptoProveedorAdvalorem = ((int)Concepto.ProveedoresAdvalorem).ToString();

                        string ConceptoIgv = ((int)Concepto.Igv).ToString();
                        string ConceptoProveedorIgv = ((int)Concepto.ProveedorIgv).ToString();

                        string Percepcion = ((int)Concepto.Percepcion).ToString();
                        string ConceptoProveedorPercepcion = ((int)Concepto.ProveedorPercepcion).ToString();

                        var FleteCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoFlete);
                        var ProveedorFleteCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref  pobjOperationResult, ConceptoProveedorFlete);
                        var FobCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoFob);
                        var ProveedorFobCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoProveedoresFob);
                        var SeguroCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoSeguro);
                        var SeguroProveedorCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoProveedorSeguro);
                        var AdvaloremCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoAdValorem);
                        var AdvaloremProveedorCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoProveedorAdvalorem);
                        var IgvCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoIgv);
                        var IgvProveedorCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, ConceptoProveedorIgv);
                        var PercepcionCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, Percepcion);
                        var PercepcionProveedorCuenta = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref pobjOperationResult, Percepcion);
                        //List<conceptoDto> ListaConcenptos =   
                        string Mensaje = "Los asientos se generarán con  " + "Flete :" + FleteCuenta.v_CuentaPVenta + "\n" + " Proveedor Flete : " + ProveedorFleteCuenta.v_CuentaPVenta  + "\n" + " Fob :" + FobCuenta.v_CuentaPVenta  + "\n"+
                            "Proveedor Fob : "+ProveedorFobCuenta.v_CuentaPVenta  +"  Seguro :"+SeguroCuenta .v_CuentaPVenta  + " Proveedor Seguro "+SeguroProveedorCuenta.v_CuentaPVenta  + "  Ad Valorem : "+AdvaloremCuenta.v_CuentaPVenta  +"  Proveedor Advalorem :"+ AdvaloremProveedorCuenta.v_CuentaPVenta +
                            "Igv :"+ IgvCuenta.v_CuentaPVenta  +" Proveedor Igv  "+ "Percepción :"+PercepcionCuenta.v_CuentaPVenta  +"Proveedor Percepción "+ PercepcionProveedorCuenta.v_CuentaPVenta ;
                        if (UltraMessageBox.Show("Los Asientos de las Importaciones se generaràn con las siguientes cuentas : " + Mensaje + "¿Desea Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            return;
                        List<KeyValueDTO> Proveedores = _objClienteBL.DevuelveProveedores();
                        List<KeyValueDTO> Productos = _objProductoBL.DevolverProductos();
                        List<KeyValueDTO> UM = _objDatahierachyBL.GetDataHierarchyForComboKeyValueDto(ref pobjOperationResult, 17, "");
                        List<KeyValueDTO> Documentos = _objDocumentosBL.TodosDocumentos(ref pobjOperationResult);


                        List<string> ProductosNoEncontrados = new List<string>();
                        List<string> ProveedoresNoEncontrados = new List<string>();
                        List<string> UmNoEncontrados = new List<string>();


                        try
                        {
                            #region Procesar Cabeceras



                            List<UltraGridRow> Importaciones = grdCabecera.Rows.AsParallel()

                                                                    .OrderBy(x => x.Cells["cncom_3"].Value.ToString()).ToList();
                            foreach (UltraGridRow Fila in Importaciones)
                            {

                                ImportacionesDto = new importacionDto();
                                try
                                {




                                    DateTime FechaRegistro = DateTime.Parse(Fila.Cells["fcomreg_3"].Value.ToString());
                                    ImportacionesDto.v_Periodo = FechaRegistro.Year.ToString("0000");
                                    ImportacionesDto.v_Mes = FechaRegistro.Month.ToString("00");
                                    // ImportacionesDto.v_Correlativo = Fila.Cells["cncom_3"].Value.ToString().Substring(2, 8);
                                    ImportacionesDto.i_Igv = 1; //Actual 18
                                    ImportacionesDto.i_IdTipoDocumento = Fila.Cells["ctdoc_3"].Value != null ? int.Parse(Fila.Cells["ctdoc_3"].Value.ToString()) : -1;
                                    ImportacionesDto.i_IdSerieDocumento = 1;
                                    ImportacionesDto.v_CorrelativoDocumento = Fila.Cells["cndoc_3"].Value != null ? Fila.Cells["cndoc_3"].Value.ToString().ToUpper() : null;
                                    ImportacionesDto.i_IdTipoVia = Fila.Cells["cvia_3"].Value != null ? int.Parse(Fila.Cells["cvia_3"].Value.ToString()) : -1;
                                    ImportacionesDto.i_IdDestino = Fila.Cells["dest_3"].Value != null ? int.Parse(Fila.Cells["dest_3"].Value.ToString()) : -1;
                                    ImportacionesDto.t_FechaRegistro = Fila.Cells["fcomreg_3"].Value == null || Fila.Cells["fcomreg_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcomreg_3"].Value.ToString());
                                    ImportacionesDto.t_FechaEmision = Fila.Cells["fecreg_3"].Value == null || Fila.Cells["fecreg_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fecreg_3"].Value.ToString());
                                    ImportacionesDto.v_Bl = "";
                                    ImportacionesDto.d_TipoCambio = Fila.Cells["ntcam_3"].Value != null ? decimal.Parse(Fila.Cells["ntcam_3"].Value.ToString()) : 0;
                                    ImportacionesDto.i_IdMoneda = Fila.Cells["nmone_3"].Value != null ? int.Parse(Fila.Cells["nmone_3"].Value.ToString()) : -1;
                                    ImportacionesDto.i_IdEstado = Fila.Cells["nflag_3"].Value != null ? int.Parse(Fila.Cells["nflag_3"].Value.ToString()) : -1;
                                    ImportacionesDto.v_NroOrden = "";
                                    ImportacionesDto.t_FechaPagoVencimiento = Fila.Cells["fecemi_3"].Value == null || Fila.Cells["fecemi_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fecemi_3"].Value.ToString());
                                    ImportacionesDto.v_Ent1Serie = "";
                                    ImportacionesDto.v_Ent1Correlativo = "";
                                    ImportacionesDto.t_FechaLLegada = ImportacionesDto.t_FechaRegistro.Value;
                                    ImportacionesDto.i_IdAgencia = -1;
                                    ImportacionesDto.v_Terminal = "";
                                    ImportacionesDto.t_FechaArrivo = ImportacionesDto.t_FechaRegistro.Value;
                                    ImportacionesDto.v_Ent2Serie = "";
                                    ImportacionesDto.v_Ent2Correlativo = "";
                                    ImportacionesDto.i_IdAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value;
                                    ImportacionesDto.d_Utilidad = 0;
                                    ImportacionesDto.i_IdEstablecimiento = Fila.Cells["codest_3"].Value != null ? int.Parse(Fila.Cells["codest_3"].Value.ToString()) : -1;
                                    ImportacionesDto.d_TotalValorFob = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["nvfobf1_3"].Value != null ? decimal.Parse(Fila.Cells["nvfobf1_3"].Value.ToString()) : 0 : Fila.Cells["nvfobf2_3"].Value != null ? decimal.Parse(Fila.Cells["nvfobf2_3"].Value.ToString()) : 0;
                                    ImportacionesDto.d_Flete = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["nflete1_3"].Value != null ? decimal.Parse(Fila.Cells["nflete1_3"].Value.ToString()) : 0 : Fila.Cells["nflete2_3"].Value != null ? decimal.Parse(Fila.Cells["nflete2_3"].Value.ToString()) : 0;
                                    ImportacionesDto.i_IdTipoDocumento1 = Fila.Cells["ctdocfle_3"].Value != null && Fila.Cells["ctdocfle_3"].Value.ToString() != "" ? int.Parse(Fila.Cells["ctdocfle_3"].Value.ToString()) : -1;
                                    ImportacionesDto.v_SerieDocumento1 = Fila.Cells["csdocfle_3"].Value != null && Fila.Cells["csdocfle_3"].Value.ToString() != "" ? Fila.Cells["csdocfle_3"].Value.ToString() : "";
                                    ImportacionesDto.v_CorrelativoDocumento1 = Fila.Cells["cndocfle_3"].Value != null && Fila.Cells["cndocfle_3"].Value.ToString() != "" ? Fila.Cells["cndocfle_3"].Value.ToString() : "";
                                    ImportacionesDto.t_FechaEmisionDoc1 = Fila.Cells["fcomfle_3"].Value == null || Fila.Cells["fcomfle_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcomfle_3"].Value.ToString());
                                    ImportacionesDto.d_TipoCambioDoc1 = Fila.Cells["tcamfle_3"].Value != null ? decimal.Parse(Fila.Cells["tcamfle_3"].Value.ToString()) : 0;
                                    string CodCliente1 = Fila.Cells["cfichfle_3"].Value.ToString().Trim();

                                    if (!string.IsNullOrEmpty(CodCliente1))
                                    {
                                        KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodCliente1).FirstOrDefault();

                                        if (Cliente != null)
                                        {
                                            ImportacionesDto.v_IdClienteDoc1 = Cliente.Id;

                                        }
                                        else
                                        {

                                            ProveedoresNoEncontrados.Add(CodCliente1);
                                        }
                                    }
                                    else
                                    {
                                        //  ProveedoresNoEncontrados.Add(CodCliente1);
                                    }

                                    ImportacionesDto.i_PagaSeguro = int.Parse(Fila.Cells["nasgr_3"].Value.ToString()) == 2 ? 0 : 1;
                                    ImportacionesDto.d_PagoSeguro = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["nsgr1_3"].Value != null ? decimal.Parse(Fila.Cells["nsgr1_3"].Value.ToString()) : 0 : Fila.Cells["nsgr2_3"].Value != null ? decimal.Parse(Fila.Cells["nsgr2_3"].Value.ToString()) : 0;
                                    ImportacionesDto.i_IdTipoDocumento2 = Fila.Cells["ctdocseg_3"].Value != null && Fila.Cells["ctdocseg_3"].Value.ToString() != "" ? int.Parse(Fila.Cells["ctdocseg_3"].Value.ToString()) : -1;
                                    ImportacionesDto.v_SerieDocumento2 = Fila.Cells["csdocseg_3"].Value != null && Fila.Cells["csdocseg_3"].Value.ToString() != "" ? Fila.Cells["csdocseg_3"].Value.ToString() : "";
                                    ImportacionesDto.v_CorrelativoDocumento2 = Fila.Cells["cndocseg_3"].Value != null && Fila.Cells["cndocseg_3"].Value.ToString() != "" ? Fila.Cells["cndocseg_3"].Value.ToString() : "";
                                    ImportacionesDto.t_FechaEmisionDoc2 = Fila.Cells["fcompse_3"].Value == null || Fila.Cells["fcompse_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcompse_3"].Value.ToString());
                                    ImportacionesDto.d_TipoCambioDoc2 = Fila.Cells["tcampse_3"].Value != null ? decimal.Parse(Fila.Cells["tcampse_3"].Value.ToString()) : 0;

                                    string CodCliente2 = Fila.Cells["cfichseg_3"].Value.ToString().Trim();

                                    if (!string.IsNullOrEmpty(CodCliente2))
                                    {
                                        KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodCliente2).FirstOrDefault();

                                        if (Cliente != null)
                                        {
                                            ImportacionesDto.v_IdClienteDoc2 = Cliente.Id;

                                        }
                                        else
                                        {

                                            ProveedoresNoEncontrados.Add(CodCliente2);
                                        }
                                    }
                                    else
                                    {
                                        //  ProveedoresNoEncontrados.Add(CodCliente2);
                                    }


                                    ImportacionesDto.i_AdValorem = int.Parse(Fila.Cells["naadv_3"].Value.ToString()) == 2 ? 0 : 1;
                                    ImportacionesDto.d_AdValorem = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["nimpad1_3"].Value != null ? decimal.Parse(Fila.Cells["nimpad1_3"].Value.ToString()) : 0 : Fila.Cells["nimpad2_3"].Value != null ? decimal.Parse(Fila.Cells["nimpad2_3"].Value.ToString()) : 0;
                                    ImportacionesDto.i_IdTipoDocumento3 = Fila.Cells["ctdocadv_3"].Value != null && Fila.Cells["ctdocadv_3"].Value.ToString() != "" ? int.Parse(Fila.Cells["ctdocadv_3"].Value.ToString()) : -1;
                                    ImportacionesDto.v_SerieDocumento3 = Fila.Cells["csdocadv_3"].Value != null && Fila.Cells["csdocadv_3"].Value.ToString() != "" ? Fila.Cells["csdocadv_3"].Value.ToString() : "";
                                    ImportacionesDto.v_CorrelativoDocumento3 = Fila.Cells["cndocadv_3"].Value != null && Fila.Cells["cndocadv_3"].Value.ToString() != "" ? Fila.Cells["cndocadv_3"].Value.ToString() : "";
                                    ImportacionesDto.t_FechaEmisionDoc3 = Fila.Cells["fcomadv_3"].Value == null || Fila.Cells["fcomadv_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcomadv_3"].Value.ToString());
                                    ImportacionesDto.d_TipoCambioDoc3 = Fila.Cells["tcamadv_3"].Value != null ? decimal.Parse(Fila.Cells["tcamadv_3"].Value.ToString()) : 0;

                                    ImportacionesDto.d_SubTotal = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["nsubt1_3"].Value != null ? decimal.Parse(Fila.Cells["nsubt1_3"].Value.ToString()) : 0 : Fila.Cells["nsubt2_3"].Value != null ? decimal.Parse(Fila.Cells["nsubt2_3"].Value.ToString()) : 0;


                                    string CodCliente3 = Fila.Cells["cfichadv_3"].Value.ToString().Trim();

                                    if (!string.IsNullOrEmpty(CodCliente3))
                                    {
                                        KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodCliente3).FirstOrDefault();

                                        if (Cliente != null)
                                        {
                                            ImportacionesDto.v_IdClienteDoc3 = Cliente.Id;

                                        }
                                        else
                                        {

                                            ProveedoresNoEncontrados.Add(CodCliente3);
                                        }
                                    }
                                    else
                                    {
                                        // ProveedoresNoEncontrados.Add(CodCliente3);
                                    }


                                    decimal Igv = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["nigv1_3"].Value != null ? decimal.Parse(Fila.Cells["nigv1_3"].Value.ToString()) : 0 : Fila.Cells["nigv2_3"].Value != null ? decimal.Parse(Fila.Cells["nigv2_3"].Value.ToString()) : 0;

                                    ImportacionesDto.d_Tax = Utils.Windows.DevuelveValorRedondeado((Igv * 16) / 18, 0);
                                    ImportacionesDto.i_IdTipoDocumento4 = Fila.Cells["ctdocigv_3"].Value != null && Fila.Cells["ctdocigv_3"].Value.ToString() != "" ? int.Parse(Fila.Cells["ctdocigv_3"].Value.ToString()) : -1;
                                    ImportacionesDto.v_SerieDocumento4 = Fila.Cells["csdocigv_3"].Value != null && Fila.Cells["csdocigv_3"].Value.ToString() != "" ? Fila.Cells["csdocigv_3"].Value.ToString() : "";
                                    ImportacionesDto.v_CorrelativoDoc4 = Fila.Cells["cndocigv_3"].Value != null && Fila.Cells["cndocigv_3"].Value.ToString() != "" ? Fila.Cells["cndocigv_3"].Value.ToString() : "";
                                    ImportacionesDto.t_FechaEmisionDoc4 = Fila.Cells["fcomigv_3"].Value == null || Fila.Cells["fcomigv_3"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["fcomigv_3"].Value.ToString());
                                    ImportacionesDto.d_TipoCambioDoc4 = Fila.Cells["tcamigv_3"].Value != null ? decimal.Parse(Fila.Cells["tcamigv_3"].Value.ToString()) : 0;

                                    string CodCliente4 = Fila.Cells["cfichigv_3"].Value.ToString().Trim();

                                    if (!string.IsNullOrEmpty(CodCliente4))
                                    {
                                        KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodCliente4).FirstOrDefault();

                                        if (Cliente != null)
                                        {
                                            ImportacionesDto.v_IdClienteDoc4 = Cliente.Id;

                                        }
                                        else
                                        {

                                            ProveedoresNoEncontrados.Add(CodCliente4);
                                        }
                                    }
                                    else
                                    {
                                        // ProveedoresNoEncontrados.Add(CodCliente4);
                                    }





                                    ImportacionesDto.d_Prom = Utils.Windows.DevuelveValorRedondeado((Igv * 2) / 18, 0);
                                    ImportacionesDto.d_TasaDespacho = 0;
                                    ImportacionesDto.d_Percepcion = 0;
                                    ImportacionesDto.d_Igv = Igv;
                                    ImportacionesDto.d_Intereses = 0;
                                    ImportacionesDto.d_OtrosGastos = ImportacionesDto.i_IdMoneda == (int)Currency.Soles ? Fila.Cells["notrgs1_3"].Value != null ? decimal.Parse(Fila.Cells["notrgs1_3"].Value.ToString()) : 0 : Fila.Cells["notrgs2_3"].Value != null ? decimal.Parse(Fila.Cells["notrgs2_3"].Value.ToString()) : 0;



                                    ImportacionesDto.KeyRegistro = Fila.Cells["cncom_3"].Value.ToString();
                                    ListaImportaciones.Add(ImportacionesDto);


                                }
                                catch (Exception ex)
                                {
                                    UltraMessageBox.Show(ex.Message);
                                    break;
                                }
                            }

                            #endregion

                            #region Procesar Detalles FOB

                            //List<UltraGridRow> DetallesFob = grdDetallesFob.Rows.Where(p => !string.IsNullOrEmpty(p.Cells["deta_35"].Value.ToString()) && !string.IsNullOrEmpty(p.Cells["tdoc_35"].Value.ToString()) && !string.IsNullOrEmpty(p.Cells["sdoc_35"].Value.ToString()) && !string.IsNullOrEmpty(p.Cells["ndoc_35"].Value.ToString())).ToList();

                            List<UltraGridRow> DetallesFob = grdDetallesFob.Rows.ToList();

                            foreach (UltraGridRow Fila in DetallesFob)
                            {

                                importaciondetallefobDto _objImportaciondetallefobDto = new importaciondetallefobDto();
                                _objImportaciondetallefobDto = new importaciondetallefobDto();
                                _objImportaciondetallefobDto.v_IdImportacion = null;


                                string CodClienteFOB = Fila.Cells["deta_35"].Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(CodClienteFOB))
                                {
                                    KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodClienteFOB).FirstOrDefault();

                                    if (Cliente != null)
                                    {

                                        _objImportaciondetallefobDto.v_IdCliente = Cliente.Id;
                                    }
                                    else
                                    {

                                        ProveedoresNoEncontrados.Add(CodClienteFOB);
                                    }
                                }
                                else
                                {
                                    //  ProveedoresNoEncontrados.Add(CodClienteFOB);
                                }


                                _objImportaciondetallefobDto.i_IdTipoDocumento = Fila.Cells["tdoc_35"].Value == null || Fila.Cells["tdoc_35"].Value.ToString() == "" ? -1 : int.Parse(Fila.Cells["tdoc_35"].Value.ToString());
                                _objImportaciondetallefobDto.v_SerieDocumento = Fila.Cells["sdoc_35"].Value == null && Fila.Cells["sdoc_35"].Value.ToString() == "" ? null : Fila.Cells["sdoc_35"].Value.ToString();
                                _objImportaciondetallefobDto.v_CorrelativoDocumento = Fila.Cells["ndoc_35"].Value == null && Fila.Cells["ndoc_35"].Value.ToString() != "" ? null : Fila.Cells["ndoc_35"].Value.ToString();
                                _objImportaciondetallefobDto.t_FechaEmision = Fila.Cells["femi_35"].Value == null || Fila.Cells["femi_35"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells["femi_35"].Value.ToString());
                                _objImportaciondetallefobDto.d_TipoCambio = Fila.Cells["tcamf_35"].Value == null ? 0 : decimal.Parse(Fila.Cells["tcamf_35"].Value.ToString());
                                _objImportaciondetallefobDto.d_ValorFob = Fila.Cells["vfob2_35"].Value == null ? 0 : decimal.Parse(Fila.Cells["vfob2_35"].Value.ToString());
                                _objImportaciondetallefobDto.v_NroPedido = Fila.Cells["npedi_35"].Value == null || Fila.Cells["npedi_35"].Value == string.Empty ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _objImportaciondetallefobDto.KeyRegistro = Fila.Cells["cncom_35"].Value.ToString();
                                ListaDetalleFOB.Add(_objImportaciondetallefobDto);

                            }

                            #endregion

                            #region Procesar Detalles Producto

                            List<UltraGridRow> DetallesProducto = grdDetallesProductos.Rows.ToList();
                            foreach (UltraGridRow FilaProd in DetallesProducto)
                            {

                                importaciondetalleproductoDto _objImportaciondetalleproductoDto = new importaciondetalleproductoDto();
                                _objImportaciondetalleproductoDto.v_IdImportacion = null;
                                //  string CodProducto = "PROD001";// FilaProd.Cells["clinea_3a"].Value.ToString().Trim();
                                string CodProducto = FilaProd.Cells["codinterno"].Value.ToString().Trim();

                                KeyValueDTO Producto = Productos.Where(p => p.Value1.Trim() == CodProducto.Trim()).FirstOrDefault();

                                if (!string.IsNullOrEmpty(CodProducto))
                                {
                                    if (Producto != null)
                                    {
                                        _objImportaciondetalleproductoDto.v_IdProductoDetalle = Producto.Value2;
                                    }
                                    else
                                    {
                                        ProductosNoEncontrados.Add(CodProducto);
                                    }
                                }
                                else
                                {
                                    // ProveedoresNoEncontrados.Add(CodProducto);
                                }

                                string UnidadMedida = FilaProd.Cells["cunid_3a"].Value.ToString().Trim();

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
                                        _objImportaciondetalleproductoDto.i_IdUnidadMedida = 8;

                                        break;
                                    case "UND":
                                        _objImportaciondetalleproductoDto.i_IdUnidadMedida = 15;
                                        break;

                                    case "CJA":
                                        _objImportaciondetalleproductoDto.i_IdUnidadMedida = 19;
                                        break;

                                    case "SET":
                                        _objImportaciondetalleproductoDto.i_IdUnidadMedida = 21;
                                        break;

                                    case "HJS":

                                        _objImportaciondetalleproductoDto.i_IdUnidadMedida = 22;
                                        break;
                                    case "KGR":
                                        _objImportaciondetalleproductoDto.i_IdUnidadMedida = 5;
                                        break;

                                    default:
                                        UmNoEncontrados.Add(UnidadMedida);
                                        break;
                                }

                                _objImportaciondetalleproductoDto.d_Cantidad = FilaProd.Cells["ncant_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["ncant_3a"].Text);
                                _objImportaciondetalleproductoDto.d_Precio = FilaProd.Cells["npu2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["npu2_3a"].Value.ToString());
                                _objImportaciondetalleproductoDto.d_ValorFob = FilaProd.Cells["nvfob2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["nvfob2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_Flete = FilaProd.Cells["nflete2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["nflete2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_Seguro = FilaProd.Cells["nsgr2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["nsgr2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_AdValorem = FilaProd.Cells["nadv2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["nadv2_3a"].Text);

                                _objImportaciondetalleproductoDto.d_Igv = FilaProd.Cells["nigv2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["nigv2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_OtrosGastos = FilaProd.Cells["notrgs2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["notrgs2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_Total = FilaProd.Cells["ntotal2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["ntotal2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_CostoUnitario = FilaProd.Cells["npui2_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["npui2_3a"].Text);
                                _objImportaciondetalleproductoDto.d_CostoUnitarioCambio = 0;

                                string CodClienteProd = FilaProd.Cells["prove_3a"].Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(CodClienteProd))
                                {
                                    KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodClienteProd).FirstOrDefault();

                                    if (Cliente != null)
                                    {

                                        _objImportaciondetalleproductoDto.v_IdCliente = Cliente.Id;
                                    }
                                    else
                                    {
                                        ProveedoresNoEncontrados.Add(CodClienteProd);
                                    }
                                }
                                else
                                {
                                    ProveedoresNoEncontrados.Add(CodClienteProd);
                                }
                                string[] Fact;
                                var NroFactura = FilaProd.Cells["nfact_3a"].Value == null ? null : FilaProd.Cells["nfact_3a"].Value.ToString();
                                Fact = NroFactura.Split(new Char[] { ' ' });

                                string TipoDoc = Documentos.Where(x => x.Id == Fact[0].Trim()).FirstOrDefault().Value2.ToString();
                                _objImportaciondetalleproductoDto.v_NroFactura = TipoDoc + " " + Fact[1].Trim();
                                _objImportaciondetalleproductoDto.v_NroPedido = FilaProd.Cells["npedi_3a"].Value == null || FilaProd.Cells["npedi_3a"].Value.ToString() == "" ? null : FilaProd.Cells["npedi_3a"].Value.ToString().Trim();
                                _objImportaciondetalleproductoDto.EsServicio = Producto != null ? int.Parse(Productos.FirstOrDefault(p => p.Value1 == CodProducto).Estado.Value.ToString()) : 0;
                                _objImportaciondetalleproductoDto.d_CantidadEmpaque = FilaProd.Cells["ncant_3a"].Value == null ? 0 : decimal.Parse(FilaProd.Cells["ncant_3a"].Text);
                                _objImportaciondetalleproductoDto.d_PrecioVenta = _objImportaciondetalleproductoDto.d_CostoUnitario.Value;
                                //_objImportaciondetalleproductoDto.d_CostoUnitarioCambio = CalcularCostoCambio(_objImportaciondetalleproductoDto.d_Cantidad.Value, decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()), Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString(), Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString());
                                _objImportaciondetalleproductoDto.KeyRegistro = FilaProd.Cells["cncom_3a"].Value.ToString();
                                ListaDetalleProducto.Add(_objImportaciondetalleproductoDto);
                            }
                            #endregion

                            #region Procesar Detalles Gastos

                            List<UltraGridRow> DetallesGastos = grdDetallesGastos.Rows.ToList();
                            foreach (UltraGridRow FilaGastos in DetallesGastos)
                            {

                                importaciondetallegastosDto _objImportaciondetallegastosDto = new importaciondetallegastosDto();
                                _objImportaciondetallegastosDto.v_IdImportacion = null;

                                //switch (FilaGastos.Cells["cgast_31"].Value.ToString())
                                //{


                                //    case "0101"://Gastos Varios
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0100";
                                //        break;

                                //    case "0102": //Gastos Operatvos
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0104";

                                //        break;
                                //    case "0103"://Handiling
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0103";
                                //        break;

                                //    case "0104": //Almacenaje
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0101";
                                //        break;

                                //    case "0107": //VISTO BUENO NAVIERA  CREAR
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0107";
                                //        break;

                                //    case "0111": //transporte
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0102";
                                //        break;
                                //    case "0112": //Gastos Adm. CREAR
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0112";
                                //        break;
                                //    case "0113": //TCC-SERVICIO LOGISTICO  CREAR
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0113";
                                //        break;
                                //    case "0109"://Aduana Crear
                                //        _objImportaciondetallegastosDto.v_GastoImportacion = "0109";

                                //        break;
                                //}
                                _objImportaciondetallegastosDto.v_GastoImportacion = FilaGastos.Cells["cgast_31"].Value.ToString();
                                _objImportaciondetallegastosDto.KeyRegistro = FilaGastos.Cells["cncom_31"].Value.ToString();
                                _objImportaciondetallegastosDto.v_IdAsientoContable = FilaGastos.Cells["cuen_31"].Value == null ? null : FilaGastos.Cells["cuen_31"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdTipoDocumento = FilaGastos.Cells["tdoc_31"].Value == null || FilaGastos.Cells["tdoc_31"].Value.ToString() == "" ? -1 : int.Parse(FilaGastos.Cells["tdoc_31"].Value.ToString());
                                _objImportaciondetallegastosDto.v_SerieDocumento = FilaGastos.Cells["sdoc_31"].Value == null || FilaGastos.Cells["sdoc_31"].Value.ToString() == "" ? "" : FilaGastos.Cells["sdoc_31"].Value.ToString().ToUpper();


                                _objImportaciondetallegastosDto.v_CorrelativoDocumento = FilaGastos.Cells["ndoc_31"].Value == null || FilaGastos.Cells["ndoc_31"].Value.ToString() == "" ? null : FilaGastos.Cells["ndoc_31"].Value.ToString().ToUpper();
                                _objImportaciondetallegastosDto.t_FechaEmision = FilaGastos.Cells["fcom_31"].Value == null || FilaGastos.Cells["fcom_31"].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(FilaGastos.Cells["fcom_31"].Value.ToString().ToUpper());
                                _objImportaciondetallegastosDto.t_FechaRegistro = _objImportaciondetallegastosDto.t_FechaEmision;
                                _objImportaciondetallegastosDto.d_TipoCambio = FilaGastos.Cells["tcamb_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["tcamb_31"].Value.ToString());



                                string CodProvGastos = FilaGastos.Cells["fich_31"].Value == null ? null : FilaGastos.Cells["fich_31"].Value.ToString(); FilaGastos.Cells["fich_31"].Value.ToString().Trim();
                                KeyValueDTO Cliente = Proveedores.Where(p => p.Value1.Trim() == CodProvGastos).FirstOrDefault();

                                if (!string.IsNullOrEmpty(CodProvGastos))
                                {


                                    if (Cliente != null)
                                    {

                                        _objImportaciondetallegastosDto.v_Detalle = Cliente.Id;
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {
                                    //  ProveedoresNoEncontrados.Add(CodProvGastos + " " + "Importación :"+_objImportaciondetallegastosDto.KeyRegistro);
                                }

                                _objImportaciondetallegastosDto.i_IdMoneda = FilaGastos.Cells["mone_31"].Value == null ? -1 : int.Parse(FilaGastos.Cells["mone_31"].Value.ToString());



                                _objImportaciondetallegastosDto.d_ValorVenta = _objImportaciondetallegastosDto.i_IdMoneda == (int)Currency.Soles ? FilaGastos.Cells["vvta1_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["vvta1_31"].Value.ToString()) : FilaGastos.Cells["vvta2_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["vvta2_31"].Value.ToString());
                                _objImportaciondetallegastosDto.d_NAfectoDetraccion = _objImportaciondetallegastosDto.i_IdMoneda == (int)Currency.Soles ? FilaGastos.Cells["vvtan1_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["vvtan1_31"].Value.ToString()) : FilaGastos.Cells["vvtan2_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["vvtan2_31"].Value.ToString());
                                _objImportaciondetallegastosDto.d_Igv = _objImportaciondetallegastosDto.i_IdMoneda == (int)Currency.Soles ? FilaGastos.Cells["igv1_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["igv1_31"].Value.ToString()) : FilaGastos.Cells["igv2_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["igv2_31"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ImporteDolares = FilaGastos.Cells["nimp2_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["nimp2_31"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ImporteSoles = FilaGastos.Cells["nimp1_31"].Value == null ? 0 : decimal.Parse(FilaGastos.Cells["nimp1_31"].Value.ToString());

                                _objImportaciondetallegastosDto.i_CCosto = FilaGastos.Cells["ccos_31"].Value == null || FilaGastos.Cells["ccos_31"].Value.ToString().Trim() == "" ? "-1" : FilaGastos.Cells["ccos_31"].Value.ToString();
                                _objImportaciondetallegastosDto.v_Glosa = FilaGastos.Cells["glos_31"].Value == null ? null : FilaGastos.Cells["glos_31"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdTipoDocRef = FilaGastos.Cells["tdocref_31"].Value == null || FilaGastos.Cells["tdocref_31"].Value.ToString() == "" ? -1 : int.Parse(FilaGastos.Cells["tdocref_31"].Value.ToString());
                                _objImportaciondetallegastosDto.v_SerieDocRef = FilaGastos.Cells["sdocref_31"].Value == null || FilaGastos.Cells["sdocref_31"].Value.ToString() == "" ? null : FilaGastos.Cells["sdocref_31"].Value.ToString();
                                _objImportaciondetallegastosDto.v_CorrelativoDocRef = FilaGastos.Cells["ndocref_31"].Value == null || FilaGastos.Cells["ndocref_31"].Value.ToString() == "" ? null : FilaGastos.Cells["ndocref_31"].Value.ToString();
                                _objImportaciondetallegastosDto.v_DetalleCodigo = Cliente == null ? null : Cliente.Value1.Trim();


                                ListaDetalleGastos.Add(_objImportaciondetallegastosDto);
                            }



                            #endregion

                            if (ProductosNoEncontrados.Count() != 0)
                            {
                                string um = "";
                                foreach (var item in ProductosNoEncontrados)
                                {
                                    um = um + " " + item;

                                }
                                var resp = UltraMessageBox.Show(string.Format("Hay {0} productos no registrados, Agregue estos productos antes de continuar", ProductosNoEncontrados.Distinct().Count() + " " + um), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                var productos = string.Join(", ", ProductosNoEncontrados);
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
                            else if (UmNoEncontrados.Count() != 0)
                            {
                                string um = "";
                                foreach (var item in UmNoEncontrados)
                                {
                                    um = um + " " + item;

                                }

                                var resp = UltraMessageBox.Show(string.Format("Hay {0} Unidad Medidad no registrados, Agregue estas unidades antes de continuar", UmNoEncontrados.Distinct().Count() + " " + um), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                var productos = string.Join(", ", UmNoEncontrados);
                                Clipboard.SetText(productos);
                                //  if (resp == DialogResult.No) 
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
        }

        private void bwkProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Empieza el guardado de las importaciones filtradas
            Globals.ProgressbarStatus.i_TotalProgress = ListaImportaciones.Count();

            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    foreach (importacionDto Imp in ListaImportaciones.AsParallel())
                    {

                        OperationResult objOperationResult = new OperationResult();
                        List<importaciondetallefobDto> DetallesFob = ListaDetalleFOB.Where(p => p.KeyRegistro == Imp.KeyRegistro).ToList();
                        List<importaciondetalleproductoDto> DetallesProd = ListaDetalleProducto.Where(p => p.KeyRegistro == Imp.KeyRegistro).ToList();
                        List<importaciondetallegastosDto> DetallesGastos = ListaDetalleGastos.Where(p => p.KeyRegistro == Imp.KeyRegistro && p.v_IdAsientoContable != null && p.v_IdAsientoContable != "").ToList();

                        // Totales Pie Formulario
                        Imp.d_ValorFob = DetallesProd.Any() ? DetallesProd.Sum(x => x.d_ValorFob) : 0;
                        Imp.d_TotalSeguro = DetallesProd.Any() ? DetallesProd.Sum(x => x.d_Seguro) : 0;
                        Imp.d_TotalIgv = DetallesProd.Any() ? DetallesProd.Sum(x => x.d_Igv) : 0;
                        Imp.d_TotalFlete = DetallesProd.Any() ? DetallesProd.Sum(x => x.d_Flete) : 0;

                        Imp.d_TotalAdValorem = DetallesProd.Any() ? DetallesProd.Sum(x => x.d_AdValorem) : 0;
                        Imp.d_TotalOtrosGastos = DetallesProd.Any() ? DetallesProd.Sum(x => x.d_OtrosGastos) : 0;


                        ImportacionBL _objImportacionBL = new ImportacionBL();

                        var _ListadoImportacionCambioFecha = _objImportacionBL.ObtenerListadoImportaciones(ref objOperationResult, Imp.v_Periodo, Imp.v_Mes);
                        int MaxMovimiento = _ListadoImportacionCambioFecha.Count() > 0 ? int.Parse(_ListadoImportacionCambioFecha[_ListadoImportacionCambioFecha.Count() - 1].Value1.ToString()) : 0;
                        MaxMovimiento++;
                        Imp.v_Correlativo = MaxMovimiento.ToString("00000000");

                        if (Imp.KeyRegistro == "05000011")
                        {
                            string x = "";
                        }
                        //foreach (var item in DetallesGastos)
                        //{
                        //    if (new ComprasBL (). ComprobarRelacionDocumentoProveedor(ref objOperationResult, item.v_Detalle, item.i_IdTipoDocumento.Value, item.v_SerieDocumento, item.v_CorrelativoDocumento, null))
                        //    {
                        //        if (objOperationResult.Success == 1)
                        //        {
                        //            var IdCompra = new ComprasBL().DevolverIdCompra(ref objOperationResult, item.v_Detalle, item.i_IdTipoDocumento.Value, item.v_SerieDocumento, item.v_CorrelativoDocumento);
                        //            new ComprasBL().EliminarCompra(ref objOperationResult, IdCompra, Globals.ClientSession.GetAsList());
                        //        }
                            
                        //    }
                        //}
                        //if (objOperationResult.Success == 1)
                        //{
                            new ImportacionBL().InsertarImportacion(ref objOperationResult, Imp, Globals.ClientSession.GetAsList(), DetallesFob, DetallesProd, DetallesGastos);
                        //}

                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Ocurrió un error al Migrar Importaciones" + Imp.KeyRegistro, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
