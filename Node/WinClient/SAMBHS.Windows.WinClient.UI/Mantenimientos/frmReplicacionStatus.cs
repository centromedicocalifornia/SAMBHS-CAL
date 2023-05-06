using System;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmReplicacionStatus : Form
    {
        private System.Timers.Timer time;
        public frmReplicacionStatus(string N)
        {
            InitializeComponent();
        }

        private void frmReplicacionStatus_Load(object sender, EventArgs e)
        {
            BackColor = BackColor = new GlobalFormColors().FormColor;
            cbEstado.Value = "-1";
            
            if (UserConfig.Default.repCurrentNode == "N")
            {
                txtSucursal.TextChanged += delegate { Filtrar(); };
                cbEstado.ValueChanged += delegate { Filtrar(); };
            }
            time = new System.Timers.Timer(5000);
            time.Elapsed += delegate {
                Invoke((MethodInvoker)BindGrid);
            };
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Filtrar();
        }

        #region Grilla
        private void BindGrid()
        {
            if (UserConfig.Default.repCurrentNode != "N") return;
            grdata.DataSource = ReplicationStatus.GetStatus();
            grdata.Rows.ToList().ForEach(fila =>
            {
                if ((bool)(fila.Cells["Status"].Value))
                {
                    fila.Cells["imageStatus"].Value = Resource.accept;

                }
                else
                {
                    fila.Cells["imageStatus"].Value = Resource.cancel;
                }
            });
        }

        private void Filtrar()
        {
            if(UserConfig.Default.repCurrentNode != "N")
            {
                UltraMessageBox.Show("Esta Ventana Solo esta Disponible para el Publicador", "Error de Busqueda");
                return;
            }
            var sucursal = txtSucursal.Text.Trim();
            var estado = cbEstado.Value.ToString() == "0";

            foreach (var row in grdata.Rows)
            {
                var ocultar = true;
                if (row.Cells["NameSuscriptor"].Value.ToString().StartsWith(sucursal))
                {
                    ocultar = cbEstado.Value.ToString() != "-1" && !(bool)row.Cells["Status"].Value == estado;
                }
                else
                    ocultar = true;
                row.Hidden = ocultar;
            }
        }

        private void grdata_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (grdata.ActiveRow == null) return;
            grHistory.DataSource = ReplicationStatus.GetHistorial((int)grdata.ActiveRow.Cells["Id"].Value, 12);

            grHistory.Rows.ToList().ForEach(fila =>
            {
                fila.Cells["image"].Value = (int)(fila.Cells["ErrorCode"].Value) == 0 ? Resource.accept : Resource.cancel;
            });
            ultraPopupControlContainer1.Show();

        }
        #endregion

        #region Refresh Time
        private void frmReplicacionStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            time.Stop();
            time.Dispose();
        }
        private void frmReplicacionStatus_Activated(object sender, EventArgs e)
        {
            BindGrid();
            time.Start();
        }

        private void frmReplicacionStatus_Deactivate(object sender, EventArgs e)
        {
            time.Stop();
        }
        #endregion
    }
}
