using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using FineUI;

namespace SAMBHS.Server.WebClientAdmin.UI.Common
{
    public partial class FRM004R : System.Web.UI.Page
    {
        ////DbConfigBL _objDbConfigBL = new DbConfigBL();
        //dbconfigDto _dbconfigDto = new dbconfigDto();
        //public string _Ruc
        //{
        //    get
        //    {
        //        if (Request.QueryString["RUC"] != null)
        //        {
        //            string Ruc = Request.QueryString["RUC"].ToString();
        //            if (!string.IsNullOrEmpty(Ruc))
        //            {
        //                return Ruc;
        //            }
        //        }
        //        return "";
        //    }
        //}

        //protected void Page_Load(object sender, EventArgs e)
        //{

        //}
        //protected void btnSaveRefresh_Click(object sender, EventArgs e)
        //{
        //    string _FileName, _FileBAK, _RutaBackup;
        //    _FileName = _Ruc + "_" + DateTime.Now.ToString().Replace(":", string.Empty).Replace("/", string.Empty);
        //    //_RutaBackup = Server.MapPath("~/BD_Backups_Clientes/");
        //    _RutaBackup = @"C:\BD_BACKUPS_CLIENTES\Upload\backups_autogenerados\";
        //    OperationResult objOperationResult = new OperationResult();
        //    List<StoredProcedureResultDto> result = _objDbConfigBL.GeneraBackup(_Ruc, _FileName, _RutaBackup, ref objOperationResult);
        //    if (result[0].Valor_Retorno == 1)
        //    {
        //        _FileBAK = @"C:\BD_BACKUPS_CLIENTES\Upload\" + "_" + DateTime.Now.ToString().Replace(":", string.Empty).Replace("/", string.Empty) + System.IO.Path.GetFileName(FileUpload1.PostedFile.FileName);
        //        FileUpload1.PostedFile.SaveAs(_FileBAK);
        //        List<StoredProcedureResultDto> restore = _objDbConfigBL.RestauraBD(_Ruc, _FileBAK, ref objOperationResult);
        //        if (restore[0].Valor_Retorno == 1)
        //        {
        //            Alert alert = new Alert();
        //            alert.Title = "Mensaje";
        //            alert.Message = "Base de datos Restaurada Correctamente！";
        //            alert.Icon = Icon.Accept;
        //            alert.Target = Target.Top;
        //            alert.Show();
        //        }
        //    }
        //    else
        //    {
        //        Alert alert = new Alert();
        //        alert.Title = "Mensaje";
        //        alert.Message = "Ocurrió un error al crear la copia de seguridad!!" + "\nNúmero de Error: " + result[0].ErrorNumber + "\nError: " + result[0].ErrorMessage;
        //        alert.Icon = Icon.Exclamation;
        //        alert.Target = Target.Top;
        //        alert.Show();
        //    }
        //}
    }
}