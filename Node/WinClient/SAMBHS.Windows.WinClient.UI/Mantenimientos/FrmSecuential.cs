using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmSecuential : Form
    {
        #region Fields
        private readonly SecuentialBL _bl = new SecuentialBL();
        private readonly Dictionary<int, string> _tables;
        #endregion

        #region Init
        public FrmSecuential(string arg)
        {
            InitializeComponent();
            _tables = new Dictionary<int, string>
            {
                {10, "condicionpago"},
                {12, "agenciatransporte"},
                {14, "cliente"},
                {15, "vendedor"},
                {16, "transportistachofer"},
                {17, "transportistaunidadtransporte"},
                {18, "avalcliente"},
                {19, "color"},
                {20, "talla"},
                {21, "linea"},
                {22, "marca"},
                {24, "producto"},
                {25, "carteracliente"},
                {26, "productodetalle"},
                {28, "movimiento"},//gasto importacion
                {29, "movimientodetalle"},
                {30, "productoalmacen"},
                {31, "compra"},
                {32, "compradetalle"},
                {34, "concepto"},
                {36, "recibohonorario"},
                {37, "recibohonorariodetalle"},
                {38, "guiaremision"},
                {39, "guiaremisiondetalle"},
                {40, "venta"},
                {41, "ventadetalle"},
                {42, "pedido"},
                {43, "pedidodetalle"},
                {44, "cobranza"},
                {46,"cobranzapendiente"},
                {45 ,"cobranzadetalle"},
                {48, "listaprecio"},
                {49, "listapreciodetalle"},
                {51, "importacion"},
                {52, "importaciondetallefob"},
                {53,"importaciondetallegastos"},
                {54, "importaciondetalleproducto"},
                {55, "tesoreria"},
                {56,"tesoreriadetalle"},
                {57, "guiaremisioncompra"},
                {58, "guiaremisioncompradetalle"},
                {59, "diario"},
                {60, "diariodetalle"},
                {61, "pendientecobrar"},
                {62, "pendientecobrardetalle"},
                {63, "saldomensualbancos"},
                {64, "pendientesconciliacion"},
                {65, "saldoestadobancario"},
                {66, "movimientoestadobancario"},
                {69, "letras"},
                {70, "ordendecompra"},
                {71, "ordendecompradetalle"},
                {72, "letrascanje"},
                {73, "letrasdetalle"},
                {74, "activofijo"},
                {75, "activofijodetalle"},
                {79, "pago"},
                {81, "pagodetalle"},
                {82, "letraspagar"},
                {83, "letraspagarcanje"},
                {84, "letraspagardetalle"},
                {85, "planillaconceptos"},
                {96, "liquidacioncompra"},
                {97, "liquidacioncompradetalle"},
                {101,"nbs_formatounicofacturacion"},
                {102, "nbs_ordentrabajo"},
                {103, "nbs_ordentrabajodetalle"},
                {104,"nbs_formatounicofacturaciondetalle"},
                 {105,"nbs_ventakardex"},
                {108, "cajachica"},
                {109,"cajachicadetalle"}

            };
        }
        #endregion

        #region Events UI

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
            btnBuscarSecuenciales.Enabled = true;
            

        }

        private void grdData_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var o = (secuentialDto)grdData.ActiveRow.ListObject;
            OperationResult r;
            if (e.Cell.Column.Key == "i_SecuentialId")
            {
                if (e.Cell.EditorResolved.Value == null) return;
                int valor;
                if (!int.TryParse(e.Cell.EditorResolved.Value.ToString(), out valor)) return;
                o.i_SecuentialId = valor;
                _bl.Update(out r, o);
                if (r.Success == 1)
                    UltraMessageBox.Show("Guardado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (grdData.ActiveRow.GetCellValue("v_Nombre") == null) return;
                var max = _bl.GetMaxFromTable(out r, o, (string)grdData.ActiveRow.GetCellValue("v_Nombre"));
                grdData.ActiveRow.Cells["v_Max"].Value = max;
                grdData.ActiveRow.Cells["i_SecuentialId"].Appearance.BackColor = o.i_SecuentialId < max ? Color.Orange : Color.White;
            }
            if (r.Success == 0)
                UltraMessageBox.Show(r.ExceptionMessage, "Error", Icono: MessageBoxIcon.Error);
        }
        #endregion

        #region Methods
        private void BindGrid()
        {
            try
            {
                grdData.DataSource = _bl.GetAll();
                foreach (var row in grdData.Rows)
                {
                    var id = (int)row.GetCellValue("i_TableId");
                    if (_tables.ContainsKey(id))
                    {
                        row.Cells["v_Nombre"].Value = _tables[id];
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception er)
            {
                UltraMessageBox.Show(er.Message, "Error", Icono: MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnBuscarSecuenciales_Click(object sender, EventArgs e)
        {

            foreach (var item in grdData.Rows)
            {

                var o = (secuentialDto)item.ListObject; // grdData.ActiveRow.ListObject;
                OperationResult r = new OperationResult();
                if (item.GetCellValue("v_Nombre") != null)
                {
                    var max = _bl.GetMaxFromTable(out r, o, (string)item.GetCellValue("v_Nombre"));
                    item.Cells["v_Max"].Value = max;
                    item.Cells["i_SecuentialId"].Appearance.BackColor = o.i_SecuentialId < max ? Color.Orange : Color.White;
                    if (r.Success == 0)
                        UltraMessageBox.Show(r.ExceptionMessage, "Error", Icono: MessageBoxIcon.Error);
                }
                else
                {
                    item.Cells["v_Max"].Value = 0;
                }


            }
            btnGuardarTodos.Enabled = true;
        }

        private void btnGuardarTodos_Click(object sender, EventArgs e)
        {
            int updates = 0;
            foreach (var item in grdData.Rows)
            {
                var o = (secuentialDto)item.ListObject;
                OperationResult r = new OperationResult();
                if (item.Cells["i_SecuentialId"].Value != null)
                {
                    int Secuential;
                    int.TryParse(item.Cells["i_SecuentialId"].Value.ToString(), out Secuential);
                    int Max;
                    int.TryParse(item.Cells["v_Max"].Value.ToString(), out Max) ;
                   
                    if (Secuential < Max)
                    {
                        o.i_SecuentialId = Max + 1;
                        _bl.Update(out r, o);
                        updates++;
                      

                    }
                    
                }
            }

            UltraMessageBox.Show(string.Concat("Se Actualizaron los Secuenciales!",
                           System.Environment.NewLine,
                           updates, " Actualizados"), "Resumen");

            btnBuscarSecuenciales_Click(sender, e);


        }

    }
}
