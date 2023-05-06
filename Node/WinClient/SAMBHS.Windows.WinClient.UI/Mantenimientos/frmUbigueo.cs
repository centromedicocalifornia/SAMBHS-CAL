using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmUbigueo : Form
    {
        public frmUbigueo(string N)
        {
            InitializeComponent();
            this.Shown += frmUbigueo_Shown;
        }

        void frmUbigueo_Shown(object sender, EventArgs e)
        {
            Form f = new Form();
            f.IsMdiContainer = true;
            this.MdiParent = f;
            f.ShowIcon = false;
            f.Size = this.Size + new Size(50,25);
            f.Show();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog {Filter = @"xlsx files (*.xlsx)|*.xlsx", Multiselect = false};
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {              
                Workbook book = Workbook.Load(op.FileName);
                var sheet = book.Worksheets.First();

                using (var trans = TransactionUtils.CreateTransactionScope())
                {
                    var objProxyCommon = new SystemParameterBL();
                    var operationResult = new OperationResult();
                    var peru = objProxyCommon.GetSystemParameter(ref operationResult, 112, 1);
                    peru.v_Value2 = "PEN";
                    peru.v_Field = "9589";
                    objProxyCommon.UpdateSystemParameter(ref operationResult, peru, Globals.ClientSession.GetAsList()); 
                    foreach (var row in sheet.Rows)
                    {
                        systemparameterDto obj = new systemparameterDto();
                        obj.i_GroupId = 112;
                        obj.i_ParameterId = int.Parse(row.Cells[0].Value.ToString());
                        obj.v_Value1 = row.Cells[1].Value.ToString();
                        obj.v_Value2 = row.Cells[2].GetText();
                        obj.v_Field = row.Cells[3].Value.ToString();
                        obj.i_ParentParameterId = -1;

                        objProxyCommon.AddSystemParameter(ref operationResult, obj, Globals.ClientSession.GetAsList());
                        if (operationResult.Success == 0)
                        {
                            UltraMessageBox.Show(operationResult.ErrorMessage);
                            return;
                        }
                    }
                    trans.Complete();
                    UltraMessageBox.Show("Completado");
                }
            }
        }

        private void btnImportUM_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = @"xlsx files (*.xlsx)|*.xlsx";
            op.Multiselect = false;
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Workbook book = Workbook.Load(op.FileName);
                var sheet = book.Worksheets.FirstOrDefault();

                using (var trans = TransactionUtils.CreateTransactionScope())
                {
                    DatahierarchyBL objProxyCommon = new DatahierarchyBL();
                    var operationResult = new OperationResult();

                    foreach (var row in sheet.Rows)
                    {
                        datahierarchyDto obj = new datahierarchyDto();
                        obj.i_GroupId = 148;
                        obj.i_ItemId = int.Parse(row.Cells[0].Value.ToString());
                        obj.v_Value1 = row.Cells[2].Value.ToString();
                        obj.v_Value2 = row.Cells[1].Value.ToString();
                        objProxyCommon.AddDataHierarchy(ref operationResult, obj, Globals.ClientSession.GetAsList());
                        if (operationResult.Success == 0)
                        {
                            UltraMessageBox.Show(operationResult.ErrorMessage);
                            return;
                        }
                    }
                    trans.Complete();
                    UltraMessageBox.Show("Completado UNIDAD DE MEDIDA");
                }
            }
        }

        private void btnPDT_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog { Filter = @"xlsx files (*.xlsx)|*.xlsx", Multiselect = false };
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Workbook book = Workbook.Load(op.FileName);
                var sheet = book.Worksheets.First();
                try
                {
                    using (var trans = TransactionUtils.CreateTransactionScope())
                    {
                        var objProxyCommon = new SystemParameterBL();
                        var operationResult = new OperationResult();
                        //var act= objProxyCommon.GetSystemParameter(ref operationResult, 0, 152);
                        //if (act != null)
                        //{
                        //    act.v_Value1 = "CONCEPTOS PDT PLANILLA";
                        //    objProxyCommon.UpdateSystemParameter(ref operationResult, act, Globals.ClientSession.GetAsList()); 
                        //}
   
                        //if (objProxyCommon.GetSystemParametersCount(ref operationResult, "i_GroupId=152") > 0)
                        //{
                        //    UltraMessageBox.Show("Grupo ya existe, Imposible Importar Datos");
                        //    return;
                        //}
                        
                        foreach (var row in sheet.Rows)
                        {
                            systemparameterDto obj = new systemparameterDto();
                            obj.i_GroupId = int.Parse(row.Cells[0].Value.ToString());
                            obj.i_ParameterId = int.Parse(row.Cells[1].Value.ToString());
                            obj.v_Value1 = row.Cells[2].GetText(); // descripcion 
                            obj.v_Value2 = row.Cells[3].Value == null ? null : int.Parse(row.Cells[3].Value.ToString()).ToString("0000");   //codigo
                            obj.v_Field = row.Cells[5].GetText(); //
                            obj.v_Value3 = row.Cells[4].GetText();
                            obj.i_ParentParameterId = row.Cells[6].Value == null ? -1 : int.Parse(row.Cells[6].Value.ToString());
                            obj.i_IsDeleted =  int.Parse(row.Cells[8].Value.ToString());
                            obj.i_InsertUserId = 1;
                            obj.d_InsertDate = DateTime.Parse("2016-07-06 16:42:36.109809");
                            objProxyCommon.AddSystemParameter(ref operationResult, obj, Globals.ClientSession.GetAsList());
                            if (operationResult.Success == 0)
                            {
                                UltraMessageBox.Show(operationResult.ErrorMessage);
                                return;
                            }
                        }
                        trans.Complete();
                        UltraMessageBox.Show("Completado");
                    }
                }
                catch (Exception ex)
                {

                    UltraMessageBox.Show("Ocurrió un error :" + ex.InnerException + ex.Message, "Error");
                }
            }
        }

    }
}
