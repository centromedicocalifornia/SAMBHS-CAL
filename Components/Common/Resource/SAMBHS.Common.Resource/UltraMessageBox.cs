using System.Windows.Forms;

namespace SAMBHS.Common.Resource
{
    public static class UltraMessageBox
    {
        /// <summary>
        /// Muestra una caja de mensaje Infragistics
        /// </summary>
        /// <param name="Mensaje"></param>
        /// <param name="Titulo"></param>
        /// <param name="Botones"></param>
        /// <param name="Icono"></param>
        /// <param name="PieDeMensaje"></param>
        /// <returns></returns>
        public static DialogResult Show(string Mensaje, string Titulo = null, MessageBoxButtons Botones = MessageBoxButtons.OK, MessageBoxIcon Icono = MessageBoxIcon.Information, string PieDeMensaje = null)
        {

            //    UltraMessageBoxManager uMessageBox = new UltraMessageBoxManager();
            //    UltraMessageBoxInfo CajaDeMensaje = new UltraMessageBoxInfo();
            //    CajaDeMensaje.Caption = Titulo;
            //    CajaDeMensaje.Text = Mensaje;
            //    CajaDeMensaje.Icon = Icono;
            //    CajaDeMensaje.Buttons = Botones;
            //    CajaDeMensaje.FooterAppearance.ForeColor = Color.Red;
            //    CajaDeMensaje.ButtonAreaAppearance.BackColor = new GlobalFormColors().BannerColor;
            //    if (!String.IsNullOrEmpty(PieDeMensaje))
            //    {
            //        CajaDeMensaje.Footer = PieDeMensaje;
            //    }

            //    return uMessageBox.ShowMessageBox(CajaDeMensaje);
            return MessageBox.Show(Mensaje, Titulo,Botones, Icono);
        }

    }
}
