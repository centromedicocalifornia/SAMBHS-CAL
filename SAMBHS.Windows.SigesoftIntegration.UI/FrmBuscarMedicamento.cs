using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class FrmBuscarMedicamento : Form
    {
        private List<string> listaProd = new List<string>();
        private string _rucCliente;
        public string service;
        public string ticket;
        public FrmBuscarMedicamento(string rucCliente)
        {
            _rucCliente = rucCliente;
            InitializeComponent();
        }

        private void FrmBuscarMedicamento_Load(object sender, EventArgs e)
        {
            UltraGridColumn c = grdTotalDiagnosticos.DisplayLayout.Bands[0].Columns["b_Seleccionar"];
            c.CellActivation = Activation.AllowEdit;
            c.CellClickAction = CellClickAction.Edit;
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!uvFiltro.Validate(true, false).IsValid) return;
            bool receta;
            if (rbReceta.Checked){receta = true;}
            else{receta = false;}
            BuscarReceta(receta);
        }

        List<RecetaDto> _listaDetalle = new List<RecetaDto>();
        private void BuscarReceta(bool receta)
        {
            var filtros = new Filtros
            {
                FechaInicio = dtpDateTimeStar.Value,
                FechaFin = dptDateTimeEnd.Value,
                Dni = txtDni.Text
            };
            _listaDetalle = FarmaciaBl.ObtenerRecetaMedica(filtros, _rucCliente, receta);
            if (_listaDetalle.Count > 0)
            {
                var descuento = _listaDetalle[0].r_MedicineDiscount;
                service = _listaDetalle[0].v_ServiceId;
                ticket = _listaDetalle[0].TicketHosp;
                if (descuento == null)
                {
                    descuento = float.Parse("0");
                }
                lblDescuento.Text = "Tiene un descuento del " + descuento + " %";
            }
            grdTotalDiagnosticos.DataSource = _listaDetalle;
        }

        public class Filtros
        {
            public DateTime? FechaInicio{ get; set; }
            public DateTime? FechaFin { get; set; }
            public string Dni { get; set; }
        }

        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
            foreach (var item in _listaDetalle)
            {
                listaProd.Add(item.IdProductoDetalle);
            }
            FarmaciaBl.DespacharMedicamento(listaProd);
            //List<RecetaDto> listaDetalleFinal = new List<RecetaDto>();
            //foreach (var item in grdTotalDiagnosticos.Rows)
            //{
            //    if ((bool)item.Cells["b_Seleccionar"].Value)
            //    {
            //        _listaDetalle.Add(x);
            //    }
            //}


            var lisaDespachado = _listaDetalle.FindAll(p => p.b_Seleccionar);
            service = _listaDetalle[0].v_ServiceId;
            Task.Factory.StartNew(() =>
            {
                ListadoVentaDetalle = Transformar(lisaDespachado);
            }).ContinueWith(t =>
            {
                if (!ListadoVentaDetalle.Any()) return;
                DialogResult = DialogResult.OK;
                Close();
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private static BindingList<ventadetalleDto> Transformar(List<RecetaDto> listaDetalle)
        {
            var l = new BindingList<ventadetalleDto>();
            foreach (var item in listaDetalle)
            {
                var o = new ventadetalleDto();
                o.i_Anticipio = 0;
                o.i_IdAlmacen = 1;
                o.i_IdCentroCosto = "0";
                o.i_IdUnidadMedida = item.IdUnidadMedida;
                o.ProductoNombre = item.Medicamento;
                o.v_DescripcionProducto = item.Medicamento;
                o.v_IdProductoDetalle = item.IdProductoDetalle;
                o.v_NroCuenta = "";
                o.d_PrecioVenta = item.PrecioVenta;
                o.d_Igv = item.Igv;
                o.d_Cantidad = item.Cantidad;
                o.d_CantidadEmpaque = item.Cantidad;
                o.d_Precio = item.PrecioVenta ;
                o.d_Valor = item.ValorV;
                o.d_ValorVenta = item.ValorV;
                o.d_PrecioImpresion = item.PrecioVenta;
                o.v_CodigoInterno = item.CodigoInterno;
                o.Empaque = 1;
                o.UMEmpaque = "UND";
                o.i_EsServicio = 1;
                o.i_IdUnidadMedidaProducto = item.IdUnidadMedida;
                l.Add(o);
            }

            return l;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var serviceId = _listaDetalle[0].v_ServiceId;
            var recomendaciones = AgendaBl.GetRecomendaciones(serviceId); // string.Join("\n-", _listDiagnosticRepositoryLists.Where(o => !string.IsNullOrWhiteSpace(o.v_RecomendationsName)).Select(p => p.v_RecomendationsName).Distinct()).Trim();
            var restricciones = AgendaBl.GetRestricciones(serviceId); //string.Join("\n-", _listDiagnosticRepositoryLists.Where(o => !string.IsNullOrWhiteSpace(o.v_RestrictionsName)).Select(p => p.v_RestrictionsName).Distinct()).Trim();

            var f = new frmReporteReceta(serviceId, recomendaciones, restricciones, "");
            f.ShowDialog();
        }

        private void ultraGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void grdTotalDiagnosticos_ClickCell(object sender, ClickCellEventArgs e)
        {
            if ((e.Cell.Column.Key == "b_Seleccionar"))
            {
                if ((e.Cell.Value.ToString() == "False"))
                {
                    e.Cell.Value = true;

                    //btnFechaEntrega.Enabled = true;
                    //btnAdjuntarArchivo.Enabled = true;
                }
                else
                {
                    e.Cell.Value = false;
                    //btnFechaEntrega.Enabled = false;
                    //btnAdjuntarArchivo.Enabled = false;
                }

            }
        }
    }
}
