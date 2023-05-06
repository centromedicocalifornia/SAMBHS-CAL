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
using SAMBHS.ActivoFijo.BL;
using System.IO;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigracionActivoFijo : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        ActivoFijoBL _objActivoFijoBL = new ActivoFijoBL();
        List<activofijoDto> ListaActivosFijos = new List<activofijoDto>();
        OperationResult pobjOperationResult = new OperationResult();
        activofijoDto _objActivoFijoDto = new activofijoDto();
        List<activofijodetalleDto> _TempDetalle_AgregarDto = new List<activofijodetalleDto>();
        List<string> ProveedoresNoEncontrados = new List<string>();
        List<string> DescripcionVacias = new List<string>();
        List<string> CodigosVacios = new List<string>();

        List<string> TipoActivosNoEncontrados = new List<string>();
        List<string> ClasesNoEncontrados = new List<string>();
        List<string> CCostoNoEncontrados = new List<string>();
        List<string> UbicacionNoEncontrados = new List<string>();
        List<string> DepreciacionNoEncontrados = new List<string>();
        List<string> PorcentajeNoEncontrados = new List<string>();
        List<string> SituacionNoEncontrados = new List<string>();
        public frmMigracionActivoFijo(string p)
        {
            InitializeComponent();
        }



        private void cboImportarCabecera_Click(object sender, EventArgs e)
        {

            int i = 1;
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
                                if (!string.IsNullOrEmpty(columnKey))
                                {
                                    UltraDataColumn column = this.DataSourceCabecera.Band.Columns.Add(columnKey);

                                    switch (columnKey)
                                    {
                                        default:
                                            column.DataType = typeof(string);
                                            break;
                                    }
                                }
                                i++;
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




            foreach (UltraGridCell cell in grdCabecera.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboCodigo.Items.Add(ColumnKey);
                    cboDescripcion.Items.Add(ColumnKey);
                    cboMarca.Items.Add(ColumnKey);
                    cboModelo.Items.Add(ColumnKey);
                    cboSerie.Items.Add(ColumnKey);
                    cboEstado.Items.Add(ColumnKey);
                    cboPlaca.Items.Add(ColumnKey);
                    cboColor.Items.Add(ColumnKey);
                    cboTipoActivo.Items.Add(ColumnKey);
                    cboResponsable.Items.Add(ColumnKey);
                    cboFechaCompra.Items.Add(ColumnKey);
                    cboOrdenCompra.Items.Add(ColumnKey);
                    cboFechaFactura.Items.Add(ColumnKey);
                    cboProveedor.Items.Add(ColumnKey);
                    cboNroFactura.Items.Add(ColumnKey);
                    cboCostoSoles.Items.Add(ColumnKey);
                    cboCostoDolares.Items.Add(ColumnKey);
                    cboFechaUso.Items.Add(ColumnKey);
                    cboSeDepreciara.Items.Add(ColumnKey);
                    cboMesesDepreciar.Items.Add(ColumnKey);
                    cboUbicacion.Items.Add(ColumnKey);
                    cboFoto.Items.Add(ColumnKey);
                    cboCodigoBarras.Items.Add(ColumnKey);
                    cboSituacion.Items.Add(ColumnKey);
                    cboClase.Items.Add(ColumnKey);
                    cboPorcentajeDepreciar.Items.Add(ColumnKey);
                    cboCentroCosto.Items.Add(ColumnKey);
                    cboTipoDocumento.Items.Add(ColumnKey);

                }
            }

        }






        private void btnImportarDetalles_Click(object sender, EventArgs e)
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

                using (new LoadingClass.PleaseWait(this.Location, "Importando ActivoFijo..."))
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


            grdCabecera.SetDataBinding(this.DataSourceGastos, "Band 0");
            lblContadorFilasCabecera.Text = string.Format("Se encontraron {0} registros", grdCabecera.Rows.Count());


        }

        private void cboFormatoFechas_Click(object sender, EventArgs e)
        {

            if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (grdCabecera.Rows.Count() > 0)
                {
                    try
                    {
                        if (cboFechaCompra.Text != "--Seleccionar--")
                        {
                            var xx = grdCabecera.Rows.Where(p => p.Cells[cboFechaCompra.Text].Value != null && p.Cells[cboFechaCompra.Text].Value.ToString() != "  -   -" && p.Cells[cboFechaCompra.Text].Value.ToString().Trim().Length == 5).ToList();
                            xx.ForEach(o => o.Cells[cboFechaCompra.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaCompra.Text].Value.ToString())));
                        }
                        if (cboFechaUso.Text != "--Seleccionar--")
                        {
                            var YY = grdCabecera.Rows.Where(p => p.Cells[cboFechaUso.Text].Value != null && p.Cells[cboFechaUso.Text].Value.ToString() != "  -   -" && p.Cells[cboFechaUso.Text].Value.ToString().Trim().Length == 5).ToList();
                            YY.ForEach(o => o.Cells[cboFechaUso.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaUso.Text].Value.ToString())));

                        }

                        if (cboFechaFactura.Text != "--Seleccionar--")
                        {
                            var zz = grdCabecera.Rows.Where(p => p.Cells[cboFechaFactura.Text].Value != null && p.Cells[cboFechaFactura.Text].Value.ToString() != "  -   -" && p.Cells[cboFechaFactura.Text].Value.ToString().Trim().Length == 5).ToList();

                            zz.ForEach(o => o.Cells[cboFechaFactura.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaFactura.Text].Value.ToString())));
                        }
                        var resp = UltraMessageBox.Show("Fechas Covertidas a formato requerido por el sistema", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnGuardar.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }

            }

        }
        public byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ListaActivosFijos = new List<activofijoDto>();
            ProveedoresNoEncontrados = new List<string>();
            TipoActivosNoEncontrados = new List<string>();
            DepreciacionNoEncontrados = new List<string>();
            PorcentajeNoEncontrados = new List<string>();
            UbicacionNoEncontrados = new List<string>();

            if (cboMesesDepreciar.Text != "--Seleccionar--" && cboPorcentajeDepreciar.Text != "--Seleccionar--")
            {
                UltraMessageBox.Show("No se puede asociar los meses y los porcentajes de depreciación a la misma vez. Seleccione solo uno.");
                return;

            }
            if (Validar.Validate(true, false).IsValid)
            {

                AdministracionConceptosBL _objAdministracionConceptosBL = new AdministracionConceptosBL();
                OperationResult pobjOperationResult = new OperationResult();
                List<KeyValueDTO> Proveedores = _objClienteBL.DevuelveProveedores();
                List<KeyValueDTO> ListaTiposActivo = new DatahierarchyBL().GetDataHierarchiesForCombo(ref pobjOperationResult, 104, null);
                List<KeyValueDTO> ListaClase = new DatahierarchyBL().GetDataHierarchiesForCombo(ref pobjOperationResult, 174, null);
                List<KeyValueDTO> ListaSituacion = new DatahierarchyBL().GetDataHierarchiesForCombo(ref pobjOperationResult, 173, null);
                List<KeyValueDTO> ListaCentroCosto= new DatahierarchyBL().GetDataHierarchiesForCombo(ref pobjOperationResult, 31, null);
                List<KeyValueDTO> ListaUbicacion = new DatahierarchyBL().GetDataHierarchiesForCombo(ref pobjOperationResult, 103, null);
                List<KeyValueDTO> Trabajadores = _objClienteBL.DevuelveTrabajadores();
                List<KeyValueDTO> ListaDepreciacion = new DatahierarchyBL().GetDataHierarchiesForCombo(ref pobjOperationResult, 109, null);

                if (grdCabecera.Rows.Count() == 0)
                {
                    UltraMessageBox.Show("Por favor importe el excel de los activos fijos ...");
                }
                else
                {

                    clienteDto _clienteDto = new clienteDto();
                    try
                    {
                        #region Procesar Cabeceras

                        List<UltraGridRow> ActivoFijo = grdCabecera.Rows.OrderBy(x => x.Cells[cboCodigo.Text].Value.ToString()).ToList();
                        int i = 0;
                        foreach (UltraGridRow Fila in ActivoFijo)
                        {

                            _objActivoFijoDto = new activofijoDto();

                            try
                            {
                                i = i + 1;
                                DateTime FechaCompra = cboFechaCompra.Text == "--Seleccionar--" ? DateTime.Now : Fila.Cells[cboFechaCompra.Text].Value == null || Fila.Cells[cboFechaCompra.Text].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells[cboFechaCompra.Text].Value.ToString());
                                DateTime FechaUso = cboFechaUso.Text == "--Seleccionar--" ? DateTime.Now : Fila.Cells[cboFechaUso.Text].Value == null || Fila.Cells[cboFechaUso.Text].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells[cboFechaUso.Text].Value.ToString());
                                DateTime FechaFactura = cboFechaFactura.Text == "--Seleccionar--" ? DateTime.Now : Fila.Cells[cboFechaFactura.Text].Value == null || Fila.Cells[cboFechaFactura.Text].Value.ToString() == "  -   -" ? DateTime.Now : DateTime.Parse(Fila.Cells[cboFechaFactura.Text].Value.ToString());
                                _objActivoFijoDto.v_CodigoAnterior = Fila.Cells[cboCodigo.Text].Value.ToString().ToUpper().ToString();//"10000"+ Fila.Cells["codigo_activo"].Value.ToString().Substring (4,3);

                                _objActivoFijoDto.i_IdTipoMotivo = 1;
                                _objActivoFijoDto.v_Descricpion = Fila.Cells[cboDescripcion.Text].Value.ToString().ToUpper();
                                _objActivoFijoDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                                _objActivoFijoDto.v_Marca = Fila.Cells[cboMarca.Text].Value.ToString().ToUpper();
                                _objActivoFijoDto.v_Modelo = Fila.Cells[cboModelo.Text].Value.ToString().ToUpper();
                                _objActivoFijoDto.v_Serie = cboSerie.Text == "--Seleccionar--" ? "" : Fila.Cells[cboSerie.Text].Value.ToString().ToUpper();
                                _objActivoFijoDto.v_Placa = cboPlaca.Text == "--Seleccionar--" ? "" : Fila.Cells[cboPlaca.Text].Value.ToString().ToUpper();
                                _objActivoFijoDto.i_IdTipoAdquisicion = 2;//COMPRAS PAIS
                                _objActivoFijoDto.v_Color = cboColor.Text == "--Seleccionar--" ? "" : Fila.Cells[cboColor.Text].Value.ToString().ToUpper();
                                _objActivoFijoDto.t_FechaOrdenCompra = FechaCompra;
                                _objActivoFijoDto.i_IdEstado = Fila.Cells[cboEstado.Text].Value.ToString().Contains("B") ? 1 : Fila.Cells[cboEstado.Text].Value.ToString().Contains("M") ? 2 : Fila.Cells[cboEstado.Text].Value.ToString().Contains("R") ? 3 : -1;
                                _objActivoFijoDto.i_IdTipoIntangible = -1;
                                _objActivoFijoDto.d_ValorActualizadoMe = 0;
                                _objActivoFijoDto.d_ValorActualizadoMn = 0;
                                _objActivoFijoDto.d_ValorAdquisicionMn = 0;     //Valor Depreciado
                                _objActivoFijoDto.d_ValorAdquisicionMe = 0;
                                _objActivoFijoDto.v_UbicacionFoto = lblDirectorioGuardar.Text;
                                _objActivoFijoDto.i_IdTipoDocumento = cboTipoDocumento.Text == "--Seleccionar--" ? -1 : int.Parse ( Fila.Cells[cboTipoDocumento.Text].Value.ToString());
                                try
                                {
                                    string Foto = lblDirectorioGuardar.Text + "\\" + Fila.Cells[cboFoto.Text].Value.ToString();
                                    Image img = Image.FromFile(Foto);
                                    byte[] bArr = imgToByteArray(img);
                                    _objActivoFijoDto.b_Foto = bArr;
                                }
                                catch (Exception ex)
                                {

                                }

                                _objActivoFijoDto.v_OrdenCompra = cboOrdenCompra.Text == "--Seleccionar--" ? "" : Fila.Cells[cboOrdenCompra.Text].Value.ToString();

                                string CodProveedor = cboProveedor.Text == "--Seleccionar--" ? "" : Fila.Cells[cboProveedor.Text].Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(CodProveedor))
                                {
                                    KeyValueDTO Proveedor = Proveedores.Where(p => p.Value1.Trim() == CodProveedor).FirstOrDefault();

                                    if (Proveedor != null)
                                    {

                                        _objActivoFijoDto.v_IdCliente = Proveedor.Id;
                                    }
                                    else
                                    {

                                        ProveedoresNoEncontrados.Add(CodProveedor);
                                    }
                                }

                                _objActivoFijoDto.v_NumeroFactura = cboNroFactura.Text == "--Seleccionar--" ? "" : Fila.Cells[cboNroFactura.Text].Value.ToString();
                                _objActivoFijoDto.t_FechaFactura = FechaFactura;
                                _objActivoFijoDto.d_MonedaNacional = cboCostoSoles.Text == "--Seleccionar--" ? 0 : Utils.Windows.DevuelveValorRedondeado ( decimal.Parse(Fila.Cells[cboCostoSoles.Text].Value.ToString()),4);
                                _objActivoFijoDto.d_MonedaExtranjera = cboCostoDolares.Text == "--Seleccionar--" ? 0 :Utils.Windows.DevuelveValorRedondeado ( decimal.Parse(Fila.Cells[cboCostoDolares.Text].Value.ToString()),4);
                                _objActivoFijoDto.i_Depreciara = cboSeDepreciara.Text == "--Seleccionar--" ? 0 : int.Parse(Fila.Cells[cboSeDepreciara.Text].Value.ToString());



                                string Clase = cboClase.Text == "--Seleccionar--" ? "" : Fila.Cells[cboClase.Text].Value.ToString().Trim();
                                if (!string.IsNullOrEmpty(Clase))
                                {
                                    switch (Clase)
                                    {
                                        case "N":
                                            _objActivoFijoDto.i_IdClaseActivoFijo =int.Parse ( ListaClase.Where(o => o.Value1 == "NORMAL").FirstOrDefault().Id);
                                            break;

                                        case "C":
                                            _objActivoFijoDto.i_IdClaseActivoFijo =int.Parse ( ListaClase.Where(o => o.Value1 == "COMPONENTE").FirstOrDefault().Id);
                                            break;
                                        default :
                                            ClasesNoEncontrados.Add(Clase);
                                            break;
                                    }
                                }
                                else
                                {
                                    ClasesNoEncontrados.Add(Clase);
                                }


                                string CCosto = cboCentroCosto.Text == "--Seleccionar--" ? "" : Fila.Cells[cboCentroCosto.Text].Value.ToString().Trim();
                                if (!string.IsNullOrEmpty(CCosto))
                                {
                                    try
                                    {
                                        _objActivoFijoDto.i_IdCentroCosto = int.Parse(ListaCentroCosto.Where(o => o.Value2 == CCosto).FirstOrDefault().Id);
                                    }
                                    catch (Exception ex)
                                    {
                                        CCostoNoEncontrados.Add(CCosto);
                                    }
                                }
                                else
                                {
                                    CCostoNoEncontrados.Add(CCosto);
                                }


                                string Situacion = cboSituacion.Text == "--Seleccionar--" ? "" : Fila.Cells[cboSituacion.Text].Value.ToString().Trim();
                                if (!string.IsNullOrEmpty(Situacion))
                                {
                                    switch (Situacion)
                                    {
                                        case "V":

                                            _objActivoFijoDto.i_IdSituacionActivoFijo  = int.Parse(ListaSituacion.Where(o => o.Value1 == "VIGENTE").FirstOrDefault().Id);
                                            break;

                                        case "D":
                                            _objActivoFijoDto.i_IdSituacionActivoFijo = int.Parse(ListaSituacion.Where(o => o.Value1.Contains("DEPRECIADO")).FirstOrDefault().Id);
                                            break;

                                        case "T":
                                            _objActivoFijoDto.i_IdSituacionActivoFijo = int.Parse(ListaSituacion.Where(o => o.Value1.Contains("VENDIDO")).FirstOrDefault().Id);
                                            break;
                                        default:
                                            SituacionNoEncontrados.Add(Situacion);
                                            break;
                                    }
                                }
                                else
                                {

                                    SituacionNoEncontrados.Add(Situacion);
                                }
                                string CodigoTipoActivo = cboTipoActivo.Text == "--Seleccionar--" ? "" : Fila.Cells[cboTipoActivo.Text].Value.ToString().Trim();
                                if (!string.IsNullOrEmpty(CodigoTipoActivo))
                                {
                                    KeyValueDTO TipoActivo = ListaTiposActivo.Where(p => p.Value2.Trim() == CodigoTipoActivo).FirstOrDefault();
                                    if (TipoActivo != null)
                                    {
                                        _objActivoFijoDto.i_IdTipoActivo = int.Parse(TipoActivo.Id);
                                    }
                                    else
                                    {
                                        TipoActivosNoEncontrados.Add(CodigoTipoActivo);
                                    }
                                }
                                else
                                {
                                    TipoActivosNoEncontrados.Add(CodigoTipoActivo);
                                }

                                string MesesDepreciacion = cboMesesDepreciar.Text == "--Seleccionar--" ? "-1" : Fila.Cells[cboMesesDepreciar.Text].Value.ToString().Trim();
                                if (MesesDepreciacion != "-1")
                                {
                                    if (!string.IsNullOrEmpty(MesesDepreciacion))
                                    {
                                        KeyValueDTO Depreciacion = ListaDepreciacion.Where(p => p.Value1.Trim() == MesesDepreciacion).FirstOrDefault();

                                        if (Depreciacion != null)
                                        {

                                            _objActivoFijoDto.i_IdMesesDepreciara = int.Parse(Depreciacion.Id);
                                        }
                                        else
                                        {
                                            if (_objActivoFijoDto.i_Depreciara != 0)
                                            {
                                                DepreciacionNoEncontrados.Add(MesesDepreciacion);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (_objActivoFijoDto.i_Depreciara != 0)
                                        {
                                            DepreciacionNoEncontrados.Add(MesesDepreciacion);
                                        }
                                    }
                                }



                                string PorcentajeDepreciacion = cboPorcentajeDepreciar.Text == "--Seleccionar--" ? "-1" : Fila.Cells[cboPorcentajeDepreciar.Text].Value.ToString().Trim();

                                if (PorcentajeDepreciacion != "-1")
                                {
                                    if (!string.IsNullOrEmpty(PorcentajeDepreciacion))
                                    {
                                        KeyValueDTO Porcentaje = ListaDepreciacion.Where(p => p.Value2.Trim() == PorcentajeDepreciacion).FirstOrDefault();

                                        if (Porcentaje != null)
                                        {

                                            _objActivoFijoDto.i_IdMesesDepreciara = int.Parse(Porcentaje.Id);
                                        }
                                        else
                                        {
                                            if (_objActivoFijoDto.i_Depreciara != 0)
                                            {
                                                PorcentajeNoEncontrados.Add(PorcentajeDepreciacion);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (_objActivoFijoDto.i_Depreciara != 0)
                                        {
                                            PorcentajeNoEncontrados.Add(PorcentajeDepreciacion);
                                        }
                                    }
                                }




                                string CodigoUbicacion = cboUbicacion.Text == "--Seleccionar--" ? "" : Fila.Cells[cboUbicacion.Text].Value.ToString().Trim();
                                if (!string.IsNullOrEmpty(CodigoUbicacion))
                                {
                                    KeyValueDTO Ubicacion = ListaUbicacion.Where(p => p.Value2.Trim() == CodigoUbicacion).FirstOrDefault();

                                    if (Ubicacion != null)
                                    {
                                        _objActivoFijoDto.i_IdUbicacion = int.Parse(Ubicacion.Id);
                                    }
                                    else
                                    {

                                        UbicacionNoEncontrados.Add(CodigoUbicacion);
                                    }
                                }
                              
                                _objActivoFijoDto.i_EsTemporal = 0;
                                _objActivoFijoDto.t_FechaUso = FechaUso;

                                ListaActivosFijos.Add(_objActivoFijoDto);
                            }
                            catch (Exception ex)
                            {
                                UltraMessageBox.Show(ex.Message + "Activo Fijo : " + _objActivoFijoDto.v_CodigoActivoFijo + " tiene inconsistencias.");
                                return;

                            }
                        }


                        if (TipoActivosNoEncontrados.Count() != 0)
                        {
                            string ta = "";

                            TipoActivosNoEncontrados = TipoActivosNoEncontrados.Distinct().ToList();
                            foreach (var item in TipoActivosNoEncontrados)
                            {
                                ta = ta + " " + item;

                            }

                            if (UltraMessageBox.Show(string.Format("Hay {0} Tipo(s) de activo(s) no registrado(s).¿Desea registrarlo(s)? ", TipoActivosNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", TipoActivosNoEncontrados);
                                //Clipboard.SetText(productos);
                                UltraMessageBox.Show(productos, "Sistema");
                                // if (resp == DialogResult.No  ) 
                                return;
                            }

                        }
                        if (ProveedoresNoEncontrados.Count() != 0)
                        {
                            string prov = "";
                            ProveedoresNoEncontrados = ProveedoresNoEncontrados.Distinct().ToList();
                            foreach (var item in ProveedoresNoEncontrados)
                            {
                                prov = prov + " " + item;

                            }


                            if (UltraMessageBox.Show(string.Format("Hay {0} proveedor(es) no registrado(s).¿Desea registrarlo(s)? ", ProveedoresNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", ProveedoresNoEncontrados, "Sistema");
                                //Clipboard.SetText(productos);
                                UltraMessageBox.Show(productos, "Sistema");
                                // if (resp == DialogResult.No) 
                                return;
                            }

                        }

                        if (UbicacionNoEncontrados.Count() != 0)
                        {
                            string um = "";
                            foreach (var item in UbicacionNoEncontrados)
                            {
                                um = um + " " + item;

                            }

                            if (UltraMessageBox.Show(string.Format("Hay {0} ubicaciones no registrado.¿Desea registrarlo(s)? ", UbicacionNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", UbicacionNoEncontrados);
                                //Clipboard.SetText(productos);
                                // if (resp == DialogResult.No) 
                                UltraMessageBox.Show(productos, "Sistema");
                                return;
                            }

                        }
                        if (DepreciacionNoEncontrados.Count() != 0)
                        {
                            string um = "";
                            DepreciacionNoEncontrados = DepreciacionNoEncontrados.Distinct().ToList();
                            foreach (var item in DepreciacionNoEncontrados)
                            {
                                um = um + " " + item;

                            }

                            if (UltraMessageBox.Show(string.Format("Hay {0} Depreciaciones no registrados,  ¿Desea registrarlas(s)?", DepreciacionNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", DepreciacionNoEncontrados);
                                // Clipboard.SetText(productos);
                                //  if (resp == DialogResult.No) 
                                UltraMessageBox.Show(productos, "Sistema");
                                return;
                            }
                        }

                        if (SituacionNoEncontrados.Count () != 0)
                        {
                            string um = "";
                            SituacionNoEncontrados = SituacionNoEncontrados.Distinct().ToList();
                            foreach (var item in SituacionNoEncontrados)
                            {
                                um = um + " " + item;

                            }

                            if (UltraMessageBox.Show(string.Format("Hay {0} Situaciones no registradas,  ¿Desea registrarlas(s)?", SituacionNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", SituacionNoEncontrados);
                                // Clipboard.SetText(productos);
                                //  if (resp == DialogResult.No) 
                                UltraMessageBox.Show(productos, "Sistema");
                                return;
                            }
                        }


                        if (ClasesNoEncontrados.Count() != 0)
                        {
                            string um = "";
                            ClasesNoEncontrados = ClasesNoEncontrados.Distinct().ToList();
                            foreach (var item in ClasesNoEncontrados)
                            {
                                um = um + " " + item;

                            }

                            if (UltraMessageBox.Show(string.Format("Hay {0} Clases no registradas,  ¿Desea registrarlas(s)?", ClasesNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", ClasesNoEncontrados);
                                // Clipboard.SetText(productos);
                                //  if (resp == DialogResult.No) 
                                UltraMessageBox.Show(productos, "Sistema");
                                return;
                            }
                        }




                        if (CCostoNoEncontrados.Count() != 0)
                        {
                            string um = "";
                            CCostoNoEncontrados = CCostoNoEncontrados.Distinct().ToList();
                            foreach (var item in CCostoNoEncontrados)
                            {
                                um = um + " " + item;

                            }

                            if (UltraMessageBox.Show(string.Format("Hay {0} Centros de costos no registrados,  ¿Desea registrarlas(s)?", CCostoNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var productos = string.Join(", ", CCostoNoEncontrados);
                                // Clipboard.SetText(productos);
                                //  if (resp == DialogResult.No) 
                                UltraMessageBox.Show(productos, "Sistema");
                                return;
                            }
                        }



                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                        bwkProceso.RunWorkerAsync();



                    }
                        #endregion



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

                    Globals.ProgressbarStatus.i_TotalProgress = ListaActivosFijos.Count();
                    #region Empieza el guardado de las importaciones filtradas
                    foreach (activofijoDto ActivoFijo in ListaActivosFijos.AsParallel())
                    {
                        ActivoFijo.v_CodigoActivoFijo = GenerarCodigoSistema();
                        string pIdMovimiento = new ActivoFijoBL().InsertarActivoFijo(ref pobjOperationResult, ActivoFijo, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, false);

                        
                        if (pobjOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Ocurrió un error al Migrar Activos Fijos" + ActivoFijo.v_CodigoAnterior, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Globals.ProgressbarStatus.b_Cancelado = false;
                return;
            }
            UltraMessageBox.Show("Importación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    #endregion
        }

        private string GenerarCodigoSistema()
        {
            List<KeyValueDTO> _ListadoActivosFijos = new List<KeyValueDTO>();
            OperationResult objOperationResult = new OperationResult();
            string CodigoSistema = "";
            _ListadoActivosFijos = _objActivoFijoBL.ObtenerListadoActivosFijos(ref objOperationResult);
            if (_ListadoActivosFijos.Count != 0)
            {
                int _MaxV = _ListadoActivosFijos.Count() - 1;
                return CodigoSistema = (int.Parse(_ListadoActivosFijos[_MaxV].Value1) + 1).ToString("00000000");

            }
            else
            {
                return CodigoSistema = "00000001";

            }
        }

        private void frmMigracionActivoFijo_Load(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;
            cboCodigo.SelectedIndex = 0;
            cboDescripcion.SelectedIndex = 0;
            cboMarca.SelectedIndex = 0;
            cboModelo.SelectedIndex = 0;
            cboSerie.SelectedIndex = 0;
            cboEstado.SelectedIndex = 0;
            cboPlaca.SelectedIndex = 0;
            cboColor.SelectedIndex = 0;
            cboTipoActivo.SelectedIndex = 0;
            cboResponsable.SelectedIndex = 0;
            cboFechaCompra.SelectedIndex = 0;
            cboOrdenCompra.SelectedIndex = 0;
            cboFechaFactura.SelectedIndex = 0;
            cboProveedor.SelectedIndex = 0;
            cboNroFactura.SelectedIndex = 0;
            cboCostoSoles.SelectedIndex = 0;
            cboCostoDolares.SelectedIndex = 0;
            cboFechaUso.SelectedIndex = 0;
            cboSeDepreciara.SelectedIndex = 0;
            cboMesesDepreciar.SelectedIndex = 0;
            cboUbicacion.SelectedIndex = 0;
            cboFoto.SelectedIndex = 0;
            cboClase.SelectedIndex = 0;
            cboCodigoBarras.SelectedIndex = 0;
            cboSituacion.SelectedIndex = 0;
            cboPorcentajeDepreciar.SelectedIndex = 0;
            cboCentroCosto.SelectedIndex = 0;
            cboTipoDocumento.SelectedIndex = 0;

        }

        private void cboRuc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboContacto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblDirectorioGuardar_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lblDirectorioGuardar.Text = Environment.MachineName+"\\"+ fbd.SelectedPath;

            }

        }
    }
}
