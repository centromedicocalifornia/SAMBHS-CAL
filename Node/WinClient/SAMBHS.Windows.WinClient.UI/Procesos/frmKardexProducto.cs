using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmKardexProducto : Form
    {
        string descripcionProducto,idProductoDetalle;
        int idAlmacen,FormatoCant;
        MovimientoBL _objMovimientoBL = new MovimientoBL(); 
        
        public frmKardexProducto(string descripcionproducto, string idproductodetalle,int idalmacen,int FormatCant)
        {
            InitializeComponent();
            descripcionProducto = descripcionproducto;
            idProductoDetalle = idproductodetalle;
            idAlmacen = idalmacen;
            FormatoCant = FormatCant;

        }

        private void frmKardexProducto_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            txtBuscarProducto.Text = descripcionProducto.Trim();
            BindGrid();
            CalcularSaldo();
            this.Location = new Point(50, 140);
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            
        }
        private void CalcularSaldo()
        {
            decimal saldo = 0;
           
            //decimal Salidas, entradas = 0;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (  Fila.Cells["EsDevolucion"].Value==null ||  int.Parse(Fila.Cells["EsDevolucion"].Value.ToString()) == 0 )
                {
                    //saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo + decimal.Parse(Fila.Cells["Ingresos"].Value.ToString()) : saldo - decimal.Parse(Fila.Cells["Salidas"].Value.ToString());
                    //Fila.Cells["Saldo"].Value = saldo;
                    saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo + decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString()) : saldo - decimal.Parse(Fila.Cells["SalidasCantidadEmpaque"].Value.ToString());
                    Fila.Cells["Saldo"].Value = saldo;
                    
                }
                else
                {

                    //saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo - decimal.Parse(Fila.Cells["Ingresos"].Value.ToString()) : saldo + decimal.Parse(Fila.Cells["Salidas"].Value.ToString());
                    //Fila.Cells["Saldo"].Value = saldo;

                    saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo - decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString()) : saldo + decimal.Parse(Fila.Cells["SalidasCantidadEmpaque"].Value.ToString());
                    Fila.Cells["Saldo"].Value = saldo;
                }
            }
        }
        private void BindGrid()
        {
            OperationResult objOperationResult = new OperationResult();
            var objData = GetData(ref objOperationResult);

            if (objOperationResult.Success == 1)
            {
                grdData.DataSource = objData;
                grdData.Focus();
                lblContadorFilas.Text = string.Format("Se encontraron {0} Movimientos.", objData.Count);
            }
            else
            {

                lblContadorFilas.Text = string.Format("Se encontraron {0} Movimientos.");
            }
        }

        private List<movimientodetalleDto> GetData(ref OperationResult objOperationResult)
        {
           
            var _objData = _objMovimientoBL.ObtenerDetalleKardex(ref objOperationResult, idAlmacen, idProductoDetalle, FormatoCant);
            return _objData;
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["EsDevolucion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["Icono"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            grdData.DisplayLayout.Bands[0].Columns["Icono"].DataType = typeof(Image); 
            //UltraGridLayout layout = e.Layout; UltraGridBand band = layout.Bands[0];
            //UltraGridColumn imageColumn = band.Columns.Add("My Image Column");
            //imageColumn.DataType = typeof(Image); imageColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;



        }

        private void frmKardexProducto_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape: this.Close();
                    break;
            
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeIngreso && e.Row.Cells["Origen"].Value.ToString() == "T")
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\refresh.ico");
            }
            else if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeIngreso)
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\redo.ico");
            }

            if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeSalida && e.Row.Cells["Origen"].Value.ToString() == "T")
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\refresh.ico");
            }
            else if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeSalida)
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\undo.ico");
            }

            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("0000");
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
               case "Ver":
               if (grdData.ActiveRow != null && grdData.ActiveRow.Cells["v_IdMovimiento"]!=null )
                 {

                 new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                if ( int.Parse(grdData.ActiveRow.Cells["TipoMovimiento"].Value.ToString())==(int)TipoDeMovimiento.NotadeIngreso)
                {

                frmNotaIngreso frm = new frmNotaIngreso("Edicion",grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString(),"KARDEX");
                frm.ShowDialog();
                }
                else 
                {
                    frmNotaSalida frm = new frmNotaSalida("Edicion", grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString(), "KARDEX");
                    frm.ShowDialog();
                }
            }
                   

                    break;
            }
        }

        
    }
}
