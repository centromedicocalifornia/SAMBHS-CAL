using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System.Reflection;
using Infragistics.Win.UltraWinToolbars;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using System.Deployment.Application;
using System.Diagnostics;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Almacen.BL;
using System.Net.NetworkInformation;
using SAMBHS.Compra.BL;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinTabs;
using CrystalDecisions.CrystalReports.Engine;
using Infragistics.Win.UltraWinStatusBar;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI
{
    public partial class frmMaster : Form
    {
        public delegate void EstadoCarga(string estado);
        public EventHandler CargaFinalizada;
        public event EstadoCarga EstadoCargaEvent;

        #region Fields
        private readonly string _clientVersion;
        private string _dataSource;
        public bool ReinicioRequerido;
        private readonly Form _frmLogin;
        #endregion

        #region Construct
        public frmMaster(Form frmLogin, string version, string dataSource)
        {
            _frmLogin = frmLogin;
            InitializeComponent();
            _clientVersion = version;
            _dataSource = dataSource;
        }
        #endregion

        private void frmMaster_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (Globals.ClientSession.i_IdEstablecimiento == null || Globals.ClientSession.i_IdAlmacenPredeterminado == null)
            {
                var f = new frmConfigurarEstablecimientoAlmacen();
                f.ShowDialog();
            }
            else
            {
                var st = new EstablecimientoBL().Getestablecimiento(ref objOperationResult, Globals.ClientSession.i_IdEstablecimiento.Value);
                if (st == null)
                {
                    var f = new frmConfigurarEstablecimientoAlmacen();
                    f.ShowDialog();
                }
                else
                {
                    SetEstablecimiento(st.v_Nombre);
                    if (new NodeWarehouseBL().ObtenerAlmacen(ref objOperationResult, (int)Globals.ClientSession.i_IdAlmacenPredeterminado.Value) == null)
                    {
                        var f = new frmConfigurarEstablecimientoAlmacen();
                        f.ShowDialog();
                    }
                }
            }

            var tipoCambio = new ComprasBL().DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Now.Date);
            if (Globals.ClientSession.UsuarioEsContable == 1 &&
                (string.IsNullOrEmpty(tipoCambio) || decimal.Parse(tipoCambio) == 0))
            {
                var f = new FrmIngresarTipoCambio();
                f.ShowDialog();
            }
        }

        public void InicializarCarga()
        {
            Task.Factory.StartNew(() =>
            {
                if (EstadoCargaEvent != null)
                    EstadoCargaEvent("Preparando...");
                CargarCR();
            }, TaskCreationOptions.AttachedToParent).ContinueWith(t1 =>
            {
                if (EstadoCargaEvent != null)
                    EstadoCargaEvent("Cargando Menús...");

                _frmLogin.Invoke((MethodInvoker)
                    delegate
                    {
                        ultraToolbarsManager1.Tools["lblPeriodo"].SharedProps.Caption = string.Format("Periodo: {0}",
                            Globals.ClientSession.i_Periodo ?? 0);
                        pNotificationBar.Visible = false;
                        statusStrip1.Appearance.ForeColor = Color.Black;
                        statusStrip1.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                        statusStrip1.Appearance.BackColor = Color.FromArgb(195, 195, 195);
                        statusStrip1.Appearance.BackColor2 = Color.FromArgb(195, 195, 195);
                        ultraTabbedMdiManager1.TabGroupSettings.CloseButtonLocation = TabCloseButtonLocation.Tab;
                        CargarMenus();
                    });
            }, TaskContinuationOptions.AttachedToParent).ContinueWith(t2 =>
            {
                if (EstadoCargaEvent != null)
                    EstadoCargaEvent("Iniciando...");

                _frmLogin.Invoke((MethodInvoker)
                    delegate
                    {
                        statusStrip1.Panels[0].Text = string.Format("Client Version: {0}", _clientVersion);
                        statusStrip1.Panels[1].Text = @"Nodo: " + Globals.ClientSession.v_CurrentExecutionNodeName + string.Format(" [{0}]", Globals.ClientSession.v_RucEmpresa);
                        statusStrip1.Panels[2].Text = @"Usuario: " + Globals.ClientSession.v_UserName;
                        statusStrip1.Panels[3].Text = @"Rol: " + Globals.ClientSession.v_RoleName;

                        if (_dataSource.Contains(@"\"))
                        {
                            int indexSlash = _dataSource.IndexOf(@"\", StringComparison.Ordinal);
                            _dataSource = _dataSource.Substring(0, indexSlash);
                        }
                        bwkBuscaActualizaciones.RunWorkerAsync();
                        bwkLlenaCacheCombos.RunWorkerAsync();
                        if (CargaFinalizada != null)
                            CargaFinalizada(this, new EventArgs { });
                    });
            });
        }

        private void CargarMenus()
        {
            try
            {
                var objOperationResult2 = new OperationResult();
                var objAuthorizationList = new SecurityBL().GetAuthorizationNode(ref objOperationResult2, int.Parse(Globals.ClientSession.i_RoleId.ToString()));
                var xxx = objAuthorizationList.GroupBy(p => p.I_ApplicationHierarchyId);
                if (objOperationResult2.Success == 0)
                {
                    UltraMessageBox.Show(objOperationResult2.ExceptionMessage, "Error Carga de Menu", Icono: MessageBoxIcon.Error);
                    return;
                }
                objAuthorizationList = objAuthorizationList.Where(p => p.I_ParentId != -1).ToList();
                if (objAuthorizationList.Any())
                {
                    Globals.ClientSession.v_RoleName = objAuthorizationList[0].V_RoleName;
                    var zzz = objAuthorizationList.Where(p => p.I_ApplicationHierarchyTypeId == 5);
                    var c = 0;
                    foreach (var menuPadre in zzz)
                    {
                        // Se cargan las etiquetas de los RibbonTabs
                        var appearance = new Infragistics.Win.Appearance();
                        var ribbonTab = new RibbonTab[1];
                        var ribbonGroup = new RibbonGroup[100];
                        ribbonTab[0] = new RibbonTab(menuPadre.I_ApplicationHierarchyId.ToString())
                        {
                            Caption = menuPadre.V_Description,
                            Tag = menuPadre.V_Form
                        };

                        var Id = menuPadre.I_ApplicationHierarchyId;
                        var FindResult = objAuthorizationList.Where(p => p.I_ParentId == menuPadre.I_ApplicationHierarchyId);
                        if (!FindResult.Any()) continue;
                        {
                            // Se cargan los Agrupadores
                            var ListaBotonesReportes = new List<ButtonTool>();
                            var ListaBotonesMenu = new List<PopupMenuTool>();
                            var popupMenuTool1 = new PopupMenuTool("BotonMenu" + c);

                            var ListaBotonesHijos = new List<ButtonTool>();
                            foreach (var Menu in objAuthorizationList.Where(p => p.I_ParentId == Id))
                            {
                                ribbonGroup[c] = new RibbonGroup(c.ToString()) { Caption = Menu.V_Description };
                                var IdControl = Menu.I_ApplicationHierarchyId;
                                //Se cargan los botones dentro de los agrupadores
                                var ListaBotones = new List<ButtonTool>();

                                foreach (var Control in objAuthorizationList.Where(p => p.I_ParentId == IdControl))
                                {
                                    if (objAuthorizationList.Count(p => p.I_ParentId == Control.I_ApplicationHierarchyId) != 0 && Menu.V_Description == "Reportes")
                                    {
                                        foreach (var ControlHijo in objAuthorizationList.Where(p => p.I_ParentId == Control.I_ApplicationHierarchyId))
                                        {
                                            //if (string.IsNullOrWhiteSpace(ControlHijo.V_Form)) continue;
                                            var buttonToolHijo = new ButtonTool("Boton")
                                            {
                                                Key =
                                                    !string.IsNullOrEmpty(ControlHijo.V_Form)
                                                        ? ControlHijo.V_Form
                                                        : ControlHijo.I_ApplicationHierarchyId.ToString(),
                                                Tag = ControlHijo.I_ParentId.ToString()
                                            };
                                            buttonToolHijo.SharedPropsInternal.Caption = ControlHijo.V_Description;
                                            var Apariencia2 = new Infragistics.Win.Appearance
                                            {
                                                Image = ObtenerImagen(ControlHijo.V_Form)
                                            };
                                            //buttonToolHijo.InstanceProps.IsFirstInGroup = Control.v_Code.StartsWith("|") ? true : false;
                                            buttonToolHijo.SharedPropsInternal.AppearancesSmall.Appearance = Apariencia2;
                                            buttonToolHijo.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Default;
                                            ListaBotonesHijos.Add(buttonToolHijo);
                                        }
                                    }

                                    var buttonTool = new ButtonTool("Boton")
                                    {
                                        Key =
                                            !string.IsNullOrEmpty(Control.V_Form)
                                                ? Control.V_Form
                                                : Control.I_ApplicationHierarchyId.ToString(),
                                        Tag = Control.I_ApplicationHierarchyId.ToString()
                                    };
                                    buttonTool.SharedPropsInternal.Caption = Control.V_Description;
                                    var apariencia = new Infragistics.Win.Appearance
                                    {
                                        Image = ObtenerImagen(Control.V_Form)
                                    };
                                    buttonTool.SharedPropsInternal.AppearancesSmall.Appearance = apariencia;
                                    //buttonTool.InstanceProps.IsFirstInGroup = Control.v_Code.StartsWith("|") ? true : false;
                                    if (Menu.V_Description == "Reportes")
                                    {
                                        buttonTool.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Default;
                                        ListaBotonesReportes.Add(buttonTool);
                                    }
                                    else
                                    {
                                        buttonTool.InstanceProps.PreferredSizeOnRibbon = Menu.V_Description == "Utilitarios" ? RibbonToolSize.Normal : RibbonToolSize.Large;
                                        ListaBotones.Add(buttonTool);
                                    }
                                }

                                //Se crea el boton menu para contner a todos los reportes
                                if (ListaBotonesReportes.Any() && Menu.V_Description == "Reportes")
                                {
                                    popupMenuTool1.AllowTearaway = true;
                                    popupMenuTool1.SharedProps.Caption = @"Ver";
                                    popupMenuTool1.InstanceProps.PreferredSizeOnRibbon = Infragistics.Win.UltraWinToolbars.RibbonToolSize.Large;
                                    var apariencia = new Infragistics.Win.Appearance
                                    {
                                        Image = Image.FromFile(@"img\documents.png")
                                    };
                                    popupMenuTool1.SharedPropsInternal.AppearancesLarge.Appearance = apariencia;
                                    ListaBotonesMenu.Add(popupMenuTool1);
                                    ribbonGroup[c].Tools.AddRange(ListaBotonesMenu.ToArray());
                                }
                                else
                                {
                                    ribbonGroup[c].Tools.AddRange(ListaBotones.ToArray());
                                }

                                c++;
                            }
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
                                ultraToolbarsManager1.Tools.AddRange(ListaBotonesReportes.ToArray());
                                ultraToolbarsManager1.Tools.AddRange(ListaBotonesHijos.ToArray());
                                ultraToolbarsManager1.Ribbon.Tabs.Add(ribbonTab[0]);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                            //se verifica que si un boton hijo pertenece a un boton menu entonces en lugar de agregar el boton agrega un boton menu conteniendo a los botones hijos.
                            foreach (var p in ListaBotonesReportes)
                            {
                                if (!ListaBotonesHijos.Select(o => o.Tag).ToList().Contains(p.Tag))
                                {
                                    popupMenuTool1.Tools.AddToolRange(new string[] { p.Key });
                                }
                                else
                                {
                                    PopupMenuTool popupMenuTool2 = new PopupMenuTool(p.Key + "--");
                                    ultraToolbarsManager1.Tools.Add(popupMenuTool2);
                                    popupMenuTool2.AllowTearaway = true;
                                    popupMenuTool2.SharedProps.Caption = p.SharedProps.Caption;
                                    popupMenuTool2.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Normal;

                                    foreach (var pp in ListaBotonesHijos.Where(o => o.Tag.ToString() == p.Tag.ToString()).ToList())
                                    {
                                        popupMenuTool2.Tools.AddToolRange(new[] { pp.Key });
                                    }

                                    popupMenuTool1.Tools.AddToolRange(new[] { popupMenuTool2.Key });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @"\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            try
            {
                if (e.Tool.GetType() == typeof(ButtonTool))
                {
                    OpenFormulario(((ButtonTool)e.Tool).Key);
                }

              
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }

        }

        private void OpenFormulario(string key)
        {
            var valorNombre = key;
            var nombreFormulario = key.Substring(0, valorNombre.Length - 2);
            var parametro = valorNombre.Substring(valorNombre.Length - 1, 1);

            var asm = Assembly.GetEntryAssembly();
            var formtype = asm.GetType(nombreFormulario);

            if (formtype == null) return;
            var form = Application.OpenForms[formtype.Name];
            if (form == null)
            {
                var f = (Form)Activator.CreateInstance(formtype, parametro);
                f.MdiParent = this;
                var bitmap = new Bitmap(ObtenerImagen(valorNombre));
                bitmap.SetResolution(72, 72);
                f.Icon = Icon.FromHandle(bitmap.GetHicon());
                f.Show();
            }
            else
                form.Activate();
        }

        public Image ObtenerImagen(string formulario)
        {
            try
            {
                Image imagen;
                switch (formulario)
                {
                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmParametrosUsuario.N":
                        imagen = Image.FromFile(@"img\administrative_tools.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmTransportista.N":
                        imagen = Image.FromFile(@"img\lorrygreen.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCondicionPago.V":
                        imagen = Image.FromFile(@"img\secure_payment.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCondicionPago.C":
                        imagen = Image.FromFile(@"img\secure_payment.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmAgenciaTransporte.N":
                        imagen = Image.FromFile(@"img\office_building.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmProducto.N":
                        imagen = Image.FromFile(@"img\product.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCliente.C":
                        imagen = Image.FromFile(@"img\customers.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmVendedor.N":
                        imagen = Image.FromFile(@"img\male.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaNotaIngreso.N":
                        imagen = Image.FromFile(@"img\Ingreso.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaNotaSalida.N":
                        imagen = Image.FromFile(@"img\Salida.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaTransferencia.N":
                        imagen = Image.FromFile(@"img\Transferencia.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmTipoCambio.N":
                        imagen = Image.FromFile(@"img\exchange.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCondicionPago":
                        imagen = Image.FromFile(@"img\secure_payment.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCliente.V":
                        imagen = Image.FromFile(@"img\suppliers.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmGastoImportacion.N":
                        imagen = Image.FromFile(@"img\money_transportation.png");
                        break;



                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmDocumentos.N":
                        imagen = Image.FromFile(@"img\rich_text_format.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaRegistroCompra.N":
                        imagen = Image.FromFile(@"img\medical_invoice_information.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmPlanCuentas.N":
                        imagen = Image.FromFile(@"img\cuentas.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCuentasMayor.N":
                        imagen = Image.FromFile(@"img\lightbrown_documents.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmConcepto.N":
                        imagen = Image.FromFile(@"img\notebook.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmAdministracionConcepto.N":
                        imagen = Image.FromFile(@"img\briefcase.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaReciboHonoarios.N":
                        imagen = Image.FromFile(@"img\recibohonorario.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaRegistroVenta.N":
                        imagen = Image.FromFile(@"img\my_documents.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaGuiaRemision.N":
                        imagen = Image.FromFile(@"img\GuiaRemision2.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaGuiaRemision.R":
                        imagen = Image.FromFile(@"img\GuiaRemision2.png");

                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmGuiasVentaPendientes.N":
                        imagen = Image.FromFile(@"img\GuiaRemisionPendientes.png");

                        break;


                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmConfiguracionEmpresa.N":
                        imagen = Image.FromFile(@"img\panel_setting.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaPedido.N":
                        imagen = Image.FromFile(@"img\pedido.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaCobranza.N":
                        imagen = Image.FromFile(@"img\cobranza.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmEstablecimiento.N":
                        imagen = Image.FromFile(@"img\configration_setting.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBuscarPedido.N":
                        imagen = Image.FromFile(@"img\buscapedido.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmBandejaListaPrecios.N":
                        imagen = Image.FromFile(@"img\price_tag.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmConsultaProductostock.N":
                        imagen = Image.FromFile(@"img\stock_ticker.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmBandejaAdelanto.N":
                        imagen = Image.FromFile(@"img\bills_diskette.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaImportaciones.N":
                        imagen = Image.FromFile(@"img\travel.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaImportaciones.C":
                        imagen = Image.FromFile(@"img\travel.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmBandejaRegistroCompraContabilidad.N":
                        imagen = Image.FromFile(@"img\medical_invoice_information.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmBandejaRegistroVentaContabilidad.N":
                        imagen = Image.FromFile(@"img\my_documents.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmBandejaRegistroTesoreria.N":
                        imagen = Image.FromFile(@"img\safety_box_info.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaGuiaRemisionCompras.N":
                        imagen = Image.FromFile(@"img\guiaremision.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.BandejaRegistroLibroDiario.N":
                        imagen = Image.FromFile(@"img\folder_home2.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmEditarUsuario.N":
                        imagen = Image.FromFile(@"img\edit_user.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmSaldoMensualBancos.N":

                        imagen = Image.FromFile(@"img\SaldoMensualBancos2.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmPendientesConciliacion.N":

                        imagen = Image.FromFile(@"img\pendienteconciliacion.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmBandejaDocumentosRetencion.N":
                        imagen = Image.FromFile(@"img\text_x_log.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmSaldoEstadoBancario.N":

                        imagen = Image.FromFile(@"img\SaldoEstadoBancario2.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmMovimientoEstadoBancario.N":
                        imagen = Image.FromFile(@"img\checkbox.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmCierreSaldosBancos.N":

                        imagen = Image.FromFile(@"img\CierreSaldosBancos.png");

                        break;

                    case "SAMBHS.Windows.WinClient.UI.Migraciones.frmMigraciones.N":
                        imagen = Image.FromFile(@"img\copy2_up.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Migraciones.frmCargaInicial.M":
                        imagen = Image.FromFile(@"img\copy2_up.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Migraciones.frmMigrarClientesProveedores.N":
                        imagen = Image.FromFile(@"img\copy2_up.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones.frmMigracionVentas.N":
                        imagen = Image.FromFile(@"img\copy2_up.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones.frmMigrarTipoCambio.N":
                        imagen = Image.FromFile(@"img\copy2_up.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones.frmMigrarPlanCuentas.N":
                        imagen = Image.FromFile(@"img\copy2_up.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmRelacionesFormaPagoDocumentos.N":
                        imagen = Image.FromFile(@"img\credit_cards.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmPendientesPorCobrarPagar.N":
                        imagen = Image.FromFile(@"img\emblem_money.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmRecalcularCuentasPorCobrar.N":
                        imagen = Image.FromFile(@"img\database_refresh.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmLibroElectronicoCompras.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmLibroElectronicoVentas.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmAjusteDiferenciaCambio.N":
                        imagen = Image.FromFile(@"img\1438287393_document-dollar.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmReprocesarSaldosContables.N":
                        imagen = Image.FromFile(@"img\1438287263_current-work.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmRecalcularDestinos.N":
                        imagen = Image.FromFile(@"img\1438287670_process.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmCierreMensual.N":
                        imagen = Image.FromFile(@"img\Candado.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.ActivoFijo.frmCuentasInventarios.N":
                        imagen = Image.FromFile(@"img\CuentasInventario.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Letras.frmBandejaSaldosInicialesLetras.N":
                        imagen = Image.FromFile(@"img\1438287393_document-dollar.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Letras.frmBandejaCanjeLetras.N":
                        imagen = Image.FromFile(@"img\1439524697_conversion_2__.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Letras.frmBandejaMantenimientoLetras.N":
                        imagen = Image.FromFile(@"img\1439524501_blank-check-cheque.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.ActivoFijo.frmBandejaActivoFijo.N":
                        imagen = Image.FromFile(@"img\BandejaActivo.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.ActivoFijo.frmProcesoDepreciacion.N":
                        imagen = Image.FromFile(@"img\Depreciar.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmAsientoInventario.N":
                        imagen = Image.FromFile(@"img\AsientoInventario.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaPagos.N":
                        imagen = Image.FromFile(@"img\pago.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Letras.Letras_por_Pagar.frmBandejaCanjeLetrasPagar.N":
                        imagen = Image.FromFile(@"img\bank_finance_cash_dollar_purchase_money_transfer_buy-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Letras.Letras_por_Pagar.frmBandejaSaldosInicialesLetrasPagar.N":
                        imagen = Image.FromFile(@"img\CIB_Cash_online_icon.PNG");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Letras.Letras_por_Pagar.frmBandejaMantenimientoLetrasPagar.N":
                        imagen = Image.FromFile(@"img\paycheck.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmEmpleado.N":
                        imagen = Image.FromFile(@"img\Name_Tag-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmConceptos.N":
                        imagen = Image.FromFile(@"img\699643-icon-86-document-list-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmAplicacion.N":
                        imagen = Image.FromFile(@"img\Config-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Planillas.frmBandejaVariablesTrabajador.N":
                        imagen = Image.FromFile(@"img\Businessmen_Resume-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Planillas.frmCalculoPlanilla.N":
                        imagen = Image.FromFile(@"img\agile_workflow-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Planillas.frmCopiarPlanilla.N":
                        imagen = Image.FromFile(@"img\Copy-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmRelacionesContabilidad.N":
                        imagen = Image.FromFile(@"img\seo3-57-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Planillas.frmPlame.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Planillas.frmGeneracionAsientos.N":
                        imagen = Image.FromFile(@"img\29-128.png");
                        break;
                    case "SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmBandejaRegistroVenta.N":
                        imagen = Image.FromFile(@"img\my_documents.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Requerimientos.Manguifajas.frmConsultaProductostock.N":
                        imagen = Image.FromFile(@"img\stock_ticker.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmPdtLiquidacionCompra.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmRetencionIgvLiquidacionCompras.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmProcesoDAOT.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Reportes.Ventas.frmEstadisticaVentas.N":
                        imagen = Image.FromFile(@"img\diagram.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Reportes.Ventas.frmEstadisticaVentas_.N":
                        imagen = Image.FromFile(@"img\diagram.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmCentroCosto.N":
                        imagen = Image.FromFile(@"img\part_to_a_whole-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmFormaDePago.N":
                        imagen = Image.FromFile(@"img\vector_210-252_13-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaOrdenCompra.N":
                        imagen = Image.FromFile(@"img\basket7-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmActualizacionesSistema.N":
                        imagen = Image.FromFile(@"img\development.desktop-128.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmLibroElectronicoDiario.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmRecalcularSeparacionPedido.N":

                        imagen = Image.FromFile(@"img\1438287670_process.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmRecalculoStock.N":
                        imagen = Image.FromFile(@"img\1438287670_process.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmDepositoMasivoDetracciones.N":
                        imagen = Image.FromFile(@"img\bills_diskette.png");
                        break;
                    case "SAMBHS.Windows.WinClient.UI.Procesos.Planillas.frmGenerarAFP.N":
                        imagen = Image.FromFile(@"img\afpnet.png");
                        break;
                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmReplicacionStatus.N":
                        imagen = Image.FromFile(@"img\database_replication.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaRegistroVentaElectronica.N":
                        imagen = Image.FromFile(@"img\laptop.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmFacturacionElectronica.N":
                        imagen = Image.FromFile(@"img\configuration.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmAlmacen.N":
                        imagen = Image.FromFile(@"img\line-segment.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmUnidadMedidaPLEValorizado.N":
                        imagen = Image.FromFile(@"img\carpentry.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.FrmExportarIrpes.N":
                        imagen = Image.FromFile(@"img\process.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmBandejaOrdenTrabajo.N":
                        imagen = Image.FromFile(@"img\folder.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmConsultaMovimientos.N":
                        imagen = Image.FromFile(@"img\hierarchical-structure.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmBandejaFormatoUnicoFacturacion.N":
                        imagen = Image.FromFile(@"img\order.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmProductoInventario.N":
                        imagen = Image.FromFile(@"img\TomaInventario.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmAsientoApertura.N":
                        imagen = Image.FromFile(@"img\open.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmAsientoCierre.N":
                        imagen = Image.FromFile(@"img\cierre.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.ActivoFijo.frmUbicacion.N":
                        imagen = Image.FromFile(@"img\placeholder.png"); break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmVentaDetalleAnexos.N":
                        imagen = Image.FromFile(@"img\AnexoVentaDetalla.png"); break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmConceptosCajaChica.N":
                        imagen = Image.FromFile(@"img\conceptoscajachica.png"); break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaCajaChica.N":
                        imagen = Image.FromFile(@"img\cajachica.png"); break;
                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmPlanCuentas2.N":
                        imagen = Image.FromFile(@"img\cuentas.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Procesos.Contabilidad.frmPDT.N":
                        imagen = Image.FromFile(@"img\ico_sunat.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmHorasExtras.N":
                        imagen = Image.FromFile(@"img\timeline.png");
                        break;
                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmConsultaVentasExportacion.N":
                        imagen = Image.FromFile(@"img\ConsultaVentasExportacion.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.FlujoEfectivo.frmMasterFlujoEfectivo.N":
                        imagen = Image.FromFile(@"img\coins.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmZonaCliente.N":
                       
                        imagen = Image.FromFile(@"img\earth-globe.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmConfiguracionBalanceGeneral.N":
                        imagen = Image.FromFile(@"img\configuracionBalances.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmRegistroAnexoContableBandeja.N":
                        imagen = Image.FromFile(@"img\team.png");
                        break;


                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmNotasEstadosSituacionFinanciera.N":
                        imagen = Image.FromFile(@"img\Notas.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmRelacionClienteUsuario.N":
                         imagen = Image.FromFile(@"img\ClientesUsuarios.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.LiquidacionCompra.frmBandejaRegistroLiquidacionCompraContabilidad.N":
                        imagen = Image.FromFile(@"img\BandejaLiquidacionCompra.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmDiasNoLaborables.N":
                        imagen = Image.FromFile(@"img\weekly-calendar.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmMantenimientoSemanas.N":
                        imagen = Image.FromFile(@"img\planning.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmBandejaTurnos.N":
                        imagen = Image.FromFile(@"img\calendar.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.Planilla.frmAsistenciaSemanal.N":
                        imagen = Image.FromFile(@"img\timing.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBandejaAgenda.N":
                        imagen = Image.FromFile(@"img\Agendar.png");
                        break;
                    case "SAMBHS.Windows.WinClient.UI.Procesos.frmBuscarServicios.N":
                        imagen = Image.FromFile(@"img\Servicios.png");
                        break;

                    case "SAMBHS.Windows.WinClient.UI.Mantenimientos.frmPacient.N":
                        imagen = Image.FromFile(@"img\Pacientes.png");
                        break;

                    default:
                        imagen = Image.FromFile(@"img\default_document.png");
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

        private void frmMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ReinicioRequerido)
            {
               
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void frmMaster_Resize(object sender, EventArgs e)
        {
            ResizeStatusPanels();
        }

        private void ResizeStatusPanels()
        {
            var w = Width;
            var paneles = statusStrip1.Panels.Cast<UltraStatusPanel>().Count(panel => panel.Visible);

            foreach (var panel in statusStrip1.Panels)
            {
                panel.Width = w / paneles;
            }

            statusStrip1.Height = WindowState == FormWindowState.Maximized ? 33 : 29;
        }

        private void bwkMasterProgressbar_DoWork(object sender, DoWorkEventArgs e)
        {
            var porc = 1;
            while (porc < 100 && !IsDisposed)
            {
                if (Globals.ProgressbarStatus.i_TotalProgress != 1 && Globals.ProgressbarStatus.i_TotalProgress != 0)
                {
                    porc = int.Parse(((Globals.ProgressbarStatus.i_Progress * 100) / Globals.ProgressbarStatus.i_TotalProgress).ToString());
                    if (porc != 0)
                    {
                        Invoke(new MethodInvoker(() => UpdateProgress(porc)));
                    }
                }
                if (Globals.ProgressbarStatus.b_Cancelado)
                {
                    break;
                }
            }
        }

        private void UpdateProgress(int percent)
        {
            try
            {
                statusStrip1.Panels[4].ProgressBarInfo.Value = percent;
            }
            catch
            {
                // ignored
            }
        }

        private void bwkMasterProgressbar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusStrip1.Panels[4].Visible = false;
            ResizeStatusPanels();
            statusStrip1.Panels[4].ProgressBarInfo.Value = 0;

            if (Globals.ProgressbarStatus.MensajeOutput != null) OutPutMessage(Globals.ProgressbarStatus.MensajeOutput);
        }

        public void ComenzarBackGroundProcess()
        {
            Globals.ProgressbarStatus.b_Cancelado = false;
            Globals.ProgressbarStatus.i_TotalProgress = 1;
            Globals.ProgressbarStatus.i_Progress = 1;
            statusStrip1.Panels[4].Visible = true;
            ResizeStatusPanels();
            bwkMasterProgressbar.RunWorkerAsync();
        }

        public bool IsBussy()
        {
            return bwkMasterProgressbar.IsBusy;
        }

        public void SetEstablecimiento(string descripcion)
        {
            var text = statusStrip1.Panels[1].Text;
            var partes = text.Split('-');
            statusStrip1.Panels[1].Text = string.Join(" - ", partes[0].TrimEnd(), descripcion);

        }

        #region Actualizacion ClickOnce
        private void bwkBuscaActualizaciones_DoWork(object sender, DoWorkEventArgs e)
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) == 1) // busca actualizaciones sólo si hay UNA instancia del sistema corriendo en la pc
            {
                var ping = new Ping();
                var result = ping.Send("198.50.230.132");
                if (result != null && result.Status == IPStatus.Success) // revisa primero si hay conexion con el servidor
                {
                    try
                    {
                        InstallUpdateSyncWithInfo(); // intenta actualizar la aplicacion sin abrir un tunel al servidor
                    }
                    catch
                    {
                        using (NetworkShareAccesser.Access("198.50.230.132", "tisoluciones", "abcABC123")) // intenta actualizar abriendo un tunel al servidor
                        {
                            InstallUpdateSyncWithInfo();
                        }
                    }
                }
            }
        }

        //Inicia la busqueda de actualizaciones
        private void InstallUpdateSyncWithInfo()
        {
            if (!ApplicationDeployment.IsNetworkDeployed) return;
            var ad = ApplicationDeployment.CurrentDeployment;

            var info = ad.CheckForDetailedUpdate();

            if (info.UpdateAvailable)
            {
                //si encuentra una actualizacion baja la nueva actualizacion desde el servidor
                UpdateApplication();
            }
            else
            {
                WorkerUpdate d = new WorkerUpdate(this.Invoke);
                d.UpdateAvailable += delegate
                {
                    UpdateApplication();
                };
                d.Start();
            }
        }

        private void UpdateApplication()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                ad.CheckForUpdateCompleted += ad_CheckForUpdateCompleted;
                ad.CheckForUpdateProgressChanged += ad_CheckForUpdateProgressChanged;

                ad.CheckForUpdateAsync();
            }
        }

        private void ad_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            lblDownloadNotification.Text = String.Format("Descargando Actualización... {0}. {1:D}K de {2:D}K descargados.", GetProgressString(e.State), e.BytesCompleted / 1024, e.BytesTotal / 1024);
        }

        private string GetProgressString(DeploymentProgressState state)
        {
            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "application files";
            }
            else if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "application manifest";
            }
            else
            {
                return "deployment manifest";
            }
        }

        private void ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                pbActualizacionImg.Image = Resource.cancel;
                lblDownloadNotification.Text = @"No se pudo Actualizar. Razón: \n" + e.Error.Message;
                return;
            }
            if (e.Cancelled)
            {
                MessageBox.Show(@"The update was cancelled.");
            }

            // Ask the user if they would like to update the application now. 
            if (e.UpdateAvailable)
            {
                BeginUpdate();
            }
        }

        private void BeginUpdate()
        {
            pNotificationBar.Visible = true;
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            ad.UpdateCompleted += ad_UpdateCompleted;

            // Indicate progress in the application's status bar.
            ad.UpdateProgressChanged += ad_UpdateProgressChanged;
            ad.UpdateAsync();
        }

        void ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            String progressText = String.Format("Descargando Actualización... {0:D}K de {1:D}K descargados - {2:D}% completado", e.BytesCompleted / 1024, e.BytesTotal / 1024, e.ProgressPercentage);
            lblDownloadNotification.Text = progressText;
        }

        void ad_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                pbActualizacionImg.Image = Resource.cancel;
                lblDownloadNotification.Text = @"The update of the application's latest version was cancelled.";
                return;
            }
            if (e.Error != null)
            {
                pbActualizacionImg.Image = Resource.cancel;
                lblDownloadNotification.Text = @"No se pudo actualizar. Razón:\n" + e.Error.Message;
                return;
            }
            pbActualizacionImg.Image = Resource.accept;
            lblDownloadNotification.Text = @"La Actualización finalizó. Los cambios se realizarán la próxima vez que reinicie el sistema.";
            linkCerrarBar.Visible = true;
            linkReiniciar.Visible = true;
            t_BuscaActualizaciones.Stop();
        }

        private void linkCerrarBar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pNotificationBar.Visible = false;
        }

        private void linkReiniciar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReinicioRequerido = true;
            Application.Restart();
        }

        private void t_BuscaActualizaciones_Tick(object sender, EventArgs e)
        {
            //se detiene el timer de busqueda y empieza a buscar actualizaciones en segundo plano.
            t_BuscaActualizaciones.Stop();
            bwkBuscaActualizaciones.RunWorkerAsync();
        }
        #endregion

        #region Revision de la conexion al servidor
        private void timer1_Tick(object sender, EventArgs e)
        {
            t_RevisaConexion.Stop();
            bwkRevisaConexion.RunWorkerAsync();
        }

        private void bwkRevisaConexion_DoWork(object sender, DoWorkEventArgs e)
        {
            int contadorDesconexion = 0;

            while (contadorDesconexion < 1)
            {
                if (!Utils.PingNetwork(_dataSource, 1700, 5))
                {
                    contadorDesconexion++;
                }
                else
                {
                    if (Application.OpenForms["ConexionPerdida"] != null)
                    {
                        try
                        {
                            foreach (Form ventana in Application.OpenForms) ventana.Enabled = true;
                            ReinicioRequerido = false;
                        }
                        catch
                        {
                            // ignored
                        }
                        finally
                        {
                            var form = Application.OpenForms["ConexionPerdida"] as ConexionPerdida;
                            if (form != null) form.Close();
                        }

                    }
                    break;
                }
            }

            if (!Utils.PingNetwork(_dataSource, 1700, 5))
            {
                ReinicioRequerido = true;
                if (Application.OpenForms["ConexionPerdida"] == null)
                {
                    try
                    {
                        foreach (Form ventana in Application.OpenForms) ventana.Enabled = false;
                        var x = new ConexionPerdidaClase();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
            {
                if (Application.OpenForms["ConexionPerdida"] != null)
                {
                    try
                    {
                        foreach (Form ventana in Application.OpenForms) ventana.Enabled = true;
                        ReinicioRequerido = false;
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        var form = Application.OpenForms["ConexionPerdida"] as ConexionPerdida;
                        if (form != null) form.Close();
                    }

                }
            }

        }

        private void bwkRevisaConexion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            t_RevisaConexion.Start();
        }
        #endregion

        #region Mensajes Output
        public void OutPutMessage(List<string> mensaje)
        {
            txtOutputMessages.Clear();
            ultraDockManager1.ShowAll();

            foreach (var item in mensaje)
            {
                txtOutputMessages.AppendText(item);
                txtOutputMessages.AppendText(Environment.NewLine);
            }

            Globals.ProgressbarStatus.MensajeOutput = new List<string>();
        }
        #endregion

        #region Llena en las variables Globales 'Cache' los combos mas utilizados
        private void bwkLlenaCacheCombos_DoWork(object sender, DoWorkEventArgs e)
        {
            var objOperationResult = new OperationResult();
            var objDatahierarchyBl = new DatahierarchyBL();
            var objEstablecimientoBl = new EstablecimientoBL();
            Globals.CacheCombosVentaDto.cboDocumentos = new DocumentoBL().ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Globals.CacheCombosVentaDto.cboDocumentosRef = new DocumentoBL().ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Globals.CacheCombosVentaDto.cboMoneda = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null);
            Globals.CacheCombosVentaDto.cboEstado = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 26, null);
            Globals.CacheCombosVentaDto.cboCondicionPago = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 23, null);
            Globals.CacheCombosVentaDto.cboEstablecimiento = objEstablecimientoBl.ObtenerEstablecimientosValueDto(ref objOperationResult, null);
            Globals.CacheCombosVentaDto.cboIGV = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 27, null);
            Globals.CacheCombosVentaDto.cboTipoVenta = new ComprasBL().ObtenerConceptosParaCombo(ref objOperationResult, 2, null);
            Globals.CacheCombosVentaDto.cboMVenta = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 39, null);
            Globals.CacheCombosVentaDto.cboPuntoDestino = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 38, null);
            Globals.CacheCombosVentaDto.cboPuntoEmbarque = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 36, null);
            Globals.CacheCombosVentaDto.cboTipoEmbarque = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 37, null);
            //Globals.CacheCombosVentaDto.cboTipoOperacion = objDatahierarchyBl.GetDataHierarchyForComboWithIDValueDto(ref objOperationResult, 35, null);
            Globals.CacheCombosVentaDto.cboTipoOperacion = objDatahierarchyBl.GetDataHierarchiesForComboWithValue2(ref objOperationResult, 35, null, "v_Field == \"1\"");//Facturacion Electronica
            Globals.CacheCombosVentaDto.cboVendedor = new VendedorBL().ObtenerListadoVendedorParaCombo(ref objOperationResult, null);
            Globals.CacheCombosVentaDto.cboVendedorRef = Globals.CacheCombosVentaDto.cboVendedor;
            Globals.CacheCombosVentaDto.cboTipoOperacionDetraccion = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 177, null);
            Globals.CacheCombosVentaDto.cboTiposKardex = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 153, null);
            Globals.CacheCombosVentaDto.cboTipoBulto = objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 160, null);
            var objDocumentoBl = new DocumentoBL();
            Globals.ListaDocumentosContable = objDocumentoBl.DocumentoEsContable();
            Globals.ListaDocumentosInversos = objDocumentoBl.DocumentoEsInverso();
            Globals.UsuariosContables = objEstablecimientoBl.ObtenerUsuariosContables();
            Globals.ListaEstablecimientoDetalle = objDocumentoBl.ListaEstablecimientoDetalle();
        }
        #endregion

        #region Carga los assemblers del Crystal Report
        /// <summary>
        /// Carga las Librerias del Crystal Report al momento de inicializar la app para que no demore tanto en consultar la primera vez.
        /// </summary>
        private void CargarCR()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var preloadCrystalReport = new ReportDocument())
                    {
                        preloadCrystalReport.Load(@"Reportes\Ventas\crCliente.rpt");
                        CrystalDecisions.Windows.Forms.CrystalReportViewer preloadCrystalGui =
                            new CrystalDecisions.Windows.Forms.CrystalReportViewer
                            {
                                ReportSource = preloadCrystalReport
                            };
                    }
                }
                catch
                {
                    // ignored
                }
            }, TaskCreationOptions.AttachedToParent);
        }
        #endregion

        #region Registrar Forms
        private int _numFrm;
        /// <summary>
        /// Registra un Formulario con un Propietario, y permite mostrarlo
        /// </summary>
        /// <param name="ownerFrm">Form owner</param>
        /// <param name="childFrm">Form a registrar</param>
        /// <param name="showForm">True ejecuta method show()</param>
        public void RegistrarForm(Form ownerFrm, Form childFrm, bool showForm = true)
        {
            _numFrm++;
            childFrm.Owner = ownerFrm;
            childFrm.FormClosed += childFrm_FormClosed;
            childFrm.Resize += objFrm_Resize;
            childFrm.ShowInTaskbar = !showForm;
            childFrm.MinimizeBox = true;
            ownerFrm.FormClosing -= ownerFrm_FormClosing; // Desuscribe algun evento anterior
            ownerFrm.FormClosing += ownerFrm_FormClosing;
            if (showForm)
                childFrm.Show();
        }

        private void objFrm_Resize(object sender, EventArgs e)
        {
            var frm = sender as Form;
            if (frm == null) return;
            if (frm.WindowState == FormWindowState.Minimized)
                InsertItem(frm.Name, frm.Text);
            else
                RemoveItem(frm.Name);
        }

        /// <summary>
        /// Inserta Item en el PopupMenu
        /// </summary>
        /// <param name="key">Key del Tool (Form FullName)</param>
        /// <param name="caption">Titulo a mostrar en el Popup Menu</param>
        private void InsertItem(string key, string caption)
        {
            var pop = (PopupMenuTool)ultraToolbarsManager1.Tools["ListForm"];
            var btnTool = new ButtonTool(key);
            btnTool.SharedProps.Caption = caption;
            btnTool.SharedPropsInternal.AppearancesSmall.Appearance.Image = Resource.application_start;
            ultraToolbarsManager1.Tools.Add(btnTool);
            btnTool.ToolClick += btnTool_ToolClick;
            pop.Tools.Add(btnTool);
            //pop.ShowPopup();
            ultraToolbarsManager1.AutoGenerateKeyTips = (_numFrm == pop.Tools.Count);
        }

        private void btnTool_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (Application.OpenForms[e.Tool.Key] != null)
                Application.OpenForms[e.Tool.Key].WindowState = FormWindowState.Normal;
        }

        private void RemoveItem(string key)
        {
            var pop = (PopupMenuTool)ultraToolbarsManager1.Tools["ListForm"];

            foreach (var item in pop.Tools)
            {
                if (!item.Key.Equals(key)) continue;
                pop.Tools.Remove(item);
                ultraToolbarsManager1.Tools.Remove(item);
                break;
            }
            ultraToolbarsManager1.AutoGenerateKeyTips = _numFrm == pop.Tools.Count;
        }

        private void ownerFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Razon: " + e.CloseReason);
            var frm = (Form)sender;
            if (frm.OwnedForms.Any())
                if (UltraMessageBox.Show("Hay una Ventana abierta por \"" + frm.Text + "\"" + Environment.NewLine + "¿Desea Cerrarlo?",
                                     "Aviso", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            frm.FormClosing -= ownerFrm_FormClosing;
        }

        private void childFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _numFrm--;
            var frm = (Form)sender;
            RemoveItem(frm.Name);
            frm.Resize -= objFrm_Resize;
            frm.FormClosed -= childFrm_FormClosed;
        }
        #endregion

        #region Text Search

        private void cbTextSearch_Enter(object sender, EventArgs e)
        {
            if (cbTextSearch.Items.Count == 0)
            {
                cbTextSearch.Items.AddRange((from ToolBase item in ultraToolbarsManager1.Tools
                                             where item.GetType() == typeof(ButtonTool)
                                             select new Infragistics.Win.ValueListItem
                                             {
                                                 DataValue = item.Key,
                                                 DisplayText = item.SharedProps.Caption,
                                                 Appearance = new Infragistics.Win.Appearance
                                                 {
                                                     Image = item.SharedPropsInternal.AppearancesSmall.Appearance.Image
                                                 },
                                             }).ToArray());

                cbTextSearch.ValueList.ItemHeight = 30;
                cbTextSearch.ValueList.Appearance = new Infragistics.Win.Appearance
                {
                    TextVAlign = Infragistics.Win.VAlign.Middle
                };
                cbTextSearch.ValueList.AutoSuggestHighlightAppearance.ForeColor = Color.FromArgb(255, 67, 168, 152);
                cbTextSearch.ValueList.DropDownResizeHandleStyle = Infragistics.Win.DropDownResizeHandleStyle.DiagonalResize;
            }
            cbTextSearch.Clear();
        }

        private void cbTextSearch_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            cbTextSearch.Clear();
        }

        private void cbTextSearch_AfterCloseUp(object sender, EventArgs e)
        {
            if (cbTextSearch.SelectedItem == null) return;
            var tool = ultraToolbarsManager1.Tools[cbTextSearch.SelectedItem.DataValue.ToString()];
            OpenFormulario(tool.Key);
            //ultraToolbarsManager1.Ribbon.SelectedTab = ultraToolbarsManager1.Ribbon.Tabs[2];
        }
        #endregion

        protected virtual void OnEstadoCargaEvent(string estado)
        {
            var handler = EstadoCargaEvent;
            if (handler != null) handler(estado);
        }
    }
}
