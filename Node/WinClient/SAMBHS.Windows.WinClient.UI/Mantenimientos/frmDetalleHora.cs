using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmDetalleHora : Form
    {
        public string detalle = "";
        string edicion = "";
        public string descrpcion = "";
        public int Intervalo = 0;
        public int estadoRegistro = 0;
        public frmDetalleHora(string detalle_, string descripcion_)
        {
            edicion = detalle_;
            descrpcion = descripcion_;
            InitializeComponent();
        }

        private void frmDetalleHora_Load(object sender, EventArgs e)
        {
            if (edicion != string.Empty)
            {
                string Time1 = edicion.Split('-')[0].Trim();
                string Time2 = edicion.Split('-')[1].Trim();

                dtTimeDesde.Value = DateTime.Parse(Time1, System.Globalization.CultureInfo.CurrentCulture);
                dtTimeHasta.Value = DateTime.Parse(Time2, System.Globalization.CultureInfo.CurrentCulture);
                txtDescrpcion.Text = descrpcion;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (edicion == string.Empty)
            {
                if (txtIntervalo.Text == string.Empty || txtIntervalo.Text == "0")
                {
                    MessageBox.Show("¡FAVOR DE REGISTRAR UN INTERVALO CORRECTO!", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    detalle = dtTimeDesde.Value.TimeOfDay.ToString() + " - " + dtTimeHasta.Value.TimeOfDay.ToString();
                    descrpcion = txtDescrpcion.Text;
                    Intervalo = txtIntervalo.Text == string.Empty ? 0 : int.Parse(txtIntervalo.Text);

                    estadoRegistro = 1;
                    this.Close();
                }
            }
            else
            {
                detalle = dtTimeDesde.Value.TimeOfDay.ToString() + " - " + dtTimeHasta.Value.TimeOfDay.ToString();
                descrpcion = txtDescrpcion.Text;
                Intervalo = txtIntervalo.Text == string.Empty ? 0 : int.Parse(txtIntervalo.Text);

                estadoRegistro = 1;
                this.Close();
            }
        }
    }
}
