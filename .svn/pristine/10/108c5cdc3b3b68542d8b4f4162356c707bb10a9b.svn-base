using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Transactions;
namespace SAMBHS.CommonWIN.BL
{
    public class DocumentoBL
    {
        public void DocumentoNuevo(ref OperationResult pobjOperationResult, documentoDto pobjDtoEntity, List<string> ClientSession, documentorolDto pobjDtoDocumentoRol = null)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                documento objEntity = documentoAssembler.ToEntity(pobjDtoEntity);

                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;

                //Guardado de la entidad Documento
                dbContext.AddTodocumento(objEntity);

                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "documento", objEntity.i_CodigoDocumento.ToString());
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoBL.DocumentoNuevo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }

        }

        private void SaveLog(string p1, string p2, string p3, LogEventType logEventType, string p4, string p5, Success success, string p6)
        {
            throw new NotImplementedException();
        }

        public List<documentoDto> ObtenDocumentosPaginadoFiltrado(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = from A in dbContext.documento
                            where A.i_Eliminado == 0
                            orderby A.i_CodigoDocumento ascending
                            select new documentoDto
                            {
                                i_CodigoDocumento = A.i_CodigoDocumento,
                                i_UsadoCompras = A.i_UsadoCompras,
                                i_UsadoContabilidad = A.i_UsadoContabilidad,
                                i_UsadoLibroDiario = A.i_UsadoLibroDiario,
                                i_UsadoTesoreria = A.i_UsadoTesoreria,
                                i_UsadoVentas = A.i_UsadoVentas,
                                v_Nombre = A.v_Nombre,
                                v_Siglas = A.v_Siglas,
                                i_Naturaleza = A.i_Naturaleza,
                                i_UsadoPedidoCotizacion = A.i_UsadoPedidoCotizacion,
                                i_RequiereSerieNumero = A.i_RequiereSerieNumero,
                                i_OperacionTransitoria = A.i_OperacionTransitoria ?? 0,
                                i_BancoDetraccion = A.i_BancoDetraccion ?? 0
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
                {
                    int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                    query = query.Skip(intStartRowIndex);
                }
                if (pintResultsPerPage.HasValue)
                {
                    query = query.Take(pintResultsPerPage.Value);
                }
                //query = query.OrderBy("i_CodigoDocumento");
                List<documentoDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public documentoDto ObtenDocumentosPorID(ref OperationResult pobjOperationResult, int intDocId)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                documentoDto objDtoEntity = null;
                var objEntity = (from a in dbContext.documento
                                 where a.i_CodigoDocumento == intDocId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = documentoAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;

                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<documentoDto> CheckByID(ref OperationResult pobjOperationResult, int pintDocumentID)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = from A in dbContext.documento
                            where A.i_Eliminado == 0 && A.i_CodigoDocumento == pintDocumentID
                            orderby A.i_CodigoDocumento ascending
                            select new documentoDto
                            {
                                i_CodigoDocumento = A.i_CodigoDocumento,
                                v_Siglas = A.v_Siglas,
                            };

                List<documentoDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<documentoDto> CheckBySigla(ref OperationResult pobjOperationResult, string pStringSigla)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = from A in dbContext.documento
                            where A.i_Eliminado == 0 && A.v_Siglas == pStringSigla
                            orderby A.i_CodigoDocumento ascending
                            select new documentoDto
                            {
                                i_CodigoDocumento = A.i_CodigoDocumento,
                            };

                List<documentoDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void DocumentoActualiza(ref OperationResult pobjOperationResult, documentoDto pobjDtoEntity, List<string> ClientSession, documentorolDto pobjDtoDocumentoRol)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.documento
                                       where a.i_CodigoDocumento == pobjDtoEntity.i_CodigoDocumento
                                       select a).FirstOrDefault();

                documento objEntity = documentoAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.documento.ApplyCurrentValues(objEntity);

                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "documento", objEntitySource.i_CodigoDocumento.ToString());
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoBL.DocumentoActualiza()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void DocumentoBorrar(ref OperationResult pobjOperationResult, int pIntDocId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.documento
                                       where a.i_CodigoDocumento == pIntDocId
                                       select a).FirstOrDefault();

                dbContext.DeleteObject(objEntitySource);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "documento", objEntitySource.i_CodigoDocumento.ToString());
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoBL.DocumentoBorrar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGrid(ref OperationResult pobjOperationResult, string pstrSortExpression, int pintUsadoCompras, int pintUsadoVentas)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 join A in dbContext.establecimientodetalle on a.i_CodigoDocumento equals A.i_IdTipoDocumento //Trae solo documentos que fueron registrados en establecimientodetalle

                                 where a.i_Eliminado == 0 && A.i_Eliminado == 0 && A.i_Eliminado == 0 && A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento
                                 select a).Distinct();

                    if (pintUsadoCompras == 1)
                    {
                        query = query.Where("i_UsadoCompras==1");
                    }
                    else if (pintUsadoVentas == 1)
                    {
                        query = query.Where("i_UsadoVentas==1");
                    }


                    query = query.OrderBy("i_CodigoDocumento");

                    var query2 = query.AsEnumerable()
                                .Select(x => new GridKeyValueDTO
                                {
                                    Id = x.i_CodigoDocumento.ToString(),
                                    Value1 = x.v_Nombre,
                                    Value2 = x.v_Siglas,
                                    EsDocInterno = x.i_UsadoDocumentoInterno == 1 ? true : false,
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridAll(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = dbContext.documento.Where(a => a.i_Eliminado == 0).Distinct();

                    query = query.OrderBy("i_CodigoDocumento");

                    var query2 = query.AsEnumerable()
                                .Select(x => new GridKeyValueDTO
                                {
                                    Id = x.i_CodigoDocumento.ToString(),
                                    Value1 = x.v_Nombre,
                                    Value2 = x.v_Siglas,
                                    EsDocInterno = x.i_UsadoDocumentoInterno == 1,
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridCompras(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 where a.i_Eliminado == 0 && a.i_UsadoCompras == 1
                                 select a).Distinct();
                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridImportaciones(ref OperationResult pobjOperationResult)
        {
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 where a.i_Eliminado == 0 && a.i_UsadoCompras == 1 && (a.i_CodigoDocumento == 50 || a.i_CodigoDocumento == 52)
                                 select a).Distinct();
                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<KeyValueDTO> TodosDocumentos(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 where a.i_Eliminado == 0
                                 select a).Distinct();
                    query = query.OrderBy("i_CodigoDocumento");

                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Id = x.i_CodigoDocumento.ToString("000"),
                                    Value1 = x.v_Nombre,
                                    Value2 = x.v_Siglas
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridComprasVentas(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 where a.i_Eliminado == 0 && a.i_UsadoCompras == 1 || a.i_UsadoVentas == 1
                                 select a).Distinct();
                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridLibroDiarios(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 where a.i_Eliminado == 0 && (a.i_UsadoCompras == 1 || a.i_UsadoVentas == 1 || a.i_UsadoLibroDiario == 1)
                                 select a).Distinct();

                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridGuiaRemision(ref OperationResult pobjOperationResult, string pstrSortExpression, int pintUsadoCompras, int pintUsadoVentas)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.documento

                                 join A in dbContext.establecimientodetalle on a.i_CodigoDocumento equals A.i_IdTipoDocumento //Trae solo documentos que fueron registrados en establecimientodetalle

                                 where a.i_Eliminado == 0 && A.i_Eliminado == 0 && A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento && (a.i_CodigoDocumento == 1 | a.i_CodigoDocumento == 3)
                                 select a).Distinct();

                    if (pintUsadoCompras == 1)
                    {
                        query = query.Where("i_UsadoCompras==1");
                    }
                    else if (pintUsadoVentas == 1)
                    {
                        query = query.Where("i_UsadoVentas==1");
                    }


                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosPedidosParaComboGrid(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.documento
                                where a.i_Eliminado == 0 && a.i_UsadoPedidoCotizacion == 1
                                select a;

                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosLibroDiarioParaComboGrid(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.documento
                                where a.i_Eliminado == 0 && a.i_UsadoContabilidad == 1
                                select a;

                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosCobranzaParaComboGrid(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.documento
                                where a.i_Eliminado == 0 && a.i_UsadoTesoreria == 1
                                select a;

                    query = query.OrderBy("i_CodigoDocumento");

                    var query2 = query.AsEnumerable()
                                .Select(x => new GridKeyValueDTO
                                {
                                    Id = x.i_CodigoDocumento.ToString(),
                                    Value1 = x.v_Nombre,
                                    Value2 = x.v_Siglas,
                                    Value3 = x.v_NroCuenta
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<KeyValueDTO> ObtenDocumentosParaCombo(ref OperationResult pobjOperationResult, string pstrSortExpression, int pintUsadoCompras, int pintUsadoVentas)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.documento
                                where a.i_Eliminado == 0
                                select a;

                    if (pintUsadoCompras == 1)
                    {
                        query = query.Where("i_UsadoCompras==1");
                    }
                    else if (pintUsadoVentas == 1)
                    {

                        query = query.Where("i_UsadoVentas==1");
                    }
                    else if (pintUsadoVentas == 2)
                    {

                        query = query.Where("i_UsadoVentas==1 or i_UsadoPedidoCotizacion==1");
                    }
                    else if (pintUsadoVentas == 3)
                    {

                        query = query.Where("i_UsadoPedidoCotizacion==1");
                    }



                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }
                    else
                    {
                        query = query.OrderBy("v_Nombre");
                    }

                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Id = x.i_CodigoDocumento.ToString(),
                                    Value1 = x.v_Nombre
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenDocumentosParaComboGridTesoreria(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = dbContext.documento.Join(dbContext.establecimientodetalle, a => a.i_CodigoDocumento,
                        A => A.i_IdTipoDocumento, (a, A) => new {a, A})
                        .Where(@t => @t.a.i_Eliminado == 0 && @t.A.i_Eliminado == 0 && @t.A.i_Eliminado == 0 &&
                                     @t.A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento
                                     && @t.a.i_UsadoTesoreria == 1).Select(@t => @t.a);

                    query = query.OrderBy("i_CodigoDocumento");

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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public string DevolverSeriePorDocumento(int? pintIdEstablecimiento, int pintIdDocumento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var _Serie = (from n in dbContext.establecimientodetalle
                    where n.i_IdEstablecimiento == pintIdEstablecimiento
                          && n.i_IdTipoDocumento == pintIdDocumento && n.i_DocumentoPredeterminado == 1
                          && n.i_Eliminado == 0
                    select new { n.v_Serie }).FirstOrDefault();

                if (_Serie != null)
                {
                    string Serie = _Serie.v_Serie.Trim();
                    return Serie;
                }
                return string.Empty;
            }
        }

        public int DevolverCorrelativoPorDocumento(int? pintIdEstablecimiento, int pintIdDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var _Correlativo = (from n in dbContext.establecimientodetalle
                                    where n.i_IdEstablecimiento == pintIdEstablecimiento
                                    && n.i_IdTipoDocumento == pintIdDocumento && n.i_DocumentoPredeterminado == 1
                                    && n.i_Eliminado == 0
                                    select new { n.i_Correlativo }).FirstOrDefault();

                if (_Correlativo != null)
                {
                    int Correlativo = _Correlativo.i_Correlativo.Value;
                    return Correlativo;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<establecimientodetalleDto> ListaEstablecimientoDetalle()
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var lista = (from a in dbContext.establecimientodetalle

                                 where a.i_Eliminado == 0 && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento && a.i_Almacen == Globals.ClientSession.i_IdAlmacenPredeterminado

                                 select new establecimientodetalleDto
                                 {
                                     v_NombreImpresora = a.v_NombreImpresora.Trim(),
                                     i_ImpresionVistaPrevia = a.i_ImpresionVistaPrevia,
                                     i_IdTipoDocumento = a.i_IdTipoDocumento,
                                     v_Serie = a.v_Serie.Trim(),
                                 }).ToList();

                    return lista;


                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public void ActualizarCorrelativoPorSerie(ref OperationResult pobjOperationResult, int? pintEstablecimiento, int tipoDocumento, string pstrSerie, int pintCorrelativo)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                establecimientodetalle establecimientodetalleEntity = (from n in dbContext.establecimientodetalle
                                                                       where n.i_IdEstablecimiento == pintEstablecimiento && n.v_Serie == pstrSerie
                                                                       && n.i_IdTipoDocumento == tipoDocumento
                                                                       && n.i_Eliminado == 0
                                                                       select n).FirstOrDefault();
                if (establecimientodetalleEntity != null)
                {

                    using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                    {
                        if (pintCorrelativo > establecimientodetalleEntity.i_Correlativo.Value)
                        {
                            establecimientodetalleEntity.i_Correlativo = pintCorrelativo;
                            dbContext.establecimientodetalle.ApplyCurrentValues(establecimientodetalleEntity);
                            dbContext.SaveChanges();
                        }

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
                else
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.ErrorMessage = "No se encontró el Documento en la Configuración de Empresa";
                    pobjOperationResult.AdditionalInformation = "DocumentoBL.ActualizarCorrelativoPorSerie()";
                }




            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoBL.ActualizarCorrelativoPorSerie()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                if (ex.InnerException != null) pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                return;
            }
        }

        public string CorrelativoxSerie(int i_IdTipoDoc, string pstrserie)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var x = (from c in dbContext.establecimientodetalle
                         where c.i_IdTipoDocumento == i_IdTipoDoc && c.v_Serie == pstrserie
                         && c.i_Eliminado == 0 && c.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento
                         select new { c.i_Correlativo }).FirstOrDefault();

                if (x == null)
                {
                    return null;
                }
                else
                {
                    return x.i_Correlativo.Value.ToString("00000000");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ImpresionVistaPrevia(int TipoDoc, string Serie)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Impresion = (from a in dbContext.establecimientodetalle
                                     where a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento && a.i_Almacen == Globals.ClientSession.i_IdAlmacenPredeterminado
                                     && a.i_IdTipoDocumento == TipoDoc && a.i_Eliminado == 0 && a.v_Serie.Trim() == Serie.Trim()

                                     select new
                                     {
                                         i_ImpresionVistaPrevia = a.i_ImpresionVistaPrevia == null || a.i_ImpresionVistaPrevia.Value == 0 ? false : true
                                     }).FirstOrDefault();

                    return Impresion != null ? Impresion.i_ImpresionVistaPrevia : false;
                }


            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public static int ObtenerDocumentoBancoDetraccion()
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var doc = dbContext.documento.FirstOrDefault(p => (p.i_BancoDetraccion ?? 0) == 1);
                    return doc == null ? -1 : doc.i_CodigoDocumento;
                }
            }
            catch
            {
                return -1;
            }
        }

        public establecimientodetalle ConfiguracionEstablecimiento(int TipoDoc, string Serie)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var establecimientoDetalle = (from a in dbContext.establecimientodetalle
                                                  where a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento && a.i_Almacen == Globals.ClientSession.i_IdAlmacenPredeterminado
                                                  && a.i_IdTipoDocumento == TipoDoc && a.i_Eliminado == 0 && a.v_Serie.Trim() == Serie.Trim()
                                                  select a).FirstOrDefault();

                    return establecimientoDetalle;
                }


            }
            catch (Exception ex)
            {
                return null;

            }

        }

        public static int ObtenerLimiteDocumento(int idDocumento, string serie)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var establecimientoActual = Globals.ClientSession.i_IdEstablecimiento ?? 0;
                    var doc =
                        dbContext.establecimientodetalle.FirstOrDefault(
                            p =>
                                p.i_IdEstablecimiento.Value == establecimientoActual &&
                                p.i_IdTipoDocumento.Value == idDocumento && p.v_Serie.Equals(serie) &&
                                p.i_Eliminado == 0);

                    if (doc != null) return (doc.i_NumeroItems ?? 10) > 0 ? doc.i_NumeroItems ?? 0 : 10;
                    return 10;
                }
            }
            catch
            {
                return 10;
            }
        }

        public bool TieneCuentaValida(int CodDocumento)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var Result = (from n in dbContext.documento
                    where n.i_CodigoDocumento == CodDocumento
                    select n).FirstOrDefault();

                if (Result != null)
                {
                    if (!string.IsNullOrEmpty(Result.v_NroCuenta) && Utils.Windows.EsCuentaImputable(Result.v_NroCuenta))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        public bool DocumentoEsContable(int TipoDocumento)
        {
            return Globals.ListaDocumentosContable.Contains(TipoDocumento);
        }

        public List<int> DocumentoEsContable()
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var doc = (from a in dbContext.documento
                           where a.i_Eliminado == 0 && a.i_UsadoDocumentoContable == 1
                           select a.i_CodigoDocumento);

                List<int> ListaFinal = doc.ToList();

                return ListaFinal;
            }

        }

        public List<int> DocumentoEsInverso()
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var doc = (from a in dbContext.documento
                           where a.i_Eliminado == 0 && a.i_UsadoDocumentoInverso == 1
                           select a.i_CodigoDocumento);

                List<int> ListaFinal = doc.ToList();

                return ListaFinal;
            }

        }

        public bool DocumentoEsInverso(int TipoDocumento)
        {
            return Globals.ListaDocumentosInversos.Any(x => x == TipoDocumento);

        }

        public bool DocumentoGeneraStock(int TipoDocumento)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var GeneraStock = (from a in dbContext.documento
                                   where a.i_Eliminado == 0 && a.i_DescontarStock != null && a.i_DescontarStock == 1 && a.i_CodigoDocumento == TipoDocumento
                                   select a).ToList();
                return GeneraStock.Any();
            }
        }

        public List<documentorolDto> ObtenerListadoDocumentosRol(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, int TipoDocumento)
        {
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.documentorol
                                where A.i_Eliminado == 0 && A.i_IdTipoDocumento == TipoDocumento

                                select new documentorolDto
                                {
                                    i_IdDocumentoRol = A.i_IdDocumentoRol,
                                    i_CodigoEnum = A.i_CodigoEnum,
                                    i_IdTipoDocumento = A.i_IdTipoDocumento,
                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<documentorolDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Indica si las siglas para el documento están siendo usando en otro documento.
        /// </summary>
        /// <param name="pstrSigas"></param>
        /// <param name="pintCodigoDocumento"></param>
        /// <returns></returns>
        public bool SiglasNoDisponibles(string pstrSigas, int pintCodigoDocumento = -1)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    return pintCodigoDocumento.Equals(-1) ? dbContext.documento.Any(p => p.v_Siglas.Equals(pstrSigas) && p.i_Eliminado.Equals(0))
                        : dbContext.documento.Any(p => p.v_Siglas.Equals(pstrSigas) && p.i_Eliminado.Equals(0) && !p.i_CodigoDocumento.Equals(pintCodigoDocumento));
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EliminaDocumentosParaMigracion(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var docs = dbContext.documento;
                    foreach (var doc in docs)
                    {
                        dbContext.documento.DeleteObject(doc);
                    }
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    return true;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoBL.EliminaDocumentosParaMigracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }



        #region Reportes
        public List<ReporteDocumentos> ReporteDocumentos(ref OperationResult objOperationResult)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                objOperationResult.Success = 1;
                var reporteDocumentos = (from n in dbContext.documento

                                         where n.i_Eliminado == 0

                                         select new ReporteDocumentos
                                         {

                                             Codigo = "",
                                             Sigla = n.v_Siglas,
                                             Nombre = n.v_Nombre,
                                             Usado_Compras = n.i_UsadoCompras == 1 ? "S" : "",
                                             Usado_Ventas = n.i_UsadoVentas == 1 ? "S" : "",
                                             Usado_Tesoreria = n.i_UsadoTesoreria == 1 ? "S" : "",
                                             Usado_Contabilidad = n.i_UsadoContabilidad == 1 ? "S" : "",
                                             CodigoInt = n.i_CodigoDocumento,

                                         }).ToList();

                List<ReporteDocumentos> ReporteDocumentosFinal = new List<ReporteDocumentos>();

                ReporteDocumentosFinal = (from a in reporteDocumentos

                                          select new ReporteDocumentos
                                          {

                                              Codigo = a.CodigoInt.Value.ToString("000"),
                                              Sigla = a.Sigla,
                                              Nombre = a.Nombre,
                                              Usado_Compras = a.Usado_Compras,
                                              Usado_Ventas = a.Usado_Ventas,
                                              Usado_Tesoreria = a.Usado_Tesoreria,
                                              Usado_Contabilidad = a.Usado_Contabilidad,
                                              CodigoInt = a.CodigoInt,
                                          }).ToList();

                return ReporteDocumentosFinal.OrderBy (o=>o.Codigo).ToList ();
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }

        }

        #endregion
    }

    public static class DocumentoRolBl
    {
        public enum RolDocumento
        {
            Seleccionar = -1,
            Pedido = 1,
            Cotizacion = 2
        }

        public static BindingList<RelacionDocumentoRol> ObtenerRolesDocumento(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var ds = dbContext.documentorol.Where(p => p.i_Eliminado == 0).ToList()
                        .Select(n => new RelacionDocumentoRol
                        {
                            IdDocumento = n.i_IdTipoDocumento ?? 0,
                            IdRol = n.i_CodigoEnum ?? 0
                        }).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<RelacionDocumentoRol>(ds);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoRolBl.ObtenerRolesDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public class RelacionDocumentoRol
        {
            public RolDocumento RolEnum
            {
                get
                {
                    return (RolDocumento)Enum.Parse(typeof(RolDocumento), IdRol.ToString());
                }

                set
                {
                    IdRol = (int)value;
                }
            }
            public int IdRol { get; set; }
            public int IdDocumento { get; set; }
        }

        public static void ActualizarDocumentosRol(ref OperationResult pobjOperationResult, List<RelacionDocumentoRol> plist)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitiesToDelete = dbContext.documentorol.ToList();
                    entitiesToDelete.ForEach(dbContext.documentorol.DeleteObject);

                    var entitiesToAdd = plist
                        .Select(o => new documentorol { i_Eliminado = 0, i_CodigoEnum = o.IdRol, i_IdTipoDocumento = o.IdDocumento })
                        .ToList();

                    entitiesToAdd.ForEach(p => dbContext.documentorol.AddObject(p));
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoRolBl.InsertarDocumentosRol()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }

    public struct DocumentoData
    {
        public string NroCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public bool BancoDetraccion { get; set; }
    }
}
