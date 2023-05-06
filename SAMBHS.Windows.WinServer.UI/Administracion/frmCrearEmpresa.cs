using Devart.Data.PostgreSql;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using System.IO;
using System.Diagnostics;
using Dapper;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmCrearEmpresa : Form
    {
        public frmCrearEmpresa()
        {
            InitializeComponent();
        }

        private void ultraLabel1_Click(object sender, EventArgs e)
        {

        }

        private void txtRUC_Validating(object sender, CancelEventArgs e)
        {
            var rucValido = Utils.Windows.ValidarRuc(txtRUC.Text.Trim());
            if (!string.IsNullOrEmpty(txtRUC.Text.Trim()) && !rucValido)
            {
                MessageBox.Show("El RUC ingresadro no es válido...", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtRUC.Focus();
            }
        }
        //Crear Empresa
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (!ultraValidator1.Validate(true, false).IsValid) return;
            bool resultCreateDB;
            //CreateDB
            if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                resultCreateDB = PostgreSqlDump.CreateDB(txtRUC.Text.Trim());
            else
                resultCreateDB = SqlServerDump.CreateDB(txtRUC.Text.Trim());

            if (resultCreateDB)
            {
                nodeDto _nodeDto = new nodeDto();
                _nodeDto.v_Direccion = txtDireccion.Text.Trim();
                _nodeDto.v_RazonSocial = txtRazonSocial.Text.Trim();
                _nodeDto.v_RUC = txtRUC.Text.Trim();

                OperationResult objOperationResult = new OperationResult();
                new NodeBL().AddNode(ref objOperationResult, _nodeDto, Globals.ClientSession.GetAsList());

                if (objOperationResult.Success == 0)
                {
                    if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                        PostgreSqlDump.DropDB(txtRUC.Text.Trim(), -1);
                    else
                        SqlServerDump.DropDB(txtRUC.Text.Trim(), -1);

                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la operación.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                gbCrearDB.Enabled = false;
                gbRestaurarDB.Enabled = true;
                UltraStatusbarManager.Mensaje(ultraStatusBar1, "Base de Datos Creada... Esperando Restauración", timer1);
            }
            else
            {
                MessageBox.Show("ERROR AL CREAR LA BASE DE DATOS", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmCrearEmpresa_Load(object sender, EventArgs e)
        {
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            this.BackColor = new GlobalFormColors().FormColor;
            ultraTextEditor1.Text = UserConfig.Default.appRutaBackupPredeterminada;
        }
        //Restaurar
        private void ultraButton2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ultraTextEditor1.Text.Trim()) && !File.Exists(ultraTextEditor1.Text))
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Archivo backup no existe!", timer1);
                return;
            }
            if (string.IsNullOrEmpty(ultraTextEditor1.Text.Trim())) { MessageBox.Show("Por favor elija una ruta para localizar el backup."); return; }
            bool ResultRestore;
            if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                ResultRestore = PostgreSqlDump.RestoreDB(ultraTextEditor1.Text, txtRUC.Text, UserConfig.Default.appBinPostgresLocation, UserConfig.Default.csServidor, "5432");
            else
                ResultRestore = SqlServerDump.RestoreDB(ultraTextEditor1.Text, txtRUC.Text);

            if (ResultRestore)
            {
                if (chkLimpiarBD.Checked)
                {
                    UltraStatusbarManager.Mensaje(ultraStatusBar1, "Procesando Restauración...", timer1);
                    RealizarLimpiadoBackUp(txtRUC.Text, chkConservarPlanCuentas.Checked, chkConservarClientes.Checked, chkConservarDestinos.Checked);
                }
                UserConfig.Default.appRutaBackupPredeterminada = ultraTextEditor1.Text.Trim();
                UserConfig.Default.Save();
                UltraStatusbarManager.Mensaje(ultraStatusBar1, "Empresa Creada Correctamente!", timer1);
                gbRestaurarDB.Enabled = false;
            }
            else
            {
                MessageBox.Show("No se pudo restaurar la base de datos");
            }
        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnEscogerBD")
            {
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = string.Format("Archivos Backup (*.{0})| *.{0}", (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL ? "backup" : "bak"));
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;

                if (choofdlog.ShowDialog() == DialogResult.OK) ultraTextEditor1.Text = choofdlog.FileName;
            }
            else
            {
                frmBuscarBinPostgres f = new frmBuscarBinPostgres();
                f.ShowDialog();
            }
        }

        void RealizarLimpiadoBackUp(string databaseName, bool conservarPlan, bool conservarClientesProveedores, bool conservarDestinos)
        {
            try
            {
                var DataSource = UserConfig.Default.csServidor;
                string InitialCatalog = databaseName;
                string UserID = UserConfig.Default.csUsuario;
                string Password = Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");

                System.Data.Common.DbConnection Cnx;
                if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                {
                    var providerString = "User Id=" + UserID + "; password=" + Password + ";Host=" + DataSource + ";Database=" + InitialCatalog + ";Initial Schema=public";
                    Cnx = new PgSqlConnection(providerString);
                }
                else
                {
                    var providerString = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3}", DataSource, UserID, Password, InitialCatalog);
                    Cnx = new SqlConnection(providerString);
                }
                Cnx.Execute("delete from nbs_formatounicofacturaciondetalle;");
                Cnx.Execute("delete from nbs_ordentrabajodetalle");
                Cnx.Execute("delete from importacionDetalleFob");
                Cnx.Execute("delete from importacionDetalleProducto");
                Cnx.Execute("delete from letraspagarcanje;");
                Cnx.Execute("delete from planillacalculo;");
                Cnx.Execute("delete from planillavariablesdiasnolabsubsidiados;");
                Cnx.Execute("delete from planillavariablesaportaciones;");
                Cnx.Execute("delete from planillavariablesdescuentos;");
                Cnx.Execute("delete from planillavariablesingresos;");
                Cnx.Execute("delete from regimenpensionariotrabajador;");
                Cnx.Execute("delete from planillavariablestrabajador;");
                Cnx.Execute("delete from areaslaboratrabajador;");
                Cnx.Execute("delete from contratodetalletrabajador;");
                Cnx.Execute("delete from contratotrabajador;");
                Cnx.Execute("delete from trabajador;");
                Cnx.Execute("delete from pagopendiente;");
                Cnx.Execute("delete from compradetalle;");
                Cnx.Execute("delete from avalcliente;");
                Cnx.Execute("delete from compra;");
                Cnx.Execute("delete from ordendecompradetalle;");
                Cnx.Execute("delete from ordendecompra;");
                Cnx.Execute("delete from cobranzaletraspendiente;");
                Cnx.Execute("delete from separacionproducto;");
                Cnx.Execute("delete from cobranzapendiente;");
                Cnx.Execute("delete from cobranzadetalle;");
                Cnx.Execute("delete from letraspagarcanje;");
                Cnx.Execute("delete from letrasmantenimientodetalle;");
                Cnx.Execute("delete from letrasmantenimiento;");
                Cnx.Execute("delete from cobranza;");
                Cnx.Execute("delete from documentoretenciondetalle;");
                Cnx.Execute("delete from documentoretencion;");
                Cnx.Execute("delete from letrascanje;");
                Cnx.Execute("delete from letrasdetalle;");
                Cnx.Execute("delete from letras;");
                Cnx.Execute("delete from ventadetalle;");
                Cnx.Execute("delete from venta;");
                Cnx.Execute("delete from movimientodetalle;");
                Cnx.Execute("delete from movimiento;");
                Cnx.Execute("delete from pedidodetalle;");
                Cnx.Execute("delete from pedido;");
                Cnx.Execute("delete from separacionproducto;");
                Cnx.Execute("delete from guiaremisiondetalle;");
                Cnx.Execute("delete from guiaremision;");
                Cnx.Execute("delete from guiaremisioncompradetalle;");
                Cnx.Execute("delete from guiaremisioncompra;");
                Cnx.Execute("delete from listapreciodetalle;");
                Cnx.Execute("delete from listaprecio;");
                Cnx.Execute("delete from importacion;");
                Cnx.Execute("delete from productoalmacen;");
                Cnx.Execute("delete from productodetalle;");
                Cnx.Execute("delete from producto;");
                Cnx.Execute("delete from pendientecobrardetalle;");
                Cnx.Execute("delete from diariodetalle;");
                Cnx.Execute("delete from diario;");
                Cnx.Execute("delete from tesoreriadetalle;");
                Cnx.Execute("delete from tesoreria;");
                Cnx.Execute("delete from saldoscontables;");
                Cnx.Execute("delete from recibohonorariodetalle;");
                Cnx.Execute("delete from recibohonorario;");
                Cnx.Execute("delete from establecimientoalmacen;");
                Cnx.Execute("delete from almacen;");
                Cnx.Execute("delete from vendedor;");
                Cnx.Execute("delete from establecimientodetalle;");
                Cnx.Execute("delete from establecimiento;");
                Cnx.Execute("delete from cobranzaletraspendiente;");
                Cnx.Execute("delete from letrasdetalle;");
                Cnx.Execute("delete from tipodecambio;");
                Cnx.Execute("delete from movimientodetalle;");
                Cnx.Execute("delete from movimiento;");
                Cnx.Execute("delete from pedidodetalle;");
                Cnx.Execute("delete from pedido;");
                Cnx.Execute("delete from separacionproducto;");
                Cnx.Execute("delete from guiaremisiondetalle;");
                Cnx.Execute("delete from guiaremision;");
                Cnx.Execute("delete from guiaremisioncompradetalle;");
                Cnx.Execute("delete from guiaremisioncompra;");
                Cnx.Execute("delete from listapreciodetalle;");
                Cnx.Execute("delete from listaprecio;");
                Cnx.Execute("delete from importacion;");
                Cnx.Execute("delete from productoalmacen;");
                Cnx.Execute("delete from productodetalle;");
                Cnx.Execute("delete from producto;");
                Cnx.Execute("delete from tesoreriadetalle;");
                Cnx.Execute("delete from tesoreria;");
                Cnx.Execute("delete from pendientecobrardetalle;");
                Cnx.Execute("delete from diariodetalle;");
                Cnx.Execute("delete from diario;");
                Cnx.Execute("delete from recibohonorariodetalle;");
                Cnx.Execute("delete from recibohonorario;");
                Cnx.Execute("delete from establecimientoalmacen;");
                Cnx.Execute("delete from almacen;");
                Cnx.Execute("delete from establecimientodetalle;");
                Cnx.Execute("delete from establecimiento;");
                Cnx.Execute("delete from tipodecambio;");
                Cnx.Execute("delete from adelanto;");
                Cnx.Execute("delete from ordendecompradetalle;");
                Cnx.Execute("delete from ordendecompra;");
                if (!conservarClientesProveedores)
                    Cnx.Execute("delete from cliente;");
                if (!conservarDestinos)
                    Cnx.Execute("delete from destino;");
                if (!conservarPlan)
                    Cnx.Execute("delete from asientocontable;");

                Cnx.Execute(@"update secuential set " + "\"" + "i_SecuentialId" + "\"" + " = 1 " +
                        "where " + "\"" + "i_TableId" + "\"" + " in (10,12,14,15,16,17,18,19,20,21,22,23,24,25,26,28,29,30,31,32,36,37,38,39," +
                                                                    "40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,62,63,64,65,66,67,70,71);");
                Cnx.Execute("delete from log;");
                Cnx.Execute("delete from loghistorial;");
                Cnx.Execute("delete from saldoscontables;");

                if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                {
                    Cnx.Execute("ALTER SEQUENCE " + "\"" + "establecimiento_i_IdEstablecimiento_seq" + "\"" + " RESTART WITH 1;");
                    Cnx.Execute("ALTER SEQUENCE " + "\"" + "saldoscontables_i_IdSaldo_seq" + "\"" + " RESTART WITH 1;");
                }
                else
                {
                    Cnx.Execute("DBCC CHECKIDENT ('establecimiento', RESEED, 0)");
                    Cnx.Execute("DBCC CHECKIDENT ('saldoscontables', RESEED, 0)");
                }

                bool isPostgress = UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL;
                Cnx.Execute("insert into producto (" + "\"" + "v_IdProducto" + "\"" + ", " + "\"" + "v_CodInterno" + "\"" + ", " + "\"" + "v_Descripcion" + "\"" + ", " + "\"" + "d_Empaque" + "\"" + ", " + "\"" + "i_IdUnidadMedida" + "\"" + ", " + "\"" + "d_Peso" + "\"" + ", " + "\"" + "i_EsAfectoDetraccion" + "\"" + ", " + "\"" + "i_EsServicio" + "\"" + ", " + "\"" + "i_NombreEditable" + "\"" + ", " + "\"" + "i_EsActivo" + "\"" + "," + "\"" + "i_EsLote" + "\"" + ", " + "\"" + "i_ValidarStock" + "\"" + ", " + "\"" + "i_Eliminado" + "\"" + ", " + "\"" + "i_InsertaIdUsuario" + "\"" + ", " + "\"" + "t_InsertaFecha" + "\"" + ", " + "\"" + "i_EsAfectoPercepcion" + "\"" + ", " + "\"" + "d_TasaPercepcion" + "\"" + ") " +
                    "values ('N002-PD000000000', '0000', 'REDONDEO', 1,15,0, 0,1,0,1,0,0,0,1, '" + (isPostgress ? "29/04/2015" : "2015-04-29") + "', 0, 0);");

                Cnx.Execute("insert into productodetalle (" + "\"" + "v_IdProductoDetalle" + "\"" + ", " + "\"" + "v_IdProducto" + "\"" + ", " + "\"" + "i_Eliminado" + "\"" + ", " + "\"" + "i_InsertaIdUsuario" + "\"" + ", " + "\"" + "t_InsertaFecha" + "\"" + ")" +
                                        "values ('N002-PE000000000', 'N002-PD000000000', 0,1,'" + (isPostgress ? "29/04/2015" : "2015-04-29") + "');");

                Cnx.Execute("insert into establecimiento(" + "\"" + "v_Nombre" + "\"" + "," + "\"" + "v_Direccion" + "\"" + ", " + "\"" + "i_Eliminado" + "\"" + ", " + "\"" + "i_InsertaIdUsuario" + "\"" + ", " + "\"" + "t_InsertaFecha" + "\"" + ")" +
                "values ('Establecimiento Predeterminado', 'Lima', 0, 0, CURRENT_TIMESTAMP);");

                Cnx.Execute("insert into almacen (" + "\"" + "i_IdAlmacen" + "\"" + ", " + "\"" + "v_Nombre" + "\"" + ", " + "\"" + "v_Direccion" + "\"" + ", " + "\"" + "i_Eliminado" + "\"" + ", " + "\"" + "i_InsertaIdUsuario" + "\"" + ", " + "\"" + "t_InsertaFecha" + "\"" + ")" +
                "values (1,'Almacén Predeterminado', 'Lima' , 0, 0, CURRENT_TIMESTAMP);");

                Cnx.Execute("insert into establecimientoalmacen (" + "\"" + "i_IdEstablecimiento" + "\"" + ", " + "\"" + "i_IdAlmacen" + "\"" + ", " + "\"" + "i_Eliminado" + "\"" + ", " + "\"" + "i_InsertaIdUsuario" + "\"" + ", " + "\"" + "t_InsertaFecha" + "\"" + ")" +
                "values (1,1, 0, 0, CURRENT_TIMESTAMP);");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void chkLimpiarBD_CheckedChanged(object sender, EventArgs e)
        {
            chkConservarClientes.Checked = chkLimpiarBD.Checked;
            chkConservarPlanCuentas.Checked = chkLimpiarBD.Checked;
            chkConservarDestinos.Checked = chkLimpiarBD.Checked;
            chkConservarClientes.Enabled = chkLimpiarBD.Checked;
            chkConservarPlanCuentas.Enabled = chkLimpiarBD.Checked;
            chkConservarDestinos.Enabled = chkLimpiarBD.Checked;
            if (chkLimpiarBD.Checked)
                chkLimpiarBD.Text = "Limpiar Base de Datos ->";
            else
                chkLimpiarBD.Text = "Limpiar Base de Datos";
        }
    }


}
