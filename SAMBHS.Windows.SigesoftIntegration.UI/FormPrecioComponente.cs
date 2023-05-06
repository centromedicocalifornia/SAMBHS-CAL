using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sigesoft.Node.WinClient.UI
{
    public partial class FormPrecioComponente : Form
    {
        public float Precio { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Descuento { get; set; }
        private string Service;
        private string NombreComponente;
        public FormPrecioComponente(string pstrNombreComponente, string pdecPrecio, string _Service)
        {          
            InitializeComponent();
            NombreComponente = pstrNombreComponente;
            lblNombreComponente.Text = pstrNombreComponente;
            txtPrecio.Text = pdecPrecio;
            Service = _Service;
            calcular();           
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtFactor.Text =="0.00")
            {
                MessageBox.Show("Ingrese datos válidos");
                return;
            }
            else {
                Precio = float.Parse(txtTotal.Text.ToString());
                Cantidad = decimal.Parse(txtFactor.Text.ToString());
                Descuento = decimal.Parse(txtDescuento.Text.ToString());
                this.DialogResult = DialogResult.OK;
            }
        }

        private void txtFactor_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFactor.Text))
            {
                txtFactor.Text = "1.00";
                MessageBox.Show("Debe completar la informacion");
                return;
            }
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

       private void FormPrecioComponente_Load(object sender, EventArgs e)
       {
           var objDescuentoProtocolo = new ServiceBL().GetDescuentosProtocolo(Service);

           var objComponentCategoria = new ServiceBL().getCategoriaExamen(NombreComponente);

           if (objComponentCategoria.v_Value1 == "LABORATORIO C")
           {
               txtDescuento.Text = objDescuentoProtocolo.d_DescuentoLaboratorio == null || objDescuentoProtocolo.d_DescuentoLaboratorio == 0 ? "0.00" : objDescuentoProtocolo.d_DescuentoLaboratorio.ToString();
           }
           else if (objComponentCategoria.v_Value1 == "MEDICINA C")
           {
                txtPrecio.Text = objDescuentoProtocolo.d_PrecioConsulta == null || objDescuentoProtocolo.d_PrecioConsulta == 0 ? "0.00" : objDescuentoProtocolo.d_PrecioConsulta.ToString();
           }
           else if (objComponentCategoria.v_Value1 == "RADIOLOGIA")
           {
               txtDescuento.Text = objDescuentoProtocolo.d_DescuentoRayosX == null || objDescuentoProtocolo.d_DescuentoRayosX == 0 ? "0.00" : objDescuentoProtocolo.d_DescuentoRayosX.ToString();
           }
           else if (objComponentCategoria.v_Value1 == "ECOGRAFIA" || objComponentCategoria.v_Value1 == "ECOGRAFIAS")
           {
               txtDescuento.Text = objDescuentoProtocolo.d_DescuentoEcografias == null || objDescuentoProtocolo.d_DescuentoEcografias == 0 ? "0.00" : objDescuentoProtocolo.d_DescuentoEcografias.ToString();
           }
           else if (objComponentCategoria.v_Value1 == "ODONTOLOGIA")
           {
               txtDescuento.Text = objDescuentoProtocolo.d_DescuentoOdontologia == null || objDescuentoProtocolo.d_DescuentoOdontologia == 0 ? "0.00" : objDescuentoProtocolo.d_DescuentoOdontologia.ToString();
           }
           else if (objComponentCategoria.v_Value1 == "AMBULANCIA")
           {
               if (NombreComponente == "TRASLADO AMBULANIA CONVENIO")
               {
                   txtPrecio.Text = objDescuentoProtocolo.d_PrecioAmbulancia == null || objDescuentoProtocolo.d_PrecioAmbulancia == 0 ? "0.00" : objDescuentoProtocolo.d_PrecioAmbulancia.ToString();
               }
           }
           else if (objComponentCategoria.v_Value1 == "SOP")
           {
               if (NombreComponente == "SALA DE OPERACIONES POR HORA (CONVENIO)")
               {
                   txtPrecio.Text = objDescuentoProtocolo.d_SalaOperaciones == null || objDescuentoProtocolo.d_SalaOperaciones == 0 ? "0.00" : objDescuentoProtocolo.d_SalaOperaciones.ToString();
               }
           }
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
                

     

    }
}
