using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Compra.BL ;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBusquedaFacturasImportacion : Form
    {
        UltraGrid GrillaImportada = new UltraGrid();
        ImportacionBL _objImportacionBL = new ImportacionBL ();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        UltraCombo ucbDocumento = new UltraCombo();
        public string _NroFactura,_CodProveedor,_NroPedido,_IdProveedor;


        public frmBusquedaFacturasImportacion(UltraGrid Grilla)
        {
            GrillaImportada = Grilla;
            InitializeComponent();
        }

        private void frmBusquedaFacturasImportacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarComboDetalle();
            OperationResult  objOperationResult= new OperationResult ();
            string ID = GrillaImportada.Rows[0].Cells["v_CodCliente"].Value.ToString ();
            grdDataFacturas.DataSource  = _objImportacionBL.ObtenerImportacionDetallesGastos(ref objOperationResult, "");

             if (GrillaImportada.Rows.Count ()>0)
             {
                 foreach (UltraGridRow Fila in GrillaImportada.Rows)
                 {
                     UltraGridRow row = grdDataFacturas.DisplayLayout.Bands[0].AddNew();
                     grdDataFacturas.Rows.Move(row, grdDataFacturas.Rows.Count() - 1);
                     this.grdDataFacturas.ActiveRowScrollRegion.ScrollRowIntoView(row);

                     row.Cells["i_IdTipoDocumento"].Value = Fila.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                     row.Cells["v_SerieDocumento"].Value = Fila.Cells["v_SerieDocumento"].Value == null ? null : Fila.Cells["v_SerieDocumento"].Value.ToString();
                     row.Cells["v_NroFactura"].Value = Fila.Cells["v_CorrelativoDocumento"].Value == null ? null : Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                     row.Cells["v_NroPedido"].Value = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                     row.Cells["v_CodigoCliente"].Value = Fila.Cells["v_CodCliente"].Value == null ? null : Fila.Cells["v_CodCliente"].Value.ToString();
                     row.Cells["v_RazonSocial"].Value = Fila.Cells["v_RazonSocial"].Value == null ? null : Fila.Cells["v_RazonSocial"].Value.ToString();
                     row.Cells["v_IdCliente"].Value = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();
                     


                 }
            }
        }


        private void CargarComboDetalle()
        {
            OperationResult objOperationResult = new OperationResult();
            //Configura Combo TipoDocumento
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            UltraGridBand UltraGridBanda0 = new UltraGridBand("Banda 0", -1);
            UltraGridColumn ultraGridColumna0 = new UltraGridColumn("Id");
            ultraGridColumna0.Width = 40;
            UltraGridColumn ultraGridColumnaDescripcion0 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion0.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion0.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion0.Width = 130;
            UltraGridColumn ultraGridColumna1 = new UltraGridColumn("Value2");
            ultraGridColumna1.Header.Caption = "Siglas";
            ultraGridColumna1.Width = 100;
            // ultraGridColumnaDescripcion0.Hidden = true;
            UltraGridBanda0.Columns.AddRange(new object[] { ultraGridColumnaDescripcion0, ultraGridColumna0, ultraGridColumna1 });
            ucbDocumento.DisplayLayout.BandsSerializer.Add(UltraGridBanda0);
            ucbDocumento.DropDownWidth = 250;
            ucbDocumento.DropDownStyle = UltraComboStyle.DropDownList;
            ucbDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select); //17-Unidad Medida 
        
        }

        private void grdDataFacturas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].EditorComponent = ucbDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void grdDataFacturas_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
             if (grdDataFacturas.Rows.Count == 0) return; //se cambio

             if (grdDataFacturas.ActiveRow != null)
             {

                 if (grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_NroFactura"].Value != null)
                 {
                     
                     _NroFactura =grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text +" "+grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString()+"-"+grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_NroFactura"].Value.ToString();
                     _NroPedido = grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString ();
                     _CodProveedor = grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_CodigoCliente"].Value.ToString(); 
                     _IdProveedor =grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString(); 
                   
                    
                 }
             }
             this.Close();
        }

        private void grdDataFacturas_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            
           
        }

        private void grdDataFacturas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (grdDataFacturas.ActiveRow != null)
                {
                    if (grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_NroFactura"].Value != null)
                    {

                        _NroFactura = grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                        _NroPedido = grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString();
                        _CodProveedor = grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_CodigoCliente"].Value.ToString();
                        _IdProveedor = grdDataFacturas.Rows[grdDataFacturas.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString(); 

                    }
                }
            }
            this.Close();
        }



    }
}
