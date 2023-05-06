using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmBuscarTicket : Form
    {
        public BindingList<GridmovimientodetalleDto> ticketDetalle { get; set; }
        public frmBuscarTicket()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var oFarmaciaBl = new FarmaciaBl();

            ticketDetalle =  oFarmaciaBl.ObtenerDetalleTicket(txtNroTicket.Text);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
