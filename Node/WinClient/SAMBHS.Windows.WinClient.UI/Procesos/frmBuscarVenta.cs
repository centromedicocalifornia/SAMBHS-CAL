using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBuscarVenta : Form
    {
        #region Fields
        private bool _isNota;
        private bool _isPedido;
        #endregion

        #region Properties
        public BusquedaExtraccionDto Result;
        /// <summary>
        /// Gets a value indicating whether this instance is nota salida.
        /// </summary>
        /// <value><c>true</c> if this instance is nota salida; otherwise, <c>false</c>.</value>
        public bool IsNotaSalida{
            get
            {
                return _isNota;
            }
        }

        public bool IsPedido {

            get
            {
                return _isPedido;
            }
        
        }
        #endregion

        #region Construct
        public frmBuscarVenta()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private List<BusquedaExtraccionDto> GetDataVenta()
        {
            var objOperationResult = new OperationResult();
            var _objData = new VentaBL().ListarBusquedaVentas(ref objOperationResult, null, null, dtpFechaInicio.Value.Date, dtpFechaFin.Value,-1,null,null,-1,Globals.ClientSession.i_IdEstablecimiento.Value);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show(string.Concat("Error en operación:", Environment.NewLine, objOperationResult.ExceptionMessage), 
                    "ERROR!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            if (_objData != null)
                return _objData.Select(o => new BusquedaExtraccionDto
                {
                    ID = o.v_IdVenta,
                    FechaDocumento = o.t_FechaRegistro,
                    NombreCliente = string.Format("{0} {1}", o.NroDocCliente, o.NombreCliente),
                    NroDocumento = string.Format("{0}-{1}", o.v_SerieDocumento, o.v_CorrelativoDocumento),
                    TipoDocumento = o.TipoDocumento,
                    Estado = o.i_IdEstado ?? 0,
                    Moneda = o.Moneda,
                    Total = o.d_Total.Value,
                    iTipoDocumento = o.i_IdTipoDocumento.Value,
                    Origen = o.Origen,
                    v_Concepto =o.v_Concepto ,
                   

                }).ToList();
            return null;
        }

        private List<BusquedaExtraccionDto> GetDataSalida()
        {
            var objOperationResult = new OperationResult();
            var objData = new MovimientoBL().ListarBusquedaMovimientos(ref objOperationResult, null, "RegistroOrigen == null", dtpFechaInicio.Value.Date, dtpFechaFin.Value, (int)TipoDeMovimiento.NotadeSalida,Globals.ClientSession.i_IdEstablecimiento.Value );

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show(string.Format("Error en operación:{0}{1}", Environment.NewLine, objOperationResult.ExceptionMessage), "ERROR!", Icono: MessageBoxIcon.Error);
            }
            if (objData != null)
            {

                return objData.Select(o =>
                {
                    var siglaDoc = "NS";
                    if (o.i_IdTipoDocumento.HasValue && o.i_IdTipoDocumento.Value != -1)
                    {
                        var obj = Enum.ToObject(typeof(TiposDocumentos), o.i_IdTipoDocumento);
                        if (obj != null) siglaDoc = obj.ToString();
                    }

                    return new BusquedaExtraccionDto
                    {
                        ID = o.v_IdMovimiento,
                        FechaDocumento = DateTime.Now,
                        NombreCliente = o.v_NombreProveedor,
                        NroDocumento = string.Join("-", o.v_SerieDocumento, o.v_CorrelativoDocumento),
                        TipoDocumento = siglaDoc,
                        //Estado = 0,
                        Moneda = o.Moneda,
                        Total = o.d_TotalCantidad ?? 0,
                        iTipoDocumento = o.i_IdTipoDocumento ?? -1,
                        Origen = o.RegistroOrigen,
                        v_Concepto =o.v_Glosa,
                    };
                }).ToList();
            }
            return null;
        }


        private List<BusquedaExtraccionDto> GetDataPedido()
        {
            var objOperationResult = new OperationResult();
            var objData =  new PedidoBL().ListarBusquedaPedidos(ref objOperationResult, "t_FechaEmision", dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Text + " 23:59"), -1, "", "", "", "", "", -1, -1, "-1");


            
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show(string.Format("Error en operación:{0}{1}", Environment.NewLine, objOperationResult.ExceptionMessage), "ERROR!", Icono: MessageBoxIcon.Error);
            }
            if (objData != null)
            {

                return objData.Select(o =>
                {
                    var siglaDoc = "PED";
                    if (o.i_IdTipoDocumento.HasValue && o.i_IdTipoDocumento.Value != -1)
                    {
                        var obj = Enum.ToObject(typeof(TiposDocumentos), o.i_IdTipoDocumento);
                        if (obj != null) siglaDoc = obj.ToString();
                    }

                    return new BusquedaExtraccionDto
                    {
                        ID = o.v_IdPedido,
                        FechaDocumento =o.t_FechaEmision.Value ,
                        NombreCliente = o.NombreCliente,
                        NroDocumento =    o.v_SerieDocumento +"-"+o.v_CorrelativoDocumento,    //string.Join("-", o.v_SerieDocumento, o.v_CorrelativoDocumento),
                        TipoDocumento = siglaDoc,
                        //Estado = 0,
                        Moneda = o.Moneda,
                        Total = o.d_PrecioVenta??0,
                        iTipoDocumento = o.i_IdTipoDocumento ?? -1,
                        Origen = "",//o.RegistroOrigen,
                        v_Concepto = o.v_Glosa,
                    };
                }).ToList();
            }
            return null;
        }







        private void ReturnResult()
        {
            if (grdData.ActiveRow == null) return;
            Result = (BusquedaExtraccionDto)grdData.ActiveRow.ListObject;            
            if(Modal)
                DialogResult = System.Windows.Forms.DialogResult.OK;
            else
                Close();
        }
        #endregion

        #region Events
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            _isNota = rbNotaSalida.Checked;
            _isPedido = rbPedido.Checked;
            var data = rbNotaSalida.Checked ? GetDataSalida() : rbPedido.Checked ? GetDataPedido ()   :  GetDataVenta();
            grdData.DataSource = data;
           // lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdData.Count);
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            ReturnResult();
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ReturnResult();
        }
        #endregion

        private void frmBuscarVenta_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }
    }
}
