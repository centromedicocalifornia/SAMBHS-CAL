﻿using SAMBHS.Security.BL;
using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using System.Net;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    // ReSharper disable once InconsistentNaming
    public partial class frmCustomerCapchaSUNAT : Form
    {
        #region Fields
        private Thread _thread;
        private ConsultaContribuyente _myInfo;

        private readonly string _ruc;
        #endregion

        #region Properties
        public string[] DatosContribuyente = new string[10];

        public bool ConectadoRecibido;
        #endregion

        #region Construct
        public frmCustomerCapchaSUNAT(string parametro)
        {
            _ruc = parametro;
            InitializeComponent();
        }
        #endregion

        #region Event UI
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCapcha.Text.Trim())) return;

            _thread = new Thread(t =>
            {
                Invoke(new MethodInvoker(() => SetStatusBar(true)));
                _myInfo.GetInfo(_ruc, txtCapcha.Text);
                Invoke(new MethodInvoker(CaptionResul));
            }) { IsBackground = true };
            _thread.Start();    
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            CargarImagen();
            txtCapcha.SelectAll();
            txtCapcha.Focus();
            txtCapcha.Clear();
        }

        private void frmVenta_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            txtCapcha.Focus();
            _thread = new Thread(CargarImagen){ IsBackground = true };
            _thread.Start();
            ConectadoRecibido = false;
        }

        private void frmCustomerCapchaSUNAT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void frmCustomerCapchaSUNAT_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_thread != null && _thread.IsAlive) _thread.Abort();
        }
        #endregion

        #region Methods
        private void SetConexionFallida()
        {
            lblObteniendoCapcha.Text = @"Error de Conexión";
            //pbLoadingCapcha.Image = Resource.delete;
        }

        private void SetStatusBar(bool buscando)
        {
            if (buscando)
            {
                uStatusBar.Panels["pMensaje"].Visible = false;
                uStatusBar.Panels["pMarquee"].Visible = true;
            }
            else
            {
                uStatusBar.Panels["pMensaje"].Visible = true;
                uStatusBar.Panels["pMarquee"].Visible = false;
            }
            Invoke(new MethodInvoker(() =>
            {
                txtCapcha.Enabled = !buscando;
                btnOK.Enabled = txtCapcha.Enabled;
                btnReload.Enabled = txtCapcha.Enabled;
            }));
        }

        private void CargaImagenCompletada()
        {
            lblObteniendoCapcha.Visible = false;
            btnOK.Enabled = true;
            btnReload.Enabled = true;
            pbLoadingCapcha.Visible = false;
        }

        private void CaptionResul()
        {
            switch (_myInfo.GetResul)
            {
                case ConsultaContribuyente.Resul.Ok:
                    DatosContribuyente[0] = _myInfo.RazonSocial;
                    DatosContribuyente[1] = _myInfo.FechaInscripcion;
                    DatosContribuyente[2] = _myInfo.FechaInicioActividades;
                    DatosContribuyente[3] = _myInfo.EstadoContribuyente;
                    DatosContribuyente[4] = _myInfo.Condicion;
                    DatosContribuyente[5] = _myInfo.DireccionFiscal;
                    DatosContribuyente[6] = (_myInfo.Telefonos??"").Replace("/", "").Trim();
                    DatosContribuyente[7] = _myInfo.ActividadComercioExterior;
                    DatosContribuyente[8] = _myInfo.SistemaContable;
                    DatosContribuyente[9] = _myInfo.TipoContribuyente;
                    ConectadoRecibido = true;
                    Close();
                    break;
                case ConsultaContribuyente.Resul.NoResul:
                    UltraMessageBox.Show("No existe RUC", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatusBar(false);
                    break;
                case ConsultaContribuyente.Resul.ErrorCapcha:
                    CargarImagen();
                    UltraMessageBox.Show("Ingrese el capcha correctamente", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatusBar(false);
                    break;
                case ConsultaContribuyente.Resul.Error:
                    UltraMessageBox.Show("Error desconocido", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatusBar(false);
                    break;
            }   
        }

        private void CargarImagen()
        {
            try
            {
                if (_myInfo == null)
                    _myInfo = new ConsultaContribuyente();

                pbCapcha.Image = _myInfo.GetCapcha;
                Invoke(new MethodInvoker(CargaImagenCompletada));
            }
            catch (WebException ex)
            {
                var httpResponse = (HttpWebResponse)ex.Response;
                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                        CargarImagen();
                }
                else
                    Invoke(new MethodInvoker(SetConexionFallida));
            }
        }
        #endregion
    }
}
