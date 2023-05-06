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
    public partial class FormPrecioComponente_Seg_ : Form
    {
        public string IdComponente { get; set; }
        public string NombreComponente { get; set; }
        public int Plan { get; set; }
        public decimal Coaseguro { get; set; }
        public FormPrecioComponente_Seg_(string _NombreComponente, string _ComponentId)
        {          
            InitializeComponent();
            IdComponente = _ComponentId;
            NombreComponente = _NombreComponente;
            calcular();           
        }

        void calcular()
        {
            //VALIDALO CUANDO ESTE NULL
            if (txtFactor.Text == "0.00")
            {
                MessageBox.Show("Ingrese datos válidos");
                return;
            }
            {
                double Precio = txtPrecio.Text == null ? 0.00 : double.Parse(txtPrecio.Text.ToString());
                double Factor = txtFactor.Text == null ? 0.00 : double.Parse(txtFactor.Text.ToString());
                double Descuento = txtDescuento.Text == null ? 0.00 : double.Parse(txtDescuento.Text.ToString());
                if (Descuento == 0.00)
                    txtTotal.Text = (Precio * Factor).ToString();
                else
                    txtTotal.Text = (((Precio * Factor)) - (((Precio * Factor) * Descuento) / 100)).ToString();

                if (Plan != 0)
                {
                    txtSaldoPaciente.Text = (decimal.Round(((decimal.Parse(txtTotal.Text) * Coaseguro) / 100), 2)).ToString();
                    txtSaldoSeguro.Text = ((decimal.Round(decimal.Parse(txtTotal.Text) - decimal.Parse(txtSaldoPaciente.Text), 2))).ToString();
                }
            }

        }

        private void FormPrecioComponente_Seg__Load(object sender, EventArgs e)
        {
            lblNombreComponente.Text = NombreComponente;

            var objserviceComponent = AgendaBl.GetServiceComponentEditPrecio(IdComponente);
            txtPrecio.Text = objserviceComponent.r_Price.ToString();
            txtFactor.Text = objserviceComponent.Cantidad.ToString();
            txtDescuento.Text = objserviceComponent.Descuento.ToString();
            Plan = objserviceComponent.PlanSeguro;
            Coaseguro = objserviceComponent.Coaseguro;
            if (Plan != 0)
            {
                lblInfoSeguro.Text = "Coaseguro al "+ Coaseguro.ToString() + " / Deducible S/. " + objserviceComponent.Descuento.ToString();

                txtSaldoPaciente.Text = objserviceComponent.SaldoPaciente.ToString();
                txtSaldoSeguro.Text = objserviceComponent.SaldoSeguro.ToString();
            }
            else
            {
                lblInfoSeguro.Text = "- - -";
                txtSaldoPaciente.Enabled = false;
                txtSaldoSeguro.Enabled = false;
            }

        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrecio.Text))
            {
                txtPrecio.Text = "0.00";
                MessageBox.Show("Debe completar la informacion");
                return;
            }
            calcular();
        }

        private void txtFactor_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFactor.Text))
            {
                txtFactor.Text = "0.00";
                MessageBox.Show("Debe completar la informacion");
                return;
            }
            calcular();
        }

        private void txtDescuento_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescuento.Text))
            {
                txtDescuento.Text = "0.00";
                MessageBox.Show("Debe completar la informacion");
                return;
            }
            calcular();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            decimal newPrice = decimal.Round((decimal.Parse(txtTotal.Text)),2);
            decimal cantidad = decimal.Round((decimal.Parse(txtFactor.Text)),2);
            decimal descuento = decimal.Round((decimal.Parse(txtDescuento.Text)),2);
            decimal saldoP = decimal.Round((decimal.Parse(txtSaldoPaciente.Text)),2);
            decimal saldoS = decimal.Round((decimal.Parse(txtSaldoSeguro.Text)),2);

           new AgendaBl().UpdateServiceComponentPrice(newPrice, cantidad, descuento, saldoP, saldoS, IdComponente, Plan);

           MessageBox.Show("GUARDADO CORRECTAMENTE", "CORRECTO", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
