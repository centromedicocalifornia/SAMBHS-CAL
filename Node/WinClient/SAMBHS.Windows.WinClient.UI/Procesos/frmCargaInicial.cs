using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;
using System.Transactions;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmCargaInicial : Form
    {
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        AlmacenBL _objAlmacen = new AlmacenBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        string Mensaje = "Ya existe carga Inicial para estos establecimientos : \n";
        string EstablecimientosCargaInicial = "";
        public frmCargaInicial(string pstParametro)
        {
            InitializeComponent();
        }

        private void frmCargaInicial_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            lblAnioAnterior.Text = (Globals.ClientSession.i_Periodo - 1).ToString();
            lblAnioSiguiente.Text = Globals.ClientSession.i_Periodo.ToString();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.All);
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            LabelCargaInicial.Text = "CARGA INICIAL";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            chkIncluirNroPedido.Visible = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta ==1 ? true :false;
            chkIncluirNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
            //lblEstablecimiento.Text = "ESTABLECIMIENTO : " + _objAlmacen.NombreEstablecimiento(Globals.ClientSession.i_IdEstablecimiento.Value);

        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando.. Carga Inicial"
                : @"Carga Inicial";
            pBuscando.Visible = estado;
            btnProcesar.Enabled = !estado;
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            var respMsg =
                MessageBox.Show(@"¿Seguro de Iniciar el Proceso Carga Incial? Esto puede tomar cierto tiempo, dependiendo de la cantidad de datos.",
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respMsg == DialogResult.Yes)
            {


                OperationResult objOperationResult = new OperationResult();
                //using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                //{

                movimientoDto objmovimientoDto = new movimientoDto();
                movimientodetalleDto objmovimientodetalleDto = new movimientodetalleDto();
                List<movimientodetalleDto> ListaMovimientos = new List<movimientodetalleDto>();
                ProductoBL _objProductoBL = new ProductoBL();
                int IdMoneda;
                //Obtener Stock Final por Productos
                string FilterExpresion = null;
                DateTime fechaIni = DateTime.Parse("01/01/" + (Globals.ClientSession.i_Periodo - 1).ToString());
                DateTime fechaFin = DateTime.Parse("31/12/" + (Globals.ClientSession.i_Periodo - 1).ToString() + " 23:59");

                #region Pre Requisitos
                DateTime Fecha = new DateTime(int.Parse(Globals.ClientSession.i_Periodo.ToString()), 01, 01);
                //Obtener Tipo Cambio
                var TipoCambio = new MovimientoBL().DevolverTipoCambioPorFecha(ref objOperationResult, Fecha);
                if (TipoCambio == string.Empty || TipoCambio == "0")
                {
                    UltraMessageBox.Show("No se puede registrar la operación mientras no exista un tipo de cambio para la fecha", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                #endregion

                IdMoneda = RbtnSoles.Checked ? 1 : 2;
                OcultarMostrarBuscar(true);
                Cursor.Current = Cursors.WaitCursor;

                Task.Factory.StartNew(() =>
                {
                    if (cboEstablecimiento.Value != null && cboEstablecimiento.Value.ToString() == "-1")
                    {
                        #region Todos Establecimientos
                        var Establecimientos = new EstablecimientoBL().ObtenerEstablecimientoAlmacen(ref objOperationResult);


                        foreach (var estab in Establecimientos)
                        {

                            var ListaAlmacenes = new EstablecimientoBL().DevolverTodosAlmacenes(estab.i_IdEstablecimiento.Value);
                            foreach (var item in ListaAlmacenes)
                            {
                                if (_objMovimientoBL.ExistenciaCargaInicial(Globals.ClientSession.i_Periodo.ToString(), int.Parse(estab.i_IdEstablecimiento.Value.ToString()), item.i_IdAlmacen))
                                {
                                    //UltraMessageBox.Show("Ya existe una carga Inicial para este Periodo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //return;
                                    EstablecimientosCargaInicial = estab.v_AlmacenNombre + "," + EstablecimientosCargaInicial;

                                }
                                else
                                {
                                    FilterExpresion = "IdAlmacen==" + item.i_IdAlmacen;

                                    #region Cabecera Nota Ingreso

                                    objmovimientoDto.d_TipoCambio = decimal.Parse(TipoCambio);
                                    objmovimientoDto.i_IdAlmacenOrigen = item.i_IdAlmacen;
                                    objmovimientoDto.i_IdMoneda = IdMoneda;
                                    objmovimientoDto.i_IdTipoMotivo = (int)MotiveType.CARGAINICIAL;
                                    objmovimientoDto.t_Fecha = Fecha;
                                    objmovimientoDto.v_Glosa = "CARGA INICIAL ,PERIODO :" + (Globals.ClientSession.i_Periodo - 1).ToString();
                                    objmovimientoDto.v_Mes = "01";
                                    objmovimientoDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                                    objmovimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                    objmovimientoDto.v_OrigenTipo = null;
                                    objmovimientoDto.v_OrigenRegPeriodo = null;
                                    objmovimientoDto.v_OrigenRegMes = null;
                                    objmovimientoDto.v_OrigenRegCorrelativo = null;
                                    objmovimientoDto.i_IdEstablecimiento = int.Parse(estab.i_IdEstablecimiento.Value.ToString()); // int.Parse ( Globals.ClientSession.i_IdEstablecimiento.Value.ToString ()); 
                                    List<KeyValueDTO> _ListadoMovimientos = new List<KeyValueDTO>();
                                    _ListadoMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, Globals.ClientSession.i_Periodo.ToString(), "01", (int)Common.Resource.TipoDeMovimiento.NotadeIngreso);
                                    if (_ListadoMovimientos.Count != 0)
                                    {
                                        objmovimientoDto.v_Correlativo = (int.Parse(_ListadoMovimientos[_ListadoMovimientos.Count() - 1].Value1) + 1).ToString("00000000");
                                    }
                                    else
                                    {
                                        objmovimientoDto.v_Correlativo = "00000001";

                                    }
                                    objmovimientoDto.i_EsDevolucion = 0;

                                    #endregion

                                    #region Detalle
                                    var Result = new AlmacenBL().ReporteStock(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), fechaIni, fechaFin, FilterExpresion, IdMoneda, "", "", "-1", 0, 0, 0, 0, 1, (int)FormatoCantidad.UnidadMedidaProducto,"",DateTime.Now,chkIncluirNroPedido.Checked ?1:0 );

                                    var ListaProductoStock = Result.Where(o => o.i_Activo == 1).ToList().GroupBy(x => new { x.v_IdProducto, x.NroPedido })
                                                                    .Select(group => group.Last())
                                                                    .OrderBy(x => x.v_IdProducto).ToList();

                                    var gg = ListaProductoStock.Where(o => o.v_IdProductoDetalle == "N001-PE000001955").ToList();

                                    var ListaProductosSinMovimientos = _objProductoBL.ListaTodosProductosDetalles().ToList();
                                   
                                    foreach (var itemProd in ListaProductoStock)
                                    {
                                       
                                        objmovimientodetalleDto = new movimientodetalleDto();
                                        objmovimientodetalleDto.v_IdProductoDetalle = itemProd.v_IdProductoDetalle;
                                        objmovimientodetalleDto.v_NroGuiaRemision = "";
                                        objmovimientodetalleDto.i_IdTipoDocumento = -1;
                                        objmovimientodetalleDto.v_NumeroDocumento = null;
                                        objmovimientodetalleDto.d_Cantidad = Utils.Windows.DevuelveValorRedondeado(itemProd.Saldo_Cantidad.Value, 4);
                                        objmovimientodetalleDto.i_IdUnidad = itemProd.i_IdUnidadMedidaProducto;
                                        objmovimientodetalleDto.d_Precio = Utils.Windows.DevuelveValorRedondeado(itemProd.Saldo_Precio.Value, 6);
                                        objmovimientodetalleDto.d_Total = Utils.Windows.DevuelveValorRedondeado(itemProd.Saldo_Total.Value, 2);
                                        objmovimientodetalleDto.v_NroPedido = itemProd.NroPedido;
                                        objmovimientodetalleDto.v_NroGuiaRemision = null;
                                        objmovimientodetalleDto.d_CantidadEmpaque = Utils.Windows.DevuelveValorRedondeado(new AlmacenBL().GetCantidadEmpaque(itemProd), 4);
                                        ListaMovimientos.Add(objmovimientodetalleDto);

                                    }
                                    if (Globals.ClientSession.v_RucEmpresa != Constants.RucJorplast)
                                    {
                                       
                                        var prodSinMovDiccionario = ListaProductosSinMovimientos.ToDictionary(p => p.v_IdProducto, o => o);
                                        var CodigosProductosSinMovimientos = ListaProductosSinMovimientos.Select(x => x.v_IdProducto).ToList();
                                        var NuevosProd = CodigosProductosSinMovimientos.Except(ListaProductoStock.Select(x => x.v_IdProducto)).ToList();
                                        List<movimientodetalleDto> ListaFaltantes = NuevosProd.Select(idProducto =>
                                        {
                                            productodetalleDto pd;
                                            productoinventarioDto Inv;
                                            pd = prodSinMovDiccionario.TryGetValue(idProducto, out pd) ? pd : new productodetalleDto();
                                            return new movimientodetalleDto
                                            {
                                                
                                                v_IdProductoDetalle = pd.v_IdProductoDetalle,
                                                i_IdTipoDocumento = -1,
                                                v_NumeroDocumento = null,
                                                d_Cantidad = 0,
                                                i_IdUnidad = pd != null ? pd.i_ActualizaIdUsuario : -1,
                                                d_Precio = 0,
                                                d_Total = 0,
                                                v_NroPedido = null,
                                                v_NroGuiaRemision = null,
                                                d_CantidadEmpaque = 0,
                                            };
                                        }
                                            ).ToList();

                                        ListaMovimientos = ListaMovimientos.Concat(ListaFaltantes).ToList();

                                      

                                    }

                                    #endregion

                                    objmovimientoDto.d_TotalCantidad = Utils.Windows.DevuelveValorRedondeado(ListaMovimientos.Sum(s => s.d_Cantidad).Value, 2);
                                    objmovimientoDto.d_TotalPrecio = Utils.Windows.DevuelveValorRedondeado(ListaMovimientos.Sum(s => s.d_Total).Value, 2);
                                    if (ListaMovimientos.Count() > 0)
                                    {
                                        ListaMovimientos = ListaMovimientos.OrderBy(o => o.v_IdProductoDetalle).ToList();
                                        var hh = ListaMovimientos.Where(o => o.v_IdProductoDetalle == "N001-PE000001955").ToList();
                                        new MovimientoBL().InsertarMovimiento(ref objOperationResult, objmovimientoDto, Globals.ClientSession.GetAsList(), ListaMovimientos);
                                        if (objOperationResult.Success == 0) return;
                                        ListaMovimientos = new List<movimientodetalleDto>();
                                    }
                                }

                            }
                        }
                        #endregion
                    }
                    else
                    {

                        #region UnEstablecimiento
                        List<almacenDto> ListaAlmacenes = new List<almacenDto>();
                        if (cboAlmacen.Value != null && cboAlmacen.Value.ToString() != "-1")
                        {
                            almacenDto UnicoAlmacen = new almacenDto();
                            UnicoAlmacen.i_IdAlmacen = int.Parse(cboAlmacen.Value.ToString());
                            ListaAlmacenes.Add(UnicoAlmacen);
                        }
                        else
                        {
                            ListaAlmacenes = new EstablecimientoBL().DevolverTodosAlmacenes(int.Parse(cboEstablecimiento.Value.ToString()));
                        }

                        foreach (var item in ListaAlmacenes)
                        {
                            if (_objMovimientoBL.ExistenciaCargaInicial(Globals.ClientSession.i_Periodo.ToString(), int.Parse(cboEstablecimiento.Value.ToString()), item.i_IdAlmacen))
                            {
                                EstablecimientosCargaInicial = "Establecimiento :" + cboEstablecimiento.Text + "," + "Almacén: " + item.i_IdAlmacen.ToString() + "\n" + EstablecimientosCargaInicial;
                            }
                            else
                            {
                                FilterExpresion = "IdAlmacen==" + item.i_IdAlmacen;

                                #region Cabecera Nota Ingreso

                                objmovimientoDto.d_TipoCambio = decimal.Parse(TipoCambio);
                                objmovimientoDto.i_IdAlmacenOrigen = item.i_IdAlmacen;
                                objmovimientoDto.i_IdMoneda = IdMoneda;
                                objmovimientoDto.i_IdTipoMotivo = (int)MotiveType.CARGAINICIAL;
                                objmovimientoDto.t_Fecha = Fecha;
                                objmovimientoDto.v_Glosa = "CARGA INICIAL ,PERIODO :" + (Globals.ClientSession.i_Periodo - 1).ToString();
                                objmovimientoDto.v_Mes = "01";
                                objmovimientoDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                                objmovimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                objmovimientoDto.v_OrigenTipo = null;
                                objmovimientoDto.v_OrigenRegPeriodo = null;
                                objmovimientoDto.v_OrigenRegMes = null;
                                objmovimientoDto.v_OrigenRegCorrelativo = null;
                                objmovimientoDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());//int.Parse(estab.i_IdEstablecimiento.Value.ToString()); // int.Parse ( Globals.ClientSession.i_IdEstablecimiento.Value.ToString ()); 
                                List<KeyValueDTO> _ListadoMovimientos = new List<KeyValueDTO>();
                                _ListadoMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, Globals.ClientSession.i_Periodo.ToString(), "01", (int)Common.Resource.TipoDeMovimiento.NotadeIngreso);
                                if (_ListadoMovimientos.Count != 0)
                                {
                                    objmovimientoDto.v_Correlativo = (int.Parse(_ListadoMovimientos[_ListadoMovimientos.Count() - 1].Value1) + 1).ToString("00000000");
                                }
                                else
                                {
                                    objmovimientoDto.v_Correlativo = "00000001";

                                }
                                objmovimientoDto.i_EsDevolucion = 0;

                                #endregion

                                #region Detalle
                                var Result = new AlmacenBL().ReporteStock(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), fechaIni, fechaFin, FilterExpresion, IdMoneda, "", "", "-1", 0, 0, 0, 0, 1, (int)FormatoCantidad.UnidadMedidaProducto,"",DateTime.Now ,chkIncluirNroPedido.Checked?1:0);
                                var ListaProductoStock = Result.Where(o => o.i_Activo == 1).ToList().GroupBy(x => new { x.v_IdProducto, x.NroPedido })
                                                                .Select(group => group.Last())
                                                                .OrderBy(x => x.v_IdProducto).ToList();
                                var gg = ListaProductoStock.Where(o => o.v_IdProductoDetalle == "N001-PE000001955").ToList();

                                var ListaProductosSinMovimientos = _objProductoBL.ListaTodosProductosDetalles().ToList();
                                foreach (var itemProd in ListaProductoStock)
                                {
                                    objmovimientodetalleDto = new movimientodetalleDto();
                                    objmovimientodetalleDto.v_IdProductoDetalle = itemProd.v_IdProductoDetalle;
                                    objmovimientodetalleDto.v_NroGuiaRemision = "";
                                    objmovimientodetalleDto.i_IdTipoDocumento = -1;
                                    objmovimientodetalleDto.v_NumeroDocumento = null;
                                    objmovimientodetalleDto.d_Cantidad = Utils.Windows.DevuelveValorRedondeado(itemProd.Saldo_Cantidad.Value, 4);
                                    objmovimientodetalleDto.i_IdUnidad = itemProd.i_IdUnidadMedidaProducto;
                                    objmovimientodetalleDto.d_Precio = Utils.Windows.DevuelveValorRedondeado(itemProd.Saldo_Precio.Value, 6);
                                    objmovimientodetalleDto.d_Total = Utils.Windows.DevuelveValorRedondeado(itemProd.Saldo_Total.Value, 2);
                                    objmovimientodetalleDto.v_NroPedido = itemProd.NroPedido;
                                    objmovimientodetalleDto.v_NroGuiaRemision = null;
                                    objmovimientodetalleDto.d_CantidadEmpaque = Utils.Windows.DevuelveValorRedondeado(new AlmacenBL().GetCantidadEmpaque(itemProd), 4);
                                    ListaMovimientos.Add(objmovimientodetalleDto);

                                }

                                if (Globals.ClientSession.v_RucEmpresa != Constants.RucJorplast)
                                {
                                   
                                    var prodSinMovDiccionario = ListaProductosSinMovimientos.ToDictionary(p => p.v_IdProducto, o => o);
                                    var CodigosProductosSinMovimientos = ListaProductosSinMovimientos.Select(x => x.v_IdProducto).ToList();
                                    var NuevosProd = CodigosProductosSinMovimientos.Except(ListaProductoStock.Select(x => x.v_IdProducto)).ToList();
                                    List<movimientodetalleDto> ListaFaltantes = NuevosProd.Select(idProducto =>
                                    {
                                        productodetalleDto pd;
                                        productoinventarioDto Inv;
                                        pd = prodSinMovDiccionario.TryGetValue(idProducto, out pd) ? pd : new productodetalleDto();
                                        return new movimientodetalleDto
                                        {
                                           
                                            v_IdProductoDetalle = pd.v_IdProductoDetalle,
                                            i_IdTipoDocumento = -1,
                                            v_NumeroDocumento = null,
                                            d_Cantidad = 0,
                                            i_IdUnidad = pd != null ? pd.i_ActualizaIdUsuario : -1,
                                            d_Precio = 0,
                                            d_Total = 0,
                                            v_NroPedido = null,
                                            v_NroGuiaRemision = null,
                                            d_CantidadEmpaque = 0,
                                        };
                                    }
                                        ).ToList();

                                    ListaMovimientos = ListaMovimientos.Concat(ListaFaltantes).ToList();
                                }

                                #endregion

                                objmovimientoDto.d_TotalCantidad = Utils.Windows.DevuelveValorRedondeado(ListaMovimientos.Sum(s => s.d_Cantidad).Value, 2);
                                objmovimientoDto.d_TotalPrecio = Utils.Windows.DevuelveValorRedondeado(ListaMovimientos.Sum(s => s.d_Total).Value, 2);
                                if (ListaMovimientos.Count() > 0)
                                {
                                    ListaMovimientos = ListaMovimientos.OrderBy(o => o.v_IdProductoDetalle).ToList();
                                    var hh = ListaMovimientos.Where(o => o.v_IdProductoDetalle == "N001-PE000001955").ToList();
                                    new MovimientoBL().InsertarMovimiento(ref objOperationResult, objmovimientoDto, Globals.ClientSession.GetAsList(), ListaMovimientos);
                                    if (objOperationResult.Success == 0) return;
                                    ListaMovimientos = new List<movimientodetalleDto>();
                                }
                            }


                        }

                        #endregion



                    }
                }, 
                _cts.Token)
                .ContinueWith(t =>
                {
                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    Cursor.Current = Cursors.Default;
                    if (objOperationResult.Success == 0)
                    {
                        if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                        {
                            UltraMessageBox.Show("Ocurrió un error al realizar Carga Inicial ," + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                        }
                        else
                        {
                            UltraMessageBox.Show("Ocurrió un error al realizar Carga Inicial," + objOperationResult.AdditionalInformation , "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(EstablecimientosCargaInicial))
                        {
                            UltraMessageBox.Show("Carga Inicial Finalizada...", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            UltraMessageBox.Show("Carga Inicial Finalizada...\n" + Mensaje + EstablecimientosCargaInicial, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }


                }
                    , TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void frmCargaInicial_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {

        }

        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            var objEstablecimientoBl = new EstablecimientoBL();
            var x = new List<KeyValueDTO>();
            string _whereAlmacenesConcatenados = null;
            string _almacenesConcatenados = null;
            if (cboEstablecimiento.Value == null || cboEstablecimiento.Value.ToString() == "-1")
            {
                cboAlmacen.Value = cboEstablecimiento.Value != null && cboEstablecimiento.Value.ToString() == "-1" ? -1 : Globals.ClientSession.i_IdAlmacenPredeterminado.Value;
                return;

            }
            x = objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString())).ToList();

            if (x.Count > 0)
            {
                foreach (var item in x.Where(l => l.Id != "1000"))
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _almacenesConcatenados = _almacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                _almacenesConcatenados = _almacenesConcatenados.Substring(0, _almacenesConcatenados.Length - 2);

            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = cboEstablecimiento.Value != null && cboEstablecimiento.Value.ToString() == "-1" ? -1 : Globals.ClientSession.i_IdAlmacenPredeterminado.Value;
        }

        private void cboAlmacen_ValueChanged(object sender, EventArgs e)
        {
            if (cboEstablecimiento.Value != null && cboEstablecimiento.Value.ToString() == "-1")
            {
                cboAlmacen.Value = "-1";
            }
        }
    }
}
