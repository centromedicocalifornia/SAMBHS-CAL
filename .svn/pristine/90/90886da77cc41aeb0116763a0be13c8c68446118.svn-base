using Infragistics.Win.UltraWinStatusBar;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Infragistics.Win;

namespace SAMBHS.Common.Resource
{
    public class UltraStatusbarManager
    {
        public static void Inicializar(UltraStatusBar Statusbar)
        {
            //Statusbar.Appearance.BorderColor = Color.FromArgb(195,195,195);
            Statusbar.Appearance.BackColor = Color.FromArgb(195,195,195);
            Statusbar.Appearance.BackColor2 = Color.FromArgb(195,195,195);
            Statusbar.Appearance.ForeColor = Color.Black;
            Statusbar.Appearance.FontData.Name = @"Arial";
            Statusbar.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
            Statusbar.Appearance.TextVAlign = VAlign.Middle;
            Statusbar.UseAppStyling = true;
            Statusbar.ViewStyle = ViewStyle.Office2007;
            if (!Statusbar.Panels.Exists("kpanel"))
            {
                var panel = Statusbar.Panels.Add("kpanel");
                panel.Text = @"Listo!";
                panel.SizingMode = PanelSizingMode.Spring;
            }
        }

        public static void Inicializar(UltraStatusBar Statusbar, decimal TipoCambio)
        {
            //Statusbar.Appearance.BorderColor = Color.FromArgb(195,195,195);
            Statusbar.Appearance.BackColor = Color.FromArgb(195,195,195);
            Statusbar.Appearance.BackColor2 = Color.FromArgb(195,195,195);
            Statusbar.Appearance.ForeColor = Color.Black;
            Statusbar.Appearance.FontData.Name = @"Arial";
            Statusbar.Appearance.TextVAlign = VAlign.Middle;
            Statusbar.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
            Statusbar.UseAppStyling = true;
            Statusbar.ViewStyle = ViewStyle.Office2007;
            if (Statusbar.Panels.Count != 2)
            {
                Statusbar.Panels.Add(new UltraStatusPanel {Width = 200});
                Statusbar.Panels[0].Text = @"Listo!";
                Statusbar.Panels.Add(new UltraStatusPanel {Width = 200});
                Statusbar.Panels[1].Text = string.Format("Tipo Cambio: {0}", TipoCambio);
            }
            else
            {
                Statusbar.Panels[0].Text = @"Listo!";
                Statusbar.Panels[1].Text = string.Format("Tipo Cambio: {0}", TipoCambio);
            }                             
        }

        public static void MarcarError(UltraStatusBar Statusbar, string MensajeError, Timer Temporizador)
        {
            Temporizador.Stop();
            Statusbar.Appearance.BackColor = Color.FromArgb(202,81,0);
            Statusbar.Appearance.BackColor2 = Color.FromArgb(202, 81, 0);
            Statusbar.Appearance.ForeColor = Color.DarkRed;
            Statusbar.Appearance.FontData.Name = @"Arial";
            Statusbar.Appearance.TextVAlign = VAlign.Middle;
            Statusbar.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
            Statusbar.Panels[0].Text = MensajeError;
            Temporizador.Start();
        }

        public static void Reestablecer(UltraStatusBar Statusbar, Timer Temporizador)
        {
            Temporizador.Stop();
            Statusbar.Appearance.BackColor = Color.FromArgb(195,195,195);
            Statusbar.Appearance.BackColor2 = Color.FromArgb(195,195,195);
            Statusbar.Appearance.ForeColor = Color.Black;
            Statusbar.Appearance.FontData.Name = @"Arial";
            Statusbar.Appearance.TextVAlign = VAlign.Middle;
            Statusbar.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
            Statusbar.Panels[0].Text = @"Listo!";
        }

        public static void Mensaje(UltraStatusBar Statusbar, string Mensaje, Timer Temporizador)
        {
            Temporizador.Stop();
            Statusbar.Appearance.BackColor = Color.FromArgb(195,195,195);
            Statusbar.Appearance.BackColor2 = Color.FromArgb(195,195,195);
            Statusbar.Appearance.ForeColor = Color.Black;
            Statusbar.Appearance.TextVAlign = VAlign.Middle;
            Statusbar.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
            Statusbar.Panels[0].Text = Mensaje;
        }

        public static void MarcarAlerta(UltraStatusBar Statusbar, string MensajeError, Timer Temporizador)
        {
            Temporizador.Stop();
            Statusbar.Appearance.BackColor = Color.Red;
            Statusbar.Appearance.BackColor2 = Color.Red;
            Statusbar.Appearance.FontData.Name = @"Arial";
            Statusbar.Appearance.ForeColor = Color.DarkRed;
            Statusbar.Appearance.TextVAlign = VAlign.Middle;
            Statusbar.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            Statusbar.Panels[0].Text = MensajeError;
            Temporizador.Start();
        }
    }
}
