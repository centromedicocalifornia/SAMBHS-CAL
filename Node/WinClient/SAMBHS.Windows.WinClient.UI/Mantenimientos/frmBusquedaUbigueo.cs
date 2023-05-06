using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBusquedaUbigueo : Form
    {
        #region Fields
        private string _ubigueo;
        private readonly SystemParameterBL _objSystemParameterBl = new SystemParameterBL();
        #endregion

        #region Properties

        public string Ubigueo
        {
            get { return _ubigueo; }
        }
        #endregion

        public frmBusquedaUbigueo(string ubigueo)
        {
            _ubigueo = ubigueo;
            Load += frmBusquedaUbigueo_Load;
            InitializeComponent();
        }

        private void frmBusquedaUbigueo_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            //Combo Departamento
            Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);

            if (!string.IsNullOrWhiteSpace(_ubigueo))
            {
                SetUbigueo();
            }
            else
            {
                ddlDepartamento.Value = "-1";
            }
        }

        private void SetUbigueo()
        {
            var combos = new[] {ddlDepartamento, ddlProvincia, ddlDistrito};
            try
            {
                for (byte i = 0; i < combos.Length; i++)
                {
                    var combo = combos[i];
                    if (_ubigueo.Length < (i + 1) * 2) return;

                    var source = combo.DataSource as List<KeyValueDTO>;
                    if (source == null) return;

                    var cod = _ubigueo.Substring(i * 2, 2);
                    if (!cod.All(char.IsNumber)) return;

                    var cd = int.Parse(cod);
                    var sel = source.FirstOrDefault(d => d.Value2 != null && int.Parse(d.Value2) == cd);
                    if (sel == null) return;
                    combo.Value = sel.Id;
                }
            }
            catch 
            {
                // Invalid
            }
        }

        private string GetUbigueo()
        {
            var combos = new[] { ddlDepartamento, ddlProvincia, ddlDistrito };
            var result = "";
            foreach (var combo in combos)
            {
                var obj = combo.SelectedItem.ListObject as KeyValueDTO;
                if (obj == null) return result;

                result += int.Parse(obj.Value2).ToString("00");
            }
            return result;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!uvUbigueo.Validate(true, false).IsValid) 
                return;

            _ubigueo = GetUbigueo();

            if (Modal)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                Close();
            }
        }

        private void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlDepartamento.Value == null) return;

            if ((string)ddlDepartamento.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
            ddlProvincia.Value = "-1";
        }

        private void ddlProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlProvincia.Value == null) return;

            if ((string)ddlProvincia.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlProvincia.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
            ddlDistrito.Value = "-1";
        }

    }
}
