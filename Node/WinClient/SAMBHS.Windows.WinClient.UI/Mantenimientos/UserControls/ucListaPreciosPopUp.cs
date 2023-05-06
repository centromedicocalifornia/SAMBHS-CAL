using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.WinClient.UI.Procesos;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls
{
    public partial class ucListaPreciosPopUp : UserControl
    {
        string IdProductoDetalle = string.Empty;
        int IdAlmacen = -1;
        TipoVenta oTipoVenta;
        ListaPreciosBL _objListaPreciosBL = new ListaPreciosBL();
        int IdListaCliente = -1;
        public ucListaPreciosPopUp(TipoVenta _TipoV, string idProductoDetalle, int idAlmacen)//,int pIntListaCliente
        {
            InitializeComponent();
            IdProductoDetalle = idProductoDetalle;
            oTipoVenta = _TipoV;
            IdAlmacen = idAlmacen;
          //  IdListaCliente = pIntListaCliente;
        }

        private void ucListaPreciosPopUp_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            using (GlobalFormColors color = new GlobalFormColors())
            {
                this.BackColor = color.FormColor;
               
            }
            grListaPrecios.DataSource = _objListaPreciosBL.ObtenerListaPrecioDetallesVenta(ref objOperationResult, IdProductoDetalle, IdAlmacen);
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_PrecioDecimales);


        }

        private void FormatoDecimalesGrilla(int DecimalesPrecio)
        {
            string FormatoPrecio;
            UltraGridColumn _Precio = this.grListaPrecios.DisplayLayout.Bands[0].Columns["d_Precio"];
            _Precio.MaskDataMode = MaskMode.IncludeLiterals;
            _Precio.MaskDisplayMode = MaskMode.IncludeLiterals;


            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "nnnnnn";
            }
            _Precio.MaskInput = "-" + FormatoPrecio;
        }
        public enum TipoVenta
        {
            VentaNormal = 1,
            VentaRapida = 2,
            Pedido = 3
        }

        private void grListaPrecios_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            switch (oTipoVenta)
            {
                case TipoVenta.VentaNormal:

                    if (Application.OpenForms["frmRegistroVenta"] != null)
                    {
                        ((frmRegistroVenta)Application.OpenForms["frmRegistroVenta"]).FijarPrecio(grListaPrecios.ActiveRow.Cells["d_Precio"].Value.ToString(), int.Parse(grListaPrecios.ActiveRow.Cells["i_IdMoneda"].Value.ToString()));
                    }
                    break;

                case TipoVenta.VentaRapida:
                    if (Application.OpenForms["frmRegistroVentaRapida"] != null)
                    {
                        ((frmRegistroVentaRapida)Application.OpenForms["frmRegistroVentaRapida"]).FijarPrecio(grListaPrecios.ActiveRow.Cells["d_Precio"].Value.ToString(), int.Parse(grListaPrecios.ActiveRow.Cells["i_IdMoneda"].Value.ToString()));
                    }
                    break;

                case TipoVenta.Pedido:

                    if (Application.OpenForms["frmPedido"] != null)
                    {
                        ((frmPedido)Application.OpenForms["frmPedido"]).FijarPrecio(grListaPrecios.ActiveRow.Cells["d_Precio"].Value.ToString(), int.Parse(grListaPrecios.ActiveRow.Cells["i_IdMoneda"].Value.ToString()));
                    }
                    break;

                default:
                    break;
            }
        }

        private void grListaPrecios_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }
    }
}
