﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors.UltraWinCalc;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmConfigSeguros : Form
    {
        public decimal? paciente;
        public decimal? aseguradora;
        public float? nuevoPrecio;
        public bool result;

        public frmConfigSeguros(int deducible, int coaseguro, decimal? importe, string precio, string factor, decimal? importeCo)
        {
            InitializeComponent();

            if (deducible == 1 || deducible == 0)
            {
                if (deducible == 1){rbDeducible.Checked = true;}
                else
                {
                    rbDeducible.Checked = false;
                    rbDeducible.Visible = false;
                    txtImporte.Visible = false;
                    lblmontoDeducible.Visible = false;
                }
            }

            if (coaseguro == 1 || coaseguro == 0)
            {
                if (coaseguro == 1){rbCoaseguro.Checked = true;}
                else
                {
                    rbCoaseguro.Checked = false;
                    rbCoaseguro.Visible = false;
                    txtCoaseguro.Visible = false;
                    lblMontoCoaseguro.Visible = false;
                }
            }
            txtCoaseguro.Text = importeCo.ToString();
            txtFactor.Text = factor;
            txtPrecioBase.Text = precio;
            txtImporte.Text = importe.ToString();
            Calculator(rbCoaseguro.Checked, rbDeducible.Checked, txtImporte.Text, txtCoaseguro.Text);
            
        }

        private void Calculator(bool coaseguro, bool deducible, string importe, string importeCo)
        {
            txtnuevoPrecio.Text = (double.Parse(txtPrecioBase.Text) * double.Parse(txtFactor.Text)).ToString();
            if (deducible == true)
            {
                txtPagoPaciente.Text = importe;
                txtPagoAseguradora.Text = (double.Parse(txtnuevoPrecio.Text) - double.Parse(txtPagoPaciente.Text)).ToString();
            }
            else if (coaseguro == true)
            {


                txtPagoPaciente.Text = (double.Parse(importeCo) * double.Parse(txtnuevoPrecio.Text) / 100).ToString();
                txtPagoAseguradora.Text = (double.Parse(txtnuevoPrecio.Text) - double.Parse(txtPagoPaciente.Text)).ToString();
                
            }
        }

        private void frmConfigSeguros_Load(object sender, EventArgs e)
        {
            
        }

        private void txtFactor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                Calculator(rbCoaseguro.Checked, rbDeducible.Checked, txtImporte.Text, txtCoaseguro.Text);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            paciente = decimal.Parse(txtPagoPaciente.Text);
            aseguradora = decimal.Parse(txtPagoAseguradora.Text);
            nuevoPrecio = float.Parse(txtnuevoPrecio.Text);
            result = true;
            this.Close();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            result = false;
            this.Close();

        }

        //public frmConfigSeguros(string p1, string p2)
        //{
        //    // TODO: Complete member initialization
        //    this.p1 = p1;
        //    this.p2 = p2;
        //}

        //public float? Precio { get; set; }

        private void frmConfigSeguros_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (result != true)
            {
                result = false;
            }
            
        }

        private void txtPrecioBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                Calculator(rbCoaseguro.Checked, rbDeducible.Checked, txtImporte.Text, txtCoaseguro.Text);
            }
        }

        private void txtImporte_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                Calculator(rbCoaseguro.Checked, rbDeducible.Checked, txtImporte.Text, txtCoaseguro.Text);
            }
        }
    }
}
