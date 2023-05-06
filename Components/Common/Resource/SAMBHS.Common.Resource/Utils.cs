using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using System.IO;
using System.Configuration;
using System.Net;
using System.Drawing;
using Infragistics.Win.UltraWinGrid;
using System.Data;
using SAMBHS.Common.DataModel;
using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;
using System.Transactions;
using System.Net.NetworkInformation;
using System.Deployment.Application;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq.Dynamic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Dapper;
using IsolationLevel = System.Transactions.IsolationLevel;


namespace SAMBHS.Common.Resource
{
    public class Utils
    {
        public int Max;
        public class Windows
        {
            
            public static void RenombrarCodigosConceptos()
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var conceptos = dbContext.planillaconceptos.Where(p => p.i_Eliminado == 0).ToList();
                        if (conceptos.Any())
                        {
                            var agrupado = conceptos.GroupBy(g => g.i_IdTipoConcepto);
                            foreach (var grupo in agrupado)
                            {
                                var counter = 1;
                                var idTipo = grupo.Key ?? 1;
                                var sufijo = idTipo == 1 ? "I" : idTipo == 2 ? "D" : "A";
                                foreach (var concepto in grupo.OrderBy(o => o.v_Codigo))
                                {
                                    concepto.v_Codigo = string.Format("{0}{1:000}", sufijo, counter);
                                    dbContext.planillaconceptos.ApplyCurrentValues(concepto);
                                    counter++;
                                }
                            }

                            dbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            private static string Periodo
            {
                get { return Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString(); }
            }

            #region Metodos utilitarios para WinForms
            public static void GeneraHistorial(LogEventType TipoEvento, string NombreUsuario, string NombreTabla = null, string IdRegistro = null)
            {
                try
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        string Evento = string.Empty;

                        if (NombreTabla != null && !NombreTabla.Contains("detalle"))
                        {
                            switch (TipoEvento)
                            {
                                case LogEventType.ACCESOSSISTEMA:
                                    string VersionSistema = "Debug";
                                    try
                                    {
                                        ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                                        VersionSistema = string.Format("{0}.{1}.{2}.{3}", ad.CurrentVersion.Major, ad.CurrentVersion.Minor, ad.CurrentVersion.Build, ad.CurrentVersion.Revision);
                                    }
                                    catch { }

                                    Evento = string.Format("'{0}' ingresó al sistema con la versión {1}", NombreUsuario, VersionSistema);
                                    break;

                                case LogEventType.CREACION:
                                    Evento = string.Format("'{0}' creó el registro '{1}' en '{2}'.", NombreUsuario, IdRegistro, NombreTabla);
                                    break;

                                case LogEventType.ACTUALIZACION:
                                    Evento = string.Format("'{0}' actualizó el registro '{1}' en '{2}'.", NombreUsuario, IdRegistro, NombreTabla);
                                    break;

                                case LogEventType.ELIMINACION:
                                    Evento = string.Format("'{0}' eliminó el registro '{1}' en '{2}'.", NombreUsuario, IdRegistro, NombreTabla);
                                    break;

                                case LogEventType.EXPORTACION:
                                    Evento = string.Format("'{0}' exportó información de '{1}'.", NombreUsuario, NombreTabla);
                                    break;

                                case LogEventType.REPROCESO:
                                    Evento = string.Format("'{0}' ejecutó un proceso de regeneración en la tabla '{1}'.", NombreUsuario, NombreTabla);
                                    break;

                                case LogEventType.CLONACION:
                                    Evento = string.Format("'{0}' clonó el registro '{1}' en '{2}'.", NombreUsuario, IdRegistro, NombreTabla);
                                    break;
                            }

                            if (!string.IsNullOrEmpty(Evento))
                            {
                                loghistorial _loghistorial = new loghistorial();
                                _loghistorial.t_FechaHora = DateTime.Now;
                                _loghistorial.v_Accion = Evento;
                                _loghistorial.v_NombrePC = System.Environment.MachineName;
                                dbContext.loghistorial.AddObject(_loghistorial);
                                dbContext.SaveChanges();
                            }
                        }

                    }
                }
                catch (Exception)
                {

                    //throw;
                }
            }

            public static Color GlobalColorForms(string Nombre)
            {
                Color _GlobalColorForms;
                _GlobalColorForms = Color.FromName(Nombre);
                return _GlobalColorForms;
            }

            public static void LoadDropDownList(ComboBox prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, List<KeyValueDTO> prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {

                prmDropDownList.DataSource = null;
                prmDropDownList.Items.Clear();
                KeyValueDTO firstItem = null;
                var priorFirstItem = prmDataSource.FirstOrDefault(p => p.Id == "-1");

                if (prmDropDownListAction != null)
                {
                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            firstItem = new KeyValueDTO() { Id = Constants.AllValue, Value1 = Constants.All };
                            break;
                        case DropDownListAction.Select:
                            firstItem = new KeyValueDTO() { Id = Constants.SelectValue, Value1 = Constants.Select };
                            break;
                    }
                }

                if (priorFirstItem == null && firstItem != null)
                {
                    prmDataSource.Insert(0, firstItem);
                }

                if (prmDataSource != null)
                {
                    if (prmDataSource.Count != 0)
                    {
                        prmDropDownList.DisplayMember = prmDataTextField;
                        prmDropDownList.ValueMember = prmDataValueField;
                        prmDropDownList.DataSource = prmDataSource;
                    }
                }

            }

