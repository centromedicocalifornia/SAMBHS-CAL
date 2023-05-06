using System;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmBandejaEmpresas : Form
    {
        readonly NodeBL _objNodeBl = new NodeBL();
        public frmBandejaEmpresas(string N)
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
         
        }

        private void frmBandejaEmpresas_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            btnCrearEmpresas.Visible = UserConfig.Default.appSistemaMultiEmpresas;
            Cargar();
        }

        private void Cargar()
        {
            OperationResult objOperationResult = new OperationResult();
            var ds = _objNodeBl.GetNodePagedAndFiltered(ref objOperationResult, null, null, null, null);

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ultraGrid1.DataSource = ds;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.MSSQLServer)
                if (!SqlServerDump.IsServerLocal())
                {
                    MessageBox.Show("Ejecute Esta Instruccion en el Servidor Principal!", "SQL SERVER", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            frmCrearEmpresa f = new frmCrearEmpresa();
            f.ShowDialog();

            btnBuscar_Click(sender, e);
            var ultimaFila = ultraGrid1.Rows.LastOrDefault();
            if (ultimaFila != null) ultimaFila.Activate();
            Cargar();
        }

        private void ultraGrid1_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if(UserConfig.Default.csTipoMotorBD == TipoMotorBD.MSSQLServer)
                if (!SqlServerDump.IsServerLocal())
                {
                    MessageBox.Show("Ejecute Esta Instruccion en el Servidor Principal!", "SQL SERVER", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            switch (e.Cell.Column.Key)
            {
                case "_btnBackup":
                    frmBackupEmpresa f = new frmBackupEmpresa(e.Cell.Row.Cells["v_RUC"].Value.ToString());
                    f.ShowDialog();
                    break;

                case "_btnEliminar":
                    var resp = MessageBox.Show("ADVERTENCIA: Si elimina esta empresa, desaparecerá de los registros y para restaurarla será necesaria asistencia técnica. ¿Desea Continuar?...", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resp == System.Windows.Forms.DialogResult.Yes)
                    {
                        var empresa = e.Cell.Row.Cells["v_RUC"].Value.ToString();
                        var nodeId = int.Parse(e.Cell.Row.Cells["i_NodeId"].Value.ToString());
                        string Password = Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");
                        bool ResultState = false;
                        if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                        {
                            PostgreSqlDump.BackupBD(UserConfig.Default.appBinPostgresLocation + "pg_dump.exe", @"C:\TISOLUCIONES\EMPRESAS_BORRADAS\" + empresa + ".backup", UserConfig.Default.csServidor, "5432", empresa, UserConfig.Default.csUsuario, Password);

                            ResultState = PostgreSqlDump.DropDB(empresa, nodeId);
                        }
                        else
                        {
                            SqlServerDump.Backup(empresa, @"C:\TISOLUCIONES\EMPRESAS_BORRADAS\" + empresa + ".bak");
                            ResultState = SqlServerDump.DropDB(empresa, nodeId);
                        }

                        if (ResultState)
                        {
                            MessageBox.Show("Empresa Eliminada Exitosamente!.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnBuscar_Click(sender, e);
                        }
                        else
                            MessageBox.Show("La Empresa no se pudo eliminar, Por favor contacte a su proveedor de Sistemas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case "_btnRestaurar":
                    var _empresa = e.Cell.Row.Cells["v_RUC"].Value.ToString();
                    frmRestaurarBd _f = new frmRestaurarBd(_empresa);
                    _f.ShowDialog();
                    break;
            }
        }
    }
}
