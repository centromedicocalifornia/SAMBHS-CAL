using SAMBHS.Windows.SigesoftIntegration.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmValidacion : Form
    {
        public string Respuesta { get; set; }
        public string Usuario { get; set; }

        public frmValidacion()
        {
            InitializeComponent();
        }

        private void frmValidacion_Load(object sender, EventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("POR FAVOR INGRESE USUARIO Y/O CONTRASEÑA CORRECTA", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                string usuario = txtUsuario.Text;
                string contraseña = txtContraseña.Text;
                var usuariosystem = new ProtocoloBl().GetUsuario(usuario);
                if (usuariosystem == null)
                {
                    MessageBox.Show("INFORMACIÓN INCORRECTA", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (usuario == usuariosystem.v_UserName && usuariosystem.v_ComentaryUpdate == "AUTORIZADO")
                {
                    var a = Encrypt(contraseña);
                    if (usuariosystem.v_Password != a)
                    {
                        MessageBox.Show("¡CONTRASEÑA INCORRECTA!", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {

                        Respuesta = "ACEPTADO";
                        Usuario = usuario;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("EL USUARIO " + usuario + " no está autorizado. Comuníquese con el Administrador", "USUARIO NO VÁLIDO!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        #region Encription
        public static string Encrypt(string contraseña)
        {
            UnicodeEncoding parser = new UnicodeEncoding();
            byte[] _original = parser.GetBytes(contraseña);
            MD5CryptoServiceProvider Hash = new MD5CryptoServiceProvider();
            byte[] _encrypt = Hash.ComputeHash(_original);
            return Convert.ToBase64String(_encrypt);

            //byte[] data = Convert.FromBase64String(pData);
            //string decodedString = Encoding.UTF8.GetString(data);
            //byte[] EnCode = Hash.ComputeHash(Encoding.UTF8.GetBytes(BitConverter.ToString(data)));

            //return decodedString;
        }
        #endregion

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnAceptar_Click(null, null);
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtContraseña_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnAceptar_Click(null, null);
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

    }
}
