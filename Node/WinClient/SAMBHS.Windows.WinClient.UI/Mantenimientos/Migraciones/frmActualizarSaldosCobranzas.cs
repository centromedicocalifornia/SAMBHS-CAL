using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
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
    public partial class frmActualizarSaldosCobranzas : Form
    {
        CobranzaBL _objCobranzaBL = new CobranzaBL();
        public frmActualizarSaldosCobranzas()
        {
            InitializeComponent();
        }




        private void cboImportarCabecera_Click_1(object sender, EventArgs e)
        {
            if (grdCabecera.Rows.Count() > 0)
            {
                grdCabecera.Selected.Rows.AddRange((UltraGridRow[])grdCabecera.Rows.All);
                grdCabecera.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName = "";
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Archivos Excel (*.*)| *.*";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;
                }
                else
                {
                    return;
                }

                using (new LoadingClass.PleaseWait(this.Location, "Importando Cabeceras..."))
                {
                    Workbook workbook = Workbook.Load(sFileName);
                    Worksheet worksheet = workbook.Worksheets[0];

                    this.DataSourceCabecera.Reset();


                    bool isHeaderRow = true;
                    foreach (WorksheetRow row in worksheet.Rows)
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                string columnKey = cell.GetText();

                                UltraDataColumn column = this.DataSourceCabecera.Band.Columns.Add(columnKey);

                                switch (columnKey)
                                {
                                    default:
                                        column.DataType = typeof(string);
                                        break;
                                }
                            }

                            isHeaderRow = false;
                        }
                        else
                        {
                            List<object> rowData = new List<object>();
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                rowData.Add(cell.Value);
                            }

                            this.DataSourceCabecera.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdCabecera.SetDataBinding(this.DataSourceCabecera, "Band 0");

            lblContadorFilasCabecera.Text = string.Format("Se encontraron {0} registros", grdCabecera.Rows.Count());

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            List<UltraGridRow> ListadoCobranzas = grdCabecera.Rows.AsParallel().ToList();
            List<SaldosCancelados>  ListaCobrados= new List<SaldosCancelados> ();
            OperationResult objOperationResult = new OperationResult();

          foreach (var item in ListadoCobranzas)
	{
		 SaldosCancelados objReporte = new SaldosCancelados ();
              objReporte.Serie  = item.Cells["SERIE"].Value != null ? item.Cells["SERIE"].Value.ToString() : "";
              objReporte.TipoDoc =item.Cells["TIPO DOC"].Value != null ? int.Parse ( item.Cells["TIPO DOC"].Value.ToString()) : -1;
             objReporte.Correlativo =item.Cells["NRO DOC"].Value != null ? item.Cells["NRO DOC"].Value.ToString() :"";
              ListaCobrados.Add (objReporte );

	}

               _objCobranzaBL.AcualizarSaldosCobranzasPendientes(ref objOperationResult, ListaCobrados);
               if (objOperationResult.Success == 1)
               {
                   UltraMessageBox.Show("Proceso Culminado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               }
               else
               {
                   UltraMessageBox.Show("Hubo Error al procesar datos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error );
               }

               
            }

        
    }
}
