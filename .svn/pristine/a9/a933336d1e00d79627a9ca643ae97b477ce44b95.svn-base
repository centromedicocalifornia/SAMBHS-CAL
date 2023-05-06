using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones;

namespace SAMBHS.Windows.WinClient.UI.Migraciones
{
    public partial class frmMigrarClientesProveedores : Form
    {
        MigracionesBL _objMigracionesBL = new MigracionesBL();
        ClienteBL _objClienteBL = new ClienteBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        clienteDto _clienteDto = new clienteDto();
        trabajadorDto _objTrabajadorDto = new trabajadorDto();
        string _mode = "New";

        public frmMigrarClientesProveedores(string p)
        {
            InitializeComponent();
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Any())
            {
                grdData.Selected.Rows.AddRange((UltraGridRow[])grdData.Rows.All);
                grdData.DeleteSelectedRows(false);
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

                Workbook workbook = Workbook.Load(sFileName);
                Worksheet worksheet = workbook.Worksheets[0];

                this.ultraDataSource1.Reset();

                bool isHeaderRow = true;
                foreach (WorksheetRow row in worksheet.Rows)
                {
                    try
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {

                                string columnKey = cell.GetText();

                                UltraDataColumn column = this.ultraDataSource1.Band.Columns.Add(columnKey);

                                switch (columnKey)
                                {
                                    case "Codigo":
                                        column.DataType = typeof(string);
                                        break;
                                    case "Descripcion":
                                        column.DataType = typeof(string);
                                        break;
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

                            this.ultraDataSource1.Rows.Add(rowData.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            grdData.SetDataBinding(this.ultraDataSource1, "Band 0");

            if (grdData.Rows.Count > 0)
            {
                if (RevisarDuplicidad() != 0)
                {
                    btnImportar.Enabled = true;
                }
            }
            else
            {
                btnImportar.Enabled = true;
            }
            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboCodigo.Items.Add(ColumnKey);
                    cboNombres.Items.Add(ColumnKey);
                    cboRuc.Items.Add(ColumnKey);
                    cboDireccion.Items.Add(ColumnKey);
                    cboDNI.Items.Add(ColumnKey);
                    cboContacto.Items.Add(ColumnKey);
                    cboCorreo.Items.Add(ColumnKey);
                    cboTelefono.Items.Add(ColumnKey);

                    if (rbTrabajador.Checked)
                    {
                        cboSexo.Items.Add(ColumnKey);
                        cboTipoEmpleado.Items.Add(ColumnKey);
                        cboEstadoCivil.Items.Add(ColumnKey);
                        cboFechaNac.Items.Add(ColumnKey);
                        cboFechaInicio.Items.Add(ColumnKey);
                        cboFechaFin.Items.Add(ColumnKey);
                        

                    }
                }
            }
        }

        private int RevisarDuplicidad()
        {
            List<string> Resultado = new List<string>();
            Resultado = _objMigracionesBL.DevuelveDuplicadosClientesProveedores(grdData.Rows.Select(p => p.Cells[0].Value.ToString().Trim()).ToList(), rbClientes.Checked == true ? "C" : "V");
            int RegNuevos = 0;

            foreach (UltraGridRow Fila in grdData.Rows.Where(p => Resultado.Contains(p.Cells[0].Value.ToString().Trim())))
            {
                Fila.Cells["Nuevo"].Value = "1";
            }
            RegNuevos = grdData.Rows.Where(p => p.Cells["Nuevo"].Value != null).Count();

            lblContadorFilas.Text = string.Format("Se importaron {0} Registros, Nuevos: {1}", grdData.Rows.Count(), RegNuevos);
            //grdData.DisplayLayout.Bands[0].SortedColumns.Add("Nuevo", true, true);
            return RegNuevos;
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["Nuevo"].Value != null && e.Row.Cells["Nuevo"].Value.ToString() == "1")
            {
                e.Row.Appearance.BackColor = Color.Honeydew;
                e.Row.Appearance.BackColor2 = Color.Honeydew;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void frmMigrarClientesProveedores_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;

            cboCodigo.SelectedIndex = 0;
            cboNombres.SelectedIndex = 0;
            cboRuc.SelectedIndex = 0;
            cboDireccion.SelectedIndex = 0;
            cboTelefono.SelectedIndex = 0;
            cboContacto.SelectedIndex = 0;
            cboDNI.SelectedIndex = 0;
            cboCorreo.SelectedIndex = 0;
            cboFechaFin.SelectedIndex = 0;
            cboFechaInicio.SelectedIndex = 0;
            cboTipoEmpleado.SelectedIndex = 0;
            cboEstadoCivil.SelectedIndex = 0;
            cboSexo.SelectedIndex = 0;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                OperationResult objOperationResult = new OperationResult();
                #region Clientes-Vendedores-Proveedores

                if (rbClientes.Checked || rbProveedores.Checked)// !rbVendedores.Checked
                {
                    List<KeyValueDTO> Clientes = new List<KeyValueDTO>();
                    if (rbClientes.Checked)
                        Clientes = _objClienteBL.DevuelveClientes();
                    else if (rbProveedores.Checked)
                        Clientes = _objClienteBL.DevuelveProveedores();

                    
                    List<clienteDto> ListaXInsertar = new List<clienteDto>();

                    var ClientesValidos = grdData.Rows.Where(p => p.Cells["Nuevo"].Value != null && p.Cells["Nuevo"].Value.ToString() == "1" && !string.IsNullOrEmpty(p.Cells[cboCodigo.Text].Value.ToString()) && !string.IsNullOrEmpty(p.Cells[cboRuc.Text].Value.ToString())).ToList();

                    foreach (UltraGridRow Fila in ClientesValidos)
                    {
                        string CodigoCliente = Fila.Cells[cboCodigo.Text].Value.ToString().Trim();

                        if (!Clientes.Select(p => p.Value1.Trim()).ToList().Contains(CodigoCliente))
                        {
                            if (_mode == "New")
                            {
                                _clienteDto = new clienteDto();
                                _objTrabajadorDto = new trabajadorDto();
                                _clienteDto.v_CodCliente = Fila.Cells[cboCodigo.Text].Value.ToString().Trim();

                                //TIPO DE PERSONA           TIPO DOCUMENTO
                                // 1: NATURAL               0: SIN DOCUMENTOS
                                // 2: JURIDICA              1: DNI
                                // 3: NO DOMICILIADO        6: RUC


                                if (Fila.Cells[cboRuc.Text].Value.ToString().Trim() != string.Empty && Fila.Cells[cboRuc.Text].Value.ToString().Trim().Length == 11)
                                {
                                    if (Fila.Cells[cboRuc.Text].Value.ToString().Trim().StartsWith("2"))
                                    {
                                        _clienteDto.i_IdTipoPersona = 2;
                                    }
                                    else if (Fila.Cells[cboRuc.Text].Value.ToString().Trim().StartsWith("1"))
                                    {
                                        _clienteDto.i_IdTipoPersona = 1;
                                    }
                                    else
                                    {
                                        _clienteDto.i_IdTipoPersona = 3;
                                    }
                                }
                                else
                                {
                                    _clienteDto.i_IdTipoPersona = Fila.Cells[cboRuc.Text].Value.ToString().Trim().Length == 8 ? 1 : 3;

                                }


                                _clienteDto.i_IdTipoIdentificacion = !string.IsNullOrEmpty(Fila.Cells[cboRuc.Text].Value.ToString().Trim()) ? Fila.Cells[cboRuc.Text].Value.ToString().Trim().Length == 11 ? 6 : 1 : 0;

                                if (_clienteDto.i_IdTipoIdentificacion == 1 || Fila.Cells[cboRuc.Text].Value.ToString().Trim().Length == 11 && Fila.Cells[cboRuc.Text].Value.ToString().Trim().StartsWith("1"))
                                {
                                    string[] Cadena = Fila.Cells[cboNombres.Text].Value.ToString().ToUpper().Trim().Split(new Char[] { ' ' });

                                    if (Cadena.GetUpperBound(0) == 1)
                                    {
                                        _clienteDto.v_ApePaterno = Cadena[0].Trim();
                                        _clienteDto.v_ApeMaterno = Cadena[1].Trim();
                                        _clienteDto.v_PrimerNombre = Cadena[Cadena.Length - 1].Trim();
                                        _clienteDto.v_SegundoNombre = string.Empty;
                                        _clienteDto.v_RazonSocial = string.Empty;
                                    }

                                    if (Cadena.GetUpperBound(0) == 2)
                                    {
                                        _clienteDto.v_ApePaterno = Cadena[0].Trim();
                                        _clienteDto.v_ApeMaterno = Cadena[1].Trim();
                                        _clienteDto.v_PrimerNombre = Cadena[Cadena.Length - 1].Trim();
                                        _clienteDto.v_SegundoNombre = string.Empty;
                                        _clienteDto.v_RazonSocial = string.Empty;
                                    }

                                    if (Cadena.GetUpperBound(0) >= 3) //el upper bound es 3 empezando desde 0 osea 4 casilleros.
                                    {
                                        _clienteDto.v_ApePaterno = Cadena[0].Trim();
                                        _clienteDto.v_ApeMaterno = Cadena[1].Trim();
                                        _clienteDto.v_PrimerNombre = Cadena[Cadena.Length - 2].Trim();
                                        _clienteDto.v_SegundoNombre = Cadena[Cadena.Length - 1].Trim();
                                        _clienteDto.v_RazonSocial = string.Empty;
                                    }
                                }
                                else
                                {
                                    _clienteDto.v_PrimerNombre = string.Empty;
                                    _clienteDto.v_SegundoNombre = string.Empty;
                                    _clienteDto.v_ApePaterno = string.Empty;
                                    _clienteDto.v_ApeMaterno = string.Empty;
                                    _clienteDto.v_RazonSocial = Fila.Cells[cboNombres.Text].Value.ToString().Trim();
                                }
                                _clienteDto.v_NombreContacto = cboContacto.Text == "--Seleccionar--" ? null : Fila.Cells[cboContacto.Text].Value.ToString();

                                if (Fila.Cells[cboRuc.Text].Value != null && Fila.Cells[cboRuc.Text].Value.ToString().Trim() != string.Empty)
                                {
                                    _clienteDto.v_NroDocIdentificacion = Fila.Cells[cboRuc.Text].Value.ToString().Trim();
                                }
                                else
                                {
                                    if (cboDNI.Text != "--Seleccionar--")
                                    {
                                        if (Fila.Cells[cboDNI.Text].Value != null && Fila.Cells[cboDNI.Text].Value.ToString().Trim() != string.Empty)
                                        {
                                            if (Fila.Cells[cboDNI.Text].Value.ToString().Trim().Length == 8)
                                                _clienteDto.v_NroDocIdentificacion = Fila.Cells[cboDNI.Text].Value.ToString().Trim().Substring(0, 8).Trim();
                                            else
                                                _clienteDto.v_NroDocIdentificacion = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        _clienteDto.v_NroDocIdentificacion = string.Empty;
                                    }
                                }

                                _clienteDto.v_DirecPrincipal = Fila.Cells[cboDireccion.Text].Value.ToString().Trim().ToUpper().Trim();
                                _clienteDto.v_DirecSecundaria = null;
                                _clienteDto.v_Correo = cboCorreo.Text == "--Seleccionar--" ? null : Fila.Cells[cboCorreo.Text].Value.ToString().Trim().ToUpper();
                                _clienteDto.v_TelefonoFax = cboTelefono.Text == "--Seleccionar--" ? null : Fila.Cells[cboTelefono.Text].Value.ToString().Trim().Trim();
                                _clienteDto.i_IdPais = 1;
                                _clienteDto.i_IdDistrito = 1393;
                                _clienteDto.i_IdDepartamento = 1391;
                                _clienteDto.i_IdListaPrecios = 1;
                                _clienteDto.i_IdProvincia = 1392;
                                _clienteDto.t_FechaNacimiento = null;
                                _clienteDto.i_Nacionalidad = 0;
                                _clienteDto.i_Activo = 1;
                                _clienteDto.i_IdSexo = -1;
                                _clienteDto.v_FlagPantalla = rbClientes.Checked == true ? "C" : "V";

                                if (_clienteDto.v_PrimerNombre != null)
                                {
                                    if (_clienteDto.v_NroDocIdentificacion != null)
                                    {
                                        ListaXInsertar.Add(_clienteDto);
                                    }
                                }
                                KeyValueDTO ClienteInsertado = new KeyValueDTO();
                                ClienteInsertado.Value1 = _clienteDto.v_CodCliente;
                                Clientes.Add(ClienteInsertado);

                            }
                        }
                    }
                    if (UltraMessageBox.Show("Se importarán sólo los nuevos clientes encontrados (" + ListaXInsertar.Count().ToString() + "), ¿Desea Continuar...?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        _objClienteBL.InsertarClientes(ref objOperationResult, ListaXInsertar, Globals.ClientSession.GetAsList());
                        if (objOperationResult.Success == 1)
                        {
                            UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            UltraMessageBox.Show(objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (rbVendedores.Checked)
                {
                    VendedorBL _objVendedorBL = new VendedorBL();

                    List<UltraGridRow> Filas = grdData.Rows.Where(p => p.Cells[cboCodigo.Text].Value.ToString().Trim() != string.Empty).ToList();

                    foreach (UltraGridRow Fila in Filas)
                    {
                        vendedorDto _vendedorDto = new vendedorDto();
                        _vendedorDto.v_NombreCompleto = Fila.Cells[cboNombres.Text].Value.ToString().Trim();
                        _vendedorDto.v_CodVendedor = Fila.Cells[cboCodigo.Text].Value.ToString().Trim();
                        _vendedorDto.v_Telefono = Fila.Cells[cboTelefono.Text].Value.ToString().Trim();
                        _vendedorDto.i_IdTipoPersona = 1;
                        _vendedorDto.i_IdTipoIdentificacion = 1;
                        _vendedorDto.i_IdPais = 1;
                        _vendedorDto.i_IdProvincia = 1392;
                        _vendedorDto.i_IdDistrito = 1393;
                        _vendedorDto.i_IdDepartamento = 1391;
                        _vendedorDto.v_NroDocIdentificacion = string.Empty;
                        _vendedorDto.v_Fax = string.Empty;
                        _vendedorDto.v_Direccion = Fila.Cells[cboDireccion.Text].Value.ToString().Trim();
                        _vendedorDto.v_Correo = string.Empty;
                        _vendedorDto.v_Contacto = string.Empty;
                       // OperationResult objOperationResult = new OperationResult();

                        _objVendedorBL.InsertarVendedor(ref objOperationResult, _vendedorDto, Globals.ClientSession.GetAsList());

                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Error al insertar vendedor " + _vendedorDto.v_NombreCompleto);
                        }
                    }

                }

                #endregion

                #region Trabajador
                else if (rbTrabajador.Checked)
                {


                    List<KeyValueDTO> Clientes = new List<KeyValueDTO>();
                    if (rbClientes.Checked)
                        Clientes = _objClienteBL.DevuelveClientes();
                    else if (rbProveedores.Checked)
                        Clientes = _objClienteBL.DevuelveProveedores();

                   
                    List<clienteDto> ListaXInsertar = new List<clienteDto>();

                    var ClientesValidos = grdData.Rows.Where(p => p.Cells["Nuevo"].Value != null && p.Cells["Nuevo"].Value.ToString() == "1" && !string.IsNullOrEmpty(p.Cells[cboCodigo.Text].Value.ToString())).ToList(); //&& !string.IsNullOrEmpty(p.Cells[cboRuc.Text].Value.ToString())



                    foreach (UltraGridRow Fila in ClientesValidos)
                    {
                        string CodigoCliente = Fila.Cells[cboCodigo.Text].Value.ToString().Trim();

                        if (!Clientes.Select(p => p.Value1.Trim()).ToList().Contains(CodigoCliente))
                        {
                            if (_mode == "New")
                            {
                                _clienteDto = new clienteDto();
                                _objTrabajadorDto = new trabajadorDto();
                                _clienteDto.v_CodCliente = Fila.Cells[cboCodigo.Text].Value.ToString().Trim();

                                //TIPO DE PERSONA           TIPO DOCUMENTO
                                // 1: NATURAL               0: SIN DOCUMENTOS
                                // 2: JURIDICA              1: DNI
                                // 3: NO DOMICILIADO        6: RUC

                                _clienteDto.i_IdTipoPersona = 0;
                                _clienteDto.i_IdTipoIdentificacion = !string.IsNullOrEmpty(Fila.Cells[cboRuc.Text].Value.ToString().Trim()) ? Fila.Cells[cboRuc.Text].Value.ToString().Trim().Length == 11 ? 6 : 1 : 7;
                                //if (_clienteDto.i_IdTipoIdentificacion == 1  || Fila.Cells[cboRuc.Text].Value.ToString().Trim().Length == 11 && Fila.Cells[cboRuc.Text].Value.ToString().Trim().StartsWith("1"))
                                if (_clienteDto.i_IdTipoIdentificacion == 1 || _clienteDto.i_IdTipoIdentificacion ==7)
                                {
                                    string[] Cadena = Fila.Cells[cboNombres.Text].Value.ToString().ToUpper().Trim().Split(new Char[] { ' ' });

                                    if (Cadena.GetUpperBound(0) == 1)
                                    {
                                        _clienteDto.v_ApePaterno = Cadena[0].Trim();
                                        _clienteDto.v_ApeMaterno = Cadena[1].Trim();
                                        _clienteDto.v_PrimerNombre = Cadena[Cadena.Length - 1].Trim();
                                        _clienteDto.v_SegundoNombre = string.Empty;
                                        _clienteDto.v_RazonSocial = string.Empty;
                                    }

                                    if (Cadena.GetUpperBound(0) == 2)
                                    {
                                        _clienteDto.v_ApePaterno = Cadena[0].Trim();
                                        _clienteDto.v_ApeMaterno = Cadena[1].Trim();
                                        _clienteDto.v_PrimerNombre = Cadena[Cadena.Length - 1].Trim();
                                        _clienteDto.v_SegundoNombre = string.Empty;
                                        _clienteDto.v_RazonSocial = string.Empty;
                                    }

                                    if (Cadena.GetUpperBound(0) >= 3) //el upper bound es 3 empezando desde 0 osea 4 casilleros.
                                    {
                                        _clienteDto.v_ApePaterno = Cadena[0].Trim();
                                        _clienteDto.v_ApeMaterno = Cadena[1].Trim();
                                        _clienteDto.v_PrimerNombre = Cadena[Cadena.Length - 2].Trim();
                                        _clienteDto.v_SegundoNombre = Cadena[Cadena.Length - 1].Trim();
                                        _clienteDto.v_RazonSocial = string.Empty;
                                    }
                                }
                                else
                                {
                                    _clienteDto.v_PrimerNombre = string.Empty;
                                    _clienteDto.v_SegundoNombre = string.Empty;
                                    _clienteDto.v_ApePaterno = string.Empty;
                                    _clienteDto.v_ApeMaterno = string.Empty;
                                    _clienteDto.v_RazonSocial = Fila.Cells[cboNombres.Text].Value.ToString().Trim();
                                }
                                _clienteDto.v_NombreContacto = cboContacto.Text == "--Seleccionar--" ? null : Fila.Cells[cboContacto.Text].Value.ToString();

                                if (Fila.Cells[cboRuc.Text].Value != null && Fila.Cells[cboRuc.Text].Value.ToString().Trim() != string.Empty)
                                {
                                    _clienteDto.v_NroDocIdentificacion = Fila.Cells[cboRuc.Text].Value.ToString().Trim();
                                }
                                else
                                {
                                    if (cboDNI.Text != "--Seleccionar--")
                                    {
                                        if (Fila.Cells[cboDNI.Text].Value != null && Fila.Cells[cboDNI.Text].Value.ToString().Trim() != string.Empty)
                                        {
                                            if (Fila.Cells[cboDNI.Text].Value.ToString().Trim().Length == 8)
                                                _clienteDto.v_NroDocIdentificacion = Fila.Cells[cboDNI.Text].Value.ToString().Trim().Substring(0, 8).Trim();
                                            else
                                                _clienteDto.v_NroDocIdentificacion = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        _clienteDto.v_NroDocIdentificacion = string.Empty;
                                    }
                                }
                                if (_objTrabajadorDto.v_CodInterno == "T0037")
                                {
                                    string x = "";
                                }
                                if (Fila.Cells[cboFechaNac.Text].Value == null || Fila.Cells[cboFechaNac.Text].Value.ToString() == "  -   -")
                                {
                                    _clienteDto.t_FechaNacimiento = null;
                                }
                                else
                                {
                                    _clienteDto.t_FechaNacimiento = DateTime.Parse(Fila.Cells[cboFechaNac.Text].Value.ToString().Trim());
                                }
                                _clienteDto.v_DirecPrincipal = Fila.Cells[cboDireccion.Text].Value.ToString().Trim().ToUpper().Trim();
                                _clienteDto.v_DirecSecundaria = null;
                                _clienteDto.v_Correo = cboCorreo.Text == "--Seleccionar--" ? null : Fila.Cells[cboCorreo.Text].Value.ToString().Trim().ToUpper();
                                _clienteDto.v_TelefonoFax = cboTelefono.Text == "--Seleccionar--" ? null : Fila.Cells[cboTelefono.Text].Value.ToString().Trim().Trim();
                                _clienteDto.i_IdPais = 1;
                                _clienteDto.i_IdDistrito = 1393;
                                _clienteDto.i_IdDepartamento = 1391;
                                _clienteDto.i_IdListaPrecios = 1;
                                _clienteDto.i_IdProvincia = 1392;
                              
                                _clienteDto.i_Nacionalidad =-1;
                                _clienteDto.i_Activo = 1;
                                _clienteDto.i_IdSexo = Fila.Cells[cboSexo.Text].Value.ToString().Trim().ToUpper().Trim()=="M"?1 :2;
                                _clienteDto.v_FlagPantalla = "T";

                                if (_clienteDto.v_PrimerNombre != null)
                                {
                                    if (_clienteDto.v_NroDocIdentificacion != null)
                                    {
                                        ListaXInsertar.Add(_clienteDto);
                                    }
                                }
                                
                                _objTrabajadorDto.v_CodInterno = _clienteDto.v_CodCliente;
                                _objTrabajadorDto.i_IdPaisNac = _clienteDto.i_IdPais.Value;
                                _objTrabajadorDto.i_IdDepartamentoNac = _clienteDto.i_IdDepartamento.Value;
                                _objTrabajadorDto.i_IdProvinciaNac = _clienteDto.i_IdProvincia.Value;
                                _objTrabajadorDto.i_IdDistritoNac = _clienteDto.i_IdDistrito.Value;
                              
                               //if ( Fila.Cells[cboFechaInicio.Text].Value == null || Fila.Cells[cboFechaInicio.Text].Value == "  -   -" )
                               // {
                               // _objTrabajadorDto.t_FechaAlta = null ;
                               // }
                               // else 
                               // {
                                _objTrabajadorDto.t_FechaAlta = null;
                                //}

                               //if (Fila.Cells[cboFechaFin.Text].Value == null || Fila.Cells[cboFechaFin.Text].Value == "  -   -")
                               //{
                                   _objTrabajadorDto.t_FechaCese = null;
                               //}
                               //else
                               //{
                               //    _objTrabajadorDto.t_FechaCese = DateTime.Parse(Fila.Cells[cboFechaFin.Text].Value.ToString().Trim());
                               //}
                                _objTrabajadorDto.i_IdTipoVia = -1;
                                _objTrabajadorDto.i_IdEstadoCivil = string.IsNullOrEmpty(Fila.Cells[cboEstadoCivil.Text].Value.ToString()) ? -1 : Fila.Cells[cboEstadoCivil.Text].Value.ToString() == "C" ? 2 : 1;
                                
                                _objTrabajadorDto.v_NombreVia = "";
                                _objTrabajadorDto.i_IdTipoZona = -1;
                                _objTrabajadorDto.v_NombreZona = "";
                                _objTrabajadorDto.v_NumeroDomicilio = "";
                                _objTrabajadorDto.v_InteriorDomicilio = "";
                                _objTrabajadorDto.v_KilometroDomicilio = "";
                                _objTrabajadorDto.v_ManzanaDomicilio = "";
                                _objTrabajadorDto.v_LoteDomicilio = "";
                                _objTrabajadorDto.v_BloqueDomicilio = "";
                                _objTrabajadorDto.v_Referencia = "";
                                _objTrabajadorDto.v_DepartamentoDomicilio = "";
                                _objTrabajadorDto.i_Domiciliado = 0;
                                // _objTrabajador.i_TieneOtrosIngresos5taCat 
                                _objTrabajadorDto.i_TieneOtrosIngresos5taCat = 0;
                                _objTrabajadorDto.i_Renta5taCatExonerada = 0;
                                _objTrabajadorDto.i_RegimenLaboral = -1;
                                _objTrabajadorDto.i_Eliminado = 0;
                                _objTrabajadorDto.i_TipoTrabajador = Fila.Cells[cboTipoEmpleado.Text].Value.ToString().Trim() == "01" ? 2 : Fila.Cells[cboTipoEmpleado.Text].Value.ToString().Trim() == "02" ? 3 : 1;
                                List<contratodetalletrabajadorDto> ListaContratoDetalle = new List<contratodetalletrabajadorDto> ();
                                List<contratotrabajadorDto > ListaContrato= new List<contratotrabajadorDto> ();
                                List<regimenpensionariotrabajadorDto> ListaRegimen = new List<regimenpensionariotrabajadorDto>();
                                List<derechohabientetrabajadorDto> ListaDH = new List<derechohabientetrabajadorDto>();
                                List<areaslaboratrabajadorDto> ListaAreas = new List<areaslaboratrabajadorDto>();
                                _objClienteBL.InsertarCliente(ref objOperationResult, _clienteDto, Globals.ClientSession.GetAsList(), null, _objTrabajadorDto, ListaContrato, ListaContratoDetalle, ListaRegimen, ListaDH, ListaAreas);

                                if (objOperationResult.Success == 0)
                                {
                                    UltraMessageBox.Show("Ocurrió un error", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                   
                }

                if (objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
                #endregion
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                gpDetalle.Location = new Point(ultraExpandableGroupBox1.Location.X + ultraExpandableGroupBox1.Height + 5, gpDetalle.Location.Y);
                gpDetalle.Width = 831;
            }
            else
            {
                gpDetalle.Location = new Point(ultraExpandableGroupBox1.Location.X + ultraExpandableGroupBox1.Height + 5, gpDetalle.Location.Y);
                gpDetalle.Width = 1089;
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var f = new frmUbigeoCliente();
            f.ShowDialog();
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count > 0)
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        //grdData.Rows.Where(p => p.Cells[cboFechaNac.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboFechaNac.Text].Value.ToString()) && p.Cells[cboFechaNac.Text].Value.ToString().Trim() != "/  /" && p.Cells[cboFechaNac.Text].Value.ToString() !=  "  -   -").ToList()
                        //            .ForEach(o => o.Cells[cboFechaNac.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaNac.Text].Value.ToString())));

                        //grdData.Rows.Where(p => p.Cells[cboFechaInicio.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboFechaInicio.Text].Value.ToString()) && p.Cells[cboFechaInicio.Text].Value.ToString().Trim() != "/  /" && p.Cells[cboFechaNac.Text].Value.ToString()!= "  -   -").ToList()
                        //            .ForEach(o => o.Cells[cboFechaInicio.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaInicio.Text].Value.ToString())));
                        //grdData.Rows.Where(p => p.Cells[cboFechaFin.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboFechaFin.Text].Value.ToString()) && p.Cells[cboFechaFin.Text].Value.ToString().Trim() != "/  /" && p.Cells[cboFechaNac.Text].Value.ToString() != "  -   -").ToList()
                        //                 .ForEach(o => o.Cells[cboFechaFin.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaFin.Text].Value.ToString())));

                        var xx = grdData.Rows.Where(p => p.Cells[cboFechaNac.Text].Value != null && p.Cells[cboFechaNac.Text].Value.ToString().Trim().Length == 5 && p.Cells[cboFechaNac.Text].Value.ToString() != "  -   -").ToList();

                        xx.ForEach(o => o.Cells[cboFechaNac.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaNac.Text].Value.ToString())));

                        var aa = grdData.Rows.Where(p => p.Cells[cboFechaInicio.Text].Value != null && p.Cells[cboFechaInicio.Text].Value.ToString().Trim().Length == 5 && p.Cells[cboFechaInicio.Text].Value.ToString() != "  -   -").ToList();

                        xx.ForEach(o => o.Cells[cboFechaInicio.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaInicio.Text].Value.ToString())));

                        var bb = grdData.Rows.Where(p => p.Cells[cboFechaFin.Text].Value != null && p.Cells[cboFechaFin.Text].Value.ToString().Trim().Length == 5 && p.Cells[cboFechaFin.Text].Value.ToString() != "  -   -").ToList();

                        xx.ForEach(o => o.Cells[cboFechaFin.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFechaFin.Text].Value.ToString())));
                        
                        var resp = UltraMessageBox.Show("Proceso Terminado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void ultraGroupBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
