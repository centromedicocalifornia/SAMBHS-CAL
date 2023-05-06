using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Common.BE;
using SAMBHS.Venta.BL;


namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmGastos : Form
    {
        UltraCombo ucbGastosImport = new UltraCombo();
        UltraCombo ucbDocumento = new UltraCombo();
        UltraCombo ucbDocumentoRef = new UltraCombo();
        UltraCombo ucbCosto = new UltraCombo();
        UltraCombo ucbMoneda = new UltraCombo();
        UltraCombo ucbDetraccion = new UltraCombo();
        List<GridKeyValueDTO> _ListadoGastosImport = new List<GridKeyValueDTO>();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        ImportacionBL _objImportacionBL = new ImportacionBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        importaciondetallegastosDto _objImportaciondetallegastosDto = new importaciondetallegastosDto();
        importacionDto _objImportacionDto = new importacionDto();
        public decimal TotalSoles = 0;
        public decimal TotalDolares = 0;
        public DateTime FechaDua = new DateTime();
        public decimal ValorVentaAfectSoles = 0;
        public decimal ValorVentaAfecDolares = 0;
        public decimal Igv = 0;
        string strIdImportacion;
        public string strModo = "Nuevo";
        bool Salir = false;
        #region Temporales DetallesGastos
        public List<importaciondetallegastosDto> _TempDetalleGastosDto_Agregar = new List<importaciondetallegastosDto>();
        public List<importaciondetallegastosDto> _TempDetalleGastosDto_Modificar = new List<importaciondetallegastosDto>();
        public List<importaciondetallegastosDto> _TempDetalleGastosDto_Eliminar = new List<importaciondetallegastosDto>();
        public bool Eliminado = false;
        public bool ImportacionGuardada = false;
        #endregion
        public List<importaciondetallegastosDto> _ListaDetalleGastosDto_Grilla = new List<importaciondetallegastosDto>();
        public frmGastos(importacionDto ImportacionDto, string idImportacion, string strModos, List<importaciondetallegastosDto> ListaDetalleGastosGrilla, bool EliminadoGastos, decimal igv, DateTime fecha, bool Guardado)
        {
            strIdImportacion = idImportacion;
            InitializeComponent();
            strModo = strModos;
            _objImportacionDto = ImportacionDto;
            _ListaDetalleGastosDto_Grilla = ListaDetalleGastosGrilla;
            Eliminado = EliminadoGastos;
            Igv = igv;
            FechaDua = fecha;
            ImportacionGuardada = Guardado;

        }

        private void frmGastos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombosDetalles();
            if (ImportacionGuardada)
            {
                _ListaDetalleGastosDto_Grilla = new List<importaciondetallegastosDto>();

            }

            ObtenerListadoDetalleGastos();
        }

        private void ObtenerListadoDetalleGastos()
        {
            switch (strModo)
            {
                case "Edicion":
                    CargarDetalleGastos(strIdImportacion);
                    btnEliminarDetalleGastos.Enabled = false;
                    break;
                case "Nuevo":
                    CargarDetalleGastos("");
                    btnEliminarDetalleGastos.Enabled = false;
                    break;
                case "Guardado":
                    CargarDetalleGastos(strIdImportacion);
                    btnEliminarDetalleGastos.Enabled = false;
                    break;
            }

        }
        //private void CargarDetalleGastos(string pstridImportacion,ref List<importaciondetallegastosDto> ListaGastos)
        private void CargarDetalleGastos(string pstridImportacion)
        {
            OperationResult objOperationResult = new OperationResult();
            #region CargarDetalleGastos
            try
            {

                if (_ListaDetalleGastosDto_Grilla.Count() > 0)
                {
                    grdDataGastos.DataSource = _objImportacionBL.ObtenerImportacionDetallesGastos(ref objOperationResult, "");
                    foreach (var Fila in _ListaDetalleGastosDto_Grilla)
                    {
                        UltraGridRow row = grdDataGastos.DisplayLayout.Bands[0].AddNew();
                        grdDataGastos.Rows.Move(row, grdDataGastos.Rows.Count() - 1);
                        this.grdDataGastos.ActiveRowScrollRegion.ScrollRowIntoView(row);

                        row.Cells["v_GastoImportacion"].Value = Fila.v_GastoImportacion == null ? null : Fila.v_GastoImportacion.ToString();
                        row.Cells["v_IdAsientoContable"].Value = Fila.v_IdAsientoContable == null ? null : Fila.v_IdAsientoContable.ToString();
                        row.Cells["i_IdTipoDocumento"].Value = Fila.i_IdTipoDocumento == null ? -1 : int.Parse(Fila.i_IdTipoDocumento.ToString());
                        row.Cells["v_SerieDocumento"].Value = Fila.v_SerieDocumento == null ? null : Fila.v_SerieDocumento.ToString();
                        row.Cells["v_CorrelativoDocumento"].Value = Fila.v_CorrelativoDocumento == null ? null : Fila.v_CorrelativoDocumento.ToString();
                        row.Cells["t_FechaEmision"].Value = Fila.t_FechaEmision;
                        row.Cells["t_FechaRegistro"].Value = Fila.t_FechaRegistro;
                        row.Cells["d_TipoCambio"].Value = Fila.d_TipoCambio == null ? 0 : decimal.Parse(Fila.d_TipoCambio.ToString());
                        row.Cells["v_Detalle"].Value = Fila.v_Detalle == null ? null : Fila.v_Detalle.ToString();
                        row.Cells["i_IdMoneda"].Value = Fila.i_IdMoneda == null ? -1 : int.Parse(Fila.i_IdMoneda.ToString());
                        row.Cells["d_ValorVenta"].Value = Fila.d_ValorVenta == null ? 0 : decimal.Parse(Fila.d_ValorVenta.ToString());
                        row.Cells["d_NAfectoDetraccion"].Value = Fila.d_NAfectoDetraccion == null ? 0 : decimal.Parse(Fila.d_NAfectoDetraccion.ToString());
                        row.Cells["d_Igv"].Value = Fila.d_Igv == null ? 0 : decimal.Parse(Fila.d_Igv.ToString());
                        row.Cells["d_ImporteSoles"].Value = Fila.d_ImporteSoles == null ? 0 : decimal.Parse(Fila.d_ImporteSoles.ToString());
                        row.Cells["d_ImporteDolares"].Value = Fila.d_ImporteDolares == null ? 0 : decimal.Parse(Fila.d_ImporteDolares.ToString());
                        row.Cells["i_CCosto"].Value = Fila.i_CCosto == null || Fila.i_CCosto.Trim() == "" || Fila.i_CCosto == " " ? -1 : int.Parse(Fila.i_CCosto.ToString());
                        row.Cells["v_Glosa"].Value = Fila.v_Glosa == null ? null : Fila.v_Glosa.ToString();
                        row.Cells["i_IdTipoDocRef"].Value = Fila.i_IdTipoDocRef == null ? -1 : int.Parse(Fila.i_IdTipoDocRef.ToString());
                        row.Cells["v_SerieDocRef"].Value = Fila.v_SerieDocRef == null ? null : Fila.v_SerieDocRef.ToString();
                        row.Cells["v_CorrelativoDocRef"].Value = Fila.v_CorrelativoDocRef == null ? null : Fila.v_CorrelativoDocRef.ToString();
                        row.Cells["i_RegistroTipo"].Value = Fila.i_RegistroTipo == null ? null : Fila.i_RegistroTipo.ToString();
                        row.Cells["i_RegistroEstado"].Value = Fila.i_RegistroEstado == null ? null : Fila.i_RegistroEstado.ToString();
                        row.Cells["i_Eliminado"].Value = Fila.i_Eliminado == null ? null : Fila.i_Eliminado.ToString();
                        row.Cells["v_DetalleCodigo"].Value = Fila.v_DetalleCodigo == null ? null : Fila.v_DetalleCodigo;
                        row.Cells["i_EsDetraccion"].Value = Fila.i_EsDetraccion == null ? 0 : Fila.i_EsDetraccion == -1 ? 0 : Fila.i_EsDetraccion;
                        row.Cells["d_PorcentajeDetraccion"].Value = Fila.d_PorcentajeDetraccion ==null ?0 : decimal.Parse ( Fila.d_PorcentajeDetraccion.ToString ()) ;
                        row.Cells["v_NroDetraccion"].Value = Fila.v_NroDetraccion == null ? "" : Fila.v_NroDetraccion.Trim();
                        row.Cells["t_FechaDetraccion"].Value = Fila.t_FechaDetraccion;
                       // row.Cells["i_CodigoDetraccion"].Value = Fila.i_CodigoDetraccion == null ? 0 : int.Parse(Fila.i_CodigoDetraccion.Value.ToString());
                        row.Cells ["d_ValorSolesDetraccion"].Value  =Fila.d_ValorSolesDetraccion ==null ?0 : decimal.Parse (Fila.d_ValorSolesDetraccion.ToString ());

                        row.Cells ["d_ValorDolaresDetraccion"].Value = Fila.d_ValorDolaresDetraccion ==null ?0 : decimal.Parse ( Fila.d_ValorDolaresDetraccion.ToString ());

                        row.Cells ["d_ValorSolesDetraccionNoAfecto"].Value =Fila.d_ValorSolesDetraccionNoAfecto ==null ?0 : decimal.Parse (Fila.d_ValorSolesDetraccionNoAfecto.ToString ());
                        row.Cells["d_ValorDolaresDetraccionNoAfecto"].Value = Fila.d_ValorSolesDetraccionNoAfecto == null ? 0 : decimal.Parse(Fila.d_ValorDolaresDetraccionNoAfecto.ToString());
                                 

                        row.Cells["i_CodigoDetraccion"].Value = Fila.i_CodigoDetraccion == null ? 0 : int.Parse ( Fila.i_CodigoDetraccion.ToString());

                        if (row.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                        {
                            row.Cells["t_InsertaFecha"].Value = Fila.t_InsertaFecha.ToString();
                            row.Cells["i_InsertaIdUsuario"].Value = Fila.i_InsertaIdUsuario.ToString();
                        }
                        row.Cells["v_IdImportacion"].Value = Fila.v_IdImportacion == null ? null : Fila.v_IdImportacion.ToString();
                        row.Cells["v_IdImportacionDetalleGastos"].Value = Fila.v_IdImportacionDetalleGastos == null ? null : Fila.v_IdImportacionDetalleGastos.ToString();
                        CalcularValoresDetalles();
                    }
                    CalcularValoresDetalles();
                }
                else
                {
                    if (_ListaDetalleGastosDto_Grilla.Count() == 0 && Eliminado == true)
                    {
                        grdDataGastos.DataSource = _objImportacionBL.ObtenerImportacionDetallesGastos(ref objOperationResult, "");
                    }

                    else
                    {

                        grdDataGastos.DataSource = _objImportacionBL.ObtenerImportacionDetallesGastos(ref objOperationResult, pstridImportacion);

                        if (grdDataGastos.Rows.Count() > 0)
                        {
                            // Habilitar en Edicion
                            BuscarProveedores();
                            for (int i = 0; i < grdDataGastos.Rows.Count(); i++)
                            {
                                grdDataGastos.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                                grdDataGastos.Rows[i].Cells["i_RegistroEstado"].Value = "NoModificado";
                            }
                            CalcularValoresDetalles();
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar carga Datos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            #endregion


        }
        private void BuscarProveedores()
        {
            OperationResult objOperationResult = new OperationResult();
            List<string[]> ListaCadena = new List<string[]>();
            ListaCadena = _objImportacionBL.DevolverNombreProveedoresGastos(grdDataGastos.Rows[0].Cells["v_IdImportacion"].Value.ToString());
            for (int i = 0; i < ListaCadena.Count; i++)
            {
                grdDataGastos.Rows[i].Cells["v_DetalleCodigo"].Value = ((string[])ListaCadena[i])[1].Trim();
            }

        }
        private void CargarCombosDetalles()
        {
            OperationResult objOperationResult = new OperationResult();
            #region DetallesGastos

            //Configura Gastos Importacion
            _ListadoGastosImport = _objImportacionBL.ObtenGastosImportacionParaComboGrid(ref objOperationResult);
            UltraGridBand UltraGridBanda0 = new UltraGridBand("Banda 0", -1);
            UltraGridColumn ultraGridColumna0 = new UltraGridColumn("Id");
            ultraGridColumna0.Width = 40;
            UltraGridColumn ultraGridColumnaDescripcion0 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion0.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion0.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion0.Width = 160;
            // ultraGridColumnaDescripcion0.Hidden = true;
            UltraGridBanda0.Columns.AddRange(new object[] { ultraGridColumnaDescripcion0, ultraGridColumna0 });
            ucbGastosImport.DisplayLayout.BandsSerializer.Add(UltraGridBanda0);
            ucbGastosImport.DropDownWidth = 200;
            ucbGastosImport.DropDownStyle = UltraComboStyle.DropDownList;
            ucbGastosImport.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;

            Utils.Windows.LoadUltraComboList(ucbGastosImport, "Id", "Id", _ListadoGastosImport, DropDownListAction.Select);

            //Configura Combo TipoDocumento Referencia
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            UltraGridBand UltraGridBanda2 = new UltraGridBand("Banda 0", -1);
            UltraGridColumn ultraGridColumna20 = new UltraGridColumn("Id");
            ultraGridColumna20.Width = 40;
            UltraGridColumn ultraGridColumnaDescripcion21 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion21.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion21.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion21.Width = 130;
            UltraGridColumn ultraGridColumna22 = new UltraGridColumn("Value2");
            ultraGridColumna22.Header.Caption = "Siglas";
            ultraGridColumna22.Width = 100;
            UltraGridBanda2.Columns.AddRange(new object[] { ultraGridColumna20, ultraGridColumnaDescripcion21, ultraGridColumna22 });
            ucbDocumentoRef.DisplayLayout.BandsSerializer.Add(UltraGridBanda2);
            ucbDocumentoRef.DropDownWidth = 250;
            ucbDocumentoRef.DropDownStyle = UltraComboStyle.DropDownList;
            ucbDocumentoRef.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbDocumentoRef, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);

            //Configura Combo TipoDocumento
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            UltraGridBand UltraGridBanda1 = new UltraGridBand("Banda 0", -1);
            UltraGridColumn ultraGridColumna10 = new UltraGridColumn("Id");
            ultraGridColumna10.Width = 40;
            UltraGridColumn ultraGridColumnaDescripcion11 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion11.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion11.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion11.Width = 130;
            UltraGridColumn ultraGridColumna12 = new UltraGridColumn("Value2");
            ultraGridColumna12.Header.Caption = "Siglas";
            ultraGridColumna12.Width = 100;
            UltraGridBanda1.Columns.AddRange(new object[] { ultraGridColumna10, ultraGridColumnaDescripcion11, ultraGridColumna12 });
            ucbDocumento.DisplayLayout.BandsSerializer.Add(UltraGridBanda0);
            ucbDocumento.DropDownWidth = 250;
            ucbDocumento.DropDownStyle = UltraComboStyle.DropDownList;
            ucbDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);


            // Configura Centro de Costos
            UltraGridBand ultraGridBanda3 = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID30 = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion31 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion31.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion31.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion31.Width = 267;
            //ultraGridColumnaID.Hidden = true;
            ultraGridBanda3.Columns.AddRange(new object[] { ultraGridColumnaID30, ultraGridColumnaDescripcion31 });
            ucbCosto.DisplayLayout.BandsSerializer.Add(ultraGridBanda3);
            ucbCosto.DropDownWidth = 270;
            ucbCosto.DropDownStyle = UltraComboStyle.DropDownList;
            ucbCosto.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbCosto, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 31, null), DropDownListAction.Select); //Centro Costo -31


            // Configura Moneda
            UltraGridBand ultraGridBanda4 = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID40 = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion41 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion41.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion41.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion41.Width = 267;
            ultraGridColumnaID40.Hidden = true;
            ultraGridBanda4.Columns.AddRange(new object[] { ultraGridColumnaID40, ultraGridColumnaDescripcion41 });
            ucbMoneda.DisplayLayout.BandsSerializer.Add(ultraGridBanda4);
            ucbMoneda.DropDownWidth = 270;
            ucbMoneda.DropDownStyle = UltraComboStyle.DropDownList;
            ucbMoneda.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 18, null), DropDownListAction.Select); //Centro Costo -31

            //Configura EsDetraccion


            UltraGridBand Detraccion = new UltraGridBand("Band 0", -1);
            UltraGridColumn Detraccion1 = new UltraGridColumn("Id");
            UltraGridColumn Detraccion2 = new UltraGridColumn("Value1");
            Detraccion2.Header.Caption = "Descripción";
            Detraccion2.Header.VisiblePosition = 0;
            Detraccion2.Width = 267;
            Detraccion1.Hidden = true;
            Detraccion.Columns.AddRange(new object[] { Detraccion1, Detraccion2 });

            ucbDetraccion.DisplayLayout.BandsSerializer.Add(ultraGridBanda4);
            ucbDetraccion.DropDownWidth = 270;
            ucbDetraccion.DropDownStyle = UltraComboStyle.DropDownList;
            ucbDetraccion.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbDetraccion, "Value1", "Id", _objDatahierarchyBL.Respuestas(ref objOperationResult), DropDownListAction.Select); //Centro Costo -31




            #endregion


        }

        #region Grilla Detalle-Gastos
        private void LLenarTemporales()
        {
            if (grdDataGastos.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdDataGastos.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objImportaciondetallegastosDto = new importaciondetallegastosDto();
                                _objImportaciondetallegastosDto.v_IdImportacion = _objImportacionDto.v_IdImportacion;
                                Fila.Cells["v_IdImportacion"].Value = _objImportacionDto.v_IdImportacion == null ? null : _objImportacionDto.v_IdImportacion;
                                _objImportaciondetallegastosDto.v_GastoImportacion = Fila.Cells["v_GastoImportacion"].Value == null ? null : Fila.Cells["v_GastoImportacion"].Value.ToString();
                                _objImportaciondetallegastosDto.v_IdAsientoContable = Fila.Cells["v_IdAsientoContable"].Value == null ? null : Fila.Cells["v_IdAsientoContable"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _objImportaciondetallegastosDto.v_SerieDocumento = Fila.Cells["v_SerieDocumento"].Value == null ? null : Fila.Cells["v_SerieDocumento"].Value.ToString();
                                _objImportaciondetallegastosDto.v_CorrelativoDocumento = Fila.Cells["v_CorrelativoDocumento"].Value == null ? null : Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                                _objImportaciondetallegastosDto.t_FechaEmision = DateTime.Parse(Fila.Cells["t_FechaEmision"].Value.ToString());
                                _objImportaciondetallegastosDto.t_FechaRegistro = DateTime.Parse(Fila.Cells["t_FechaRegistro"].Value.ToString());
                                _objImportaciondetallegastosDto.d_TipoCambio = Fila.Cells["d_TipoCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString());
                                _objImportaciondetallegastosDto.v_Detalle = Fila.Cells["v_Detalle"].Value == null ? null : Fila.Cells["v_Detalle"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdMoneda = Fila.Cells["i_IdMoneda"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdMoneda"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _objImportaciondetallegastosDto.d_NAfectoDetraccion = Fila.Cells["d_NAfectoDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                _objImportaciondetallegastosDto.i_CCosto = Fila.Cells["i_CCosto"].Value == null ? "-1" : Fila.Cells["i_CCosto"].Value.ToString();
                                _objImportaciondetallegastosDto.v_Glosa = Fila.Cells["v_Glosa"].Value == null ? null : Fila.Cells["v_Glosa"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdTipoDocRef = Fila.Cells["i_IdTipoDocRef"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocRef"].Value.ToString());
                                _objImportaciondetallegastosDto.v_SerieDocRef = Fila.Cells["v_SerieDocRef"].Value == null ? null : Fila.Cells["v_SerieDocRef"].Value.ToString();
                                _objImportaciondetallegastosDto.v_CorrelativoDocRef = Fila.Cells["v_CorrelativoDocRef"].Value == null ? null : Fila.Cells["v_CorrelativoDocRef"].Value.ToString();
                                _objImportaciondetallegastosDto.v_DetalleCodigo = Fila.Cells["v_DetalleCodigo"].Value == null ? null : Fila.Cells["v_DetalleCodigo"].Value.ToString();
                                _objImportaciondetallegastosDto.i_EsDetraccion = Fila.Cells["i_EsDetraccion"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsDetraccion"].Value.ToString()) == -1 ? 0 : int.Parse(Fila.Cells["i_EsDetraccion"].Value.ToString());

                                _objImportaciondetallegastosDto.i_CodigoDetraccion = Fila.Cells["i_CodigoDetraccion"].Value == null ? 0 : int.Parse(Fila.Cells["i_CodigoDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_PorcentajeDetraccion = Fila.Cells["d_PorcentajeDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.v_NroDetraccion = Fila.Cells["v_NroDetraccion"].Value == null ? "" : Fila.Cells["v_NroDetraccion"].Value.ToString().Trim();
                                _objImportaciondetallegastosDto.t_FechaDetraccion = Fila.Cells["t_FechaDetraccion"].Value ==null ?DateTime.Now :  DateTime.Parse(Fila.Cells["t_FechaDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorSolesDetraccion = Fila.Cells["d_ValorSolesDetraccion"].Value  == null ? 0 : decimal.Parse(Fila.Cells["d_ValorSolesDetraccion"].Value.ToString ());

                                _objImportaciondetallegastosDto.d_ValorDolaresDetraccion = Fila.Cells["d_ValorDolaresDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorDolaresDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorSolesDetraccionNoAfecto = Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorDolaresDetraccionNoAfecto = Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value.ToString());
                                
                                _TempDetalleGastosDto_Agregar.Add(_objImportaciondetallegastosDto);

                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objImportaciondetallegastosDto = new importaciondetallegastosDto();
                                _objImportaciondetallegastosDto.v_IdImportacion = Fila.Cells["v_IdImportacion"].Value == null ? null : Fila.Cells["v_IdImportacion"].Value.ToString();
                                _objImportaciondetallegastosDto.v_IdImportacionDetalleGastos = Fila.Cells["v_IdImportacionDetalleGastos"].Value == null ? null : Fila.Cells["v_IdImportacionDetalleGastos"].Value.ToString();
                                _objImportaciondetallegastosDto.v_GastoImportacion = Fila.Cells["v_GastoImportacion"].Value == null ? null : Fila.Cells["v_GastoImportacion"].Value.ToString();
                                _objImportaciondetallegastosDto.v_IdAsientoContable = Fila.Cells["v_IdAsientoContable"].Value == null ? null : Fila.Cells["v_IdAsientoContable"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _objImportaciondetallegastosDto.v_SerieDocumento = Fila.Cells["v_SerieDocumento"].Value == null ? null : Fila.Cells["v_SerieDocumento"].Value.ToString();
                                _objImportaciondetallegastosDto.v_CorrelativoDocumento = Fila.Cells["v_CorrelativoDocumento"].Value == null ? null : Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                                _objImportaciondetallegastosDto.t_FechaEmision = DateTime.Parse(Fila.Cells["t_FechaEmision"].Value.ToString());
                                _objImportaciondetallegastosDto.t_FechaRegistro = DateTime.Parse(Fila.Cells["t_FechaRegistro"].Value.ToString());
                                _objImportaciondetallegastosDto.d_TipoCambio = Fila.Cells["d_TipoCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString());
                                _objImportaciondetallegastosDto.v_Detalle = Fila.Cells["v_Detalle"].Value == null ? null : Fila.Cells["v_Detalle"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdMoneda = Fila.Cells["i_IdMoneda"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdMoneda"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _objImportaciondetallegastosDto.d_NAfectoDetraccion = Fila.Cells["d_NAfectoDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                _objImportaciondetallegastosDto.i_CCosto = Fila.Cells["i_CCosto"].Value == null ? "-1" : Fila.Cells["i_CCosto"].Value.ToString();
                                _objImportaciondetallegastosDto.v_Glosa = Fila.Cells["v_Glosa"].Value == null ? null : Fila.Cells["v_Glosa"].Value.ToString();
                                _objImportaciondetallegastosDto.i_IdTipoDocRef = Fila.Cells["i_IdTipoDocRef"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocRef"].Value.ToString());
                                _objImportaciondetallegastosDto.v_SerieDocRef = Fila.Cells["v_SerieDocRef"].Value == null ? null : Fila.Cells["v_SerieDocRef"].Value.ToString();
                                _objImportaciondetallegastosDto.v_CorrelativoDocRef = Fila.Cells["v_CorrelativoDocRef"].Value == null ? null : Fila.Cells["v_CorrelativoDocRef"].Value.ToString();
                                _objImportaciondetallegastosDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objImportaciondetallegastosDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value.ToString());
                                _objImportaciondetallegastosDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _objImportaciondetallegastosDto.v_DetalleCodigo = Fila.Cells["v_DetalleCodigo"].Value == null ? null : Fila.Cells["v_DetalleCodigo"].Value.ToString();
                                _objImportaciondetallegastosDto.i_EsDetraccion = Fila.Cells["i_EsDetraccion"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsDetraccion"].Value.ToString()) == -1 ? 0 : int.Parse(Fila.Cells["i_EsDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.i_CodigoDetraccion = Fila.Cells["i_CodigoDetraccion"].Value == null ? 0 : int.Parse(Fila.Cells["i_CodigoDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_PorcentajeDetraccion = Fila.Cells["d_PorcentajeDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.v_NroDetraccion = Fila.Cells["v_NroDetraccion"].Value == null ? "" : Fila.Cells["v_NroDetraccion"].Value.ToString().Trim();
                                _objImportaciondetallegastosDto.t_FechaDetraccion = Fila.Cells["t_FechaDetraccion"].Value ==null ? DateTime.Now : DateTime.Parse(Fila.Cells["t_FechaDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorSolesDetraccion = Fila.Cells["d_ValorSolesDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorSolesDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorDolaresDetraccion = Fila.Cells["d_ValorDolaresDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorDolaresDetraccion"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorSolesDetraccionNoAfecto = Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value.ToString());
                                _objImportaciondetallegastosDto.d_ValorDolaresDetraccionNoAfecto = Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value.ToString());
                                

                                _TempDetalleGastosDto_Modificar.Add(_objImportaciondetallegastosDto);

                            }
                            break;
                    }
                }
            }



        }
        public List<importaciondetallegastosDto> LlenarListaGastos()
        {
            _ListaDetalleGastosDto_Grilla = new List<importaciondetallegastosDto>();

            if (grdDataGastos.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdDataGastos.Rows)
                {

                    _objImportaciondetallegastosDto = new importaciondetallegastosDto();
                    _objImportaciondetallegastosDto.v_GastoImportacion = Fila.Cells["v_GastoImportacion"].Value == null ? null : Fila.Cells["v_GastoImportacion"].Value.ToString();
                    _objImportaciondetallegastosDto.v_IdAsientoContable = Fila.Cells["v_IdAsientoContable"].Value == null ? null : Fila.Cells["v_IdAsientoContable"].Value.ToString();
                    _objImportaciondetallegastosDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                    _objImportaciondetallegastosDto.v_SerieDocumento = Fila.Cells["v_SerieDocumento"].Value == null ? null : Fila.Cells["v_SerieDocumento"].Value.ToString();
                    _objImportaciondetallegastosDto.v_CorrelativoDocumento = Fila.Cells["v_CorrelativoDocumento"].Value == null ? null : Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                    _objImportaciondetallegastosDto.t_FechaEmision = DateTime.Parse(Fila.Cells["t_FechaEmision"].Value.ToString());
                    _objImportaciondetallegastosDto.t_FechaRegistro = DateTime.Parse(Fila.Cells["t_FechaRegistro"].Value.ToString());
                    _objImportaciondetallegastosDto.d_TipoCambio = Fila.Cells["d_TipoCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString());
                    _objImportaciondetallegastosDto.v_Detalle = Fila.Cells["v_Detalle"].Value == null ? null : Fila.Cells["v_Detalle"].Value.ToString();
                    _objImportaciondetallegastosDto.i_IdMoneda = Fila.Cells["i_IdMoneda"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdMoneda"].Value.ToString());
                    _objImportaciondetallegastosDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                    _objImportaciondetallegastosDto.d_NAfectoDetraccion = Fila.Cells["d_NAfectoDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Value.ToString());
                    _objImportaciondetallegastosDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                    _objImportaciondetallegastosDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                    _objImportaciondetallegastosDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                    _objImportaciondetallegastosDto.i_CCosto = Fila.Cells["i_CCosto"].Value == null ? "-1" : Fila.Cells["i_CCosto"].Value.ToString();
                    _objImportaciondetallegastosDto.v_Glosa = Fila.Cells["v_Glosa"].Value == null ? null : Fila.Cells["v_Glosa"].Value.ToString();
                    _objImportaciondetallegastosDto.i_IdTipoDocRef = Fila.Cells["i_IdTipoDocRef"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocRef"].Value.ToString());
                    _objImportaciondetallegastosDto.v_SerieDocRef = Fila.Cells["v_SerieDocRef"].Value == null ? null : Fila.Cells["v_SerieDocRef"].Value.ToString();
                    _objImportaciondetallegastosDto.v_CorrelativoDocRef = Fila.Cells["v_CorrelativoDocRef"].Value == null ? null : Fila.Cells["v_CorrelativoDocRef"].Value.ToString();
                    _objImportaciondetallegastosDto.i_RegistroEstado = Fila.Cells["i_RegistroEstado"].Value == null ? null : Fila.Cells["i_RegistroEstado"].Value.ToString();
                    _objImportaciondetallegastosDto.i_RegistroTipo = Fila.Cells["i_RegistroTipo"].Value == null ? null : Fila.Cells["i_RegistroTipo"].Value.ToString();
                    _objImportaciondetallegastosDto.i_Eliminado = Fila.Cells["i_Eliminado"].Value == null ? 0 : int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                    _objImportaciondetallegastosDto.v_DetalleCodigo = Fila.Cells["v_DetalleCodigo"].Value == null ? null : Fila.Cells["v_DetalleCodigo"].Value.ToString();
                    _objImportaciondetallegastosDto.i_EsDetraccion = Fila.Cells["i_EsDetraccion"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsDetraccion"].Value.ToString()) == -1 ? 0 : int.Parse(Fila.Cells["i_EsDetraccion"].Value.ToString());
                    _objImportaciondetallegastosDto.i_CodigoDetraccion = Fila.Cells["i_CodigoDetraccion"].Value == null ? 0 : int.Parse(Fila.Cells["i_CodigoDetraccion"].Value.ToString());
                    _objImportaciondetallegastosDto.d_PorcentajeDetraccion = Fila.Cells["d_PorcentajeDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Value.ToString());
                    _objImportaciondetallegastosDto.v_NroDetraccion = Fila.Cells["v_NroDetraccion"].Value == null ? "" : Fila.Cells["v_NroDetraccion"].Value.ToString().Trim();
                    _objImportaciondetallegastosDto.t_FechaDetraccion = Fila.Cells["t_FechaDetraccion"].Value==null ?DateTime.Now :  DateTime.Parse(Fila.Cells["t_FechaDetraccion"].Value.ToString());

                    if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                    {
                        _objImportaciondetallegastosDto.t_InsertaFecha = DateTime.Parse(Fila.Cells["t_InsertaFecha"].Value.ToString());
                        _objImportaciondetallegastosDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                    }
                    _objImportaciondetallegastosDto.v_IdImportacion = Fila.Cells["v_IdImportacion"].Text == null ? null : Fila.Cells["v_IdImportacion"].Text.ToString();
                    _objImportaciondetallegastosDto.v_IdImportacionDetalleGastos = Fila.Cells["v_IdImportacionDetalleGastos"].Text == null ? null : Fila.Cells["v_IdImportacionDetalleGastos"].Text.ToString();
                    _ListaDetalleGastosDto_Grilla.Add(_objImportaciondetallegastosDto);

                }
                return _ListaDetalleGastosDto_Grilla;

            }
            else
            {

                return null;
            }

        }
        private void CalcularValoresDetalles()
        {
            if (grdDataGastos.Rows.Count() == 0)
            {
                decimal resultado = 0;
                txtTotalSoles.Text = resultado.ToString("0.00");
                txtTotalDolares.Text = resultado.ToString("0.00");
                TotalSoles = decimal.Parse(txtTotalSoles.Text);
                TotalDolares = decimal.Parse(txtTotalDolares.Text);
                return;

            }

            foreach (UltraGridRow Fila in grdDataGastos.Rows)
            {
                CalcularValoresFila(Fila);
            }

        }
        private void grdDataGastos_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminarDetalleGastos.Enabled = false;
            }
            else
            {
                //btnEliminar.Enabled = true;
                btnEliminarDetalleGastos.Enabled = true;
            }
        }
        private void grdDataGastos_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdDataGastos.ActiveCell == null) return;

            if (this.grdDataGastos.ActiveCell.Column.Key != "i_CCosto" && grdDataGastos.ActiveCell.Column.Key != "i_IdTipoDocRef" && grdDataGastos.ActiveCell.Column.Key != "i_IdMoneda" && grdDataGastos.ActiveCell.Column.Key != "i_IdTipoDocumento" && grdDataGastos.ActiveCell.Column.Key != "v_GastoImportacion")
            {

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        grdDataGastos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataGastos.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        grdDataGastos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataGastos.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Right:
                        grdDataGastos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataGastos.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataGastos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataGastos.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Enter:
                        DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdDataGastos.ActiveCell);
                        grdDataGastos_DoubleClickCell(sender, eventos);
                        e.Handled = true;
                        break;

                }

            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Right:
                        grdDataGastos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataGastos.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataGastos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataGastos.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }


            }
        }
        private void grdDataGastos_CellChange(object sender, CellEventArgs e)
        {
            grdDataGastos.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
            ImportacionGuardada = false;
        }
        private void grdDataGastos_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_IdAsientoContable":


                    frmPlanCuentasConsulta formPlanCuentas = new frmPlanCuentasConsulta(grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_IdAsientoContable"].Text == null ? "60" : grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_IdAsientoContable"].Text.ToString());
                    formPlanCuentas.ShowDialog();

                    if (formPlanCuentas._NroSubCuenta != null)
                    {
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_IdAsientoContable"].Value = formPlanCuentas._NroSubCuenta;
                        lblDescripcion.Text = formPlanCuentas._NombreCuenta;
                    }
                    break;

                case "v_DetalleCodigo":
                    if (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {

                        string codigoCliente = grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_DetalleCodigo"].Text == null ? "" : grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_DetalleCodigo"].Text.Trim();
                        Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(codigoCliente, "CODIGO");
                        frm.ShowDialog();
                        if (frm._IdProveedor != null)
                        {
                            grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_DetalleCodigo"].Value = frm._CodigoProveedor.ToString().Trim();
                            grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_Detalle"].Value = frm._IdProveedor.ToString().Trim();

                            lblDescripcion.Text = frm._RazonSocial;


                        }
                    }

                    break;





            }
        }
        private void grdDataGastos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["v_GastoImportacion"].EditorComponent = ucbGastosImport;
            e.Layout.Bands[0].Columns["v_GastoImportacion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].EditorComponent = ucbDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            e.Layout.Bands[0].Columns["i_IdTipoDocRef"].EditorComponent = ucbDocumentoRef;
            e.Layout.Bands[0].Columns["i_IdTipoDocRef"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            e.Layout.Bands[0].Columns["i_CCosto"].EditorComponent = ucbCosto;
            e.Layout.Bands[0].Columns["i_CCosto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            e.Layout.Bands[0].Columns["i_IdMoneda"].EditorComponent = ucbMoneda;
            e.Layout.Bands[0].Columns["i_IdMoneda"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;


            e.Layout.Bands[0].Columns["i_EsDetraccion"].EditorComponent = ucbDetraccion;
            e.Layout.Bands[0].Columns["i_EsDetraccion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;


        }
        private void grdDataGastos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }
        private void grdDataGastos_AfterExitEditMode(object sender, EventArgs e)
        {
            CalcularValoresFila(grdDataGastos.Rows[grdDataGastos.ActiveRow.Index]);
           
            switch (grdDataGastos.ActiveCell.Column.Key)
            {

                case "v_SerieDocumento":
                    if (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_SerieDocumento"].Value != null && grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString() != string.Empty)
                    {
                        string Serie;
                        Serie = grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString();
                        // grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_SerieDocumento"].Value = (int.Parse(Serie)).ToString("0000");

                        if (Serie != string.Empty)
                        {
                            int Leng = Serie.Trim().Length, i = 2;
                            string CadenaCeros = "0";
                            if (Leng < 4)
                            {
                                while (i <= (4 - Leng))
                                {
                                    CadenaCeros = CadenaCeros + "0";
                                    i = i + 1;
                                }
                                grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_SerieDocumento"].Value = CadenaCeros + Serie.Trim();
                            }
                        }



                    }

                    break;


                case "v_CorrelativoDocumento":
                    if (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value != null && grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString() != string.Empty)
                    {
                        string Correlativo;
                        Correlativo = grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString();
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value = (int.Parse(Correlativo)).ToString("00000000");

                    }
                    break;

                case "i_EsDetraccion":
                    if (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_EsDetraccion"].Value.ToString() == "-1" || grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_EsDetraccion"].Value.ToString() == "0")
                    {
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_CodigoDetraccion"].Value = "0";
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["d_PorcentajeDetraccion"].Value = "0";
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["d_ValorSolesDetraccion"].Value = "0";
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["d_ValorDolaresDetraccion"].Value = "0";
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["d_ValorSolesDetraccionNoAfecto"].Value = "0";
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["d_ValorDolaresDetraccionNoAfecto"].Value = "0";
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_NroDetraccion"].Value = "";
                    }
                    break;

              

               

            }

        }
        private void CalcularValoresFila(UltraGridRow Fila)
        {

            if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
            if (Fila.Cells["d_NAfectoDetraccion"].Value == null) { Fila.Cells["d_NAfectoDetraccion"].Value = "0"; }
            if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
            if (Fila.Cells["d_ImporteDolares"].Value == null) { Fila.Cells["d_ImporteDolares"].Value = "0"; }
            if (Fila.Cells["d_ImporteSoles"].Value == null) { Fila.Cells["d_ImporteSoles"].Value = "0"; }
            if (Fila.Cells["d_TipoCambio"].Value == null) { Fila.Cells["d_TipoCambio"].Value = "0"; }
            if (Fila.Cells["i_IdMoneda"].Value == null) { return; }
            if (Fila.Cells["d_PorcentajeDetraccion"].Value == null) { Fila.Cells["d_PorcentajeDetraccion"].Value = "0"; } 

            if (Igv != 0)
            {
                Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Text) * (Igv / 100), 2);
            }
            if (Fila.Cells["i_IdMoneda"].ToString() == "-1") { return; }


            switch (Fila.Cells["i_IdMoneda"].Value.ToString())
            {


                case "1":   //Soles
                    //igv = (decimal.Parse(Fila.Cells["d_Igv"].Value.ToString()) * decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) / 100);
                    Fila.Cells["d_ImporteSoles"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text) + decimal.Parse(Fila.Cells["d_Igv"].Text), 2);
                    Fila.Cells["d_ImporteDolares"].Value = decimal.Parse(Fila.Cells["d_TipoCambio"].Text.ToString()) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ImporteSoles"].Text) / decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2);

                    if (Fila.Cells["i_EsDetraccion"].Value.ToString() == "1")
                    {

                        var calculoDetraccion=( decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_Igv"].Text)) * (decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Text) / 100) ;
                        var calculoNoAfecto = decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text) * (decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Text) / 100 );
                        Fila.Cells["d_ValorSolesDetraccion"].Value = Utils.Windows.DevuelveValorRedondeado(calculoDetraccion, 0);
                        Fila.Cells["d_ValorDolaresDetraccion"].Value = decimal.Parse(Fila.Cells["d_TipoCambio"].Text.ToString()) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(calculoDetraccion / decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2);
                        Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value = Utils.Windows.DevuelveValorRedondeado(calculoNoAfecto, 0);
                        Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value = decimal.Parse(Fila.Cells["d_TipoCambio"].Text.ToString()) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(calculoNoAfecto / decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2);


                    }
                    else
                    {
                        Fila.Cells["d_ValorSolesDetraccion"].Value = "0";
                        Fila.Cells["d_ValorDolaresDetraccion"].Value = "0";
                        Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value = "0";
                        Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value = "0";

                    }
                    
                    break;

                case "2": //Dolares

           
                    var calculoDetraccionD=( decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_Igv"].Text)) * (decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Text) / 100) ;
                     var calculoNoAfectoD = decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text) * (decimal.Parse(Fila.Cells["d_PorcentajeDetraccion"].Text) / 100 );

                    Fila.Cells["d_ImporteDolares"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text) + decimal.Parse(Fila.Cells["d_Igv"].Text), 2);
                    Fila.Cells["d_ImporteSoles"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ImporteDolares"].Text) * decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2);

                    if (Fila.Cells["i_EsDetraccion"].Value.ToString() == "1")
                    {

                        Fila.Cells["d_ValorDolaresDetraccion"].Value = Utils.Windows.DevuelveValorRedondeado(calculoDetraccionD, 0);
                        Fila.Cells["d_ValorSolesDetraccion"].Value = decimal.Parse(Fila.Cells["d_TipoCambio"].Text.ToString()) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(calculoDetraccionD / decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2);
                        Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value = Utils.Windows.DevuelveValorRedondeado(calculoNoAfectoD, 0);
                        Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value = decimal.Parse(Fila.Cells["d_TipoCambio"].Text.ToString()) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(calculoNoAfectoD / decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2);


                    }
                    else
                    {
                        Fila.Cells["d_ValorSolesDetraccion"].Value = "0";
                        Fila.Cells["d_ValorDolaresDetraccion"].Value = "0";
                        Fila.Cells["d_ValorSolesDetraccionNoAfecto"].Value = "0";
                        Fila.Cells["d_ValorDolaresDetraccionNoAfecto"].Value = "0";

                    }
                    
                    
                    
                    
                    
                    break;
            }

            CalcularTotales();

        }
        private void CalcularTotales()
        {
            decimal pTotalSoles = 0;
            decimal pTotalDolares = 0;
            decimal pVentaAfectoDolares = 0;
            decimal pVentaAfectoSoles = 0;

            if (grdDataGastos.Rows.Count() > 0)
            {
                foreach (UltraGridRow Fila in grdDataGastos.Rows)
                {
                    if (Fila.Cells["d_ImporteDolares"].Value == null) { Fila.Cells["d_ImporteDolares"].Value = "0"; }
                    if (Fila.Cells["d_ImporteSoles"].Value == null) { Fila.Cells["d_ImporteSoles"].Value = "0"; }
                    if (Fila.Cells["i_IdMoneda"].Value == null) { return; }
                    if (Fila.Cells["i_IdMoneda"].ToString() == "-1") { return; }
                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_NAfectoDetraccion"].Value == null) { Fila.Cells["d_NAfectoDetraccion"].Value = "0"; }
                    if (Fila.Cells["d_TipoCambio"].Value == null) { Fila.Cells["d_TipoCambio"].Value = "0"; }

                    switch (Fila.Cells["i_IdMoneda"].Value.ToString())
                    {
                        case "1"://Soles

                            pTotalSoles = pTotalSoles + decimal.Parse(Fila.Cells["d_ImporteSoles"].Text);
                            pTotalDolares = pTotalDolares + decimal.Parse(Fila.Cells["d_ImporteDolares"].Text);
                            pVentaAfectoSoles = decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text) + pVentaAfectoSoles;
                            pVentaAfectoDolares = Fila.Cells["d_TipoCambio"].Value.ToString() == "0" ? pVentaAfectoDolares : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text)) / decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2) + pVentaAfectoDolares;
                            pVentaAfectoSoles = Utils.Windows.DevuelveValorRedondeado(pVentaAfectoSoles, 2);

                            break;
                        case "2": //Dolares
                            pTotalSoles = pTotalSoles + decimal.Parse(Fila.Cells["d_ImporteSoles"].Text);
                            pTotalDolares = pTotalDolares + decimal.Parse(Fila.Cells["d_ImporteDolares"].Text);
                            pVentaAfectoDolares = decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text) + pVentaAfectoDolares;
                            pVentaAfectoSoles = Fila.Cells["d_TipoCambio"].Value.ToString() == "0" ? pVentaAfectoSoles : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorVenta"].Text) + decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Text)) * decimal.Parse(Fila.Cells["d_TipoCambio"].Text), 2) + pVentaAfectoSoles;
                            pVentaAfectoDolares = Utils.Windows.DevuelveValorRedondeado(pVentaAfectoDolares, 2);
                            break;
                    }

                }
                txtTotalSoles.Text = pTotalSoles.ToString("0.00");
                txtTotalDolares.Text = pTotalDolares.ToString("0.00");
                //TotalSoles = pTotalSoles;
                //TotalDolares = pTotalDolares;
                TotalSoles = pVentaAfectoSoles;
                TotalDolares = pVentaAfectoDolares;
            }



        }
        #endregion
        #region ComportamientoControles
        private void btnSalir_Click(object sender, EventArgs e)
        {


            this.Close();


        }

        private bool ValidarVaciosNulos()
        {

            OperationResult objOperationResult = new OperationResult();
            if (grdDataGastos.Rows.Where(p => p.Cells["v_IdAsientoContable"].Value == null || p.Cells["v_IdAsientoContable"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente las Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["v_IdAsientoContable"].Value == null || x.Cells["v_IdAsientoContable"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["v_IdAsientoContable"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_IdAsientoContable"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }

            if (grdDataGastos.Rows.Where(p => p.Cells["v_GastoImportacion"].Value == null || p.Cells["v_GastoImportacion"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los Códigos Gastos Importación", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["v_GastoImportacion"].Value == null || x.Cells["v_GastoImportacion"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["v_GastoImportacion"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_GastoImportacion"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }

            if (grdDataGastos.Rows.Where(p => p.Cells["i_IdTipoDocumento"].Value == null || p.Cells["i_IdTipoDocumento"].Value.ToString().Trim() == "-1").Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los Tipos Documentos", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["i_IdTipoDocumento"].Value == null || x.Cells["i_IdTipoDocumento"].Value.ToString().Trim() == "-1").FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["i_IdTipoDocumento"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["i_IdTipoDocumento"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }

            if (grdDataGastos.Rows.Where(p => p.Cells["v_SerieDocumento"].Value == null || p.Cells["v_SerieDocumento"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente las Series", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["v_SerieDocumento"].Value == null || x.Cells["v_SerieDocumento"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["v_SerieDocumento"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_SerieDocumento"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }

            if (grdDataGastos.Rows.Where(p => p.Cells["v_CorrelativoDocumento"].Value == null || p.Cells["v_CorrelativoDocumento"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los Correlativos", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["v_CorrelativoDocumento"].Value == null || x.Cells["v_CorrelativoDocumento"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["v_CorrelativoDocumento"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_CorrelativoDocumento"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }


            if (grdDataGastos.Rows.Where(p => p.Cells["v_Detalle"].Value == null || p.Cells["v_Detalle"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los Detalles", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["v_Detalle"].Value == null || x.Cells["v_Detalle"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["v_DetalleCodigo"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_DetalleCodigo"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }

            if (grdDataGastos.Rows.Where(p => p.Cells["d_TipoCambio"].Value == null || p.Cells["d_TipoCambio"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_TipoCambio"].Value.ToString().Trim()) <= 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los Tipos de Cambios", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["d_TipoCambio"].Value == null || x.Cells["d_TipoCambio"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_TipoCambio"].Value.ToString().Trim()) <= 0).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["d_TipoCambio"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["d_TipoCambio"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }


            if (grdDataGastos.Rows.Where(p => p.Cells["i_IdMoneda"].Value == null || p.Cells["i_IdMoneda"].Value.ToString().Trim() == string.Empty || p.Cells["i_IdMoneda"].Value.ToString().Trim() == "-1").Count() != 0)
            {
                UltraMessageBox.Show("Por favor seleccione  correctamente las Monedas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(p => p.Cells["i_IdMoneda"].Value == null || p.Cells["i_IdMoneda"].Value.ToString().Trim() == string.Empty || p.Cells["i_IdMoneda"].Value.ToString().Trim() == "-1").FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["i_IdMoneda"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["i_IdMoneda"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }

            if (grdDataGastos.Rows.Where(p => p.Cells["v_Glosa"].Value == null || p.Cells["v_Glosa"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente la Glosa", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataGastos.Rows.Where(p => p.Cells["v_Glosa"].Value == null || p.Cells["v_Glosa"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataGastos.Selected.Cells.Add(Row.Cells["v_Glosa"]);
                grdDataGastos.Focus();
                Row.Activate();
                grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_Glosa"];
                this.grdDataGastos.ActiveCell = aCell;
                return false;
            }
            foreach (var Fila in grdDataGastos.Rows)
            {

                decimal Nafecto = decimal.Parse(Fila.Cells["d_NAfectoDetraccion"].Value.ToString());
                decimal Venta = decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                decimal Igv = decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());

                if (Venta == 0 && Igv == 0 && Nafecto == 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente los Valores", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    grdDataGastos.Selected.Cells.Add(Fila.Cells["d_ValorVenta"]);
                    grdDataGastos.Focus();
                    Fila.Activate();
                    grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["d_ValorVenta"];
                    this.grdDataGastos.ActiveCell = aCell;
                    return false;


                }
                else if (Venta == 0 && Igv == 0)
                {
                    if (Nafecto == 0)
                    {
                        UltraMessageBox.Show("Por favor ingrese correctamente los Valores de No Afecto ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        grdDataGastos.Selected.Cells.Add(Fila.Cells["d_NAfectoDetraccion"]);
                        grdDataGastos.Focus();
                        Fila.Activate();
                        grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["d_NAfectoDetraccion"];
                        this.grdDataGastos.ActiveCell = aCell;
                        return false;
                    }


                }
                else if (Nafecto == 0)
                {
                    if (Venta == 0 || Igv == 0)
                    {

                        UltraMessageBox.Show("Por favor ingrese correctamente los Valores de No Afecto ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        grdDataGastos.Selected.Cells.Add(Fila.Cells["d_NAfectoDetraccion"]);
                        grdDataGastos.Focus();
                        Fila.Activate();
                        grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["d_NAfectoDetraccion"];
                        this.grdDataGastos.ActiveCell = aCell;
                        return false;

                    }


                }

                else if (Igv == 0)
                {
                    if (Venta == 0 || Nafecto == 0)
                    {

                        UltraMessageBox.Show("Por favor ingrese correctamente los Valores de No Afecto ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        grdDataGastos.Selected.Cells.Add(Fila.Cells["d_NAfectoDetraccion"]);
                        grdDataGastos.Focus();
                        Fila.Activate();
                        grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["d_NAfectoDetraccion"];
                        this.grdDataGastos.ActiveCell = aCell;
                        return false;

                    }

                }

            }

            foreach (var Fila in grdDataGastos.Rows)
            {
                string Cuenta = Fila.Cells["v_IdAsientoContable"].Value.ToString().Trim();
                if (!_objImportacionBL.ValidarCuenta(ref objOperationResult, Fila.Cells["v_IdAsientoContable"].Value.ToString()))
                {

                    UltraMessageBox.Show("Por favor ingrese correctamente las Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["v_IdAsientoContable"].Value == null || x.Cells["v_IdAsientoContable"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                    grdDataGastos.Selected.Cells.Add(Fila.Cells["v_IdAsientoContable"]);
                    grdDataGastos.Focus();
                    Fila.Activate();
                    grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_IdAsientoContable"];
                    this.grdDataGastos.ActiveCell = aCell;
                    return false;
                }

                if (Fila.Cells["i_EsDetraccion"].Value.ToString() == "1" && (Fila.Cells["v_NroDetraccion"].Value == null || Fila.Cells["v_NroDetraccion"].Value.ToString().Trim() == ""))
                {
                    UltraMessageBox.Show("Por favor ingrese el número detracción", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["i_EsDetraccion"].Value.ToString() == "1" && (x.Cells["v_NroDetraccion"].Value == null || x.Cells["v_NroDetraccion"].Value.ToString().Trim() == "")).FirstOrDefault();
                    grdDataGastos.Selected.Cells.Add(Fila.Cells["v_NroDetraccion"]);
                    grdDataGastos.Focus();
                    Fila.Activate();
                    grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_NroDetraccion"];
                    this.grdDataGastos.ActiveCell = aCell;
                    return false;

                }




                if (Fila.Cells["i_EsDetraccion"].Value.ToString() == "1" && (Fila.Cells["d_PorcentajeDetraccion"].Value == null || Fila.Cells["d_PorcentajeDetraccion"].Value.ToString().Trim() == "0"))
                {
                    UltraMessageBox.Show("Por favor ingrese el porcentaje detracción", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataGastos.Rows.Where(x => x.Cells["i_EsDetraccion"].Value.ToString() == "1" && (x.Cells["d_PorcentajeDetraccion"].Value == null || x.Cells["d_PorcentajeDetraccion"].Value.ToString().Trim() == "0")).FirstOrDefault();
                    grdDataGastos.Selected.Cells.Add(Fila.Cells["d_PorcentajeDetraccion"]);
                    grdDataGastos.Focus();
                    Fila.Activate();
                    grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["d_PorcentajeDetraccion"];
                    this.grdDataGastos.ActiveCell = aCell;
                    return false;

                }



            }

            return true;

        }
        private void frmGastos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Salir)
            {
                //btnSalir_Click(sender, e);
                if (ValidarVaciosNulos())
                {
                    LLenarTemporales();
                    LlenarListaGastos();
                    //this.Close();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void btnEliminarDetalleGastos_Click(object sender, EventArgs e)
        {
            if (grdDataGastos.ActiveRow == null) return;

            if (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_RegistroTipo"].Value != null && grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Sistemas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    _objImportaciondetallegastosDto = new importaciondetallegastosDto();
                    _objImportaciondetallegastosDto.v_IdImportacionDetalleGastos = grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_IdImportacionDetalleGastos"].Value.ToString();
                    _TempDetalleGastosDto_Eliminar.Add(_objImportaciondetallegastosDto);
                    grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Delete(false);


                    CalcularValoresDetalles();
                }
                Eliminado = true;
            }
            else
            {
                grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Delete(false);
                CalcularValoresDetalles();
                Eliminado = true;
            }
        }

        private void btnAgregarDetalleGastos_Click(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (grdDataGastos.ActiveRow != null)
            {

                if (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_GastoImportacion"].Value != null)
                {
                    UltraGridRow row = grdDataGastos.DisplayLayout.Bands[0].AddNew();
                    grdDataGastos.Rows.Move(row, grdDataGastos.Rows.Count() - 1);
                    this.grdDataGastos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    //row.Cells["i_RegistroEstado"].Value = "Agregado";
                    //row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["v_GastoImportacion"].Value = "-1";
                    row.Cells["t_FechaEmision"].Value = FechaDua;
                    row.Cells["t_FechaRegistro"].Value = FechaDua;
                    row.Cells["t_FechaDetraccion"].Value = FechaDua;
                    DateTime Fechas = Convert.ToDateTime(row.Cells["t_FechaEmision"].Value.ToString());
                    row.Cells["d_TipoCambio"].Value = decimal.Parse(_objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, Fechas.Date).ToString());
                    row.Cells["i_IdTipoDocRef"].Value = "-1";
                    row.Cells["i_IdMoneda"].Value = "-1";
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Cells["v_GastoImportacion"].Value = "0100";
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_CCosto"].Value = "-1";
                    row.Cells["i_EsDetraccion"].Value = "0";
                    row.Cells["d_PorcentajeDetraccion"].Value = "0";
                    row.Cells["i_IdMoneda"].Value = Globals.ClientSession.i_IdMoneda.ToString();

                }


            }
            else
            {
                UltraGridRow row = grdDataGastos.DisplayLayout.Bands[0].AddNew();
                grdDataGastos.Rows.Move(row, grdDataGastos.Rows.Count() - 1);
                this.grdDataGastos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["v_GastoImportacion"].Value = "-1";
                row.Cells["t_FechaEmision"].Value = FechaDua;
                row.Cells["t_FechaRegistro"].Value = FechaDua;
                row.Cells["t_FechaDetraccion"].Value = FechaDua;
                DateTime Fechas = Convert.ToDateTime(row.Cells["t_FechaEmision"].Value.ToString());
                row.Cells["d_TipoCambio"].Value = decimal.Parse(_objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, Fechas.Date).ToString());
                row.Cells["i_CCosto"].Value = "-1";
                row.Cells["i_IdTipoDocRef"].Value = "-1";
                row.Cells["i_IdMoneda"].Value = "-1";
                row.Cells["i_IdTipoDocumento"].Value = "-1";
                row.Cells["v_GastoImportacion"].Value = "0100";
                row.Cells["i_EsDetraccion"].Value = "0";
                row.Cells["d_PorcentajeDetraccion"].Value = "0";
                row.Cells["i_IdMoneda"].Value = Globals.ClientSession.i_IdMoneda.ToString();

            }

            UltraGridCell aCell = this.grdDataGastos.ActiveRow.Cells["v_GastoImportacion"];
            this.grdDataGastos.ActiveCell = aCell;
            grdDataGastos.PerformAction(UltraGridAction.EnterEditMode, false, false);
            grdDataGastos.Focus();
            grdDataGastos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);

        }

        #endregion

        private void frmGastos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (!Salir)
                {
                    btnSalir_Click(sender, e);
                }
                //this.Close();
            }
        }

        private void grdDataGastos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (grdDataGastos.ActiveRow == null) return;

            switch (e.Cell.Column.Key)
            {

                case "t_FechaEmision":
                    DateTime Fechas = Convert.ToDateTime(grdDataGastos.ActiveRow.Cells["t_FechaEmision"].Value.ToString());
                    grdDataGastos.ActiveRow.Cells["d_TipoCambio"].Value = decimal.Parse(_objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, Fechas.Date).ToString());
                    break;


                case "v_GastoImportacion":
                    // grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_IdAsientoContable"].Value =int.Parse (grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_GastoImportacion"].Value.ToString () )== -1 ?"" :   _ListadoGastosImport.Where(x => x.Id == grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["v_GastoImportacion"].Value.ToString()).FirstOrDefault().Value2.ToString();
                    string codGasto = grdDataGastos.ActiveRow.Cells["v_GastoImportacion"].Value.ToString();
                    var gastito = _ListadoGastosImport.Where(x => x.Id == codGasto);
                    grdDataGastos.ActiveRow.Cells["v_IdAsientoContable"].Value = int.Parse(grdDataGastos.ActiveRow.Cells["v_GastoImportacion"].Value.ToString()) == -1 ? "" : gastito != null && gastito.Any() ? gastito.FirstOrDefault().Value2.ToString() : "";
                    break;

                case "t_FechaRegistro":
                    grdDataGastos.ActiveRow.Cells["t_FechaEmision"].Value = grdDataGastos.ActiveRow.Cells["t_FechaRegistro"].Value;
                    break;

                case "i_CodigoDetraccion":
                    CalcularValoresFila(grdDataGastos.Rows[grdDataGastos.ActiveRow.Index]);
                   
                    break;

            }
        }

        private void grdDataGastos_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (grdDataGastos.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdDataGastos.ActiveCell.Column.Key)
                {
                    case "d_TipoCambio":

                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;

                    case "d_ValorVenta":
                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;

                    case "d_NAfectoDetraccion":
                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;

                    case "d_Igv":
                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;

                    case "v_IdAsientoContable":
                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);

                        break;


                    case "v_SerieDocumento":
                        //Celda = grdDataGastos.ActiveCell;
                        //Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;

                    case "v_CorrelativoDocumento":
                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);

                        break;
                    case "v_SerieDocRef":
                        //Celda = grdDataGastos.ActiveCell;
                        //Utils.Windows.NumeroEnteroCelda(Celda, e);

                        break;
                    case "v_CorrlativoDocRef":
                        //Celda = grdDataGastos.ActiveCell;
                        //Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;

                    case "d_PorcentajeDetraccion":
                        Celda = grdDataGastos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda (Celda, e);
                        break;


                }
            }


        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {

            foreach (UltraGridRow Fila in grdDataGastos.Rows)
            {

                CalcularValoresFila(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
            UltraMessageBox.Show("Cálculos Finalizados");
        }

        private void grdDataGastos_ClickCell(object sender, ClickCellEventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (e.Cell.Column.Key == "v_GastoImportacion" || e.Cell.Column.Key == "v_IdAsientoContable" || e.Cell.Column.Key == "i_CCosto" || e.Cell.Column.Key == "v_DetalleCodigo")
            {
                switch (e.Cell.Column.Key)
                {
                    case "v_IdAsientoContable":
                        var cta = e.Cell.Value != null ? Utils.Windows.DevuelveCuentaDatos(e.Cell.Value.ToString()) : null;
                        lblDescripcion.Text = cta != null ? cta.v_NombreCuenta : string.Empty;

                        break;



                    case "i_CCosto":
                        var ccosto = grdDataGastos.ActiveRow.Cells["i_CCosto"].Value != null && grdDataGastos.ActiveRow.Cells["i_CCosto"].Value.ToString() != "-1" ? _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 31, null) : null;
                        var costito = ccosto != null && ccosto.Any() ? ccosto.Where(x => x.Id == grdDataGastos.ActiveRow.Cells["v_GastoImportacion"].Value.ToString()) : null;
                        lblDescripcion.Text = costito != null && costito.Any() ? costito.FirstOrDefault().Value1 : "";

                        break;

                    case "v_GastoImportacion":
                        string codGasto = grdDataGastos.ActiveRow.Cells["v_GastoImportacion"].Value == null ? "" : grdDataGastos.ActiveRow.Cells["v_GastoImportacion"].Value.ToString();
                        var gastito = _ListadoGastosImport.Where(x => x.Id == codGasto);
                        lblDescripcion.Text = gastito != null && gastito.Any() ? gastito.FirstOrDefault().Value1.ToString() : string.Empty;
                        break;


                    case "v_DetalleCodigo":

                        string IdCliente = grdDataGastos.ActiveRow.Cells["v_Detalle"].Value != null ? grdDataGastos.ActiveRow.Cells["v_Detalle"].Value.ToString() : string.Empty;


                        if (!string.IsNullOrEmpty(IdCliente))
                        {
                            string[] Cadena = new string[4];
                            Cadena = new VentaBL().DevolverClientePorIdCliente(ref objOperationResult, IdCliente);
                            lblDescripcion.Text = Cadena != null ? Cadena[2] : string.Empty;
                        }
                        else
                        {
                            lblDescripcion.Clear();
                        }
                        break;

                    default:
                        lblDescripcion.Clear();
                        break;
                }
            }
        }

        private void grdDataGastos_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {

                case "i_CodigoDetraccion":

                    Mantenimientos.frmBuscarDatahierarchy frmDetraccion = new Mantenimientos.frmBuscarDatahierarchy(29, "Buscar Detracción");
                    frmDetraccion.ShowDialog();
                    if (frmDetraccion._itemId != null)
                    {
                        //txtCodigoDetraccion.Text = frm._itemId;
                        //txtNombreDetraccion.Text = frm._value1;
                        //txtPorcDetraccion.Text = frm._value2;
                        //CalcularDetraccionesDetalle();
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_CodigoDetraccion"].Value = frmDetraccion._itemId;
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["d_PorcentajeDetraccion"].Value = frmDetraccion._value2;
                        CalcularValoresFila(grdDataGastos.Rows[grdDataGastos.ActiveRow.Index]);
                        grdDataGastos.Rows[grdDataGastos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    }


                    break;
            }
        }

        //private void CalcularDetraccionesDetalle(int ValorVentaIgv, int NoAfecto, int PorcentajeDetraccion, int TipoCambio, int Moneda, out decimal DetraccionSoles, out decimal DetraccionDolares, out decimal DetraccionSolesNoAfecto, out decimal DetraccionDolaresNoAfecto)
        //{
        //    try
        //    {

        //        if (Moneda == 1)
        //        {
        //            DetraccionSoles = ValorVentaIgv * (PorcentajeDetraccion / 100);
        //            DetraccionDolares = TipoCambio != 0 ? DetraccionSoles / TipoCambio : 0;
        //            DetraccionSolesNoAfecto = NoAfecto * (PorcentajeDetraccion / 100);
        //            DetraccionDolaresNoAfecto = TipoCambio != 0 ? DetraccionSolesNoAfecto / TipoCambio : 0;

        //        }
        //        else
        //        {
        //            DetraccionDolares = ValorVentaIgv * (PorcentajeDetraccion / 100);
        //            DetraccionSoles = TipoCambio != 0 ? DetraccionDolares * TipoCambio : 0;
        //            DetraccionSolesNoAfecto = NoAfecto * (PorcentajeDetraccion / 100);
        //            DetraccionDolaresNoAfecto = TipoCambio != 0 ? DetraccionDolares * TipoCambio : 0;
        //        }

        //        DetraccionSoles = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(DetraccionSoles.ToString()), 0);
        //        DetracionDolares = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(DetraccionDolares.ToString()), 2);

        //        DetraccionSolesNoAfecto = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(DetraccionSolesNoAfecto.ToString()), 0);
        //        DetraccionDolaresNoAfecto = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(DetraccionDolaresNoAfecto.ToString()), 2);

        //    }
        //    catch (Exception ex)
        //    {
        //        UltraMessageBox.Show(ex.Message + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
        //    }

        //}

    }
}
