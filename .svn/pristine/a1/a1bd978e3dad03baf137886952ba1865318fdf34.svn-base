using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    // ReSharper disable once InconsistentNaming
    public partial class frmUbigeoCliente2 : Form
    {
        #region Fields
        private readonly SystemParameterBL _objSystemParameterBl = new SystemParameterBL();
        private List<KeyValueDTO> _listaUbigeo;
        #endregion

        #region Construct
        // ReSharper disable once UnusedParameter.Local
        public frmUbigeoCliente2(string arg)
        {
            InitializeComponent();
        }
        private void frmUbigeoCliente_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            _listaUbigeo = _objSystemParameterBl.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null);
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show(objOperationResult.ExceptionMessage);
            }
        }
        #endregion

        #region Events UI
        //Load Excel
        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var choofdlog = new OpenFileDialog
            {
                Filter = @"Archivos Excel|*.xlsx;*.xls|Todos los Archivos|*.*",
                Multiselect = false
            };
            if (choofdlog.ShowDialog() != DialogResult.OK) return;
            Worksheet worksheet;
            try
            {
                var workbook = Workbook.Load(choofdlog.FileName);
                worksheet = workbook.Worksheets[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            ultraDataSource1.Reset();
            var listFields = new Queue<string>();
            foreach (var cell in worksheet.Rows[0].Cells)
            {
                var columnKey = cell.GetText();
                ultraDataSource1.Band.Columns.Add(columnKey);
                listFields.Enqueue(columnKey);
            }
            var items = listFields.Select(o => (object)o).ToArray();
            foreach (var ctr in pnCampos.Controls)
            {
                var cbo = ctr as ComboBox;
                if (cbo == null) continue;
                cbo.Items.Clear();
                cbo.Items.AddRange(items);
            }
            try
            {
                foreach (var row in worksheet.Rows.Skip(1))
                {
                    ultraDataSource1.Rows.Add(row.Cells.Select(cell => cell.Value).ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            grdData.SetDataBinding(ultraDataSource1, "Band 0");
        }

        //Save
        private void ultraButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnCampos.Controls.OfType<ComboBox>().Any(cbo => cbo.SelectedItem == null))
                {
                    UltraMessageBox.Show("Relecione todos los campos", "Alerta", Icono:MessageBoxIcon.Exclamation);
                    return;
                }
                ActualizarClientes();
                UltraMessageBox.Show("Completado!", "Proceso Finalizado");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Methods
        private void ActualizarClientes()
        {
            var objResult = new OperationResult();
            var clientBl = new ClienteBL();
            var carteraClienteBl = new CarteraClienteBL();
            var vendedorbl = new VendedorBL();
            foreach (var row in grdData.Rows)
            {
                var client = clientBl.ObtenerClienteDocumentoIdentificacion(ref objResult, row.GetCellValue(cboCodigo.Text).ToString().Trim(), null, "C");
                if(client == null) continue;
                client.v_TelefonoFijo = row.GetCellValue(cboTelf.Text).ToString().TrimEnd();
                client.v_TelefonoFax = row.GetCellValue(cboFax.Text).ToString().TrimEnd();
                client.v_TelefonoMovil = row.GetCellValue(cboCelular.Text).ToString().TrimEnd();
                client.v_Correo = row.GetCellValue(cboEmail.Text).ToString().TrimEnd();

                var dep = ObtenerRegion("1", row.GetCellValue(cboDepartamento.Text).ToString());
                client.i_IdDepartamento = dep == null ? -1 : int.Parse(dep.Id) ;
                if (dep != null)
                {
                    var prov = ObtenerRegion(dep.Id, row.GetCellValue(cboProvincia.Text).ToString());
                    client.i_IdProvincia = prov == null ? -1 : int.Parse(prov.Id);
                    if (prov != null)
                    {
                        var dist = ObtenerRegion(prov.Id, row.GetCellValue(cboDistrito.Text).ToString());
                        client.i_IdDistrito = dist == null ? -1 : int.Parse(dist.Id);
                    }
                }
                #region Cartera Cliente-Vendedor
                var vendedor = vendedorbl.ObtenerVendeorPorCodigo(ref objResult, row.GetCellValue(cboCodVendedor.Text).ToString().Trim());
                if (vendedor != null)
                {
                    client.v_IdVendedor = vendedor.v_IdVendedor;
                    var result = carteraClienteBl.ConsultarSiExisteCliente(ref objResult, client.v_IdCliente);
                    if (result == 0) //SI es 0 no existe en los registros y se procede al Insert
                    {
                        var objCarteraclienteDto = new carteraclienteDto
                        {
                            v_IdCliente = client.v_IdCliente,
                            v_IdVendedor = vendedor.v_IdVendedor
                        };
                        carteraClienteBl.InsertarCarteraCliente(ref objResult, objCarteraclienteDto,
                            Globals.ClientSession.GetAsList());
                    }
                }

                #endregion 
                clientBl.Actualizarcliente(ref objResult, client, Globals.ClientSession.GetAsList(), null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                if (objResult.Success == 0)
                    UltraMessageBox.Show(objResult.ExceptionMessage, "Error", Icono:MessageBoxIcon.Warning);
            }
        }

        private KeyValueDTO ObtenerRegion(string parent, string ubigueo)
        {
            int ub;
            if (!int.TryParse(ubigueo, out ub)) return null;
            var res = _listaUbigeo.Where(i => i.Value3 == parent && int.Parse(i.Value2) == ub);
            return res.FirstOrDefault();
        }
        #endregion
    }
}
