using System;
using System.Collections.Generic;
using System.Linq;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.DataModel;
using System.ComponentModel;

namespace SAMBHS.Common.Resource
{
    public class RucEditor : UltraCombo
    {
        #region Properties
        private String PrevValue { get; set; }
        private String Flag { set; get; }
        private String TipoBl { get; set; }

        /// <summary>
        /// Longitu de la Cadena de Texto.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TextLength
        {
            get { return Text.Length; }
        }
        
        #endregion

        #region Events
        public event EventHandler ItemSelectedAfterDropClosed;
        private void OnSelectedItemAfterClose()
        {
            if (ItemSelectedAfterDropClosed != null)
                ItemSelectedAfterDropClosed(this, new EventArgs());
        }
        #endregion

        public RucEditor()
        {
            DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
        }
        /// <summary>
        /// Carga la configuracion inicial del Ultracombo
        /// </summary>
        /// <param name="flag">cadena de filtro</param>
        /// <param name="bl">"CL" = Cliente, "TR" = Transportista</param>
        public void LoadConfig(string flag, string bl = "CL")
        {
            Flag = flag;
            TipoBl = bl;
            var appearance1 = new Infragistics.Win.Appearance();
            var ultraGridBand1 = new UltraGridBand("", -1);
            var ultraGridColumn1 = new UltraGridColumn("Id", 0);
            var ultraGridColumn2 = new UltraGridColumn("Value1", 1);
            var ultraGridColumn3 = new UltraGridColumn("Value2", 2);

            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            DisplayLayout.Appearance = appearance1;
            ultraGridColumn1.Header.Caption = @"ID";
            ultraGridColumn1.Header.VisiblePosition = 0;
            //ultraGridColumn2.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(100, 0);
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.Caption = @"Doc.";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(100, 0);

            ultraGridColumn3.Header.Caption = @"Nombre/Razon Social";
            ultraGridColumn3.Header.VisiblePosition = 2;
            //ultraGridColumn3.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(320,0);
            //ultraGridColumn3.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3});
 
            DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            DropDownWidth = 400;
            DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            DisplayMember = "Value1";
            ValueMember = "Id";
            TextChanged += ComboBoxSuggest_TextChanged;
            BeforeDropDown += ComboBoxSuggest_BeforeDropDown;
            AfterCloseUp += ComboBoxSuggest_AfterCloseUp;
            RowSelected += ComboBoxSuggest_RowSelected;
            KeyPress += ComboBoxSuggest_KeyPress;
        }

        #region Events Owner
        void ComboBoxSuggest_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEntero(null, e);
        }
        void ComboBoxSuggest_RowSelected(object sender, RowSelectedEventArgs e)
        {
            if (e.Row != null)
            {
                PrevValue = e.Row.Cells["Value1"].Text;
            }
        }


        void ComboBoxSuggest_AfterCloseUp(object sender, EventArgs e)
        {
            if (Text.Trim().Equals(PrevValue))
                OnSelectedItemAfterClose();
        }

        void ComboBoxSuggest_BeforeDropDown(object sender, CancelEventArgs e)
        {
            if (Text.Length < 5) e.Cancel = true;
        }

        void ComboBoxSuggest_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length == 5)
            {
                var objReuslt = (TipoBl.Equals("TR")) ? GetDataTrans(Text) : GetDataCl(Text);

                 if (objReuslt != null && objReuslt.Any())
                 {
                     Utils.Windows.LoadUltraComboList(this, "Value1", "Id", objReuslt);
                     DisplayLayout.Bands[0].Columns["Value1"].PerformAutoResize(PerformAutoSizeType.AllRowsInBand);

                    if (Rows.Any())
                    {
                        PerformAction(UltraComboAction.Dropdown);
                        DeselectText();
                    }
                 }
            }
            if (Text.Length < 5)
            {
                //this.DataSource = null;
                if (IsDroppedDown)
                {
                    PerformAction(UltraComboAction.CloseDropdown);
                    DeselectText();
                }
            }
        }
        #endregion

        private List<GridKeyValueDTO> GetDataCl(string pstrNroDocumento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                OperationResult objOperationResult = new OperationResult();
                var objData = dbContext.cliente.Where(p => p.v_NroDocIdentificacion.StartsWith(pstrNroDocumento) && p.v_FlagPantalla.Equals(Flag) && p.i_Eliminado == 0).ToList().
                    Select(x => new GridKeyValueDTO
                    {
                        Id = x.v_IdCliente,
                        Value2 = (x.v_ApePaterno + " " + x.v_ApeMaterno + " " + x.v_PrimerNombre + " " + x.v_RazonSocial).Trim(),
                        Value1 = x.v_NroDocIdentificacion,
                    }).ToList();

                objOperationResult.Success = 1;
                return objData;
            }
        }
        private List<GridKeyValueDTO> GetDataTrans(string pstrNroDocumento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var objOperationResult = new OperationResult();
                var objData = dbContext.transportista.Where(p => p.v_NumeroDocumento.Contains(pstrNroDocumento) && p.i_Eliminado == 0).ToList().
                    Select(x => new GridKeyValueDTO
                    {
                        Id = x.v_IdTransportista,
                        Value2 = x.v_NombreRazonSocial,
                        Value1 = x.v_NumeroDocumento,
                    }).ToList();

                objOperationResult.Success = 1;
                return objData;
            }
        }

        /// <summary>
        /// Deselecciona el Texto y coloca el cursor al final.
        /// </summary>
        public void DeselectText()
        {
            SelectionStart = Text.Length;
        }
        /// <summary>
        /// Limpia el Texto del UltraCombo
        /// </summary>
        public void Clear()
        {
            Text = string.Empty;
        }
    }
}