            public static void LoadUltraComboEditorList(UltraComboEditor prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, List<KeyValueDTO> prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {

                prmDropDownList.DataSource = null;
                prmDropDownList.Items.Clear();
                KeyValueDTO firstItem = null;
                var priorFirstItem = prmDataSource.FirstOrDefault(p => p.Id == "-1");

                if (prmDropDownListAction != null)
                {
                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            firstItem = new KeyValueDTO() { Id = Constants.AllValue, Value1 = Constants.All };
                            break;
                        case DropDownListAction.Select:
                            firstItem = new KeyValueDTO() { Id = Constants.SelectValue, Value1 = Constants.Select };
                            break;
                    }
                }

                if (priorFirstItem == null && firstItem != null)
                {
                    prmDataSource.Insert(0, firstItem);
                }

                if (prmDataSource != null)
                {
                    if (prmDataSource.Count != 0)
                    {
                        prmDropDownList.DisplayMember = prmDataTextField;
                        prmDropDownList.ValueMember = prmDataValueField;
                        prmDropDownList.DataSource = prmDataSource;
                    }
                }

            }
            public static void LoadUltraComboEditorList2(UltraComboEditor prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, List<KeyValueDTO> prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {

                prmDropDownList.DataSource = null;
                prmDropDownList.Items.Clear();

                if (prmDropDownListAction != null)
                {
                    KeyValueDTO firstItem = null;
                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            firstItem = new KeyValueDTO() { Id = Constants.AllValue, Value1 = Constants.All };
                            break;
                        case DropDownListAction.Select:
                            firstItem = new KeyValueDTO() { Id = Constants.SelectValue, Value1 = Constants.Select };
                            break;
                    }
                    if (firstItem != null)
                    {
                        if (prmDataSource != null)
                        {
                            if (prmDataSource.FirstOrDefault(p => p.Id == "-1") == null)
                                prmDataSource.Insert(0, firstItem);
                        }
                        else
                            prmDataSource = new List<KeyValueDTO>(1)
                            {
                                firstItem
                            };
                    }
                }
                prmDropDownList.DisplayMember = prmDataTextField;
                prmDropDownList.ValueMember = prmDataValueField;
                if (prmDataSource != null)
                {
                    prmDropDownList.DataSource = prmDataSource;
                }

            }
            public static void LoadUltraComboList(UltraComboEditor prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, List<GridKeyValueDTO> prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {
                GridKeyValueDTO firstItem = null;
                var priorFirstItem = prmDataSource.FirstOrDefault(p => p.Id == "-1");

                if (prmDropDownListAction != null)
                {
                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            if (prmDataTextField == "Value1")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.AllValue, Value1 = Constants.All };
                            }
                            else if (prmDataTextField == "Value2")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.AllValue, Value2 = Constants.All };
                            }
                            break;
                        case DropDownListAction.Select:
                            if (prmDataTextField == "Value1")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.SelectValue, Value1 = Constants.Select };
                            }
                            else if (prmDataTextField == "Value2")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.SelectValue, Value2 = Constants.Select };
                            }

                            break;
                    }
                }
                if (priorFirstItem == null && firstItem != null)
                {
                    prmDataSource.Insert(0, firstItem);
                }

                if (prmDataSource != null)
                {
                    if (prmDataSource.Count != 0)
                    {
                        prmDropDownList.DataSource = prmDataSource;
                        prmDropDownList.DisplayMember = prmDataTextField;
                        prmDropDownList.ValueMember = prmDataValueField;
                    }
                }
            }

            public static void LoadUltraComboList(UltraCombo prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, List<GridKeyValueDTO> prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {
                GridKeyValueDTO firstItem = null;
                var priorFirstItem = prmDataSource.FirstOrDefault(p => p.Id == "-1");

                if (prmDropDownListAction != null)
                {
                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            if (prmDataTextField == "Value1")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.AllValue, Value1 = Constants.All };
                            }
                            else if (prmDataTextField == "Value2")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.AllValue, Value2 = Constants.All };
                            }
                            break;
                        case DropDownListAction.Select:
                            if (prmDataTextField == "Value1")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.SelectValue, Value1 = Constants.Select };
                            }
                            else if (prmDataTextField == "Value2")
                            {
                                firstItem = new GridKeyValueDTO() { Id = Constants.SelectValue, Value2 = Constants.Select };
                            }

                            break;
                    }
                }
                if (priorFirstItem == null && firstItem != null)
                {
                    prmDataSource.Insert(0, firstItem);
                }

                if (prmDataSource != null)
                {
                    if (prmDataSource.Count != 0)
                    {
                        prmDropDownList.DataSource = prmDataSource;
                        prmDropDownList.DisplayMember = prmDataTextField;
                        prmDropDownList.ValueMember = prmDataValueField;
                    }
                }
            }
            public static void LoadUltraComboList(UltraCombo prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, List<KeyValueDTO> prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {

                //prmDropDownList.DataSource = null;
                KeyValueDTO firstItem = null;
                var priorFirstItem = prmDataSource.FirstOrDefault(p => p.Id == "-1");

                if (prmDropDownListAction != null)
                {
                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            if (prmDataTextField == "Value1")
                            {
                                firstItem = new KeyValueDTO() { Id = Constants.AllValue, Value1 = Constants.All };
                            }
                            else if (prmDataTextField == "Value2")
                            {
                                firstItem = new KeyValueDTO() { Id = Constants.AllValue, Value2 = Constants.All };
                            }
                            break;
                        case DropDownListAction.Select:
                            if (prmDataTextField == "Value1")
                            {
                                firstItem = new KeyValueDTO() { Id = Constants.SelectValue, Value1 = Constants.Select };
                            }
                            else if (prmDataTextField == "Value2")
                            {
                                firstItem = new KeyValueDTO() { Id = Constants.SelectValue, Value2 = Constants.Select };
                            }

                            break;
                    }
                }
                if (priorFirstItem == null && firstItem != null)
                {
                    prmDataSource.Insert(0, firstItem);
                }

                if (prmDataSource != null)
                {
                    if (prmDataSource.Count != 0)
                    {
                        prmDropDownList.DataSource = prmDataSource;
                        prmDropDownList.DisplayMember = prmDataTextField;
                        prmDropDownList.ValueMember = prmDataValueField;
                    }
                }
            }
            /// <summary>
            /// OBSOLETO
            /// Le da los limites al datetimePicker para que no se pueda escoger fechas de otros periodos.
            /// </summary>
            /// <param name="dtpFecha"></param>
            public static void SetLimitesPeriodo(DateTimePicker dtpFecha)
            {
                var periodo = dtpFecha.Value.Year;
                var diasDiciembre = DateTime.DaysInMonth(periodo, 12);
                dtpFecha.MinDate = DateTime.Parse("01/01/" + periodo);
                dtpFecha.MaxDate = DateTime.Parse(diasDiciembre + "/12/" + periodo);
            }

            public static string GetApplicationExecutingFolder()
            {
                string strExecutingFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string strFolder = System.IO.Path.GetDirectoryName(strExecutingFile);
                return strFolder;
            }

            public static List<string> GetFolderFiles(string pstrFolder, string pstrFileExtensions)
            {
                List<string> files = Directory.GetFiles(pstrFolder, "*.*", SearchOption.TopDirectoryOnly).Where(s => pstrFileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();
                return files;
            }

            public static System.Drawing.Image BinaryToImage(Binary binaryData)
            {
                if (binaryData == null) return null;

                byte[] buffer = binaryData.ToArray();
                MemoryStream memStream = new MemoryStream();
                memStream.Write(buffer, 0, buffer.Length);
                return System.Drawing.Image.FromStream(memStream);
            }

            public static bool IsActionEnabled(string pstrActionCode, List<KeyValueDTO> FormActions)
            {
                List<KeyValueDTO> objFormAction = FormActions;

                if (objFormAction != null)
                {
                    bool isExists = objFormAction.Exists(p => p.Value2 != null && p.Value2.Equals(pstrActionCode.Trim()));

                    if (isExists)
                        return true;
                }

                return false;
            }

            #region Config Files
            public static string GetConnectionString(string nombreCadena)
            {
                return ConfigurationManager.ConnectionStrings[nombreCadena].ConnectionString;
            }

            public static string GetApplicationConfigValue(string nombre)
            {
                return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
            }
            #endregion

            public static void NumeroEntero(System.Windows.Forms.TextBox Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else
                    e.Handled = true;
            }

            public static void NumeroEnteroMaxDecimales(System.Windows.Forms.TextBox Text, KeyPressEventArgs e)
            {

                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar >= 48 && e.KeyChar <= 54)
                    e.Handled = false;
                else
                    e.Handled = true;

            }

            public static void NumeroEnteroMaxDecimalesUltraTextBox(UltraTextEditor Text, KeyPressEventArgs e)
            {

            }

            public static void NumeroEnteroUltraTextBox(UltraTextEditor Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else
                    e.Handled = true;
            }

            public static void CaracteresValidos(UltraTextEditor Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 124)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }


            public static bool NumeroEntero(char CharCadena)
            {

                if (CharCadena == 8)
                {

                    return true;
                }
                if (CharCadena >= 48 && CharCadena <= 57)
                    return true;
                else
                    return false;

            }

            public static void NumeroEnteroConComaUltraTextBox(UltraTextEditor Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8 || e.KeyChar == 44)
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else
                    e.Handled = true;
            }

            public static void NumeroDecimal(System.Windows.Forms.TextBox Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 4)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }

            public static void NumeroDecimalUltraTextBox(UltraTextEditor Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 4)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }


            public static void NumeroDecimalNegativoUltraTextBox(UltraTextEditor Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 4)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if ((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 45)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }


            public static void NumeroDecimal2(System.Windows.Forms.TextBox Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 2)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }

            public static void NumeroDecimal2UltraTextBox(UltraTextEditor Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 2)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }
            public static void MesesValidos(string Text, KeyPressEventArgs e)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    e.Handled = false;
                    return;
                }

                if ((int.Parse(Text.ToString()) >= 1 && int.Parse(Text.ToString()) <= 12) || e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }
                else
                {
                    e.Handled = true;
                    return;
                }


            }
            public static void NumeroDecimalCelda(UltraGridCell Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;
                var gg = Text.Text.Length;
                var yy = Text.Text;
                for (int i = 0; i < Text.Text.Length; i++)
                {
                    var ttt = Text.Text[i];
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 7)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }


            public static void NumeroDecimalNegativoCelda(UltraGridCell Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '.')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 7)
                    {
                        e.Handled = true;
                        return;
                    }


                }

                if ((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 45)
                    e.Handled = false;
                else if (e.KeyChar == 46)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }


            public static void CaracteresValidosCelda(UltraGridCell Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 124)
                {
                    e.Handled = true;
                    return;
                }

            }
            public static void NumeroEnteroCelda(UltraGridCell Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else
                    e.Handled = true;
            }

            public static void NumeroDocumentoCelda(UltraGridCell Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }


                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {
                    if (Text.Text[i] == '-')
                        IsDec = true;

                    if (IsDec && nroDec++ >= 8)
                    {
                        e.Handled = true;
                        return;
                    }
                }

                if (e.KeyChar >= 48 && e.KeyChar <= 57)
                    e.Handled = false;
                else if (e.KeyChar == 45)
                    e.Handled = (IsDec) ? true : false;
                else
                    e.Handled = true;
            }

            public static void NumeroSerieDocumento(UltraGridCell Text, KeyPressEventArgs e)
            {
                if (e.KeyChar == 8)
                {
                    e.Handled = false;
                    return;
                }
                bool IsDec = false;
                int nroDec = 0;

                for (int i = 0; i < Text.Text.Length; i++)
                {

                    if (Text.Text.Length < 5)
                    {
                        if (i == 4)
                        {
                            if (Text.Text[i] == '-')
                            {
                                IsDec = true;
                                e.Handled = false;
                            }

                            if (IsDec && nroDec++ >= 8)
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                        else
                        {
                            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                                e.Handled = false;
                            else if (e.KeyChar == 45)
                                e.Handled = (IsDec) ? true : false;
                            else
                                e.Handled = true;
                        }
                    }
                    else
                    {

                        e.Handled = true;
                        return;

                    }
                }


            }

            public static void FijarFormato(System.Windows.Forms.TextBox _TextBox, string Mascara)
            {
                int numero;
                if (!string.IsNullOrEmpty(_TextBox.Text))
                {
                    if (IsNumeric(_TextBox.Text) == true)
                    {
                        numero = Convert.ToInt32(_TextBox.Text);
                        _TextBox.Text = string.Format(Mascara, numero);
                    }
                    else
                    {
                        _TextBox.Text = string.Empty;
                    }
                }
            }

            public static void FijarFormatoUltraText(UltraTextEditor _TextBox, string Mascara)
            {
                int numero;
                if (!string.IsNullOrEmpty(_TextBox.Text))
                {
                    if (IsNumeric(_TextBox.Text) == true)
                    {
                        numero = Convert.ToInt32(_TextBox.Text);
                        _TextBox.Text = string.Format(Mascara, numero);
                    }
                    else
                    {
                        _TextBox.Text = string.Empty;
                    }
                }
            }

            public static void FijarFormatoUltraTextBox(Infragistics.Win.UltraWinEditors.UltraTextEditor _TextBox, string Mascara)
            {
                int numero;
                if (!string.IsNullOrEmpty(_TextBox.Text))
                {
                    if (IsNumeric(_TextBox.Text) == true)
                    {
                        numero = Convert.ToInt32(_TextBox.Text);
                        _TextBox.Text = string.Format(Mascara, numero);
                    }
                    else
                    {
                        _TextBox.Text = string.Empty;
                    }
                }
            }

            public static void FormatoDecimalesCajasTexto(int CantidadDecimales, System.Windows.Forms.TextBox _TextBox)
            {

                decimal valor = _TextBox.Text == string.Empty ? 0 : decimal.Parse(_TextBox.Text.ToString());

                string FormatoDecimales;

                if (CantidadDecimales > 0)
                {
                    var sharp = "0";
                    FormatoDecimales = "0.";
                    for (var i = 0; i < CantidadDecimales; i++)
                    {
                        FormatoDecimales = FormatoDecimales + sharp;
                    }
                }
                else
                {
                    FormatoDecimales = "0";

                }


                if (!string.IsNullOrEmpty(_TextBox.Text))
                {
                    if (Utils.Windows.IsNumeric(_TextBox.Text) == true)
                    {

                        _TextBox.Text = valor.ToString(FormatoDecimales);
                    }
                    else
                    {
                        _TextBox.Text = string.Empty;
                    }
                }
            }

            private static bool IsNumeric(object Expression)
            {
                bool isNum;
                double retNum;
                isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                return isNum;
            }

            public static bool ValidarRuc(string nroDocumento)
            {
                nroDocumento = nroDocumento.Trim();

                if (string.IsNullOrEmpty(nroDocumento) || nroDocumento.Length != 11) return false;
                int[] factores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                var productos = new int[10];
                var longitudDocumento = nroDocumento.Length;
                var nroIdentificador = int.Parse(nroDocumento.Substring(longitudDocumento - 1, 1));

                for (var i = 0; i < 10; i++)
                {
                    var valor = int.Parse(nroDocumento.Substring(i, 1));
                    productos[i] = valor * factores[i];
                }

                var sumaProductos = productos.Sum();
                var resultado = 11 - sumaProductos % 11;

                switch (resultado)
                {
                    case 10:
                        resultado = 0;
                        break;

                    case 11:
                        resultado = 1;
                        break;
                }

                if (resultado > 11)
                {
                    var result = resultado.ToString();
                    resultado = int.Parse(result.Substring(result.Length - 1, 1));
                }

                return resultado == nroIdentificador;
            }

            public static bool EsCuentaImputable(string pNroCuenta)
            {
                try
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        pNroCuenta = pNroCuenta.Trim();

                        var Cuenta = (from c in dbContext.asientocontable
                                      where c.i_Eliminado == 0 && c.i_Imputable == 1 && c.v_NroCuenta == pNroCuenta && c.v_Periodo == Periodo
                                      select c).FirstOrDefault();

                        if (Cuenta == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            public static int CalcularEdad(DateTime birthDay)
            {
                int years = DateTime.Now.Year - birthDay.Year;

                if ((birthDay.Month > DateTime.Now.Month) || (birthDay.Month == DateTime.Now.Month && birthDay.Day > DateTime.Now.Day))
                    years--;

                return years;
            }

            public static string DevuelveSerieFormateada4Digitos(ref OperationResult objOperationResult, string serie)
            {
                try
                {
                    objOperationResult.Success = 1;
                    string SerieNueva = serie;

                    if (!string.IsNullOrEmpty(serie))
                    {
                        int Leng = serie.Trim().Length, i = 2;
                        string CadenaCeros = "0";
                        if (Leng < 4)
                        {
                            while (i <= (4 - Leng))
                            {
                                CadenaCeros = CadenaCeros + "0";
                                i = i + 1;
                            }
                            SerieNueva = CadenaCeros + serie.Trim();
                        }
                    }
                    return SerieNueva;
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    return serie;
                }

            }

            /// <summary>
            /// Limita el maxlenght de las series segun el tipo de documento.
            /// Los valores son establecidos por la sunat según Anexo 2.
            /// </summary>
            /// <param name="cboTipoDocumento"></param>
            /// <param name="textSerie"></param>
            public static void LimitarSeriePorTipoDocumento(UltraCombo cboTipoDocumento, UltraTextEditor textSerie)
            {
                try
                {
                    if (cboTipoDocumento.Value != null)
                    {
                        #region Limite documentos

                        var documentoLimitesSunat = new List<Tuple<int, int>>
                        {
                            new Tuple<int, int>(1, 4),
                            new Tuple<int, int>(2, 4),
                            new Tuple<int, int>(3, 4),
                            new Tuple<int, int>(4, 4),
                            new Tuple<int, int>(5, 1),
                            new Tuple<int, int>(6, 4),
                            new Tuple<int, int>(7, 4),
                            new Tuple<int, int>(8, 4),
                            new Tuple<int, int>(10, 4),
                            new Tuple<int, int>(11, 20),
                            new Tuple<int, int>(12, 20),
                            new Tuple<int, int>(13, 20),
                            new Tuple<int, int>(14, 20),
                            new Tuple<int, int>(15, 20),
                            new Tuple<int, int>(16, 20),
                            new Tuple<int, int>(17, 20),
                            new Tuple<int, int>(18, 20),
                            new Tuple<int, int>(19, 20),
                            new Tuple<int, int>(21, 20),
                            new Tuple<int, int>(22, 4),
                            new Tuple<int, int>(23, 4),
                            new Tuple<int, int>(24, 20),
                            new Tuple<int, int>(25, 4),
                            new Tuple<int, int>(26, 20),
                            new Tuple<int, int>(27, 20),
                            new Tuple<int, int>(28, 20),
                            new Tuple<int, int>(29, 20),
                            new Tuple<int, int>(30, 20),
                            new Tuple<int, int>(32, 20),
                            new Tuple<int, int>(34, 4),
                            new Tuple<int, int>(35, 4),
                            new Tuple<int, int>(36, 4),
                            new Tuple<int, int>(37, 20),
                            new Tuple<int, int>(42, 20),
                            new Tuple<int, int>(43, 20),
                            new Tuple<int, int>(44, 20),
                            new Tuple<int, int>(45, 20),
                            new Tuple<int, int>(46, 4),
                            new Tuple<int, int>(48, 4),
                            new Tuple<int, int>(49, 20),
                            new Tuple<int, int>(50, 3),
                            new Tuple<int, int>(51, 3),
                            new Tuple<int, int>(52, 3),
                            new Tuple<int, int>(53, 3),
                            new Tuple<int, int>(54, 3),
                            new Tuple<int, int>(55, 1),
                            new Tuple<int, int>(56, 4),
                            new Tuple<int, int>(87, 20),
                            new Tuple<int, int>(88, 20),
                            new Tuple<int, int>(89, 4),
                            new Tuple<int, int>(91, 20),
                            new Tuple<int, int>(96, 20),
                            new Tuple<int, int>(97, 20),
                            new Tuple<int, int>(98, 20)
                        };

                        #endregion

                        var dicdocumentoLimitesSunat = documentoLimitesSunat.ToDictionary(k => k.Item1, o => o.Item2);

                        var id = int.Parse(cboTipoDocumento.Value.ToString());
                        int max;

                        if (dicdocumentoLimitesSunat.TryGetValue(id, out max))
                        {
                            textSerie.MaxLength = max;

                            if (textSerie.TextLength > max)
                                textSerie.Text = textSerie.Text.Substring(0, max);

                            return;
                        }
                    }
                    textSerie.MaxLength = 4;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public static string DevuelveCorrelativoFormateada8Digitos(ref OperationResult objOperationResult, string Correlativo)
            {
                try
                {
                    objOperationResult.Success = 1;
                    string CorrelativoNuevo = Correlativo;

                    if (!string.IsNullOrEmpty(Correlativo))
                    {
                        int Leng = Correlativo.Trim().Length, i = 2;
                        string CadenaCeros = "0";
                        if (Leng < 8)
                        {
                            while (i <= (8 - Leng))
                            {
                                CadenaCeros = CadenaCeros + "0";
                                i = i + 1;
                            }
                            CorrelativoNuevo = CadenaCeros + Correlativo.Trim();
                        }
                    }
                    return CorrelativoNuevo;
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    return Correlativo;
                }

            }
            /// <summary>
            /// Devuelve la entidad de la cuenta por el nro de cuenta, si no existe devuelve nulo.
            /// </summary>
            /// <param name="pNroCuenta"></param>
            /// <returns></returns>
            public static asientocontableDto DevuelveCuentaDatos(string pNroCuenta)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (string.IsNullOrEmpty(pNroCuenta)) return null;
                        var periodo = Globals.ClientSession.i_Periodo.ToString();
                        var asiento = (dbContext.asientocontable.Where(
                            n => n.v_NroCuenta == pNroCuenta && n.i_Eliminado == 0 && n.v_Periodo == periodo))
                            .FirstOrDefault();
                        return asiento != null ? asiento.ToDTO() : null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            /// <summary>
            /// Devuelve un texto que se puede usar como tooltip para las celdas que almacenan cuentas contables.
            /// </summary>
            /// <param name="pNroCuenta"></param>
            /// <returns></returns>
            public static string DevuelveToolTipCuentaDatos(string pNroCuenta)
            {
                try
                {
                    List<destino> destinos;
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        destinos = dbContext.destino.Where(p => p.v_CuentaOrigen.Equals(pNroCuenta) && p.i_Eliminado == 0 && p.v_Periodo.Equals(Periodo)).ToList();
                    }
                    var objCta = DevuelveCuentaDatos(pNroCuenta);
                    if (objCta == null) return "Cuenta no existe";
                    var sb = new StringBuilder();
                    sb.AppendLine("Nombre: " + objCta.v_NombreCuenta.Trim());
                    sb.AppendLine("Moneda: " + (objCta.i_IdMoneda == 1 ? "SOLES" : "DÓLARES"));
                    sb.AppendLine("Naturaleza: " + (objCta.i_Naturaleza == 1 ? "DEBE" : "HABER"));
                    sb.AppendLine("Req. Detalle: " + (objCta.i_Detalle == 1 ? "SI" : "NO"));
                    sb.AppendLine("Tipo Existencia: " + objCta.v_TipoExistencia);
                    if (destinos.Any())
                    {
                        sb.AppendLine("---------------------------");
                        sb.AppendLine("DESTINOS:");
                        foreach (var destino in destinos)
                        {
                            if (destino.i_Porcentaje != null)
                                sb.AppendLine(destino.v_CuentaDestino + " >>> " + destino.v_CuentaTransferencia + " - " + destino.i_Porcentaje.Value.ToString("00") + "%");
                        }
                    }

                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            public static string RetornaCorrelativoPorFecha(ref OperationResult pobjOperationResult, ListaProcesos Proceso, Object pFechaActual, DateTime pFechaNueva, string pstrActualCorrelativo, int pintIdTipoDocumento)
            {
                try
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        string periodoActual = "", mesActual = "";

                        var periodoNuevo = pFechaNueva.Year.ToString().Trim();
                        var mesNuevo = pFechaNueva.Month.ToString("00").Trim();
                        var almacenPredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                        string replicationID = Globals.ClientSession.ReplicationNodeID;

                        if (pFechaActual != null)
                        {
                            periodoActual = DateTime.Parse(pFechaActual.ToString()).Year.ToString().Trim();
                            mesActual = DateTime.Parse(pFechaActual.ToString()).Month.ToString("00").Trim();
                        }

                        if ((periodoActual != periodoNuevo || mesActual != mesNuevo) || Proceso == ListaProcesos.Cobranza)
                        {
                            List<KeyValueDTO> Listado = new List<KeyValueDTO>();

                            #region Enlista los correlativos según tabla
                            switch (Proceso)
                            {
                                case ListaProcesos.Venta:
                                    Listado = (from n in dbContext.venta
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdVenta.Substring(2, 2) == almacenPredeterminado && n.v_IdVenta.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdVenta }).ToList();
                                    break;

                                case ListaProcesos.NotaIngreso:
                                    Listado = (from n in dbContext.movimiento
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdMovimiento.Substring(2, 2) == almacenPredeterminado && n.v_IdMovimiento.Substring(0, 1) == replicationID
                                               && n.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdMovimiento }).ToList();
                                    break;

                                case ListaProcesos.Compra:
                                    Listado = (from n in dbContext.compra
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdCompra.Substring(2, 2) == almacenPredeterminado && n.v_IdCompra.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdCompra }).ToList();
                                    break;

                                case ListaProcesos.GuiaRemisionCompra:
                                    Listado = (from n in dbContext.guiaremisioncompra
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdGuiaCompra.Substring(2, 2) == almacenPredeterminado && n.v_IdGuiaCompra.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdGuiaCompra }).ToList();
                                    break;

                                case ListaProcesos.Tesoreria:
                                    Listado = (from n in dbContext.tesoreria
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.i_IdTipoDocumento == pintIdTipoDocumento && n.v_IdTesoreria.Substring(2, 2) == almacenPredeterminado && n.v_IdTesoreria.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdTesoreria }).ToList();
                                    break;

                                case ListaProcesos.Diario:
                                    Listado = (from n in dbContext.diario
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.i_IdTipoDocumento == pintIdTipoDocumento && n.v_IdDiario.Substring(2, 2) == almacenPredeterminado && n.v_IdDiario.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdDiario }).ToList();
                                    break;

                                case ListaProcesos.Adelanto:
                                    Listado = (from n in dbContext.adelanto
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdAdelanto.Substring(2, 2) == almacenPredeterminado && n.v_IdAdelanto.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdAdelanto }).ToList();
                                    break;

                                case ListaProcesos.Cobranza:
                                    Listado = (from n in dbContext.cobranza
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.i_IdTipoDocumento == pintIdTipoDocumento && n.v_IdCobranza.Substring(2, 2) == almacenPredeterminado && n.v_IdCobranza.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdCobranza }).ToList();
                                    break;

                                case ListaProcesos.Retencion:
                                    Listado = (from n in dbContext.documentoretencion
                                               where n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdDocumentoRetencion.Substring(2, 2) == almacenPredeterminado && n.v_IdDocumentoRetencion.Substring(0, 1) == replicationID && n.i_Eliminado == 0
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdDocumentoRetencion }).ToList();
                                    break;

                                case ListaProcesos.OrdenCompra:
                                    Listado = (from n in dbContext.ordendecompra
                                               where n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdOrdenCompra.Substring(2, 2) == almacenPredeterminado && n.v_IdOrdenCompra.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdOrdenCompra }).ToList();
                                    break;

                                case ListaProcesos.LetrasCanje:
                                    Listado = (from n in dbContext.letras
                                               where n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdLetras.Substring(2, 2) == almacenPredeterminado && n.v_IdLetras.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdLetras }).ToList();
                                    break;

                                case ListaProcesos.LetrasMantenimiento:
                                    Listado = (from n in dbContext.letrasmantenimiento
                                               where n.v_Periodo == periodoNuevo && n.v_IdLetrasMantenimiento.Substring(2, 2) == almacenPredeterminado && n.v_IdLetrasMantenimiento.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdLetrasMantenimiento }).ToList();
                                    break;

                                case ListaProcesos.LetrasCanjePago:
                                    Listado = (from n in dbContext.letraspagar
                                               where n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdLetrasPagar.Substring(2, 2) == almacenPredeterminado && n.v_IdLetrasPagar.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdLetrasPagar }).ToList();
                                    break;

                                case ListaProcesos.LiquidacionCompra:
                                    Listado = (from n in dbContext.liquidacioncompra
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdLiquidacionCompra.Substring(2, 2) == almacenPredeterminado && n.v_IdLiquidacionCompra.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdLiquidacionCompra }).ToList();
                                    break;

                                case ListaProcesos.OrdenTrabajo:

                                    Listado = (from n in dbContext.nbs_ordentrabajo
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdOrdenTrabajo.Substring(2, 2) == almacenPredeterminado && n.v_IdOrdenTrabajo.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdOrdenTrabajo }).ToList();
                                    break;

                                case ListaProcesos.FormatoUnicoFacturacion:

                                    Listado = (from n in dbContext.nbs_formatounicofacturacion
                                               where n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdFormatoUnicoFacturacion.Substring(2, 2) == almacenPredeterminado && n.v_IdFormatoUnicoFacturacion.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdFormatoUnicoFacturacion }).ToList();
                                    break;

                                case ListaProcesos.Pagos:

                                    Listado = (from n in dbContext.pago
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdPago.Substring(2, 2) == almacenPredeterminado && n.v_IdPago.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdPago }).ToList();
                                    break;


                                case ListaProcesos.GuiaRemision:

                                    Listado = (from n in dbContext.guiaremision
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdGuiaRemision.Substring(2, 2) == almacenPredeterminado && n.v_IdGuiaRemision.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdGuiaRemision }).ToList();
                                    break;

                                case ListaProcesos.ReciboHonoarios:
                                    Listado = (from n in dbContext.recibohonorario
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdReciboHonorario.Substring(2, 2) == almacenPredeterminado && n.v_IdReciboHonorario.Substring(0, 1) == replicationID
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdReciboHonorario }).ToList();

                                    break;



                                case ListaProcesos.OrdenProduccion:
                                    Listado = (from n in dbContext.ordenproduccion
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo
                                               orderby n.v_Correlativo ascending
                                               select new
                                               {
                                                   Value1 = n.v_Correlativo,
                                                   Id = n.i_IdOrdenProduccion
                                               }
                                                   ).ToList()
                                                   .Select(o => new KeyValueDTO
                                               {
                                                   Value1 = o.Value1,
                                                   Id = o.Id.ToString(),
                                               }).ToList();

                                    break;


                                case ListaProcesos.CajaChica:

                                    Listado = (from n in dbContext.cajachica
                                               where n.i_Eliminado == 0 && n.v_Periodo == periodoNuevo && n.v_Mes == mesNuevo && n.v_IdCajaChica.Substring(2, 2) == almacenPredeterminado && n.i_IdTipoDocumento == pintIdTipoDocumento
                                               orderby n.v_Correlativo ascending
                                               select new KeyValueDTO { Value1 = n.v_Correlativo, Id = n.v_IdCajaChica }).ToList();

                                    break;

                                default:
                                    pobjOperationResult.Success = 1;
                                    return "#Error#";
                            }
                            #endregion

                            if (Listado.Count != 0)
                            {
                                pobjOperationResult.Success = 1;

                                //if (!string.IsNullOrEmpty(pstrActualCorrelativo) && int.Parse(pstrActualCorrelativo) <= int.Parse(Listado.Max(p => p.Value1)))
                                //{
                                //    return pstrActualCorrelativo;
                                //}
                                return almacenPredeterminado + (int.Parse(Listado.Max(p => p.Value1.Substring(2, 6))) + 1).ToString("000000");
                            }

                            pobjOperationResult.Success = 1;
                            return almacenPredeterminado + "000001";


                        }
                        pobjOperationResult.Success = 1;
                        return !string.IsNullOrEmpty(pstrActualCorrelativo) ? pstrActualCorrelativo : almacenPredeterminado + "000001";
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "Utils.RetornaCorrelativoPorFecha()";
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return null;
                }
            }

            /// <summary>
            /// Retorna true si la cuenta esta marcada con el check 'Detalle' de la tabla asientocontable.
            /// </summary>
            /// <param name="pNroCuenta"></param>
            /// <returns></returns>
            public static bool CuentaRequiereDetalle(string pNroCuenta)
            {
                string cta = pNroCuenta;
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (pNroCuenta == null) return false;
                        pNroCuenta = pNroCuenta.Trim();
                        if (EsCuentaImputable(pNroCuenta))
                        {
                            var cuenta = dbContext.asientocontable.FirstOrDefault(p => p.v_NroCuenta == pNroCuenta && p.v_Periodo == Periodo && p.i_Eliminado.Value == 0);

                            if (cuenta != null)
                            {
                                return cuenta.i_Detalle == 1;
                            }
                            return false;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Devuelve la cantidad empaque (Unidad mínima) del articulo a evaluar
            /// </summary>
            /// <param name="pIdProductoDetalle">El Id del articulo en la tabla productodetalle</param>
            /// <param name="pCantidad">La cantidad que se está requiriendo</param>
            /// <param name="pIdUnidadMedida">La Unidad de medida de la venta/compra.</param>
            /// <returns></returns>
            public static decimal DevolverTotalUnidades(string pIdProductoDetalle, decimal pCantidad, int pIdUnidadMedida)
            {
                try
                {
                    decimal total = 0;
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var datosProducto = (from n in dbContext.productodetalle
                                             join J1 in dbContext.producto on n.v_IdProducto equals J1.v_IdProducto into J1_join
                                             from J1 in J1_join.DefaultIfEmpty()
                                             where n.v_IdProductoDetalle == pIdProductoDetalle && n.i_Eliminado == 0
                                             select new { J1.i_IdUnidadMedida, J1.d_Empaque }).FirstOrDefault();

                        var _UM = (from n in dbContext.datahierarchy
                                   where n.i_GroupId == 17 && n.i_IsDeleted == 0 && n.i_ItemId == pIdUnidadMedida
                                   select n).FirstOrDefault();

                        if (_UM == null) throw new ArgumentNullException("Una de las Unidades de Medida no Existe.");

                        switch (_UM.v_Value1)
                        {
                            case "CAJA":
                                var um = (from n in dbContext.datahierarchy
                                          where n.i_GroupId == 17 && n.i_IsDeleted == 0 && n.i_ItemId == datosProducto.i_IdUnidadMedida.Value
                                          select n).FirstOrDefault();

                                if (datosProducto != null)
                                {
                                    var caja = datosProducto.d_Empaque.Value * (um != null && !string.IsNullOrEmpty(um.v_Value2) ? decimal.Parse(um.v_Value2) : 0);
                                    total = pCantidad * caja;
                                }
                                break;

                            default:
                                total = pCantidad * (!string.IsNullOrEmpty(_UM.v_Value2) ? decimal.Parse(_UM.v_Value2) : 0);
                                break;
                        }
                    }
                    return total;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            /// <summary>
            /// Devuelve el valor redondeado del decimal enviado.
            /// </summary>
            /// <param name="valor"></param>
            /// <param name="nroDecimales">La precisión del redondeo</param>
            /// <param name="midpointRounding"></param>
            /// <returns></returns>
            public static decimal DevuelveValorRedondeado(decimal valor, int nroDecimales, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
            {
                return decimal.Parse(Math.Round(valor, nroDecimales, midpointRounding).ToString(CultureInfo.InvariantCulture));
            }

            public static double DevuelveValorRedondeadoDouble(decimal valor, int nroDecimales, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
            {
                return double.Parse(Math.Round(valor, nroDecimales, midpointRounding).ToString(CultureInfo.InvariantCulture));
            }


            /// <summary>
            /// Filtra los elementos de una grilla por coincidencia del texto ingresado sin importar el orden de las palabras.
            /// Devuelve el número de filas encontradas - Funciona sólo con grillas Infragistics (UltraGrid)
            /// </summary>
            /// <param name="grdData"></param>
            /// <param name="CadenaBusqueda"></param>
            /// <returns></returns>
            public static int FiltrarGrilla(UltraGrid grdData, string CadenaBusqueda)
            {
                try
                {
                    //recorre cada fila de la grilla
                    foreach (var row in grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList())
                    {
                        var filterRow = true;
                        //recorre cada columna de la fila de la banda 0
                        foreach (var column in grdData.DisplayLayout.Bands[0].Columns)
                        {
                            //si la columna es visible
                            if (column.IsVisibleInLayout)
                            {
                                var CumpleBusqueda = false;
                                //separa el texto de busqueda por espacios en blancos y lo almacena en una lista
                                var Cadenas = CadenaBusqueda.Split(new char[] { ' ' }).ToList();

                                if (Cadenas.Count(p => !string.IsNullOrEmpty(p.Trim())) > 0)
                                {
                                    //por cada item de la lista que no sea nulo
                                    foreach (var cadena in Cadenas.Where(p => !string.IsNullOrEmpty(p.Trim())))
                                    {
                                        //revisa que el texto de la columna contenga el item
                                        if (row.Cells[column].Text.ToUpper().Contains(cadena.ToUpper()))
                                        {
                                            CumpleBusqueda = true;
                                        }
                                        else
                                        {
                                            //si no contiene sale del for
                                            CumpleBusqueda = false;
                                            break;
                                        }
                                    }

                                    //si todos los elementos de la lista fueron encontrados en la celda
                                    if (!CumpleBusqueda) continue;
                                    //la fila no sera filtrada
                                    filterRow = false;
                                    break;
                                }
                                filterRow = false;
                                break;
                            }
                        }

                        row.Hidden = filterRow;
                    }

                    return grdData.Rows.Count(p => !p.Hidden);
                }
                catch (Exception)
                {
                    return 0;
                }
            }


            public static bool CuentaRequierePatrimonioNeto(string pNroCuenta)
            {
                string cta = pNroCuenta;
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (pNroCuenta == null) return false;
                        pNroCuenta = pNroCuenta.Trim();
                        if (EsCuentaImputable(pNroCuenta))
                        {
                            var cuenta = dbContext.asientocontable.FirstOrDefault(p => p.v_NroCuenta == pNroCuenta && p.v_Periodo == Periodo && p.i_Eliminado.Value == 0);

                            if (cuenta != null)
                            {
                                return cuenta.i_UsaraPatrimonioNeto == 1;
                            }
                            return false;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static int FiltrarGrillaPorColumnas(UltraGrid grdData, string pstrCadenaBusqueda, List<string> pstrColumnas)
            {
                try
                {
                    var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();

                    if (!string.IsNullOrWhiteSpace(pstrCadenaBusqueda))
                    {
                        var rows = new List<UltraGridRow>();
                        foreach (var columna in pstrColumnas)
                        {
                            var preSelectedRows = filas;
                            foreach (var criterio in pstrCadenaBusqueda.Split(' ').Select(p => p.Trim()))
                            {
                                var column = columna;
                                var criterio1 = criterio;
                                preSelectedRows = preSelectedRows.AsParallel().Where(
                                    f =>
                                        f.Cells[column].Value != null &&
                                        f.Cells[column].Value.ToString()
                                            .Trim()
                                            .ToUpper()
                                            .Contains(criterio1.Trim().ToUpper())).ToList();
                            }
                            rows.AddRange(preSelectedRows);
                        }

                        rows.ForEach(f => f.Hidden = false);
                        foreach (var ultraGridRow in filas.Except(rows))
                            ultraGridRow.Hidden = true;
                    }
                    else
                    {
                        foreach (var ultraGridRow in filas.AsParallel())
                        {
                            ultraGridRow.Hidden = false;
                        }
                    }

                    return grdData.Rows.Count(p => !p.Hidden);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public static int FiltrarGrilla2(UltraGrid grdData, string CadenaBusqueda)
            {
                var Cadenas = CadenaBusqueda.Split(new char[] { ' ' }).Where(p => !string.IsNullOrEmpty(p)).ToList();
                //if (!Cadenas.Any()) return grdData.Rows.Count(p => !p.Hidden);

                try
                {
                    //recorre cada fila de la grilla
                    foreach (var row in grdData.Rows)
                    {
                        var filterRow = true;
                        //recorre cada columna de la fila de la banda 0
                        foreach (var column in grdData.DisplayLayout.Bands[0].Columns)
                        {
                            //si la columna es visible
                            if (column.IsVisibleInLayout)
                            {
                                var CumpleBusqueda = true;
                                //por cada item de la lista que no sea nulo
                                foreach (var cadena in Cadenas)
                                {
                                    //revisa que el texto de la columna contenga el item
                                    if (!row.Cells[column].Text.ToUpper().Contains(cadena.ToUpper()))
                                    {
                                        //si no contiene sale del for
                                        CumpleBusqueda = false;
                                        break;
                                    }
                                }
                                //si todos los elementos de la lista fueron encontrados en la celda
                                if (CumpleBusqueda)
                                {
                                    //la fila no sera filtrada
                                    filterRow = false;
                                    break;
                                }
                            }
                        }
                        row.Hidden = filterRow;
                    }
                    return grdData.Rows.Count(p => !p.Hidden);
                }
                catch (Exception)
                {

                    throw;
                }
            }

            /// <summary>
            /// Selecciona la fila de un ultragrid, que cumpla la relacion Celda-Valor
            /// </summary>
            /// <param name="grilla">Grid de Datos</param>
            /// <param name="column">Nombre de la Columna</param>
            /// <param name="valor">Valor a buscar</param>
            public static void SelectRowForKeyValue(UltraGrid grilla, string column, string valor)
            {
                foreach (var row in grilla.Rows)
                    if (row.Cells[column].Value.ToString().Equals(valor))
                    {
                        row.Activate();
                        break;
                    }
            }

            /// <summary>
            /// Filtra los elementos de una grilla por coincidencia del texto ingresado sin importar el orden de las palabras.
            /// Devuelve el número de filas encontradas - Funciona sólo con grillas Infragistics (UltraGrid)
            /// </summary>
            /// <param name="grdData"></param>
            /// <param name="CadenaBusqueda"></param>
            /// <returns></returns>
            public static int FiltrarGrilla3(UltraGrid grdData, string CadenaBusqueda)
            {
                try
                {
                    //recorre cada fila de la grilla
                    foreach (var row in grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList())
                    {
                        var filterRow = true;
                        //recorre cada columna de la fila de la banda 0
                        foreach (var column in grdData.DisplayLayout.Bands[0].Columns)
                        {
                            var cumpleBusqueda = false;
                            //separa el texto de busqueda por espacios en blancos y lo almacena en una lista
                            var cadenas = CadenaBusqueda.Split(' ').ToList();

                            if (cadenas.Count(p => !string.IsNullOrEmpty(p.Trim())) > 0)
                            {
                                //por cada item de la lista que no sea nulo
                                foreach (var cadena in cadenas.Where(p => !string.IsNullOrEmpty(p.Trim())))
                                {
                                    //revisa que el texto de la columna contenga el item
                                    if (row.Cells[column].Text.ToUpper().Contains(cadena.ToUpper()))
                                    {
                                        cumpleBusqueda = true;
                                    }
                                    else
                                    {
                                        //si no contiene sale del for
                                        cumpleBusqueda = false;
                                        break;
                                    }
                                }

                                //si todos los elementos de la lista fueron encontrados en la celda
                                if (!cumpleBusqueda) continue;
                                //la fila no sera filtrada
                                filterRow = false;
                                break;
                            }
                            else
                            {
                                filterRow = false;
                                break;
                            }
                        }

                        row.Hidden = filterRow;
                    }

                    return grdData.Rows.Count(p => !p.Hidden);
                }
                catch (Exception)
                {

                    throw;
                }
            }

            public static void HacerGrillaAgrupable(UltraGrid grdData, bool estado)
            {
                grdData.DisplayLayout.GroupByBox.Hidden = !estado;
                grdData.DisplayLayout.GroupByBox.Prompt = @"Arraste una columna para agrupar...";
                grdData.DisplayLayout.GroupByBox.Style = GroupByBoxStyle.Compact;
                grdData.DisplayLayout.ClearGroupByColumns();
            }

            public static void MostrarOcultarFiltrosGrilla(UltraGrid grdData)
            {
                grdData.DisplayLayout.Override.FilterUIType = grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default ? FilterUIType.FilterRow : FilterUIType.Default;
                grdData.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
                grdData.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
            }

            /// <summary>
            /// Formatea una cadena recibida separada por '+' al formato {00}+{00}+{00}
            /// </summary>
            /// <param name="descuento"></param>
            /// <returns></returns>
            public static string DarFormatoDescuento(string descuento)
            {
                try
                {
                    if (descuento.Count(p => p.Equals('+')) <= 3)
                    {
                        var desctoLista = descuento.Trim().Split('+');
                        string descto = desctoLista.Select(_nro => int.Parse(_nro).ToString("00"))
                            .Aggregate(string.Empty,
                                (current, nro) => string.IsNullOrEmpty(current) ? nro : current + "+" + nro);
                        return descto;
                    }
                    return "0";
                }
                catch (Exception)
                {
                    return "0";
                }
            }


            public static string DarFormatoDescuentoUnDecimal(string descuento)
            {
                try
                {
                    if (descuento.Count(p => p.Equals('+')) <= 3)
                    {
                        var desctoLista = descuento.Trim().Split('+');
                        string descto = desctoLista.Select(_nro => decimal.Parse(_nro).ToString())
                            .Aggregate(string.Empty,
                                (current, nro) => string.IsNullOrEmpty(current) ? nro : current + "+" + nro);
                        return descto;
                    }
                    return "0";
                }
                catch (Exception)
                {
                    return "0";
                }
            }
            /// <summary>
            /// Aplica la fórmula administrativa de descuentos sucesivos, soporta hasta un máximo de 3 descuentos. 
            /// Retorna el Descuento Ponderado aplicado al Valor de la venta.
            /// </summary>
            /// <param name="descuentoFormateado"></param>
            /// <param name="valorVenta"></param>
            /// <returns></returns>
            public static decimal CalcularDescuentosSucesivos(string descuentoFormateado, decimal valorVenta)
            {
                try
                {
                    switch (descuentoFormateado.Count(p => p.Equals('+')))
                    {
                        case 0:
                            return (decimal)(((float)valorVenta * int.Parse(descuentoFormateado) / 100));

                        case 1:
                            var descuentos = descuentoFormateado.Split('+');
                            var dcto1 = (float)int.Parse(descuentos[0]);
                            var dcto2 = (float)int.Parse(descuentos[1]);
                            var dctoPonderado = (decimal)((dcto1 + dcto2) - ((dcto1 * dcto2) / 100));
                            return (valorVenta * dctoPonderado / 100);

                        case 2:
                            var _descuentos = descuentoFormateado.Split('+');
                            var _dcto1 = (float)int.Parse(_descuentos[0]);
                            var _dcto2 = (float)int.Parse(_descuentos[1]);
                            var _dcto3 = (float)int.Parse(_descuentos[2]);

                            var _dctoU = (_dcto1 + _dcto2) - ((_dcto1 * _dcto2) / 100);
                            var _dctoPonderado = (decimal)((_dctoU + _dcto3) - ((_dctoU * _dcto3) / 100));

                            return (valorVenta * _dctoPonderado / 100);

                        default:
                            return 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return 0;
                }
            }

            /// <summary>
            /// Determina el el centro de costo es un centro de costo existente en los registros.
            /// </summary>
            /// <param name="pstrCentroCosto"></param>
            /// <param name="permiteNulos"></param>
            /// <returns></returns>
            public static bool EsCentroCostoValido(string pstrCentroCosto, bool permiteNulos = true)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (permiteNulos && (string.IsNullOrEmpty(pstrCentroCosto.Trim()) || pstrCentroCosto == "0")) return true;
                        var cc = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 31 && p.i_IsDeleted == 0 && p.i_Header == 0 && p.v_Value2.Trim().Equals(pstrCentroCosto.Trim(), StringComparison.OrdinalIgnoreCase));
                        return cc != null;
                    }
                }
                catch
                {
                    return false;
                }
            }

            public static decimal CalcularDescuentosSucesivosDecimales(string descuentoFormateado, decimal valorVenta)
            {
                try
                {
                    switch (descuentoFormateado.Count(p => p.Equals('+')))
                    {
                        case 0:
                            return (decimal)(((float)valorVenta * float.Parse(descuentoFormateado) / 100));

                        case 1:
                            var descuentos = descuentoFormateado.Split('+');
                            var dcto1 = (float)int.Parse(descuentos[0]);
                            var dcto2 = (float)int.Parse(descuentos[1]);
                            var dctoPonderado = (decimal)((dcto1 + dcto2) - ((dcto1 * dcto2) / 100));
                            return (valorVenta * dctoPonderado / 100);

                        case 2:
                            var _descuentos = descuentoFormateado.Split('+');
                            var _dcto1 = (float)int.Parse(_descuentos[0]);
                            var _dcto2 = (float)int.Parse(_descuentos[1]);
                            var _dcto3 = (float)int.Parse(_descuentos[2]);

                            var _dctoU = (_dcto1 + _dcto2) - ((_dcto1 * _dcto2) / 100);
                            var _dctoPonderado = (decimal)((_dctoU + _dcto3) - ((_dctoU * _dcto3) / 100));

                            return (valorVenta * _dctoPonderado / 100);

                        default:
                            return 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return 0;
                }
            }

            /// <summary>
            /// Determina si el Formulario tiene abierto Formulario de un tipo determinado, si es asi Restaura el Form Child
            /// </summary>
            /// <param name="owner">Form Owner</param>
            /// <param name="typeFormChild">Type of Form Child</param>
            /// <param name="showMessage">Muestra un Mensaje al usuario</param>
            /// <returns>True if exists, otherwise false</returns>
            public static bool HaveFormChild(Form owner, Type typeFormChild, bool showMessage = false)
            {
                bool res = false;
                foreach (var item in owner.OwnedForms)
                    if (item.GetType() == typeFormChild)
                    {
                        item.WindowState = FormWindowState.Normal;
                        res = true;
                        if (showMessage) MessageBox.Show("Ya existe un Formulario abierto. \n Cierrelo primero!");
                    }
                return res;
            }

            /// <summary>
            /// Determina si una cuenta contable es de categoría superior a otra cuenta.
            /// </summary>
            /// <param name="ctaIngresada"></param>
            /// <param name="ctaComparativa"></param>
            /// <returns></returns>
            public static bool CuentaEsMayorIgualQue(string ctaIngresada, string ctaComparativa)
            {
                try
                {
                    if (ctaIngresada.Equals(ctaComparativa, StringComparison.OrdinalIgnoreCase))
                        return true;
                    var ctaIArray = ctaIngresada.Select(p => int.Parse(p.ToString())).ToArray();
                    var ctaCArray = ctaComparativa.Select(p => int.Parse(p.ToString())).ToArray();

                    for (var i = 0; i <= ctaIArray.GetUpperBound(0); i++)
                    {
                        try
                        {
                            var ctaI = ctaIArray[i];
                            var ctaC = ctaCArray[i];

                            if (ctaI != ctaC)
                                return ctaI > ctaC;
                        }
                        catch
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Devuelve en una lista de string las cuentas que pertenecen al rango de cuentas indicado.
            /// </summary>
            /// <param name="ctaInicio"></param>
            /// <param name="ctaFinal"></param>
            /// <returns></returns>
            public static List<string> RangoDeCuentas(string ctaInicio, string ctaFinal)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (string.IsNullOrWhiteSpace(ctaInicio) || string.IsNullOrWhiteSpace(ctaFinal)) return new List<string>();
                        var listadoCuentas =
                            dbContext.asientocontable
                            .Where(p => p.i_Eliminado == 0 && p.v_Periodo == Periodo)
                                .Select(x => x.v_NroCuenta)
                                .Distinct()
                                .OrderBy(p => p)
                                .ToList();

                        var posicionMenor = listadoCuentas.IndexOf(ctaInicio);
                        var posicionMayor = listadoCuentas.IndexOf(listadoCuentas.LastOrDefault(p => p.StartsWith(ctaFinal)));

                        return listadoCuentas.Skip(posicionMenor).Take(posicionMayor - (posicionMenor - 1)).ToList();
                    }
                }
                catch
                {
                    return new List<string>();
                }
            }

            public static List<asientocontableDto> RangoDeCuentas(string ctaInicio, string ctaFinal, bool dolares, bool soles)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (string.IsNullOrWhiteSpace(ctaInicio) || string.IsNullOrWhiteSpace(ctaFinal)) return null;
                        var listadoCompleto = RangoDeCuentas(ctaInicio, ctaFinal);
                        var listadoCuentas =
                            dbContext.asientocontable
                            .Where(p => p.i_Eliminado == 0 && p.v_Periodo == Periodo && p.i_Imputable == 1
                                && (dolares && p.i_IdMoneda == 2 || soles && p.i_IdMoneda == 1))
                                .Select(x => x.v_NroCuenta)
                                .Distinct()
                                .OrderBy(p => p)
                                .ToList();

                        var posicionMenor = listadoCompleto.IndexOf(ctaInicio);
                        var posicionMayor = listadoCompleto.IndexOf(ctaFinal);

                        var filtrado = listadoCompleto.Skip(posicionMenor).Take(posicionMayor - (posicionMenor - 1)).ToList();
                        filtrado = filtrado.Where(p => listadoCuentas.Contains(p)).ToList();
                        return dbContext.asientocontable.Where(p => p.v_Periodo.Equals(Periodo) && filtrado.Contains(p.v_NroCuenta)).ToDTOs();
                    }
                }
                catch
                {
                    return null;
                }
            }


            public static List<string> RangoDeDocumentosIdentidad(string DniInicio, string DniFinal)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {



                        if (string.IsNullOrWhiteSpace(DniInicio) || string.IsNullOrWhiteSpace(DniFinal)) return new List<string>();
                        var listadoDnis =
                            dbContext.cliente
                            .Where(p => p.i_Eliminado == 0 && p.v_FlagPantalla == "V")
                                .Select(x => x.v_NroDocIdentificacion)
                                .Distinct()
                                .OrderBy(p => p)
                                .ToList();

                        var posicionMenor = listadoDnis.IndexOf(DniInicio);
                        var posicionMayor = listadoDnis.IndexOf(DniFinal);

                        return listadoDnis.Skip(posicionMenor).Take(posicionMayor - (posicionMenor - 1)).ToList();
                    }
                }
                catch
                {
                    return new List<string>();
                }
            }

            public static string RemoverSignosAcentos(string texto)
            {
                string ConSignos = "áàäéèëíìïóòöúùuñÁÀÄÉÈËÍÌÏÓÒÖÚÙÜçÇÑ";
                string SinSignos = "aaaeeeiiiooouuunAAAEEEIIIOOOUUUcCN ";
                var textoSinAcentos = string.Empty;

                foreach (var caracter in texto)
                {
                    var indexConAcento = ConSignos.IndexOf(caracter);
                    if (indexConAcento > -1)
                        textoSinAcentos = textoSinAcentos + (SinSignos.Substring(indexConAcento, 1));
                    else
                        textoSinAcentos = textoSinAcentos + (caracter);
                }
                return textoSinAcentos;
            }

            /// <summary>
            /// Setea limites para el control de fechas al periodo de inicio de sesion
            /// </summary>
            /// <param name="dtp"></param>
            public static void SetearLimiteFechas(DateTimePicker[] dtp)
            {
                try
                {
                    var periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
                    var ultimoDiaPeriodo = DateTime.DaysInMonth(periodo, 12);
                    foreach (var dateTimePicker in dtp)
                    {
                        dateTimePicker.MinDate = new DateTime(periodo, 1, 1);
                        dateTimePicker.Value = new DateTime(periodo, DateTime.Now.Month, DateTime.Now.Day);
                        dateTimePicker.MaxDate = new DateTime(periodo, 12, ultimoDiaPeriodo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }


            //public static List<KeyValueDTO> SetFormActionsInSession(string pstrFormCode, int pintCurrentExecutionNodeId, int pintRoleId, int pintSystemUserId)
            //{
            //    try
            //    {
            //        SecurityBL _objSecurityBL = new SecurityBL();
            //        // Obtener acciones de un formulario especifico
            //        OperationResult objOperationResult = new OperationResult();
            //        List<KeyValueDTO> formActions = _objSecurityBL.GetFormAction(ref objOperationResult,
            //            pintCurrentExecutionNodeId,
            //            pintRoleId,
            //            pintSystemUserId,
            //            pstrFormCode.Trim());
            //        return formActions;
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }

            //}
            #endregion
        }

        public class Web
        {
            #region Metodos utilitarios para WebForms

            public static bool InternetConnectivityAvailable(string strServer)
            {
                try
                {
                    var reqFp = (HttpWebRequest)WebRequest.Create(strServer);
                    var rspFp = (HttpWebResponse)reqFp.GetResponse();

                    if (HttpStatusCode.OK == rspFp.StatusCode)
                    {
                        // HTTP = 200 - Internet connection available, server online
                        rspFp.Close();
                        return true;
                    }
                    else
                    {
                        // Other status - Server or connection not available
                        rspFp.Close();
                        return false;
                    }
                }
                catch (WebException)
                {
                    // Exception - connection not available
                    return false;
                }
            }

            #region LoadDropDownList
            public static void LoadDropDownList(FineUI.DropDownList prmDropDownList, string prmDataTextField = null, string prmDataValueField = null, object prmDataSource = null, DropDownListAction? prmDropDownListAction = null)
            {
                prmDropDownList.Items.Clear();
                FineUI.ListItem firstItem = null;
                if (prmDropDownListAction != null)
                {
                    //prmDropDownList. AppendDataBoundItems = true;

                    switch (prmDropDownListAction)
                    {
                        case DropDownListAction.All:
                            firstItem = new FineUI.ListItem(Constants.All, Constants.AllValue);
                            break;
                        case DropDownListAction.Select:
                            firstItem = new FineUI.ListItem(Constants.Select, Constants.SelectValue);
                            break;
                    }
                }

                if (prmDataSource != null)
                {
                    prmDropDownList.DataTextField = prmDataTextField;
                    prmDropDownList.DataValueField = prmDataValueField;
                    prmDropDownList.DataSource = prmDataSource;
                }

                prmDropDownList.DataBind();

                if (firstItem != null)
                {
                    firstItem.Selected = true;
                    prmDropDownList.Items.Insert(0, firstItem);
                }

            }

            #endregion

            #region Seguridad

            public static bool IsActionEnabled(string pstrActionCode)
            {
                if (HttpContext.Current.Session["objFormAction"] != null)
                {
                    List<KeyValueDTO> objFormAction = HttpContext.Current.Session["objFormAction"] as List<KeyValueDTO>;

                    if (objFormAction != null)
                    {
                        bool isExists = objFormAction.Exists(p => p.Value2.Equals(pstrActionCode.Trim()));

                        if (isExists)
                            return true;
                    }
                }

                return false;
            }


            public static void SetFormActionsInSession(string pstrFormCode, List<KeyValueDTO> formActions)
            {
                try
                {
                    if (HttpContext.Current.Session["objClientSession"] != null)
                    {
                        var objClientSession = (ClientSession)HttpContext.Current.Session["objClientSession"];

                        // Guardar las acciones
                        HttpContext.Current.Session["objFormAction"] = formActions;
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }

            public void AddPaginasToCache(string key, AutorizationList[] Pages, System.Web.HttpContext context)
            {
                try
                {
                    if (context.Cache.Get(key) != null)
                        context.Cache.Remove(key); //remuevo si lo encuentra para reemplazarlo

                    context.Cache.Add(key,
                        Pages, null,
                        DateTime.Now.AddMonths(1),
                        TimeSpan.Zero,
                        System.Web.Caching.CacheItemPriority.High, null); //agrego al cache
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }



            #endregion

            #endregion
        }

        #region Encription
        public static string Encrypt(string pData)
        {
            UnicodeEncoding parser = new UnicodeEncoding();
            byte[] _original = parser.GetBytes(pData);
            MD5CryptoServiceProvider Hash = new MD5CryptoServiceProvider();
            byte[] _encrypt = Hash.ComputeHash(_original);
            return Convert.ToBase64String(_encrypt);
        }
        #endregion

        public string ExtractFormName(string pstrPath)
        {
            return pstrPath.Remove(pstrPath.LastIndexOf(".")).Substring(pstrPath.LastIndexOf("/") + 1);
        }

        public static string GetNewId(int pintNodeId, int pintSequential, string pstrPrefix)
        {
            var nodeSufix = Globals.ClientSession != null ? Globals.ClientSession.ReplicationNodeID : "N";
            return string.Format("{0}{1}-{2}{3}", nodeSufix, pintNodeId.ToString("000"), pstrPrefix, pintSequential.ToString("000000000"));
        }

        public static string GetNewVoucherId(string pstrSerialNumber, int pintSequential)
        {
            return string.Format("{0}-{1}", pstrSerialNumber, pintSequential.ToString("00000000"));
        }

        #region Config Files
        public static string GetConnectionString(string nombreCadena)
        {
            return ConfigurationManager.ConnectionStrings[nombreCadena].ConnectionString;
        }

        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }
        #endregion

        #region Exception Handling
        public static string ExceptionFormatter(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MESSAGE: " + ex.Message);
            sb.AppendLine("STACK TRACE: " + ex.StackTrace);
            sb.AppendLine("SOURCE: " + ex.Source);
            if (ex.InnerException != null)
            {
                sb.AppendLine("INNER EXCEPTION MESSAGE: " + ex.InnerException.Message);
                sb.AppendLine("INNER EXCEPTION STACK TRACE: " + ex.InnerException.StackTrace);
            }
            return sb.ToString();
        }

        public static void ExceptionToLog(int IdUser, OperationResult pobjOperationResult)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                log _log = new log();
                _log.i_SystemUserId = IdUser;
                _log.t_FechaHoraError = DateTime.Now;
                _log.v_Error = pobjOperationResult.ErrorMessage;
                _log.v_Exception = pobjOperationResult.ExceptionMessage;
                _log.v_InformacionAdicional = pobjOperationResult.AdditionalInformation;
                _log.v_NombrePC = System.Environment.MachineName;
                dbContext.log.AddObject(_log);
                try
                {
                    dbContext.SaveChanges();
                }
                catch
                {
                }
            }
        }

        #endregion

        public static DataTable ConvertToDatatable<T>(List<T> data)
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;

        }

        public static string ConvertLetter(string _textNumber, string _currency)
        {
            string Words = string.Empty;
            string Number = string.Empty;
            string auxNumber = string.Empty;
            string decimalPart = string.Empty;
            string integerPart = string.Empty;
            string Fl = string.Empty;
            string Fl_II = string.Empty;
            int numberAlone = -1;

            auxNumber = _textNumber.Replace("$", "").Replace(",", "").Replace("+", "").Trim();

            if (isFloatNumber(auxNumber))
            {

                //-------Si es un número negativo
                if (auxNumber.Substring(0, 1).Equals("-"))
                {
                    Words = "Menos ";
                    auxNumber = auxNumber.Substring(1);
                }

                //-------Si tiene ceros a la izquierda

                for (int i = 0; i < auxNumber.Length; i++)
                {
                    if (auxNumber.Substring(i, 1).Equals("0"))
                    {
                        Number = auxNumber.Substring(i + 1);
                    }
                    else
                    {
                        break;
                    }
                }

                if (string.IsNullOrEmpty(Number)) { Number = auxNumber; }

                //-------Separa la parte entera de la decimal 

                string[] arrayNumber = splitString(Number, '.');

                integerPart = arrayNumber[0];

                if (arrayNumber[1].Length > 2)
                {
                    decimalPart = arrayNumber[1].Substring(0, 2);
                }
                else if (arrayNumber[1].Length == 2)
                {
                    decimalPart = arrayNumber[1];
                }
                else if (arrayNumber[1].Length == 1)
                {
                    decimalPart = arrayNumber[1] + "0";
                }

                //-------Proceso de conversión

                if (float.Parse(Number) <= 1000000)
                {
                    int sbt = 0;

                    if (int.Parse(integerPart) != 0)
                    {

                        for (int i = integerPart.Length; i > 0; i--)
                        {

                            numberAlone = int.Parse(integerPart.Substring(sbt, 1));


                            switch (i)
                            {

                                //--------Arma las centenas
                                case 6:
                                case 3:

                                    switch (numberAlone)
                                    {
                                        case 1:
                                            if (integerPart.Substring(sbt + 1, 1).Equals("0") &&
                                            integerPart.Substring(sbt + 2, 1).Equals("0"))
                                            { Words = Words + "Cien "; }
                                            else { Words = Words + "Ciento "; }
                                            break;

                                        case 2:
                                            Words = Words + "Doscientos ";
                                            break;

                                        case 3:
                                            Words = Words + "Trescientos ";
                                            break;

                                        case 4:
                                            Words = Words + "Cuatrocientos ";
                                            break;

                                        case 5:
                                            Words = Words + "Quinientos ";
                                            break;

                                        case 6:
                                            Words = Words + "SeisCientos ";
                                            break;

                                        case 7:
                                            Words = Words + "Setecientos ";
                                            break;

                                        case 8:
                                            Words = Words + "Ochocientos ";
                                            break;

                                        case 9:
                                            Words = Words + "Novecientos ";
                                            break;
                                    }

                                    break;
                                //--------Arma las decenas
                                case 5:
                                case 2:

                                    switch (numberAlone)
                                    {
                                        case 1:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Diez "; Fl = "D"; if (i == 2) { Fl_II = "X"; } else { Fl_II = string.Empty; } }
                                            else if (integerPart.Substring(sbt + 1, 1).Equals("1"))
                                            { Words = Words + "Once "; Fl = "D"; if (i == 2) { Fl_II = "X"; } else { Fl_II = string.Empty; } }
                                            else if (integerPart.Substring(sbt + 1, 1).Equals("2"))
                                            { Words = Words + "Doce "; Fl = "D"; if (i == 2) { Fl_II = "X"; } else { Fl_II = string.Empty; } }
                                            else if (integerPart.Substring(sbt + 1, 1).Equals("3"))
                                            { Words = Words + "Trece "; Fl = "D"; if (i == 2) { Fl_II = "X"; } else { Fl_II = string.Empty; } }
                                            else if (integerPart.Substring(sbt + 1, 1).Equals("4"))
                                            { Words = Words + "Catorce "; Fl = "D"; if (i == 2) { Fl_II = "X"; } else { Fl_II = string.Empty; } }
                                            else if (integerPart.Substring(sbt + 1, 1).Equals("5"))
                                            { Words = Words + "Quince "; Fl = "D"; if (i == 2) { Fl_II = "X"; } else { Fl_II = string.Empty; } }
                                            else
                                            { Words = Words + "Dieci"; }

                                            break;

                                        case 2:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Veinte "; }
                                            else
                                            { Words = Words + "Veinti"; }

                                            break;

                                        case 3:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Treinta "; }
                                            else
                                            { Words = Words + "Treinta Y "; }

                                            break;

                                        case 4:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Cuarenta "; }
                                            else
                                            { Words = Words + "Cuarenta Y "; }

                                            break;

                                        case 5:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Cincuenta "; }
                                            else
                                            { Words = Words + "Cincuenta Y "; }

                                            break;

                                        case 6:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Sesenta "; }
                                            else
                                            { Words = Words + "Sesenta Y "; }

                                            break;

                                        case 7:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Setenta "; }
                                            else
                                            { Words = Words + "Setenta Y "; }

                                            break;

                                        case 8:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Ochenta "; }
                                            else
                                            { Words = Words + "Ochenta Y "; }

                                            break;

                                        case 9:

                                            if (integerPart.Substring(sbt + 1, 1).Equals("0"))
                                            { Words = Words + "Noventa "; }
                                            else
                                            { Words = Words + "Noventa Y "; }

                                            break;

                                    }

                                    break;


                                //--------Arma las unidades
                                case 7:
                                case 4:
                                case 1:

                                    switch (numberAlone)
                                    {
                                        case 1:

                                            if (!Fl.Equals("D"))
                                            {
                                                if (i == 4)
                                                {
                                                    Words = Words + "Un ";
                                                }
                                                else
                                                {
                                                    Words = Words + "Un "; //UNO
                                                }
                                            }
                                            else if (Fl.Equals("D") && string.IsNullOrEmpty(Fl_II))
                                            { Words = Words + "Uno "; }

                                            break;

                                        case 2:

                                            if (!Fl.Equals("D"))
                                            {
                                                Words = Words + "Dos ";
                                            }
                                            else if (Fl.Equals("D") && string.IsNullOrEmpty(Fl_II))
                                            { Words = Words + "Dos "; }

                                            break;

                                        case 3:

                                            if (!Fl.Equals("D"))
                                            {
                                                Words = Words + "Tres ";
                                            }
                                            else if (Fl.Equals("D") && string.IsNullOrEmpty(Fl_II))
                                            { Words = Words + "Tres "; }

                                            break;

                                        case 4:

                                            if (!Fl.Equals("D"))
                                            {
                                                Words = Words + "Cuatro ";
                                            }
                                            else if (Fl.Equals("D") && string.IsNullOrEmpty(Fl_II))
                                            { Words = Words + "Cuatro "; }

                                            break;

                                        case 5:

                                            if (!Fl.Equals("D"))
                                            {
                                                Words = Words + "Cinco ";
                                            }
                                            else if (Fl.Equals("D") && string.IsNullOrEmpty(Fl_II))
                                            { Words = Words + "Cinco "; }

                                            break;

                                        case 6:
                                            Words = Words + "Seis ";
                                            break;

                                        case 7:
                                            Words = Words + "Siete ";
                                            break;

                                        case 8:
                                            Words = Words + "Ocho ";
                                            break;

                                        case 9:
                                            Words = Words + "Nueve ";
                                            break;
                                    }

                                    break;

                            }
                            if (i == 4)
                            {
                                Words = Words + "Mil ";
                            }

                            if (i == 7 && integerPart.Substring(0, 1).Equals("1"))
                            {
                                Words = Words + "Millón ";
                            }
                            else if (i == 7 && !integerPart.Substring(0, 1).Equals("1"))
                            {
                                Words = Words + "Millones ";
                            }

                            sbt += 1;
                        }
                    }
                    else
                    {
                        Words = "Cero ";
                    }

                    //-------Une la parte entera con la decimal y asigna la moneda

                    if (_currency.Equals("MX"))
                    {
                        Words = Words + " Pesos " + decimalPart + "/100 M.N.";
                    }
                    else
                    {
                        Words = Words + " Con " + decimalPart + "/100";
                    }


                }
                else
                {
                    Words = "NÚMERO FUERA DE RANGO [XXXXXXX.XX]";
                }

            }
            else
            {
                Words = "DATO NO NUMÉRICO";
            }

            return Words;
        }
        //-------El código anterior requiere de las siguientes funciones

        public static bool isFloatNumber(string _numberText)
        {
            float Result = 0;
            bool numberResult = false;

            if (float.TryParse(_numberText, out Result))
            {
                numberResult = true;
            }

            return numberResult;
        }

        public static T ToEnum<T>(int id)
        {
            return (T)Enum.ToObject(typeof(T), id);
        }
        public static string[] splitString(string _textString, char _character)
        {
            string[] split = null;

            if (!string.IsNullOrEmpty(_textString))
            {
                if (_textString.Contains(_character.ToString()))
                {
                    split = _textString.Split(new char[] { _character });

                    if (string.IsNullOrEmpty(split[0])) { split[0] = "0"; }

                }
                else
                {
                    split = new string[2];
                    split[0] = _textString;
                    split[1] = "00";
                }
            }

            return split;
        }

        public static string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        #region Nuevo algoritmo CONVERTIR NUMERO A LETRAS

        public static string ConvertirenLetras(string num)
        {
            double nro;
            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }
            var dec = "";
            var entero = Convert.ToInt64(Math.Truncate(nro));
            var decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));
            if (decimales > 0)
            {
                dec = " CON " + decimales + "/100";
            }
            else
            {
                dec = " CON " + "00/100";
            }

            var res = toText(Convert.ToDouble(entero)) + dec;
            return res;

        }






        #region NumerosLetrasIngles


        #endregion


        private static string toText(double value)
        {


            string Num2Text = "";


            value = Math.Truncate(value);


            if (value == 0) Num2Text = "CERO";


            else if (value == 1) Num2Text = "UNO";


            else if (value == 2) Num2Text = "DOS";


            else if (value == 3) Num2Text = "TRES";


            else if (value == 4) Num2Text = "CUATRO";


            else if (value == 5) Num2Text = "CINCO";


            else if (value == 6) Num2Text = "SEIS";


            else if (value == 7) Num2Text = "SIETE";


            else if (value == 8) Num2Text = "OCHO";


            else if (value == 9) Num2Text = "NUEVE";


            else if (value == 10) Num2Text = "DIEZ";


            else if (value == 11) Num2Text = "ONCE";


            else if (value == 12) Num2Text = "DOCE";


            else if (value == 13) Num2Text = "TRECE";


            else if (value == 14) Num2Text = "CATORCE";


            else if (value == 15) Num2Text = "QUINCE";


            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);


            else if (value == 20) Num2Text = "VEINTE";


            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);


            else if (value == 30) Num2Text = "TREINTA";


            else if (value == 40) Num2Text = "CUARENTA";


            else if (value == 50) Num2Text = "CINCUENTA";


            else if (value == 60) Num2Text = "SESENTA";


            else if (value == 70) Num2Text = "SETENTA";


            else if (value == 80) Num2Text = "OCHENTA";


            else if (value == 90) Num2Text = "NOVENTA";


            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);


            else if (value == 100) Num2Text = "CIEN";


            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);


            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";


            else if (value == 500) Num2Text = "QUINIENTOS";


            else if (value == 700) Num2Text = "SETECIENTOS";


            else if (value == 900) Num2Text = "NOVECIENTOS";


            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);


            else if (value == 1000) Num2Text = "MIL";


            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);


            else if (value < 1000000)
            {


                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";


                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);


            }


            else if (value == 1000000) Num2Text = "UN MILLON";


            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);


            else if (value < 1000000000000)
            {

                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";

                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }
            else if (value == 1000000000000) Num2Text = "UN BILLON";

            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {

                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            }
            return Num2Text;
        }

        public static string DecimalToWords(decimal number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + DecimalToWords(Math.Abs(number));

            string words = "";

            int intPortion = (int)number;
            decimal fraction = (number - intPortion) * 100;
            int decPortion = (int)fraction;

            words = NumberToWords(intPortion);
            if (decPortion > 0)
            {
                words += " and ";
                words += NumberToWords(decPortion);
            }
            return words.ToUpper();
        }
        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        #endregion

        /// <summary>
        /// Realiza un ping a una dirección IP o un HostName y arroja verdadero si el ping fue exitoso o falso si el ping falló indicando la conectividad de la dirección
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        /// <param name="timeOut"></param>
        /// <returns>Verdadero o Falso</returns>
        public static bool PingNetwork(string hostNameOrAddress, int timeOut, int retries)
        {
            bool pingStatus = false;
            int counter = 0;

            while (counter < retries)
            {
                using (Ping p = new Ping())
                {
                    string data = "Cadena de datos sólo para ser enviada por el ping...";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    try
                    {
                        PingReply reply = p.Send(hostNameOrAddress, timeOut, buffer);
                        pingStatus = (reply.Status == IPStatus.Success);

                        if (pingStatus) break;
                    }
                    catch (Exception)
                    {
                        pingStatus = false;
                    }
                }
                counter++;
            }

            return pingStatus;
        }

        public static class ObjectCopier
        {
            /// <summary>
            /// Perform a deep Copy of the object.
            /// </summary>
            /// <typeparam name="T">The type of object being copied.</typeparam>
            /// <param name="source">The object instance to copy.</param>
            /// <returns>The copied object.</returns>
            public static T Clone<T>(T source)
            {
                if (!typeof(T).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                // Don't serialize a null object, simply return the default for that object
                if (Object.ReferenceEquals(source, null))
                {
                    return default(T);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
        }

        #region Ubigueo
        public class Ubigeo
        {
            /// <summary>
            /// Obtiene el Ubigueo incluido en una cadena de texto
            /// </summary>
            /// <param name="contenido">Cadena de Texto a buscar Ubigueo</param>
            /// <returns>key = i_ParameterId, value = v_Value1</returns>
            public static List<KeyValuePair<int, string>> GetUbigueo(string contenido)
            {
                contenido = Regex.Replace(contenido.Trim(), @"\s+", " ").ToUpper();
                ComprobarDistrito(ref contenido);

                var parts1 = contenido.Split('-');
                var indice = (parts1.Length > 3) ? (parts1.Length - 3) : 0;

                parts1[indice] = Regex.Replace(parts1[indice], @"[\W\d]", " ").Trim();

                try
                {
                    var departamento = ObtenerDepartamento(parts1[indice]);
                    if (departamento.Key != -1)
                    {
                        using (var dbContext = new SAMBHSEntitiesModelWin())
                        {

                            var prov = parts1[parts1.Length - 2].Trim();
                            var provincia = (from a in dbContext.systemparameter
                                             where a.i_GroupId == 112 && a.i_ParentParameterId == departamento.Key && a.v_Value1.Equals(prov)
                                             select a).First();
                            var dist = parts1.Last().Trim();

                            var distrito = (from a in dbContext.systemparameter
                                            where a.i_GroupId == 112 && a.i_ParentParameterId == provincia.i_ParameterId && dist.Contains(a.v_Value1.Replace("-", ""))
                                            orderby a.v_Value2.Length descending
                                            select a).First();

                            return new List<KeyValuePair<int, string>>{ 
                            new KeyValuePair<int,string>(departamento.Key, departamento.Value),
                            new KeyValuePair<int,string>(provincia.i_ParameterId, provincia.v_Value1),
                            new KeyValuePair<int,string>(distrito.i_ParameterId, distrito.v_Value1)
                            };
                        }
                    }
                }
                catch
                {
                    // ignored
                }
                return null;
            }
            /// <summary>
            /// Obtiene el ultimo departamento en una cadena.
            /// </summary>
            /// <param name="pstrSearch">Cadena a buscar departamento</param>
            /// <returns>key = i_ParameterId, Value = v_Value1</returns>
            public static KeyValuePair<int, string> ObtenerDepartamento(String pstrSearch)
            {
                var result = new KeyValuePair<int, string>(-1, string.Empty);
                try
                {
                    var deps = new List<KeyValuePair<int, systemparameter>>();
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var departamentos = (from n in dbContext.systemparameter
                                             where n.i_GroupId == 112 && n.i_ParentParameterId == 1
                                             orderby n.v_Value1 ascending
                                             select n).ToList();
                        foreach (var dep in departamentos)
                        {
                            if (pstrSearch.Contains(dep.v_Value1))
                            {
                                var ids = pstrSearch.LastIndexOf(dep.v_Value1);
                                deps.Add(new KeyValuePair<int, systemparameter>(ids, dep));

                                while (pstrSearch.Contains(dep.v_Value1))
                                    pstrSearch = Replace(pstrSearch, pstrSearch.IndexOf(dep.v_Value1), dep.v_Value1.Length, ' ');
                            }
                        }
                        deps = deps.OrderBy(item => item.Key).ToList();
                        if (deps.Any()) result = new KeyValuePair<int, string>(deps.LastOrDefault().Value.i_ParameterId,
                                                                              deps.LastOrDefault().Value.v_Value1);
                    }
                }
                catch (Exception)
                {

                }
                return result;
            }
            public static String Replace(string obj, int startIndex, int count, char newchar)
            {
                var arr = obj.ToCharArray();
                for (int i = 0; i < count; i++)
                {
                    arr[startIndex + i] = newchar;
                }
                return new string(arr);
            }
            private static void ComprobarDistrito(ref string contenido)
            {
                if (!contenido.Contains("-")) return;
                int PenUltimo = contenido.Substring(0, contenido.LastIndexOf('-')).LastIndexOf('-');

                var distrito = contenido.Substring(PenUltimo + 1);

                distrito = distrito.Replace(" ", "");
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var result = (from n in dbContext.systemparameter
                                      where n.i_GroupId == 112 && n.v_Value1.Contains("-")
                                      select n.v_Value1).ToList();

                        if (result.Any(i => i.Equals(distrito)))
                            contenido = contenido.Remove(PenUltimo + 1) + distrito.Replace("-", "");
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        #endregion

        public static class AperturaData
        {
            public static void IniciarDirecionesClientes(ref OperationResult objOperationResult, TipoMotorBD tipoMotor)
            {
                try
                {
                    List<ClienteDireccion> clientes;
                    switch (tipoMotor)
                    {
                        case TipoMotorBD.MSSQLServer:
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {
                                var sqlCnx = ((System.Data.EntityClient.EntityConnection)dbContext.Connection)
                                    .StoreConnection;
                                if (sqlCnx.State != ConnectionState.Open) sqlCnx.Open();

                                clientes = sqlCnx.Query<ClienteDireccion>("select \"v_IdCliente\" as 'idcliente', \"v_DirecPrincipal\" as 'direccion' " +
                                                                          "from cliente where \"i_Eliminado\" = 0").ToList();

                                var sqlTrans = sqlCnx.BeginTransaction();
                                sqlCnx.Execute("delete from clientedirecciones", transaction: sqlTrans);
                                foreach (var c in clientes)
                                {
                                    var dir = c.direccion != null ? c.direccion.Replace("'", "''") : string.Empty;
                                    sqlCnx.Execute(
                                       "INSERT INTO clientedirecciones(\"v_Direccion\", \"i_IdZona\", \"v_IdCliente\", " +
                                       "\"i_EsDireccionPredeterminada\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\") " +
                                       "VALUES ('" + dir + "', -1, '" + c.idcliente + "', 1, 0, 1, getdate());", transaction: sqlTrans);
                                }
                                sqlTrans.Commit();
                                sqlCnx.Close();
                            }
                            break;

                        case TipoMotorBD.PostgreSQL:
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {
                                var pgSqlCnx = ((System.Data.EntityClient.EntityConnection)dbContext.Connection)
                                    .StoreConnection;
                                if (pgSqlCnx.State != ConnectionState.Open) pgSqlCnx.Open();

                                clientes = pgSqlCnx.Query<ClienteDireccion>("select \"v_IdCliente\" as \"idcliente\", \"v_DirecPrincipal\" as \"direccion\" " +
                                                                            "from cliente where \"i_Eliminado\" = 0").ToList();

                                var sqlTrans = pgSqlCnx.BeginTransaction();
                                pgSqlCnx.Execute("delete from clientedirecciones", transaction: sqlTrans);
                                foreach (var c in clientes)
                                {
                                    var dir = c.direccion != null ? c.direccion.Replace("'", "''") : string.Empty;
                                    pgSqlCnx.Execute(
                                       "INSERT INTO clientedirecciones(\"v_Direccion\", \"i_IdZona\", \"v_IdCliente\", " +
                                       "\"i_EsDireccionPredeterminada\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\") " +
                                       "VALUES ('" + dir + "', -1, '" + c.idcliente + "', 1, 0, 1, current_date);", transaction: sqlTrans);
                                }
                                sqlTrans.Commit();
                                pgSqlCnx.Close();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("tipoMotor", tipoMotor, null);
                    }
                    objOperationResult.Success = 1;
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    objOperationResult.AdditionalInformation = "Utils.IniciarDirecionesClientes()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    objOperationResult.ErrorMessage = ex.Message;
                    objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                }
            }

            private static int GetNextSecuentialId(int pintNodeId, int pintTableId)
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    string ReplicationID = "N";
                    secuential objSecuential = (from a in dbContext.secuential where a.i_TableId == pintTableId && a.i_NodeId == pintNodeId && a.v_ReplicationID == ReplicationID select a).SingleOrDefault();

                    // Actualizar el campo con el nuevo valor a efectos de reservar el ID autogenerado para evitar colisiones entre otros nodos
                    if (objSecuential != null)
                    {
                        objSecuential.i_SecuentialId = objSecuential.i_SecuentialId + 1;
                    }
                    else
                    {
                        objSecuential = new secuential
                        {
                            i_NodeId = pintNodeId,
                            i_TableId = pintTableId,
                            i_SecuentialId = 1,
                            v_ReplicationID = ReplicationID
                        };
                        dbContext.AddTosecuential(objSecuential);
                    }

                    dbContext.SaveChanges();

                    return objSecuential.i_SecuentialId.Value;
                }
            }

            private static void InsertarConcepto(ref OperationResult pobjOperationResult, conceptoDto pobjDtoEntity, List<string> ClientSession)
            {
                string newId = string.Empty;

                try
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    concepto objEntity = conceptoAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = GetNextSecuentialId(intNodeId, 34);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZT");
                    objEntity.v_IdConcepto = newId;
                    dbContext.AddToconcepto(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "concepto", newId);
                    return;
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "ConceptoBL.InsertarConcepto()";
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return;
                }
            }

            private static void InsertarAdministracionConceptos(ref OperationResult pobjOperationResult, administracionconceptosDto pobjDtoEntity, List<string> ClientSession)
            {
                string newId = string.Empty;

                try
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    administracionconceptos objEntity = administracionconceptosAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = GetNextSecuentialId(intNodeId, 35);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZV");
                    objEntity.v_IdAdministracionConceptos = newId;
                    dbContext.AddToadministracionconceptos(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "administracionconceptos", newId);
                    return;
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "AdministracionConceptosBL.InsertarAdministracionConceptos()";
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return;
                }
            }

            public static bool DestinoNuevo(ref OperationResult pobjOperationResult, List<destinoDto> pTemp_Insertar)
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    int SecuentialId = 0;
                    string newIdDestino = string.Empty;
                    decimal SumaPorcentajes = 0;

                    foreach (destinoDto DestinoDto in pTemp_Insertar)
                    {
                        destino objEntityDestino = DestinoDto.ToEntity();
                        SecuentialId = GetNextSecuentialId(1, 67);
                        newIdDestino = GetNewId(1, SecuentialId, "LM");
                        objEntityDestino.v_IdDestino = newIdDestino;
                        objEntityDestino.t_InsertaFecha = DateTime.Now;
                        objEntityDestino.i_Eliminado = 0;
                        dbContext.AddTodestino(objEntityDestino);
                    }
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    return true;
                }
            }

            /// <summary>
            /// Llena la tabla lineacuenta para separar las cuentas contables enlazadas con las lineas por periodos.
            /// </summary>
            /// <param name="pobjOperationResult"></param>
            /// <param name="periodo"></param>
            public static void InicializaLineas(ref OperationResult pobjOperationResult, string periodo)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var lineascuentaToDelete = dbContext.lineacuenta.Where(p => p.v_Periodo.Equals(periodo)).ToList();
                        lineascuentaToDelete.ForEach(o => dbContext.DeleteObject(o));
                        var lineasActivas = dbContext.linea.Where(p => p.i_Eliminado == 0).ToList();
                        var listaInsertar = lineasActivas.Select(linea => new lineacuenta
                        {
                            i_Eliminado = 0,
                            i_InsertaIdUsuario = 1,
                            v_IdLinea = linea.v_IdLinea,
                            v_NroCuentaCompra = linea.v_NroCuentaCompra,
                            v_NroCuentaDConsumo = linea.v_NroCuentaDConsumo,
                            v_NroCuentaHConsumo = linea.v_NroCuentaHConsumo,
                            v_NroCuentaVenta = linea.v_NroCuentaVenta,
                            v_Periodo = periodo
                        }).ToList();

                        listaInsertar.ForEach(o => dbContext.AddTolineacuenta(o));
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "AperturaData.InicializaLineas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                }
            }

            public static void InicializaConceptos(ref OperationResult pobjOperationResult, string periodoOrigen, string periodoDestino)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var or = new OperationResult();
                        var conceptosBase = dbContext.concepto.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToDTOs();
                        var administracionConceptosBase = dbContext.administracionconceptos.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToDTOs();

                        if (conceptosBase.Any() && administracionConceptosBase.Any())
                        {
                            var conceptosEliminar = dbContext.concepto.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();
                            var administracionConceptosEliminar = dbContext.administracionconceptos.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();

                            conceptosEliminar.ForEach(o => dbContext.DeleteObject(o));
                            administracionConceptosEliminar.ForEach(o => dbContext.DeleteObject(o));
                            dbContext.SaveChanges();

                            conceptosBase.ForEach(o =>
                            {
                                o.v_Periodo = periodoDestino;
                                InsertarConcepto(ref or, o, Globals.ClientSession.GetAsList());
                            });

                            administracionConceptosBase.ForEach(o =>
                            {
                                o.v_Periodo = periodoDestino;
                                InsertarAdministracionConceptos(ref or, o, Globals.ClientSession.GetAsList());
                            });
                        }
                        pobjOperationResult.Success = 1;
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "AperturaData.InicializaConceptos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                }
            }

            /// <summary>
            /// Copia el plan de cuentas del año de origen con el año de destino.
            /// </summary>
            /// <param name="pobjOperationResult"></param>
            /// <param name="periodoOrigen"></param>
            /// <param name="periodoDestino"></param>
            ///     
            private static void MigrarPlanCuentasPeriodo(ref OperationResult pobjOperationResult, string periodoOrigen, string periodoDestino)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var listaCuentas = dbContext.asientocontable.Where(p => p.v_Periodo.Trim().Equals(periodoOrigen) && p.i_Eliminado == 0).ToList();

                        if (listaCuentas.Any())
                        {
                            var listaEliminar = dbContext.asientocontable.Where(p => p.v_Periodo.Trim().Equals(periodoDestino) && p.i_Eliminado == 0).ToList();
                            listaEliminar.ForEach(o => dbContext.DeleteObject(o));

                            var listaInsertar = listaCuentas.Select(n => new asientocontable
                            {
                                i_Eliminado = 0,
                                i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                i_ACM = n.i_ACM,
                                i_ActivarRubro = n.i_ActivarRubro,
                                i_AjusteDiferenciaCambio = n.i_AjusteDiferenciaCambio,
                                i_Analisis = n.i_Analisis,
                                i_AplicacionDestino = n.i_AplicacionDestino,
                                i_CentroCosto = n.i_CentroCosto,
                                i_Detalle = n.i_Detalle,
                                i_EntFinanciera = n.i_EntFinanciera,
                                i_EsActivo = n.i_EsActivo,
                                i_IdCuenta = n.i_IdCuenta,
                                i_IdentificaCtaBancos = n.i_IdentificaCtaBancos,
                                i_IdMoneda = n.i_IdMoneda,
                                i_Imputable = n.i_Imputable,
                                i_LongitudJerarquica = n.i_LongitudJerarquica,
                                i_Naturaleza = n.i_Naturaleza,
                                i_PermiteItem = n.i_PermiteItem,
                                i_RendicionFondos = n.i_RendicionFondos,
                                v_Area = n.v_Area,
                                v_NombreCuenta = n.v_NombreCuenta,
                                v_NroCuenta = n.v_NroCuenta,
                                v_Periodo = periodoDestino,
                                v_TipoExistencia = n.v_TipoExistencia,
                                t_InsertaFecha = DateTime.Now
                            }).ToList();

                            listaInsertar.ForEach(o => dbContext.AddToasientocontable(o));
                            dbContext.SaveChanges();
                            pobjOperationResult.Success = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "AperturaData.MigrarPlanCuentasPeriodo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                }
            }

            public static void InicializarDestinos(ref OperationResult objOperationResult, string pstrPeriodoOrigen, string pstrPeriodoDestino)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var destinosEliminar = dbContext.destino.Where(p => p.v_Periodo.Equals(pstrPeriodoDestino) && p.i_Eliminado == 0).ToList();

                        if (destinosEliminar.Any())
                        {
                            destinosEliminar.ForEach(d => dbContext.DeleteObject(d));
                            dbContext.SaveChanges();
                        }

                        var destinos = dbContext.destino.Where(p => p.v_Periodo.Equals(pstrPeriodoOrigen) && p.i_Eliminado == 0).ToList();

                        var insertar = (from d in destinos
                                        where dbContext.asientocontable.Any(p => p.v_NroCuenta.Equals(d.v_CuentaOrigen) && p.v_Periodo.Equals(pstrPeriodoDestino) && p.i_Eliminado == 0)
                                        select new destinoDto
                                        {
                                            v_Periodo = pstrPeriodoDestino,
                                            i_Eliminado = 0,
                                            i_InsertaIdUsuario = 1,
                                            i_Porcentaje = d.i_Porcentaje,
                                            t_InsertaFecha = DateTime.Now,
                                            v_CuentaDestino = d.v_CuentaDestino,
                                            v_CuentaOrigen = d.v_CuentaOrigen,
                                            v_CuentaTransferencia = d.v_CuentaTransferencia
                                        }).ToList();

                        DestinoNuevo(ref objOperationResult, insertar);
                        if (objOperationResult.Success == 0) return;
                        objOperationResult.Success = 1;
                    }
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    objOperationResult.AdditionalInformation = "UtilsBL.InicializarDestinos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    objOperationResult.ErrorMessage = ex.Message;
                    objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                }
            }


            public static void InicializarProductoAlmacen(ref OperationResult objOperationResult, string PeriodoApertura)
            {
                try
                {
                    objOperationResult.Success = 1;
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var EstablecimientoAlmacen = (from a in dbContext.establecimientoalmacen where a.i_Eliminado == 0 select a.i_IdAlmacen).ToList();
                        foreach (var Idalmacen in EstablecimientoAlmacen)
                        {
                            InscribirProductosEnAlmacen(ref objOperationResult, Idalmacen.Value, PeriodoApertura, Globals.ClientSession.GetAsList());
                            if (objOperationResult.Success == 0)
                            {
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    objOperationResult.AdditionalInformation = "UtilsBL.InicializarProductoAlmacen()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    objOperationResult.ErrorMessage = ex.Message;
                    objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                }
            }

            public static void InscribirProductosEnAlmacen(ref OperationResult objOperationResult, int Almacen, string periodo, List<string> ClientSession)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        objOperationResult.Success = 1;
                        var productosDetalles = dbContext.productodetalle.Where(l => l.i_Eliminado == 0 && l.producto.i_EsActivoFijo != 1).ToList().Select(l => l.v_IdProductoDetalle).ToList();
                        var productoAlmacenExistente = dbContext.productoalmacen.Where(l => l.i_IdAlmacen == Almacen && l.v_Periodo == periodo && l.i_Eliminado == 0).ToList();
                        var xx = productoAlmacenExistente.Where(l => l.v_ProductoDetalleId == "N001-PE000037184").ToList();
                        var productosInsertar = productoAlmacenExistente.Where(p => !productosDetalles.Contains(p.v_ProductoDetalleId)).Select(o => o.v_ProductoDetalleId).ToList();
                        if (productosInsertar.Any())
                        {
                            // Globals.ProgressbarStatus.i_TotalProgress = productosInsertar.Count;
                            foreach (var item in productosInsertar)
                            {
                                var productoAlmacenExistent = productoAlmacenExistente.Any(l => l.v_ProductoDetalleId == item);
                                if (!productoAlmacenExistent)
                                {
                                    var productoalmacen = new productoalmacen();
                                    var intNodeId = int.Parse(ClientSession[0]);
                                    var SecuentialId = GetNextSecuentialId(intNodeId, 30);
                                    var newIdProductoAlmacen = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PA");
                                    productoalmacen.v_IdProductoAlmacen = newIdProductoAlmacen;
                                    productoalmacen.i_IdAlmacen = Almacen;
                                    productoalmacen.v_NroPedido = null;
                                    productoalmacen.v_ProductoDetalleId = item;
                                    productoalmacen.t_InsertaFecha = DateTime.Now;
                                    productoalmacen.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                    productoalmacen.i_Eliminado = 0;
                                    productoalmacen.d_StockMinimo = 0;
                                    productoalmacen.d_StockMaximo = 0;
                                    productoalmacen.d_StockActual = 0;
                                    productoalmacen.d_SeparacionTotal = 0;
                                    productoalmacen.i_Eliminado = 0;
                                    productoalmacen.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                                    productoalmacen.t_InsertaFecha = DateTime.Now;
                                    productoalmacen.v_Periodo = periodo;
                                    dbContext.AddToproductoalmacen(productoalmacen);

                                    //Globals.ProgressbarStatus.i_Progress++;
                                }
                                else
                                {
                                    //Globals.ProgressbarStatus.i_Progress++;
                                }
                            }
                        }
                        else
                        {
                            // Globals.ProgressbarStatus.i_TotalProgress = productosDetalles.Count;
                            foreach (var item in productosDetalles)
                            {
                                var productoAlmacenExistent = productoAlmacenExistente.Any(l => l.v_ProductoDetalleId == item);
                                if (!productoAlmacenExistent)
                                {
                                    var productoalmacen = new productoalmacen();
                                    var intNodeId = int.Parse(ClientSession[0]);
                                    var SecuentialId = GetNextSecuentialId(intNodeId, 30);
                                    var newIdProductoAlmacen = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PA");
                                    productoalmacen.v_IdProductoAlmacen = newIdProductoAlmacen;
                                    productoalmacen.i_IdAlmacen = Almacen;
                                    productoalmacen.v_NroPedido = null;
                                    productoalmacen.v_ProductoDetalleId = item;
                                    productoalmacen.t_InsertaFecha = DateTime.Now;
                                    productoalmacen.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                    productoalmacen.i_Eliminado = 0;
                                    productoalmacen.d_StockMinimo = 0;
                                    productoalmacen.d_StockMaximo = 0;
                                    productoalmacen.d_StockActual = 0;
                                    productoalmacen.d_SeparacionTotal = 0;
                                    productoalmacen.i_Eliminado = 0;
                                    productoalmacen.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                                    productoalmacen.t_InsertaFecha = DateTime.Now;
                                    productoalmacen.v_Periodo = periodo;
                                    dbContext.AddToproductoalmacen(productoalmacen);

                                    //Globals.ProgressbarStatus.i_Progress++;
                                }
                            }
                        }

                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    objOperationResult.Success = 0;
                    objOperationResult.AdditionalInformation = "AperturaData.InscribirProductosEnAlmacen()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    objOperationResult.ErrorMessage = ex.Message;
                    objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                }
            }

            /// <summary>
            /// Inicia el proceso de apertura de datos, necesario para separar la información por periodos.
            /// </summary>
            /// <param name="pobjOperationResult"></param>
            /// <param name="periodoOrigen"></param>
            /// <param name="periodoDestino"></param>
            /// <param name="lineas"></param>
            /// <param name="plancuenta"></param>
            /// <param name="conceptos"></param>
            /// <param name="productoalmacen"></param>
            /// <param name="planillas"></param>
            /// <param name="destinos"></param>
            public static void AperturarCuentasPorPeriodo(ref OperationResult pobjOperationResult, string periodoOrigen, string periodoDestino, bool lineas = true, bool plancuenta = true, bool conceptos = true, bool productoalmacen = true, bool planillas = true, bool destinos = true)
            {
                try
                {
                    if (lineas)
                    {
                        InicializaLineas(ref pobjOperationResult, periodoDestino);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }

                    if (plancuenta)
                    {
                        MigrarPlanCuentasPeriodo(ref pobjOperationResult, periodoOrigen, periodoDestino);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }

                    if (conceptos)
                    {
                        InicializaConceptos(ref pobjOperationResult, periodoOrigen, periodoDestino);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }

                    if (productoalmacen)
                    {
                        InicializarProductoAlmacen(ref pobjOperationResult, periodoDestino);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }

                    if (planillas)
                    {
                        InicializarRelacionesPlanilla(ref pobjOperationResult, periodoOrigen, periodoDestino);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }

                    if (destinos)
                    {
                        InicializarDestinos(ref pobjOperationResult, periodoOrigen, periodoDestino);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }

                    pobjOperationResult.Success = 1;
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    throw;
                }
            }

            /// <summary>
            /// Copia las relaciones contables de los conceptos de planilla de un año a otro año. 
            /// </summary>
            /// <param name="objOperationResult"></param>
            /// <param name="periodoOrigen"></param>
            /// <param name="periodoDestino"></param>
            public static void InicializarRelacionesPlanilla(ref OperationResult objOperationResult, string periodoOrigen, string periodoDestino)
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Relaciones anterior

                        var planillarelaciondescuentosAnterior = dbContext.planillarelaciondescuentos.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToList();

                        var planillarelacionesaportacionesAnterior = dbContext.planillarelacionesaportaciones.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToList();

                        var planillarelacionesdescuentosafpAnterior = dbContext.planillarelacionesdescuentosafp.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToList();

                        var planillarelacionesnetopagarAnterior = dbContext.planillarelacionesnetopagar.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToList();

                        var planillarelacioningresosAnterior = dbContext.planillarelacioningresos.Where(p => p.v_Periodo.Equals(periodoOrigen) && p.i_Eliminado == 0).ToList();

                        #endregion

                        #region Relaciones eliminar anteriores

                        var planillarelaciondescuentosEliminar = dbContext.planillarelaciondescuentos.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();

                        var planillarelacionesaportacionesEliminar = dbContext.planillarelacionesaportaciones.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();

                        var planillarelacionesdescuentosafpEliminar = dbContext.planillarelacionesdescuentosafp.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();

                        var planillarelacionesnetopagarEliminar = dbContext.planillarelacionesnetopagar.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();

                        var planillarelacioningresosEliminar = dbContext.planillarelacioningresos.Where(p => p.v_Periodo.Equals(periodoDestino) && p.i_Eliminado == 0).ToList();

                        planillarelaciondescuentosEliminar.ForEach(o => dbContext.DeleteObject(o));
                        planillarelacionesaportacionesEliminar.ForEach(o => dbContext.DeleteObject(o));
                        planillarelacionesdescuentosafpEliminar.ForEach(o => dbContext.DeleteObject(o));
                        planillarelacionesnetopagarEliminar.ForEach(o => dbContext.DeleteObject(o));
                        planillarelacioningresosEliminar.ForEach(o => dbContext.DeleteObject(o));
                        dbContext.SaveChanges();

                        #endregion

                        #region Inserta las relaciones del nuevo periodo

                        planillarelaciondescuentosAnterior.ForEach(o =>
                        {
                            var oo = new planillarelaciondescuentos
                            {
                                i_Eliminado = 0,
                                i_IdTipoPlanilla = o.i_IdTipoPlanilla,
                                v_IdConceptoPlanilla = o.v_IdConceptoPlanilla,
                                v_NroCuenta = o.v_NroCuenta,
                                v_Periodo = periodoDestino,
                                i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                t_InsertaFecha = DateTime.Now,
                                t_ActualizaFecha = null,
                                i_ActualizaIdUsuario = null,
                            };
                            dbContext.planillarelaciondescuentos.AddObject(oo);
                        });

                        planillarelacionesaportacionesAnterior.ForEach(o =>
                        {
                            var oo = new planillarelacionesaportaciones
                            {
                                i_Eliminado = 0,
                                i_IdTipoPlanilla = o.i_IdTipoPlanilla,
                                v_IdConceptoPlanilla = o.v_IdConceptoPlanilla,
                                v_NroCuenta_A = o.v_NroCuenta_A,
                                v_NroCuenta_B = o.v_NroCuenta_B,
                                v_Periodo = periodoDestino,
                                i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                t_InsertaFecha = DateTime.Now,
                                t_ActualizaFecha = null,
                                i_ActualizaIdUsuario = null,
                                i_IdCentroCosto = o.i_IdCentroCosto
                            };
                            dbContext.planillarelacionesaportaciones.AddObject(oo);
                        });
                        planillarelacionesdescuentosafpAnterior.ForEach(o =>
                        {
                            var oo = new planillarelacionesdescuentosafp
                            {
                                i_Eliminado = 0,
                                v_IdConceptoPlanilla = o.v_IdConceptoPlanilla,
                                v_NroCuenta = o.v_NroCuenta,
                                i_IdRegimenPensionario = o.i_IdRegimenPensionario,
                                v_Periodo = periodoDestino,
                                i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                t_InsertaFecha = DateTime.Now,
                                t_ActualizaFecha = null,
                                i_ActualizaIdUsuario = null,
                            };
                            dbContext.planillarelacionesdescuentosafp.AddObject(oo);
                        });
                        planillarelacionesnetopagarAnterior.ForEach(o =>
                        {
                            var oo = new planillarelacionesnetopagar
                            {
                                i_Eliminado = 0,
                                v_NroCuenta = o.v_NroCuenta,
                                v_Periodo = periodoDestino,
                                i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                t_InsertaFecha = DateTime.Now,
                                t_ActualizaFecha = null,
                                i_ActualizaIdUsuario = null,
                                i_IdTipoPlanilla = o.i_IdTipoPlanilla
                            };
                            dbContext.planillarelacionesnetopagar.AddObject(oo);
                        });
                        planillarelacioningresosAnterior.ForEach(o =>
                        {
                            var oo = new planillarelacioningresos
                            {
                                i_Eliminado = 0,
                                v_NroCuenta = o.v_NroCuenta,
                                v_Periodo = periodoDestino,
                                i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                t_InsertaFecha = DateTime.Now,
                                t_ActualizaFecha = null,
                                i_ActualizaIdUsuario = null,
                                i_IdCentroCosto = o.i_IdCentroCosto,
                                i_IdTipoPlanilla = o.i_IdTipoPlanilla,
                                v_IdConceptoPlanilla = o.v_IdConceptoPlanilla
                            };
                            dbContext.planillarelacioningresos.AddObject(oo);
                        });

                        #endregion

                        dbContext.SaveChanges();
                        objOperationResult.Success = 1;
                    }
                }
                catch (Exception ex)
                {
                    objOperationResult.Success = 0;
                    objOperationResult.AdditionalInformation = "UtilsBL.InicializarRelacionesPlanilla()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    objOperationResult.ErrorMessage = ex.Message;
                    objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                }
            }
        }
    }

    // falta definir el almacenamiento de la session en windows y para web
    public static class Globals
    {
        /// <summary>
        /// Variable global que se usa para almacenar los datos del Cliente.
        /// </summary>
        public static ClientSession ClientSession { get; set; }

        /// <summary>
        /// Variable globar que se usa para almacenar los datos del progressbar usado en frmMaster.
        /// </summary>
        public static ProgressbarStatus ProgressbarStatus { get; set; }

        /// <summary>
        /// Contiene informacion relevante para la facturacion electronica
        /// </summary>
        public static FacturacionData FacturacionInfo { get; set; }

        /// <summary>
        /// Temporal que se usa para almacenar los datos de los combos mas usados en los formularios de ventas.
        /// </summary>
        public static CacheCombosVentaDto CacheCombosVentaDto { get; set; }

        public static List<int> ListaDocumentosContable { get; set; }
        public static List<int> ListaDocumentosInversos { get; set; }
        public static List<string> UsuariosContables { get; set; }
        public static List<establecimientodetalleDto> ListaEstablecimientoDetalle { get; set; }
        public static TipoMotorBD TipoMotor { get; set; }
        public static string CadenaConexion { get; set; }
        public static bool ActualizadoStocks { get; set; }
    }

    public class TransactionUtils
    {
        /// <summary>
        /// Crea una nueva instancia de un bloque Transaccional con las configuraciones óptimas para su uso.
        /// </summary>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.RepeatableRead,
                Timeout = TransactionManager.MaximumTimeout
            };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        /// <summary>
        /// Metodo que sirve para cambiar el tiempo limite de las transacciones en el machine.config
        /// </summary>
        /// <param name="timeOut"></param>
        public static void OverrideTransactionScopeMaximumTimeout(TimeSpan timeOut)
        {
            var oSystemType = typeof(TransactionManager);
            System.Reflection.FieldInfo oCachedMaxTimeout = oSystemType.GetField(@"_cachedMaxTimeout", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Reflection.FieldInfo oMaximumTimeout = oSystemType.GetField(@"_maximumTimeout", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (oCachedMaxTimeout != null) oCachedMaxTimeout.SetValue(null, true);
            if (oMaximumTimeout != null) oMaximumTimeout.SetValue(null, timeOut);
        }
    }

    /// <summary>
    /// Define un par clave/valor que se puede establecer o recuperar.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la clave.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    [Serializable]
    public class KeyValuePar<TKey, TValue>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la estructura System.Collections.Generic.KeyValuePair<TKey,TValue>
        /// con la clave y valor especificados.
        /// </summary>
        /// <param name="key">Objeto definido en cada par clave/valor.</param>
        /// <param name="value">Definición asociada a key.</param>
        public KeyValuePar(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Obtiene la clave del par clave/valor.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// Obtiene el valor del par clave/valor.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Devuelve una representación de cadena de la estructura System.Collections.Generic.KeyValuePair,
        /// utilizando las representaciones de cadena de la clave y el valor.
        /// </summary>
        /// <returns>Representación de cadena de la estructura System.Collections.Generic.KeyValuePair,
        /// que incluye las representaciones de cadena de la clave y el valor.</returns>
        public override string ToString()
        {
            return Key.ToString() + "-" + Value.ToString();
        }
    }

    public class FacturacionData
    {
        /// <summary>
        /// Id el Grupo de Unidades de Medida Internacionales
        /// </summary>
        public int GroupUndInter { get; set; }

        /// <summary>
        /// Identifica el Tipo de Servicio
        /// <para>
        /// Beta = 0,
        /// Produccion = 1,
        /// Homologacion = 2,
        /// </para>
        /// </summary>
        public int TipoServicio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FacturacionData"/> is automatic.
        /// </summary>
        /// <value><c>true</c> if automatic; otherwise, <c>false</c>.</value>
        public bool Automatic { get; set; }
    }
}