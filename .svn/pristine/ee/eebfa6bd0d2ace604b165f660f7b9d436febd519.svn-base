using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System.Reflection;

namespace SAMBHS.Windows.WinServer.UI
{
    public partial class frmMaster : Form
    {
        readonly SecurityBL _objSecurityBL = new SecurityBL();
        List<AutorizationList> objAuthorizationList = new List<AutorizationList>();

        public frmMaster()
        {
            InitializeComponent();
        }

        private void frmMaster_Load(object sender, EventArgs e)
        {
            CargarMenus();
        }

        private void CargarMenus()
        {
            try
            {
                OperationResult objOperationResult2 = new OperationResult();
                objAuthorizationList = _objSecurityBL.GetAuthorizationNodeWeb(ref objOperationResult2, int.Parse(Globals.ClientSession.i_RoleId.ToString()));
                objAuthorizationList = objAuthorizationList.FindAll(p => p.I_ParentId != -1);
                {
                    Globals.ClientSession.v_RoleName = objAuthorizationList[0].V_RoleName;
                    var aa = 0;
                    var zzz = objAuthorizationList.FindAll(p => p.I_ApplicationHierarchyTypeId == 1);
                    var c = 0;
                    foreach (var menuPadre in zzz)
                    {
                        // Se cargan las etiquetas de los RibbonTabs
                        Infragistics.Win.Appearance appearance = new Infragistics.Win.Appearance();
                        var ribbonTab = new RibbonTab[1];
                        var ribbonGroup = new RibbonGroup[100];

                        ribbonTab[0] = new RibbonTab(menuPadre.I_ApplicationHierarchyId.ToString());
                        ribbonTab[0].Caption = menuPadre.V_Description;
                        ribbonTab[0].Tag = menuPadre.V_Form;
                        var Id = menuPadre.I_ApplicationHierarchyId;
                        var findResult = objAuthorizationList.FindAll(p => p.I_ParentId == menuPadre.I_ApplicationHierarchyId);
                        if (findResult.Count != 0)
                        {
                            // Se cargan los Agrupadores
                            var listaBotonesReportes = new List<ButtonTool>();
                            var listaBotonesMenu = new List<PopupMenuTool>();
                            PopupMenuTool popupMenuTool1 = new PopupMenuTool("BotonMenu" + c.ToString());

                            List<ButtonTool> ListaBotonesHijos = new List<ButtonTool>();
                            ribbonGroup[c] = new RibbonGroup(c.ToString());
                            ribbonGroup[c].Caption = @"ADMINISTRACIÓN";
                            int IdControl = menuPadre.I_ApplicationHierarchyId;
                            //Se cargan los botones dentro de los agrupadores
                            List<ButtonTool> ListaBotones = new List<ButtonTool>();

                            foreach (var Control in findResult)
                            {
                                var buttonTool = new ButtonTool("Boton")
                                {
                                    Key =
                                        !string.IsNullOrEmpty(Control.V_Form)
                                            ? Control.V_Form
                                            : Control.I_ApplicationHierarchyId.ToString(),
                                    Tag = Control.I_ApplicationHierarchyId.ToString()
                                };
                                buttonTool.SharedPropsInternal.Caption = Control.V_Description;
                                Infragistics.Win.Appearance apariencia = new Infragistics.Win.Appearance
                                {
                                    Image = ObtenerImagen(Control.V_Form)
                                };
                                buttonTool.SharedPropsInternal.AppearancesSmall.Appearance = apariencia;
                                buttonTool.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
                                ListaBotones.Add(buttonTool);
                            }

                            ribbonGroup[c].Tools.AddRange(ListaBotones.ToArray());
                            c++;
                            //Se agregan los agrupadores al RibbonTab

                            for (var i = 0; i < c; i++)
                            {
                                if (ribbonGroup[i] != null)
                                {
                                    ribbonTab[0].Groups.Add(ribbonGroup[i]);
                                }
                            }

                            try
                            {
                                ultraToolbarsManager1.Tools.AddRange(listaBotonesReportes.ToArray());
                                ultraToolbarsManager1.Tools.AddRange(ListaBotonesHijos.ToArray());
                                ultraToolbarsManager1.Ribbon.Tabs.Add(ribbonTab[0]);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        public Image ObtenerImagen(string formulario)
        {
            try
            {
                Image imagen;
                switch (formulario)
                {
                    case "SAMBHS.Windows.WinServer.UI.Administracion.frmMenus.N":
                        imagen = Image.FromFile(@"img\binary-tree128.png");
                        break;

                    case "SAMBHS.Windows.WinServer.UI.Administracion.frmBandejaRoles.N":
                        imagen = Image.FromFile(@"img\User-Role-Reviewer-icon.png");
                        break;

                    case "SAMBHS.Windows.WinServer.UI.Administracion.frmBandejaUsuarios.N":
                        imagen = Image.FromFile(@"img\1448780328_user-group.png");
                        break;

                    case "SAMBHS.Windows.WinServer.UI.Administracion.frmBandejaEmpresas.N":
                        imagen = Image.FromFile(@"img\grid_09-128.png");
                        break;

                    case "SAMBHS.Windows.WinServer.UI.Administracion.frmMantenimientoLicencias.N":
                        imagen = Image.FromFile(@"img\diploma.png");
                        break;

                    default:
                        imagen = null;
                        break;
                }

                return imagen;
            }

            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return null;
            }



        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            try
            {
                if (e.Tool.GetType() == typeof(ButtonTool))
                {
                    var valorNombre = ((ButtonTool)e.Tool).Key.ToString();
                    var nombreFormulario = ((ButtonTool)e.Tool).Key.ToString().Substring(0, valorNombre.Length - 2);
                    var parametro = valorNombre.Substring(valorNombre.Length - 1, 1);

                    Assembly asm = Assembly.GetEntryAssembly();
                    Type formtype = asm.GetType(nombreFormulario);

                    if (formtype == null) return;
                    if (System.Windows.Forms.Application.OpenForms[formtype.Name] == null)
                    {
                        Form f = (Form)Activator.CreateInstance(formtype, parametro);
                        f.MdiParent = this;
                        Bitmap bitmap = new Bitmap(ObtenerImagen(valorNombre));
                        bitmap.SetResolution(72, 72);
                        f.Icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
                        f.Show();
                    }
                    else
                    {
                        System.Windows.Forms.Application.OpenForms[formtype.Name].Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }
    }
}
