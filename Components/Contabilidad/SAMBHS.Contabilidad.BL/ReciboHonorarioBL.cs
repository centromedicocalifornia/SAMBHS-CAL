using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Common.BL;
using System.Transactions;
using System.ComponentModel;


namespace SAMBHS.Contabilidad.BL
{
    public class ReciboHonorarioBL
    {
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();

        #region Formulario
        public List<KeyValueDTO> ObtenerListadoReciboHonorarios(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.recibohonorario
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.v_IdReciboHonorario.Substring(2, 2) == almacenpredeterminado && n.v_IdReciboHonorario.Substring(0, 1) == replicationID
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdReciboHonorario = n.v_IdReciboHonorario

                             }
                             );

                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Value1 = x.v_Correlativo,
                                    Value2 = x.v_IdReciboHonorario
                                }).ToList();

                    return query2;
                }
                else
                {
                    return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<KeyValueDTO> ObtenerListadoReciboHonorariosProcesoMigracion(ref OperationResult pobjOperationResult, string pstringPeriodo)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.recibohonorario
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdReciboHonorario = n.v_IdReciboHonorario

                             }
                             );

                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Value1 = x.v_Correlativo,
                                    Value2 = x.v_IdReciboHonorario
                                }).ToList();

                    return query2;
                }
                else
                {

                    return null;
                    //return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            string replicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.recibohonorario
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo && n.v_IdReciboHonorario == replicationID

                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public BindingList<BindgListReciboHonorarioDetalleDto> ObtenerReciboHonorariosDetalles(ref OperationResult pobjOperationResult, string pstrIdReciboHonorarios)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from a in dbContext.recibohonorariodetalle
                             join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, per = periodo } equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, per = b.v_Periodo } into b_join
                             from b in b_join.DefaultIfEmpty()
                             where a.i_Eliminado == 0 && a.v_IdReciboHonorario == pstrIdReciboHonorarios
                             orderby a.t_InsertaFecha ascending
                             select new BindgListReciboHonorarioDetalleDto
                             {
                                 v_IdReciboHonorarioDetalle = a.v_IdReciboHonorarioDetalle,
                                 v_IdReciboHonorario = a.v_IdReciboHonorario,
                                 v_NroCuenta = a.v_NroCuenta,
                                 i_CCosto = a.i_CCosto,
                                 d_ImporteSoles = a.d_ImporteSoles.Value,
                                 d_ImporteDolares = a.d_ImporteDolares.Value,
                                 i_Eliminado = a.i_Eliminado,
                                 i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                 t_InsertaFecha = a.t_InsertaFecha,
                                 i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = a.t_ActualizaFecha,
                                 i_ValidarCentroCosto = b.i_CentroCosto == null ? 0 : b.i_CentroCosto.Value,
                             }).ToList();

                pobjOperationResult.Success = 1;
                return new BindingList<BindgListReciboHonorarioDetalleDto>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public recibohonorarioDto ObtenerReciboHonorarioCabecera(ref OperationResult pobjOperationResult, string pstrIdReciboHonorario)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntity = (from a in dbContext.recibohonorario

                                 join A in dbContext.cliente on a.v_IdProveedor equals A.v_IdCliente into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join B in dbContext.datahierarchy on new { a = a.i_IdEstado.Value, b = 30 } //Estado
                                                             equals new { a = B.i_ItemId, b = B.i_GroupId } into B_join
                                 from B in B_join.DefaultIfEmpty()

                                 join C in dbContext.documento on new { Doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { Doc = C.i_CodigoDocumento, eliminado = C.i_Eliminado.Value } into C_join
                                 from C in C_join.DefaultIfEmpty()
                                 where a.v_IdReciboHonorario == pstrIdReciboHonorario && a.i_Eliminado == 0

                                 select new recibohonorarioDto
                                 {

                                     v_IdReciboHonorario = a.v_IdReciboHonorario,
                                     v_Periodo = a.v_Periodo,
                                     v_Mes = a.v_Mes,
                                     v_Correlativo = a.v_Correlativo,
                                     i_IdIgv = a.i_IdIgv,
                                     i_IdTipoDocumento = a.i_IdTipoDocumento,
                                     v_SerieDocumento = a.v_SerieDocumento,
                                     v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                     v_IdProveedor = a.v_IdProveedor,
                                     t_FechaRegistro = a.t_FechaRegistro,
                                     t_FechaEmision = a.t_FechaEmision,
                                     d_TipoCambio = a.d_TipoCambio,
                                     v_Glosa = a.v_Glosa,
                                     i_IdMoneda = a.i_IdMoneda,
                                     i_IdEstado = a.i_IdEstado,
                                     i_RentaCuartaCategoria = a.i_RentaCuartaCategoria,
                                     d_Importe = a.d_Importe,
                                     d_RentaCuartaCategoria = a.d_RentaCuartaCategoria,
                                     d_PorPagar = a.d_PorPagar,
                                     d_TotalDebe = a.d_TotalDebe,
                                     d_TotalHaber = a.d_TotalHaber,
                                     d_Diferencia = a.d_Diferencia,
                                     i_Eliminado = a.i_Eliminado,
                                     i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                     t_InsertaFecha = a.t_InsertaFecha,
                                     i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     NombreProveedor = (A.v_ApePaterno.Trim() + " " + A.v_ApeMaterno.Trim() + " " + A.v_PrimerNombre + " " + A.v_SegundoNombre + " " + A.v_RazonSocial).Trim(),
                                     CodigoProveedor = A.v_CodCliente,
                                     RUCProveedor = A.v_NroDocIdentificacion,
                                     i_PorcentajeCuartaCategoria = a.i_PorcentajeCuartaCategoria == null ? -1 : a.i_PorcentajeCuartaCategoria.Value,
                                     v_SerieDocumentoRef = a.v_SerieDocumentoRef,
                                     v_CorrelativoDocumentoRef = a.v_CorrelativoDocumentoRef,
                                     i_IdDocumentoReferencia = a.i_IdDocumentoReferencia,
                                     TipoDocumento = C.v_Siglas,
                                 }
                                 ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                return objEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaCompra(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }



        public bool ValidarNumeroRegistro(int TipoDoc, string serie, string correlativodoc, string IdRecibo, string IdProveedor, string Periodo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                recibohonorarioDto registro = new recibohonorarioDto();

                if (IdRecibo == null)
                {
                    var registroC = (from a in dbContext.recibohonorario
                                     where a.i_Eliminado == 0 && a.i_IdTipoDocumento == TipoDoc && a.v_SerieDocumento == serie && a.v_CorrelativoDocumento == correlativodoc
                                     && a.v_IdProveedor == IdProveedor && a.v_Periodo.Trim() == Periodo

                                     select a).FirstOrDefault();
                    registro = recibohonorarioAssembler.ToDTO(registroC);


                }
                else
                {
                    var registroC = (from a in dbContext.recibohonorario
                                     where a.i_Eliminado == 0 && a.i_IdTipoDocumento == TipoDoc && a.v_SerieDocumento == serie && a.v_CorrelativoDocumento == correlativodoc
                                     && a.v_IdProveedor == IdProveedor
                                     && a.v_IdReciboHonorario != IdRecibo
                                     select a).FirstOrDefault();
                    registro = recibohonorarioAssembler.ToDTO(registroC);

                }



                if (registro != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }













        public string InsertarReciboHonorarios(ref OperationResult pobjOperationResult, recibohonorarioDto pobjDtoEntity, List<string> ClientSession, List<recibohonorariodetalleDto> pTemp_Insertar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        recibohonorario objEntityReciboHonorario = recibohonorarioAssembler.ToEntity(pobjDtoEntity);
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        int SecuentialId = 0;
                        string newIdReciboHonorario = string.Empty, CodigoPorPagar = string.Empty;
                        string newIdReciboHonorarioDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera
                        objEntityReciboHonorario.t_InsertaFecha = DateTime.Now;
                        objEntityReciboHonorario.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityReciboHonorario.i_Eliminado = 0;
                        // Autogeneramos el Pk de la tabla
                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 36);
                        newIdReciboHonorario = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZO");
                        objEntityReciboHonorario.v_IdReciboHonorario = newIdReciboHonorario;
                        dbContext.AddTorecibohonorario(objEntityReciboHonorario);

                        #endregion

                        #region Inserta Detalle

                        foreach (recibohonorariodetalleDto recibohonorariodetalleDto in pTemp_Insertar)
                        {
                            recibohonorariodetalle objEntityReciboHonorarioDetalle = recibohonorariodetalleAssembler.ToEntity(recibohonorariodetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 37);
                            newIdReciboHonorarioDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZP");
                            objEntityReciboHonorarioDetalle.v_IdReciboHonorarioDetalle = newIdReciboHonorarioDetalle;
                            objEntityReciboHonorarioDetalle.v_IdReciboHonorario = newIdReciboHonorario;
                            objEntityReciboHonorarioDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityReciboHonorarioDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityReciboHonorarioDetalle.i_Eliminado = 0;
                            dbContext.AddTorecibohonorariodetalle(objEntityReciboHonorarioDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ReciboHonorariosDetalle", newIdReciboHonorarioDetalle);

                        }
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ReciboHonorarios", newIdReciboHonorario);
                        #endregion


                        if (_objDocumentoBL.DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento.Value))
                        {
                            if (pobjDtoEntity.i_IdEstado != 0)
                            {

                                #region Genera Libro Diario
                                DiarioBL _objDiarioBL = new DiarioBL();
                                diarioDto _diarioDto = new diarioDto();
                                List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                                List<diariodetalleDto> TempDiarioInsertarRH = new List<diariodetalleDto>();


                                diariodetalleDto D_ImporteReciboHonorarios = new diariodetalleDto();
                                diariodetalleDto H_TotalPorPagar = new diariodetalleDto();
                                diariodetalleDto H_4taCategoria = new diariodetalleDto();
                                OperationResult objOperationResult = new OperationResult();


                                var DetalleReciboHonorario = (from d in dbContext.recibohonorariodetalle
                                                              where d.v_IdReciboHonorario == newIdReciboHonorario && d.i_Eliminado == 0
                                                              select d).ToList();


                                var pstringNombreCliente = (from a in dbContext.cliente
                                                            where a.v_IdCliente == pobjDtoEntity.v_IdProveedor && a.v_FlagPantalla == "V"
                                                            select new { nombre = (a.v_ApePaterno + " " + a.v_ApeMaterno + " " + a.v_PrimerNombre + " " + a.v_SegundoNombre + " " + a.v_RazonSocial).Trim(), codigo = a.v_CodCliente }).FirstOrDefault();

                                _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)LibroDiarios.ReciboHonorario);

                                int _MaxMovimiento;
                                _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                _MaxMovimiento++;
                                _diarioDto.v_IdDocumentoReferencia = newIdReciboHonorario;
                                _diarioDto.v_Periodo = pobjDtoEntity.v_Periodo;
                                _diarioDto.v_Mes = pobjDtoEntity.v_Mes;
                                _diarioDto.v_Nombre = pstringNombreCliente.nombre;
                                _diarioDto.v_Glosa = pobjDtoEntity.v_Glosa;
                                _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                                _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                                _diarioDto.i_IdTipoDocumento = (int)LibroDiarios.ReciboHonorario;
                                _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                _diarioDto.i_IdTipoComprobante = 2;

                                List<String> CuentasDetalle = new List<string>();

                                CuentasDetalle = DetalleReciboHonorario.Select(p => p.v_NroCuenta).Distinct().ToList();

                                foreach (String CuentaDetalle in CuentasDetalle)
                                {

                                    var Lista = DetalleReciboHonorario.FindAll(p => p.v_NroCuenta == CuentaDetalle).GroupBy(q => q.i_CCosto).ToList();
                                    foreach (var Fila in Lista)
                                    {
                                        var Lista2 = pobjDtoEntity.i_IdMoneda == 1 ? Fila.Sum(w => w.d_ImporteSoles) : Fila.Sum(o => o.d_ImporteDolares);
                                        decimal SubTotal = Lista2.Value;
                                        D_ImporteReciboHonorarios.d_Importe = Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) > 0 ? Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) : Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) * -1;
                                        D_ImporteReciboHonorarios.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(SubTotal / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(SubTotal * pobjDtoEntity.d_TipoCambio.Value, 2);
                                        D_ImporteReciboHonorarios.i_IdCentroCostos = Fila.Select(p => p.i_CCosto).First();
                                        D_ImporteReciboHonorarios.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                        D_ImporteReciboHonorarios.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                        D_ImporteReciboHonorarios.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                        // D_ImporteReciboHonorarios.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) > 0 ?  pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        D_ImporteReciboHonorarios.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                                        D_ImporteReciboHonorarios.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                        D_ImporteReciboHonorarios.v_NroCuenta = CuentaDetalle;
                                        D_ImporteReciboHonorarios.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdDocumentoReferencia.Value;
                                        D_ImporteReciboHonorarios.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" + pobjDtoEntity.v_CorrelativoDocumentoRef;
                                        TempDiarioInsertarRH.Add(D_ImporteReciboHonorarios);
                                        D_ImporteReciboHonorarios = new diariodetalleDto();

                                    }
                                }


                                if (objEntityReciboHonorario.i_RentaCuartaCategoria == 1)
                                {
                                    string codigoCuartacat = "06";

                                    var CuentaCuartaCategoria = (from n in dbContext.administracionconceptos
                                                                 where n.v_Codigo == codigoCuartacat && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                                 select n).FirstOrDefault();

                                    if (CuentaCuartaCategoria != null && CuentaCuartaCategoria.v_CuentaPVenta != string.Empty)
                                    {

                                        H_4taCategoria.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_RentaCuartaCategoria.Value, 2);
                                        H_4taCategoria.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_RentaCuartaCategoria.Value / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_RentaCuartaCategoria.Value * pobjDtoEntity.d_TipoCambio.Value, 2);
                                        H_4taCategoria.i_IdCentroCostos = "";
                                        H_4taCategoria.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                        H_4taCategoria.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                        H_4taCategoria.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                        //H_4taCategoria.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_4taCategoria.d_Importe.Value, 2) > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                        H_4taCategoria.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_4taCategoria.d_Importe.Value, 2) > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                        H_4taCategoria.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                        H_4taCategoria.v_NroCuenta = CuentaCuartaCategoria.v_CuentaPVenta;
                                        H_4taCategoria.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdDocumentoReferencia.Value;
                                        H_4taCategoria.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" + pobjDtoEntity.v_CorrelativoDocumentoRef;
                                        TempDiarioInsertarRH.Add(H_4taCategoria);
                                        H_4taCategoria = new diariodetalleDto();
                                    }
                                }


                                if (pobjDtoEntity.i_IdMoneda == 1)
                                {
                                    CodigoPorPagar = "08";
                                }

                                else
                                {
                                    CodigoPorPagar = "09";
                                }

                                var administracionConceptoRH = (from n in dbContext.administracionconceptos
                                                                where n.v_Codigo == CodigoPorPagar && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                                select new { n.v_CuentaDetraccion, n.v_CuentaIGV, n.v_CuentaPVenta }).FirstOrDefault();


                                if (administracionConceptoRH != null && administracionConceptoRH.v_CuentaPVenta != string.Empty)
                                {

                                    H_TotalPorPagar.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PorPagar.Value, 2);
                                    H_TotalPorPagar.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PorPagar.Value / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PorPagar.Value * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    H_TotalPorPagar.i_IdCentroCostos = "";
                                    H_TotalPorPagar.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_TotalPorPagar.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_TotalPorPagar.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    //H_TotalPorPagar.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_TotalPorPagar.d_Importe.Value, 2) > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                    H_TotalPorPagar.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_TotalPorPagar.d_Importe.Value, 2) > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    H_TotalPorPagar.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                    H_TotalPorPagar.v_NroCuenta = administracionConceptoRH.v_CuentaPVenta;

                                    H_TotalPorPagar.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdDocumentoReferencia.Value;
                                    H_TotalPorPagar.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" + pobjDtoEntity.v_CorrelativoDocumentoRef;

                                    TempDiarioInsertarRH.Add(H_TotalPorPagar);
                                    H_TotalPorPagar = new diariodetalleDto();
                                }


                                decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                                TotDebe = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                                TotHaber = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                                TotDebeC = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                                TotHaberC = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                                _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                                _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                                _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                                _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                                _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                                _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                                _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarRH.Where(x => x.v_NroCuenta != string.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);

                                if (objOperationResult.Success == 0) return string.Empty;
                                #endregion
                            }
                        }
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return newIdReciboHonorario;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string ActualizarReciboHonorarios(ref OperationResult pobjOperationResult, recibohonorarioDto pobjDtoEntity, List<string> ClientSession, List<recibohonorariodetalleDto> pTemp_Insertar, List<recibohonorariodetalleDto> pTemp_Editar, List<recibohonorariodetalleDto> pTemp_Eliminar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    recibohonorario objEntityReciboHonorario = recibohonorarioAssembler.ToEntity(pobjDtoEntity);
                    recibohonorariodetalleDto pobjDtoCompraDetalle = new recibohonorariodetalleDto();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    int SecuentialId = 0;
                    string newIdReciboHonorarioDetalle = string.Empty, CodigoPorPagar = string.Empty;

                    int intNodeId;

                    #region Actualiza Cabecera
                    intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (from a in dbContext.recibohonorario
                                           where a.v_IdReciboHonorario == pobjDtoEntity.v_IdReciboHonorario
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    recibohonorario objEntity = recibohonorarioAssembler.ToEntity(pobjDtoEntity);

                    dbContext.recibohonorario.ApplyCurrentValues(objEntity);
                    #endregion

                    #region Actualiza Detalle
                    foreach (recibohonorariodetalleDto reciboHonorariodetalleDto in pTemp_Insertar)
                    {
                        recibohonorariodetalle objEntityReciboHonorarioDetalle = recibohonorariodetalleAssembler.ToEntity(reciboHonorariodetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 37);
                        newIdReciboHonorarioDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZP");
                        objEntityReciboHonorarioDetalle.v_IdReciboHonorarioDetalle = newIdReciboHonorarioDetalle;
                        objEntityReciboHonorarioDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityReciboHonorarioDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityReciboHonorarioDetalle.i_Eliminado = 0;
                        dbContext.AddTorecibohonorariodetalle(objEntityReciboHonorarioDetalle);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ReciboHonorariosDetalle", newIdReciboHonorarioDetalle);

                    }


                    foreach (recibohonorariodetalleDto reciboHonorariodetalleDto in pTemp_Editar)
                    {
                        recibohonorariodetalle _objEntity = recibohonorariodetalleAssembler.ToEntity(reciboHonorariodetalleDto);
                        var query = (from n in dbContext.recibohonorariodetalle
                                     where n.v_IdReciboHonorarioDetalle == reciboHonorariodetalleDto.v_IdReciboHonorarioDetalle
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.recibohonorariodetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "ReciboHonorariosDetalle", query.v_IdReciboHonorarioDetalle);
                    }

                    foreach (recibohonorariodetalleDto reciboHonorariodetalleDto in pTemp_Eliminar)
                    {
                        recibohonorariodetalle _objEntity = recibohonorariodetalleAssembler.ToEntity(reciboHonorariodetalleDto);
                        var query = (from n in dbContext.recibohonorariodetalle
                                     where n.v_IdReciboHonorarioDetalle == reciboHonorariodetalleDto.v_IdReciboHonorarioDetalle
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.recibohonorariodetalle.ApplyCurrentValues(query);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "ReciboHonorariosDetalle", query.v_IdReciboHonorarioDetalle);
                    }
                    #endregion

                    if (_objDocumentoBL.DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento.Value))
                    {


                        DiarioBL _objDiarioBL = new DiarioBL();
                        diarioDto _diarioDto = new diarioDto();
                        List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                        List<diariodetalleDto> TempDiarioInsertarRH = new List<diariodetalleDto>();
                        diariodetalleDto D_ImporteReciboHonorarios = new diariodetalleDto();
                        diariodetalleDto H_TotalPorPagar = new diariodetalleDto();
                        diariodetalleDto H_4taCategoria = new diariodetalleDto();
                        OperationResult objOperationResult = new OperationResult();
                        string[] IdRegistroEliminado = new string[3];

                        var DetalleReciboHonorario = (from d in dbContext.recibohonorariodetalle
                                                      where d.v_IdReciboHonorario == pobjDtoEntity.v_IdReciboHonorario && d.i_Eliminado == 0
                                                      select d).ToList();


                        var pstringNombreCliente = (from a in dbContext.cliente
                                                    where a.v_IdCliente == pobjDtoEntity.v_IdProveedor && a.v_FlagPantalla == "V"
                                                    select new { nombre = (a.v_ApePaterno + " " + a.v_ApeMaterno + " " + a.v_PrimerNombre + " " + a.v_SegundoNombre + " " + a.v_RazonSocial).Trim(), codigo = a.v_CodCliente }).FirstOrDefault();
                        IdRegistroEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, pobjDtoEntity.v_IdReciboHonorario, ClientSession, false);
                        if (pobjDtoEntity.i_IdEstado == 1)
                        {
                            #region Genera Libro Diario


                            if (IdRegistroEliminado[0] == null || (IdRegistroEliminado != null && (IdRegistroEliminado[1].Trim() != pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") || IdRegistroEliminado[0] != pobjDtoEntity.t_FechaRegistro.Value.Year.ToString())))
                            {
                                int _MaxMovimiento;
                                _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), (int)LibroDiarios.ReciboHonorario);
                                _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                _MaxMovimiento++;
                                _diarioDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                _diarioDto.v_Mes = int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                                _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                            }
                            else
                            {
                                _diarioDto.v_Periodo = IdRegistroEliminado[0];
                                _diarioDto.v_Mes = IdRegistroEliminado[1];
                                _diarioDto.v_Correlativo = IdRegistroEliminado[2];
                            }

                            //_ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)LibroDiarios.ReciboHonorario);
                            //int _MaxMovimiento;
                            //_MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                            //_MaxMovimiento++;
                            _diarioDto.v_IdDocumentoReferencia = pobjDtoEntity.v_IdReciboHonorario;
                            //  _diarioDto.v_Periodo = IdRegistroEliminado[0];
                            //  _diarioDto.v_Mes = IdRegistroEliminado[1];
                            _diarioDto.v_Nombre = pstringNombreCliente.nombre;
                            _diarioDto.v_Glosa = pobjDtoEntity.v_Glosa;
                            //   _diarioDto.v_Correlativo = IdRegistroEliminado[2];
                            _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                            _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                            _diarioDto.i_IdTipoDocumento = (int)LibroDiarios.ReciboHonorario;
                            _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            _diarioDto.i_IdTipoComprobante = 2;

                            List<String> CuentasDetalle = new List<string>();

                            CuentasDetalle = DetalleReciboHonorario.Select(p => p.v_NroCuenta).Distinct().ToList();

                            foreach (String CuentaDetalle in CuentasDetalle)
                            {
                                var Lista = DetalleReciboHonorario.FindAll(p => p.v_NroCuenta == CuentaDetalle).GroupBy(q => q.i_CCosto).ToList();

                                foreach (var Fila in Lista)
                                {
                                    var Lista2 = pobjDtoEntity.i_IdMoneda == 1 ? Fila.Sum(w => w.d_ImporteSoles) : Fila.Sum(o => o.d_ImporteDolares);
                                    decimal SubTotal = Lista2.Value;
                                    D_ImporteReciboHonorarios.d_Importe = Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) > 0 ? Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) : Utils.Windows.DevuelveValorRedondeado(SubTotal * -1, 2);
                                    D_ImporteReciboHonorarios.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(SubTotal / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(SubTotal * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    D_ImporteReciboHonorarios.i_IdCentroCostos = Fila.Select(p => p.i_CCosto).First();
                                    D_ImporteReciboHonorarios.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    D_ImporteReciboHonorarios.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    D_ImporteReciboHonorarios.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    //D_ImporteReciboHonorarios.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                    D_ImporteReciboHonorarios.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(SubTotal, 2) > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                                    D_ImporteReciboHonorarios.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                    D_ImporteReciboHonorarios.v_NroCuenta = CuentaDetalle;

                                    D_ImporteReciboHonorarios.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdDocumentoReferencia.Value;
                                    D_ImporteReciboHonorarios.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" + pobjDtoEntity.v_CorrelativoDocumentoRef;

                                    TempDiarioInsertarRH.Add(D_ImporteReciboHonorarios);
                                    D_ImporteReciboHonorarios = new diariodetalleDto();

                                }
                            }


                            if (objEntityReciboHonorario.i_RentaCuartaCategoria == 1)
                            {
                                string codigoCuartacat = "06";

                                var CuentaCuartaCategoria = (from n in dbContext.administracionconceptos
                                                             where n.v_Codigo == codigoCuartacat && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                             select n).FirstOrDefault();

                                if (CuentaCuartaCategoria != null && CuentaCuartaCategoria.v_CuentaPVenta != string.Empty)
                                {

                                    H_4taCategoria.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_RentaCuartaCategoria.Value, 2);
                                    H_4taCategoria.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_RentaCuartaCategoria.Value / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_RentaCuartaCategoria.Value * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    H_4taCategoria.i_IdCentroCostos = "";
                                    H_4taCategoria.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_4taCategoria.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_4taCategoria.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    // H_4taCategoria.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_4taCategoria.d_Importe.Value, 2) > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                    H_4taCategoria.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_4taCategoria.d_Importe.Value, 2) > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    H_4taCategoria.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                    H_4taCategoria.v_NroCuenta = CuentaCuartaCategoria.v_CuentaPVenta;

                                    H_4taCategoria.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdDocumentoReferencia.Value;
                                    H_4taCategoria.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" + pobjDtoEntity.v_CorrelativoDocumentoRef;

                                    TempDiarioInsertarRH.Add(H_4taCategoria);
                                    H_4taCategoria = new diariodetalleDto();
                                }
                            }


                            if (pobjDtoEntity.i_IdMoneda == 1)
                            {
                                CodigoPorPagar = "08";
                            }

                            else
                            {
                                CodigoPorPagar = "09";
                            }

                            var administracionConceptoRH = (from n in dbContext.administracionconceptos
                                                            where n.v_Codigo == CodigoPorPagar && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                            select new { n.v_CuentaDetraccion, n.v_CuentaIGV, n.v_CuentaPVenta }).FirstOrDefault();


                            if (administracionConceptoRH != null && administracionConceptoRH.v_CuentaPVenta != string.Empty)
                            {

                                H_TotalPorPagar.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PorPagar.Value, 2);
                                H_TotalPorPagar.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PorPagar.Value / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PorPagar.Value * pobjDtoEntity.d_TipoCambio.Value, 2);
                                H_TotalPorPagar.i_IdCentroCostos = "";
                                H_TotalPorPagar.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_TotalPorPagar.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_TotalPorPagar.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                //H_TotalPorPagar.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_TotalPorPagar.d_Importe.Value, 2) > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                H_TotalPorPagar.v_Naturaleza = Utils.Windows.DevuelveValorRedondeado(H_TotalPorPagar.d_Importe.Value, 2) > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                H_TotalPorPagar.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                H_TotalPorPagar.v_NroCuenta = administracionConceptoRH.v_CuentaPVenta;
                                H_TotalPorPagar.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdDocumentoReferencia.Value;
                                H_TotalPorPagar.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" + pobjDtoEntity.v_CorrelativoDocumentoRef;
                                TempDiarioInsertarRH.Add(H_TotalPorPagar);
                                H_TotalPorPagar = new diariodetalleDto();
                            }


                            decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                            TotDebe = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                            TotHaber = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                            TotDebeC = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                            TotHaberC = TempDiarioInsertarRH.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                            _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                            _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                            _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                            _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                            _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                            _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarRH.Where(x => x.v_NroCuenta != string.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);
                            if (objOperationResult.Success == 0) return string.Empty;
                            #endregion
                        }
                    }
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return pobjDtoEntity.v_IdReciboHonorario;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }




        public string DevolverVariableCuartaGeneracion(ref OperationResult pobjOperationResult, int pintGroupId, int item)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.datahierarchy
                             where n.i_IsDeleted == 0 && n.i_GroupId == pintGroupId && n.i_ItemId == item
                             select n
                             ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    string Cadena;
                    Cadena = query.v_Value2;
                    //Cadena[1] = query.v_CodCliente;
                    //Cadena[2] = (query.v_PrimerNombre + " " + query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_RazonSocial).Trim();
                    return Cadena;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public bool ExistenciaCuentaImputable(string pstrCuenta)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var query = (from n in dbContext.asientocontable
                         where n.i_Eliminado == 0 & n.i_Imputable == 1 & n.v_NroCuenta == pstrCuenta && n.v_Periodo == periodo
                         select n).FirstOrDefault();

            if (query != null)

                return true;
            else
                return false;

        }
        public string[] DevolverProveedorPorNroDocumento(ref OperationResult pobjOperationResult, string NroDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.cliente
                             where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_NroDocIdentificacion == NroDocumento
                             select n
                             ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    string[] Cadena = new string[3];
                    Cadena[0] = query.v_IdCliente;
                    Cadena[1] = query.v_CodCliente;
                    Cadena[2] = (query.v_PrimerNombre + " " + query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_RazonSocial).Trim();
                    return Cadena;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        public bool ValidarNroCuentaGeneracionLibro(string Codigo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var AdministracionConcepto = (from n in dbContext.administracionconceptos

                                          where n.v_Codigo.Trim() == Codigo.Trim() && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                          select n).FirstOrDefault();


            if (AdministracionConcepto != null)
            {

                var PlanCuenta = (from a in dbContext.asientocontable
                                  where a.v_NroCuenta.Trim() == AdministracionConcepto.v_CuentaPVenta.Trim() && a.i_Imputable == 1 && a.i_Eliminado == 0 && a.v_Periodo == periodo
                                  select a).FirstOrDefault();
                if (PlanCuenta != null)
                {
                    return true;

                }
                else
                {
                    return false;
                }

            }
            else
            {

                return false;
            }

        }
        public string NroDiario(string pstrDocumentoReferencia)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


            var NroDiario = (from n in dbContext.diario
                             where n.i_Eliminado == 0 && n.v_IdDocumentoReferencia == pstrDocumentoReferencia
                             select n).FirstOrDefault();
            if (NroDiario != null)
            {
                return NroDiario.v_IdDiario;
            }
            else
            {
                return string.Empty;
            }

        }
        public bool ConsultarSiTieneTesorerias(string pstrIdDiario)
        {

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string IdPendienteCobrar = null;
                    var PendienteCobrar = (from v in dbContext.pendientecobrardetalle
                                           where v.v_IdDiario == pstrIdDiario
                                           select new { v.v_IdPendienteCobrar }).FirstOrDefault();


                    if (PendienteCobrar != null)
                    {
                        IdPendienteCobrar = PendienteCobrar.v_IdPendienteCobrar;
                        pendientecobrar Entidad = (from e in dbContext.pendientecobrar
                                                   where e.v_IdPendienteCobrar == IdPendienteCobrar
                                                   select e).FirstOrDefault();

                        if (Entidad.pendientecobrardetalle.ToList().Where(p => p.v_IdTesoreria != null).Count() != 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {

                        return false;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }






        }



        public static void ModificarNroRegistrosReciboHonorario(ref OperationResult pobjOperationResult,
           IDictionary<string, string> lista,List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    foreach (var item in lista)
                    {
                        var recibohonorario = dbContext.recibohonorario.FirstOrDefault(p => p.v_IdReciboHonorario.Equals(item.Value));
                        if (recibohonorario == null) continue;
                        var diario = dbContext.diario.FirstOrDefault(p => p.v_IdDocumentoReferencia.Equals(recibohonorario.v_IdReciboHonorario));
                        var reg = item.Key.Split('-')[1];
                        recibohonorario.v_Correlativo = reg.Trim();
                        recibohonorario.t_ActualizaFecha = DateTime.Now;
                        recibohonorario.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.recibohonorario.ApplyCurrentValues(recibohonorario);

                        if (diario == null) continue;
                        diario.v_Correlativo = reg.Trim();
                        diario.t_ActualizaFecha = DateTime.Now;
                        diario.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.diario.ApplyCurrentValues(diario);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ReciboHonorarioBL.ModificarNroRegistros()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }


        #endregion

        #region Bandeja
        public List<recibohonorarioDto> ListarBusquedaReciboHonorarios(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.recibohonorario

                             join B in dbContext.cliente on n.v_IdProveedor equals B.v_IdCliente into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { TipoDoc = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join

                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.documento on new { TipoDocRef = n.i_IdDocumentoReferencia.Value, eliminado = 0 } equals new { TipoDocRef = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Inicio && n.t_FechaRegistro <= F_Fin


                             select new recibohonorarioDto
                             {
                                 v_IdReciboHonorario = n.v_IdReciboHonorario,
                                 v_Periodo = n.v_Periodo,
                                 v_Mes = n.v_Mes.Trim (),
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdProveedor = n.v_IdProveedor,
                                 t_FechaEmision = n.t_FechaEmision,
                                 t_FechaRegistro = n.t_FechaRegistro,
                                 d_TipoCambio = n.d_TipoCambio,
                                 i_IdMoneda = n.i_IdMoneda,
                                 v_SerieDocumento = n.v_SerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 v_Glosa = n.v_Glosa,
                                 d_PorPagar = n.d_PorPagar,
                                 i_Eliminado = n.i_Eliminado,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_NumeroDocumento = n.v_SerieDocumento.Trim() + " - " + n.v_CorrelativoDocumento.Trim(),
                                 v_NumeroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                 i_IdEstado = n.i_IdEstado,
                                 v_IdentificacionProveedor = (B.v_CodCliente.Trim() + " - " + B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() + " " + B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim(),
                                 v_UsuarioModificacion = J2.v_UserName,
                                 v_UsuarioCreacion = J3.v_UserName,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento.Value,
                                 TipoDocumento = J4 != null ? J4.v_Siglas : "",
                                 i_IdDocumentoReferencia = n.i_IdDocumentoReferencia,
                                 v_SerieDocumentoRef = n.v_SerieDocumentoRef,
                                 v_CorrelativoDocumentoRef = n.v_CorrelativoDocumentoRef,
                                 TipoDocumentoRef = J5 != null ? J5.v_Siglas + " " + n.v_SerieDocumentoRef + "-" + n.v_CorrelativoDocumentoRef : "",
                                 Moneda = n.i_IdMoneda == 1 ? "S" : "D" ,
                                
                                 
                             }
                            );

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<recibohonorarioDto> objData = query.OrderBy(pstrSortExpression).ToList();
                pobjOperationResult.Success = 1;
                //objData.OrderBy(pstrSortExpression);

                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<KeyValueDTO> BuscarProveedoresParaCombo(ref OperationResult pobjOperationResult, string pstrRucRazonSocial, string Flag)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var query = (from n in dbcontext.cliente
                             where n.i_Eliminado == 0 &&
                                   n.v_PrimerNombre.Contains(pstrRucRazonSocial) | n.v_SegundoNombre.Contains(pstrRucRazonSocial) | n.v_ApeMaterno.Contains(pstrRucRazonSocial)
                                   | n.v_ApePaterno.Contains(pstrRucRazonSocial) | n.v_RazonSocial.Contains(pstrRucRazonSocial) | n.v_NroDocIdentificacion.Contains(pstrRucRazonSocial)
                                   && n.v_FlagPantalla == Flag && pstrRucRazonSocial.Trim() != string.Empty

                             orderby n.v_RazonSocial ascending
                             select new
                             {
                                 v_IdCliente = n.v_IdCliente,
                                 v_RazonSocial = (n.v_PrimerNombre + " " + n.v_ApePaterno + " " + n.v_ApeMaterno + " " + n.v_RazonSocial).Trim(),
                                 v_NroDocIdentificacion = n.v_NroDocIdentificacion
                             }
                             );

                var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Id = x.v_IdCliente,
                                Value1 = x.v_NroDocIdentificacion + " | " + x.v_RazonSocial
                            }).ToList();

                return query2;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void EliminarReciboHonorarios(ref OperationResult pobjOperationResult, string pstrIdReciboHonorarios, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.recibohonorario
                                           where a.v_IdReciboHonorario == pstrIdReciboHonorarios
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesReciboHonorarios = (from a in dbContext.recibohonorariodetalle
                                                                   where a.v_IdReciboHonorario == pstrIdReciboHonorarios
                                                                   select a).ToList();

                    foreach (var RegistroReciboHonorariosDetalle in objEntitySourceDetallesReciboHonorarios)
                    {
                        RegistroReciboHonorariosDetalle.t_ActualizaFecha = DateTime.Now;
                        RegistroReciboHonorariosDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        RegistroReciboHonorariosDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "ReciboHonorariosDetalle", RegistroReciboHonorariosDetalle.v_IdReciboHonorarioDetalle);

                    }
                    #endregion
                    #region Elimina Diario
                    DiarioBL _objDiarioBL = new DiarioBL();
                    _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, pstrIdReciboHonorarios, ClientSession, true);
                    if (pobjOperationResult.Success == 0) return;
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "RecibosHonorarios", pstrIdReciboHonorarios);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }


        public bool TieneDocReferenciasAsociados(int IdReferencia, string SerieRef, string CorreRef)
        {

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var recibonorarios = (from a in dbContext.recibohonorario

                                          where a.i_Eliminado == 0 && a.i_IdDocumentoReferencia == IdReferencia && a.v_SerieDocumentoRef == SerieRef && a.v_CorrelativoDocumentoRef == CorreRef
                                          select a).ToList();

                    if (recibonorarios.Any())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }


            }
            catch (Exception ex)
            {

                return false;
            }


        }

        public List<GridKeyValueDTO> ObtenerDocumentosParaComboGridReciboHonorarios(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from a in dbContext.documento
                            where a.i_Eliminado == 0 &&
                                 (a.i_CodigoDocumento == 2 || a.i_CodigoDocumento == 7)
                            select a;

                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                else
                {
                    query = query.OrderBy("v_Nombre");
                }

                var query2 = query.AsEnumerable()
                            .Select(x => new GridKeyValueDTO
                            {
                                Id = x.i_CodigoDocumento.ToString(),
                                Value1 = x.v_Nombre,
                                Value2 = x.v_Siglas
                            }).ToList();

                pobjOperationResult.Success = 1;
                return query2;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        #endregion

        #region Reportes
        public List<ReporteReciboHonorarios> ReporteListadoReciboHonorarios(ref OperationResult objOperationResult, string _strFilterExpression, string Orden, DateTime FechaInicio, DateTime FechaFin)
        {


            try
            {
                objOperationResult.Success = 1;
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


                List<ReporteReciboHonorarios> query4 = new List<ReporteReciboHonorarios>();
                var query = (from n in dbContext.recibohonorario

                             join a in dbContext.documento on new { doc = n.i_IdTipoDocumento.Value } equals new { doc = a.i_CodigoDocumento } into a_join
                             from a in a_join.DefaultIfEmpty()

                             join b in dbContext.cliente on new { prov = n.v_IdProveedor } equals new { prov = b.v_IdCliente } into b_join
                             from b in b_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0

                             && n.t_FechaRegistro >= FechaInicio && n.t_FechaRegistro <= FechaFin

                             select new ReporteReciboHonorarios
                             {

                                 Fecha = n.t_FechaRegistro.Value,
                                 Documento = a.v_Siglas + " " + n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                                 NroComprobante = n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim(),
                                 RucProveedor = b.v_NroDocIdentificacion.Trim(),
                                 Moneda = n.i_IdMoneda == 1 ? "S/" : "US$",
                                 Monto = n.d_Importe,
                                 MontoNacional = n.i_IdMoneda == 1 ? n.d_Importe : n.d_Importe * n.d_TipoCambio.Value,
                                 ImpuestoCuartaCategoria = n.i_IdMoneda == 1 ? n.d_RentaCuartaCategoria : n.d_RentaCuartaCategoria * n.d_TipoCambio,
                                 Total = n.i_IdMoneda == 1 ? n.d_PorPagar : n.d_PorPagar * n.d_TipoCambio,
                                 TipoCambio = n.d_TipoCambio,
                                 Proveedor = (b.v_CodCliente.Trim() + "-" + b.v_ApePaterno.Trim() + " " + b.v_ApeMaterno.Trim() + " " + b.v_PrimerNombre.Trim() + " " + b.v_SegundoNombre.Trim() + " " + b.v_RazonSocial.Trim()).Trim(),
                                 ImpuestoExtSolidaridad = 0,
                                 v_Mes = n.v_Mes.Trim(),
                                 v_IdProveedor = n.v_IdProveedor,
                                 i_IdMoneda = n.i_IdMoneda,
                                 Glosa = n.v_Glosa.Trim(),
                                 Servicio = b.v_Servicio,

                                 NombreProveedor = b == null ? "" : b.v_ApePaterno.Trim() + " " + b.v_ApeMaterno.Trim() + " " + b.v_PrimerNombre.Trim() + " " + b.v_SegundoNombre.Trim() + " " + b.v_RazonSocial.Trim(),
                                 DireccionProveedor = b == null ? "" : b.v_DirecPrincipal,

                             });

                if (!string.IsNullOrEmpty(_strFilterExpression))
                {

                    query = query.Where(_strFilterExpression);
                }

                if (!string.IsNullOrEmpty(Orden))
                {
                    query = query.OrderBy(Orden);
                }
                List<ReporteReciboHonorarios> objData = query.ToList();

                return objData;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }


        }


        public List<ReporteCuartaCategoria> ReporteCuartaCategoria(ref OperationResult objOperationResult, string _strFilterExpression, DateTime FechaFin, string FechaEmision, string nroDniIni, string nroDniFin, bool SoloPagosRetencion)
        {
            OperationResult ObjOperationResult = new OperationResult();


            var rangoDni = Utils.Windows.RangoDeDocumentosIdentidad(nroDniIni, nroDniFin).ToList();
            var Recibos = ReporteListadoReciboHonorarios(ref objOperationResult, _strFilterExpression, "", DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), FechaFin);
            List<ReporteReciboHonorarios> ListaAgrupada = new List<ReporteReciboHonorarios>();
            List<ReporteReciboHonorarios> Proveedores = Recibos.GroupBy(o => new { o.v_IdProveedor }).Select(o =>
                               {
                                   ReporteReciboHonorarios obj = new ReporteReciboHonorarios();
                                   obj = o.LastOrDefault();
                                   obj.MontoNacional = o.Sum(p => p.MontoNacional);
                                   obj.ImpuestoCuartaCategoria = o.Sum(p => p.ImpuestoCuartaCategoria);
                                   return obj;


                               }).ToList();



            if (SoloPagosRetencion)
            {
                ListaAgrupada = Proveedores.Where(o => o.ImpuestoCuartaCategoria > 0).ToList();
            }
            else
            {
                ListaAgrupada = Proveedores;
            }

            List<ReporteCuartaCategoria> ListaReporte = new List<ReporteCuartaCategoria>();
            foreach (var provRecib in ListaAgrupada)
            {
                if (!rangoDni.Any() || (rangoDni.Any() && rangoDni.Contains(provRecib.RucProveedor)))
                {
                    ReporteCuartaCategoria objCuarta = new ReporteCuartaCategoria();
                    var Trabajador = ObtenerTrabajadorbyIdTrabajador(ref ObjOperationResult, Globals.ClientSession.v_IdRepresentanteLegal);
                    var NombreRepresentanteLegal = Trabajador != null ? Trabajador.v_RazonSocial : "";
                    var RucRepresentanteLegal = Trabajador != null ? Trabajador.v_NroDocIdentificacion : "";
                    objCuarta.NroLey = "CERTIFICADO DE RENTAS Y RETENCIONES A CUENTA DEL IMPUESTO A LA RENTA SOBRE RENTAS DE CUARTA CATEGORIA\n" + Globals.ClientSession.v_NroLeyCuartaCategoria;
                    objCuarta.Ejercicio = "EJERCICIO " + Globals.ClientSession.i_Periodo.ToString();
                    objCuarta.Texto1 = "Fecha de Emisión: " + FechaEmision;
                    objCuarta.Texto2 = Globals.ClientSession.v_CurrentExecutionNodeName + " con R.U.C. " + Globals.ClientSession.v_RucEmpresa + " , debidamente presentado por  Don(ña) " + NombreRepresentanteLegal + " con DNI : " + RucRepresentanteLegal;
                    objCuarta.Texto3 = "Por la presente se certifica que Don(ña) " + provRecib.NombreProveedor.ToUpper() + ", con R.U.C. N° : " + provRecib.RucProveedor + " y con domicilio fiscal en : " + provRecib.DireccionProveedor.ToUpper() + ", se le  ha efectuado  retenciones , según el siguiente detalle :\n";
                    objCuarta.Servicio = provRecib.Servicio;
                    objCuarta.RentasBrutas = "S/ " + Utils.Windows.DevuelveValorRedondeado(provRecib.MontoNacional.Value, 2).ToString("#.00") + "(" + SAMBHS.Common.Resource.Utils.ConvertLetter(provRecib.MontoNacional.ToString(), "0").ToUpper() + ")";
                    objCuarta.ImporteRetenido = "S/ " + Utils.Windows.DevuelveValorRedondeado(provRecib.ImpuestoCuartaCategoria.Value, 2).ToString("#.00") + "(" + SAMBHS.Common.Resource.Utils.ConvertLetter(provRecib.ImpuestoCuartaCategoria.ToString(), "0").ToUpper() + ")";
                    objCuarta.v_IdTrabajador = provRecib.v_IdProveedor;
                    objCuarta.NombreRepresentante = NombreRepresentanteLegal + "\n" + "REPRESENTANTE LEGAL";
                    DateTime Fec = DateTime.Parse(FechaEmision);//DateTime.Now;
                    objCuarta.FechaHoraImprimir = "LIMA , " + Fec.Date.Day.ToString() + " de " + Mes(Fec.Date.Month) + " del " + Fec.Date.Year.ToString();
                    ListaReporte.Add(objCuarta);
                }



            }

            return ListaReporte;

        }


        private string Mes(int mes)
        {
            switch (mes)
            {
                case 1: return "ENERO";
                case 2: return "FEBRERO";
                case 3: return "MARZO";
                case 4: return "ABRIL";
                case 5: return "MAYO";
                case 6: return "JUNIO";
                case 7: return "JULIO";
                case 8: return "AGOSTO";
                case 9: return "SETIEMBRE";
                case 10: return "OCTUBRE";
                case 11: return "NOVIEMBRE";
                case 12: return "DICIEMBRE";


            }
            return string.Empty;
        }
        public trabajadorDto ObtenerTrabajadorbyIdTrabajador(ref OperationResult pobjOperationResult, string IdTrabajador)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    //trabajadorDto objDtoEntity = null;

                    var Ubigeo = new SystemParameterBL().GetSystemParameterForCombo(ref pobjOperationResult, 112, "");

                    var objEntity = (from A in dbContext.trabajador

                                     join B in dbContext.datahierarchy on new {Grupo=113, Eliminado =0,Via= A.i_IdTipoVia.Value  } equals new {Grupo=B.i_GroupId ,Eliminado= B.i_IsDeleted.Value ,Via=B.i_ItemId }  into B_join
                                     from  B in B_join.DefaultIfEmpty ()
                                     where A.v_IdTrabajador == IdTrabajador
                                     select new
                                     {
                                         v_RazonSocial = A.cliente.v_ApePaterno + " " + A.cliente.v_ApeMaterno + " " + A.cliente.v_PrimerNombre + " " + A.cliente.v_SegundoNombre,
                                         v_NroDocIdentificacion = A.cliente.v_NroDocIdentificacion,
                                         v_DirecPrincipal = A.cliente.v_DirecPrincipal,
                                         v_NombreVia= A.v_NombreVia ,
                                         iProvincia = A.cliente.i_IdProvincia ,
                                         iDistrito = A.cliente.i_IdDistrito,
                                         Via=B==null ?"": string.IsNullOrEmpty (B.v_Value1)?"": B.v_Value1 ,
                                         v_NumeroDomicilio = A.v_NumeroDomicilio,
                                     }).ToList().Select(o =>
                                         {
                                             var Provincia = o.iProvincia == -1 || o.iProvincia == null ? "" : Ubigeo.Where(l => l.Id == o.iProvincia.ToString()) != null ? Ubigeo.Where(l => l.Id == o.iProvincia.ToString()).FirstOrDefault().Value1 : "";
                                             var distrito = o.iDistrito == -1 || o.iDistrito == null ? "" : Ubigeo.Where(l => l.Id == o.iDistrito.ToString()) != null ? Ubigeo.Where(l => l.Id == o.iDistrito.ToString()).FirstOrDefault().Value1 : "";
                                             return new trabajadorDto
                                                   {
                                                       v_RazonSocial = o.v_RazonSocial,
                                                       v_NroDocIdentificacion = o.v_NroDocIdentificacion,
                                                       v_DirecPrincipal = o.v_DirecPrincipal,
                                                       Departamento = Provincia,
                                                       Distrito = distrito,
                                                       Via =o.Via ,
                                                       v_NombreVia =o.v_NombreVia +" "+ o.v_NumeroDomicilio ,

                                                   };
                                         }).FirstOrDefault();


                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public string ValidarNombreRepresentanteLegal(ref OperationResult ObjOperationResult, bool ValidarAmbos = true)
        {
            string Mensaje = "";
            var Trabajador = ObtenerTrabajadorbyIdTrabajador(ref ObjOperationResult, Globals.ClientSession.v_IdRepresentanteLegal);
            var NroLey = Globals.ClientSession.v_NroLeyCuartaCategoria.Trim();
            if (Trabajador == null)
            {
                Mensaje = Mensaje + " Nombre del Representante Legal\n";
            }
            if (ValidarAmbos)
            {
                if (string.IsNullOrEmpty(NroLey))
                {
                    Mensaje = Mensaje + " Nro.Ley\n";
                }
            }
            if (Mensaje != "")
            {
                return "Debe ir a la sección de Configuración - Configuración de Empresa - Planilla para configurar :\n" + Mensaje;
            }
            return Mensaje;


        }
        #endregion

    }
}
