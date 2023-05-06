using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Windows.WinClient.UI.Migraciones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmIgualarLineas : Form
    {
        UltraCombo ucLinea = new UltraCombo();
        List<GridKeyValueDTO> DSComboLineas = new List<GridKeyValueDTO>();
        LineaBL _objLineaBL = new LineaBL();
        List<string> _Lineas = new List<string>();

        public frmIgualarLineas(List<string> Lineas)
        {
            InitializeComponent();
            _Lineas = Lineas;
        }

        private void frmIgualarLineas_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            try
            {
                UltraDataColumn columnUM = this.uDataSource.Band.Columns.Add("v_UM");
                UltraDataColumn columnUMdb = this.uDataSource.Band.Columns.Add("i_UMdb");
                columnUM.DataType = typeof(string);
                columnUMdb.DataType = typeof(string);
                CargarCombos();

                foreach (string UM in _Lineas)
                {
                    List<object> rowData = new List<object>();
                    rowData.Add(UM);
                    rowData.Add(-1);
                    uDataSource.Rows.Add(rowData.ToArray());
                }

                grdData.DataSource = uDataSource;
                if (grdData.Rows.Count() > 0)
                {
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        var xl = DSComboLineas.Where(p => p.Value1 == Fila.Cells["v_UM"].Value.ToString().Trim().ToUpper());
                        if (xl.Count() != 0)
                        {
                            string Id = xl.Select(x => x.Id).First().ToString();
                            Fila.Cells["i_UMdb"].Value = Id != null | Id == string.Empty ? Id : "-1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }

        void CargarCombos()
        {
            #region Configura Combo Unidad Medida
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 267;
            ultraGridColumnaID.Hidden = true;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID });
            ucLinea.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucLinea.DropDownWidth = 270;
            ucLinea.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            OperationResult objOperationResult = new OperationResult();
            ucLinea.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            DSComboLineas = _objLineaBL.ObtenLineasParaComboGrid(ref objOperationResult);
            Utils.Windows.LoadUltraComboList(ucLinea, "Value1", "Id", DSComboLineas, DropDownListAction.Select);
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_UMdb"].EditorComponent = ucLinea;
            e.Layout.Bands[0].Columns["i_UMdb"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            List<string[]> ListaResultado = new List<string[]>();

            foreach (UltraGridRow Fila in grdData.Rows.Where(o => o.Cells["i_UMdb"].Value != null && o.Cells["i_UMdb"].Value.ToString() != "-1"))
            {
                string[] Cadena = new string[2];
                Cadena[0] = Fila.Cells[0].Value.ToString();
                Cadena[1] = Fila.Cells[1].Value.ToString();
                ListaResultado.Add(Cadena);
            }

            if (ListaResultado.Count() != grdData.Rows.Count())
            {
                UltraMessageBox.Show("Por favor seleccione una unidad de medida a todos los registros de la grilla.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (System.Windows.Forms.Application.OpenForms["frmMigraciones"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["frmMigraciones"] as frmMigraciones).RecibirListaLineas(ListaResultado);
                }
                this.Close();
            }
        }
    }
}
