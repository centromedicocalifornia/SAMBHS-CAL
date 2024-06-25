using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmBuscarProtocolo : Form
    {
        private List<EsoDtoProt> _listaProtocolos = new List<EsoDtoProt>();
        private List<EsoDtoProt> _listaProtocolosTemp = new List<EsoDtoProt>();
        public string protocoloId = "-1";

        public frmBuscarProtocolo(List<EsoDtoProt> listaProtocolos)
        {
            _listaProtocolos = listaProtocolos;

            InitializeComponent();
        }

        private void frmBuscarProtocolo_Load(object sender, EventArgs e)
        {
            _listaProtocolos.OrderBy(p => p.Nombre).ToList();
            grdDataCalendar.DataSource = _listaProtocolos;
            if (_listaProtocolos != null)
            {
                lblRecordCount.Text = string.Format("Se encontraron {0} registros.", _listaProtocolos.Count());

            }
        }

        private void txtPacient_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtPacient.Text != string.Empty)
            {

                _listaProtocolosTemp = new List<EsoDtoProt>(_listaProtocolos.Where(p => p.Nombre.Contains(txtPacient.Text.ToUpper())));

                grdDataCalendar.DataSource = _listaProtocolosTemp;

                if (_listaProtocolosTemp != null)
                {
                    lblRecordCount.Text = string.Format("Se encontraron {0} registros.", _listaProtocolosTemp.Count());

                }

            }
            else
            {
                grdDataCalendar.DataSource = _listaProtocolos;
                if (grdDataCalendar != null)
                {
                    lblRecordCount.Text = string.Format("Se encontraron {0} registros.", _listaProtocolos.Count());

                }
            }
        }

        private void grdDataCalendar_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            protocoloId = grdDataCalendar.Selected.Rows[0].Cells["Id"].Value.ToString();
            this.Close();
        }
    }
}
