using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Contabilidad.BL
{
    public class FlujoEfectivoBl
    {
        public List<flujoefectivoconfiguracionDto> ObtenerConfiguracionCuentas(ref OperationResult pobjOperationResult,
            string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data = (from n in dbContext.flujoefectivoconfiguracion
                                join c in dbContext.asientocontable on new { cta = n.v_NroCuenta, l = 2, e = 0, p = periodo }
                                    equals new { cta = c.v_NroCuenta, l = c.i_LongitudJerarquica.Value, e = c.i_Eliminado.Value, p = c.v_Periodo }
                                    into cJoin
                                from c in cJoin.DefaultIfEmpty()
                                where n.v_Periodo.Equals(periodo)
                                select new flujoefectivoconfiguracionDto
                                {
                                    v_Periodo = n.v_Periodo,
                                    v_NroCuenta = n.v_NroCuenta,
                                    i_Id = n.i_Id,
                                    NombreCuenta = c.v_NroCuenta + " - " + c.v_NombreCuenta,
                                    i_EsCuentaActivo = n.i_EsCuentaActivo,
                                    i_IdConceptoFlujo = n.i_IdConceptoFlujo,
                                    i_IdTipoActividad = n.i_IdTipoActividad,
                                    i_IdTipoCuenta = n.i_IdTipoCuenta,
                                    v_NroCuentaPasivos = n.v_NroCuentaPasivos
                                }).ToList();

                    if (!data.Any())
                    {
                        var ctasMayores =
                            dbContext.asientocontable.Where(
                                p => p.v_Periodo.Equals(periodo) && p.i_LongitudJerarquica == 2 && p.i_Eliminado == 0)
                                .ToList();

                        data = ctasMayores.Select(n => new flujoefectivoconfiguracionDto
                        {
                            v_Periodo = n.v_Periodo,
                            v_NroCuenta = n.v_NroCuenta,
                            NombreCuenta = n.v_NroCuenta + " - " + n.v_NombreCuenta,
                            i_EsCuentaActivo = (n.i_Naturaleza ?? 0) == 1 ? 1 : 0,
                            i_IdTipoActividad = -1,
                            i_IdTipoCuenta = -1
                        }).ToList();
                    }
                    pobjOperationResult.Success = 1;
                    return data.OrderBy(p => p.v_NroCuenta).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerConfiguracionCuentas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public void ActualizarConfiguracionCuentas(ref OperationResult pobjOperationResult,
            List<flujoefectivoconfiguracionDto> lista, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var anteriores = dbContext.flujoefectivoconfiguracion.Where(p => p.v_Periodo.Equals(periodo));
                    anteriores.ToList().ForEach(o => dbContext.DeleteObject(o));
                    lista.ForEach(p => dbContext.flujoefectivoconfiguracion.AddObject(p.ToEntity()));
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ActualizarConfiguracionCuentas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public List<CalculoFlujoEfectivoDto> ObtenerSaldosParaCalculo(ref OperationResult pobjOperationResult,
            int periodoActual)
        {
            try
            {
                var rr = new List<CalculoFlujoEfectivoDto>();
                var objBl = new ContabilidadDao();
                var fIni = new DateTime(periodoActual - 1, 1, 1);
                var fFin = new DateTime(periodoActual, 12, 31, 23, 59, 1);
                var lista = Utils.Windows.RangoDeCuentas("10", "59");
                var asientosExcepciones = new[] { 5 }; //<- 5: Asiento de cierre

                var data = objBl.ObtenerDataLibroMayor(ref pobjOperationResult, fIni, fFin, null, lista, 1);
                data = data.Where(p => !asientosExcepciones.Contains(p.TipoComprobante)).ToList();
                if (pobjOperationResult.Success == 0) return null;
                foreach (var n in data.GroupBy(g => new { c = g.cuenta.Trim().Substring(0, 2) }))
                {
                    var provicionA = n.Where(p => p.fecha.Year == periodoActual && p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var provicionesActual = provicionA.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var provicionB = n.Where(p => p.fecha.Year == periodoActual && !p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var cancelacionesActual = provicionB.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var provicionC = n.Where(p => p.fecha.Year == periodoActual - 1 && p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var provicionesAnterior = provicionC.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var provicionD = n.Where(p => p.fecha.Year == periodoActual - 1 && !p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var cancelacionesAnterior = provicionD.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var r = new CalculoFlujoEfectivoDto
                    {
                        CtaMayor = n.Key.c.Substring(0, 2),
                        DescripcionCuenta = n.FirstOrDefault().cuentaMayor,
                        BalancePeriodoActual = provicionesActual - cancelacionesActual,
                        BalancePeriodoAnterior = provicionesAnterior - cancelacionesAnterior,
                        NaturalezaCuenta = n.FirstOrDefault().NaturalezaMayor
                    };

                    if (r.BalancePeriodoActual == 0 && r.BalancePeriodoAnterior == 0) continue;
                    rr.Add(r);
                }
                var result = rr.OrderBy(o => o.NaturalezaCuenta).ThenBy(oo => oo.CtaMayor).ToList();

                var resultadoEjercicio = new CalculoFlujoEfectivoDto
                {
                    BalancePeriodoActual = result.Where(p => p.NaturalezaCuenta == 1).Sum(s => s.BalancePeriodoActual) - result.Where(p => p.NaturalezaCuenta == 2).Sum(s => s.BalancePeriodoActual),
                    BalancePeriodoAnterior = result.Where(p => p.NaturalezaCuenta == 1).Sum(s => s.BalancePeriodoAnterior) - result.Where(p => p.NaturalezaCuenta == 2).Sum(s => s.BalancePeriodoAnterior),
                    DescripcionCuenta = "89 - RESULTADO DEL EJERCICIO",
                    CtaMayor = "89",
                    NaturalezaCuenta = 2
                };
                result.Insert(result.Count, resultadoEjercicio);

                var sumatoriaActivos = new CalculoFlujoEfectivoDto
                {
                    BalancePeriodoActual = result.Where(p => p.NaturalezaCuenta == 1).Sum(s => s.BalancePeriodoActual),
                    BalancePeriodoAnterior = result.Where(p => p.NaturalezaCuenta == 1).Sum(s => s.BalancePeriodoAnterior),
                    DescripcionCuenta = "Activos",
                    NaturalezaCuenta = -1
                };

                var sumatoriaPasivos = new CalculoFlujoEfectivoDto
                {
                    BalancePeriodoActual = result.Where(p => p.NaturalezaCuenta == 2).Sum(s => s.BalancePeriodoActual),
                    BalancePeriodoAnterior = result.Where(p => p.NaturalezaCuenta == 2).Sum(s => s.BalancePeriodoAnterior),
                    DescripcionCuenta = "Pasivos y patrimonio",
                    NaturalezaCuenta = -1
                };

                result.Insert(result.Count(p => p.NaturalezaCuenta == 1), sumatoriaActivos);
                result.Insert(result.Count, sumatoriaPasivos);
                pobjOperationResult.Success = 1;
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerSaldosParaCalculo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public List<CalculoFlujoEfectivoDto> ObtenerCuentasPorFuncion(ref OperationResult pobjOperationResult,
            int periodoActual)
        {
            try
            {
                var rr = new List<CalculoFlujoEfectivoDto>();
                var objBl = new ContabilidadDao();
                var fIni = new DateTime(periodoActual - 1, 1, 1);
                var fFin = new DateTime(periodoActual, 12, 31, 23, 59, 1);
                var lista = Utils.Windows.RangoDeCuentas("69", "971");
                var asientosExcepciones = new[] { 5 }; //<- 5: Asiento de cierre

                var data = objBl.ObtenerDataLibroMayor(ref pobjOperationResult, fIni, fFin, null, lista.ToList(), 1);
                if (pobjOperationResult.Success == 0) return null;
                data = data.Where(p => !asientosExcepciones.Contains(p.TipoComprobante) &&
                                !p.cuenta.StartsWith("79") && !p.cuenta.StartsWith("71")).ToList();

                foreach (var n in data.GroupBy(g => new { c = g.cuenta.Trim().Substring(0, 2) }))
                {
                    var provicionA = n.Where(p => p.fecha.Year == periodoActual && p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var provicionesActual = provicionA.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var provicionB = n.Where(p => p.fecha.Year == periodoActual && !p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var cancelacionesActual = provicionB.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var provicionC = n.Where(p => p.fecha.Year == periodoActual - 1 && p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var provicionesAnterior = provicionC.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var provicionD = n.Where(p => p.fecha.Year == periodoActual - 1 && !p.Naturaleza.Equals(p.NaturalezaReal)).ToList();
                    var cancelacionesAnterior = provicionD.Sum(s => s.monedaTransaccion.Equals("S/.") ? s.Importe : s.Cambio);

                    var r = new CalculoFlujoEfectivoDto
                    {
                        CtaMayor = n.Key.c.Substring(0, 2),
                        DescripcionCuenta = n.FirstOrDefault().cuentaMayor,
                        BalancePeriodoActual = provicionesActual - cancelacionesActual,
                        BalancePeriodoAnterior = provicionesAnterior - cancelacionesAnterior,
                        NaturalezaCuenta = n.FirstOrDefault().NaturalezaMayor
                    };

                    if (r.BalancePeriodoActual == 0 && r.BalancePeriodoAnterior == 0) continue;
                    rr.Add(r);
                }

                var utilidadNeta = new CalculoFlujoEfectivoDto
                {
                    BalancePeriodoActual = rr.Where(p => p.NaturalezaCuenta == 2).Sum(s => s.BalancePeriodoActual) - rr.Where(p => p.NaturalezaCuenta == 1).Sum(s => s.BalancePeriodoActual),
                    BalancePeriodoAnterior = rr.Where(p => p.NaturalezaCuenta == 2).Sum(s => s.BalancePeriodoAnterior) - rr.Where(p => p.NaturalezaCuenta == 1).Sum(s => s.BalancePeriodoAnterior),
                    DescripcionCuenta = "Utilidad Neta",
                    NaturalezaCuenta = -2
                };
                rr = rr.OrderBy(oo => oo.CtaMayor).ToList();

                rr.Add(utilidadNeta);
                pobjOperationResult.Success = 1;
                var result = rr.ToList();
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerCuentasPorFuncion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public void ActualizarFlujoEfectivo(ref OperationResult pobjOperationResult, List<CalculoFlujoEfectivoDto> data)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var idCabecera = 0;
                        var periodo = Globals.ClientSession.i_Periodo.ToString();
                        var c = dbContext.flujoefectivocabecera.FirstOrDefault(p => p.v_PeriodoProceso.Equals(periodo));
                        if (c == null)
                        {
                            var cabecera = dbContext.flujoefectivocabecera.CreateObject();
                            cabecera.v_PeriodoProceso = periodo;
                            dbContext.flujoefectivocabecera.AddObject(cabecera);
                            dbContext.SaveChanges();
                            c = dbContext.flujoefectivocabecera.FirstOrDefault(p => p.v_PeriodoProceso.Equals(periodo));
                            if (c != null) idCabecera = c.i_IdFlujoEfectivoCabecera;
                        }
                        else
                            idCabecera = c.i_IdFlujoEfectivoCabecera;

                        if (idCabecera > 0)
                        {
                            var flujoToDelete = dbContext.flujoefectivo.Where(p => p.i_IdFlujoEfectivoCabecera == idCabecera).ToList();
                            flujoToDelete.ForEach(o => dbContext.DeleteObject(o));
                            dbContext.SaveChanges();

                            var dataToInsert = data.Select(n => new flujoefectivo
                            {
                                d_AjusteDebe = n.AjusteDebe,
                                d_AjusteHaber = n.AjusteHaber,
                                d_Aumento = n.Aumento,
                                d_BalancePeriodoActual = n.BalancePeriodoActual,
                                d_BalancePeriodoAnterior = n.BalancePeriodoAnterior,
                                d_Disminucion = n.Disminucion,
                                d_Financiamiento = n.Financiamiento,
                                d_Inversion = n.Inversion,
                                d_MetodoDirecto = n.MetodoDirecto,
                                d_Operacion = n.Operacion,
                                i_NaturalezaCuenta = n.NaturalezaCuenta,
                                i_NroAsientoD = n.NroAsientoD,
                                i_NroAsientoH = n.NroAsientoH,
                                v_CtaMayor = n.CtaMayor,
                                v_DescripcionCuenta = n.DescripcionCuenta,
                                i_IdFlujoEfectivoCabecera = idCabecera,
                                d_Aplicacion = n.Aplicacion,
                                d_Origen = n.Origen
                            }).ToList();

                            dataToInsert.ForEach(o => dbContext.flujoefectivo.AddObject(o));
                            dbContext.SaveChanges();
                        }
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.GuardarFlujoEfectivo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public List<CalculoFlujoEfectivoDto> ObtenerFlujoGuardado(ref OperationResult pobjOperationResult,
            string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.flujoefectivo
                                  join c in dbContext.flujoefectivocabecera on n.i_IdFlujoEfectivoCabecera equals
                                      c.i_IdFlujoEfectivoCabecera into cjoin
                                  from c in cjoin.DefaultIfEmpty()
                                  where c.v_PeriodoProceso.Equals(periodo)
                                  orderby n.i_IdFlujoEfectivo
                                  select new CalculoFlujoEfectivoDto
                                  {
                                      BalancePeriodoActual = n.d_BalancePeriodoActual ?? 0,
                                      BalancePeriodoAnterior = n.d_BalancePeriodoAnterior ?? 0,
                                      NaturalezaCuenta = n.i_NaturalezaCuenta ?? 0,
                                      CtaMayor = n.v_CtaMayor,
                                      DescripcionCuenta = n.v_DescripcionCuenta,
                                      NroAsientoH = n.i_NroAsientoH ?? 0,
                                      NroAsientoD = n.i_NroAsientoD ?? 0,
                                      AjusteHaber = n.d_AjusteHaber ?? 0,
                                      AjusteDebe = n.d_AjusteDebe ?? 0
                                  }).ToList();
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerFlujoGuardado()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public List<CalculoFlujoEfectivoDto> AplicarAsientoAjustes(ref OperationResult pobjOperationResult,
            List<CalculoFlujoEfectivoDto> data, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientos = (from n in dbContext.flujoefectivoasientoajustedetalle
                                    join c in dbContext.flujoefectivoasientoajuste on n.i_IdAsientoAjuste equals c.i_IdAsientoAjuste
                                        into cjoin
                                    from c in cjoin.DefaultIfEmpty()
                                    join cc in dbContext.flujoefectivocabecera on c.i_IdFlujoEfectivoCabecera equals
                                        cc.i_IdFlujoEfectivoCabecera into ccjoin
                                    from cc in ccjoin.DefaultIfEmpty()
                                    where cc.v_PeriodoProceso.Equals(periodo)
                                    select new flujoefectivoasientoajustedetalleDto
                                    {
                                        v_NroCuenta = n.v_NroCuenta,
                                        d_Cambio = n.d_Cambio,
                                        d_Importe = n.d_Importe,
                                        i_IdAsientoAjuste = n.i_IdAsientoAjuste,
                                        i_IdAsientoAjusteDetalle = n.i_IdAsientoAjusteDetalle,
                                        v_Naturaleza = n.v_Naturaleza,
                                        NroAsiento = c.i_NroAsiento ?? 0
                                    }).ToList();

                    data.ForEach(d =>
                    {
                        d.AjusteDebe = 0m;
                        d.AjusteHaber = 0m;
                        d.NroAsientoD = 0;
                        d.NroAsientoH = 0;
                    });

                    foreach (var a in asientos)
                    {
                        var fila = data.FirstOrDefault(p => p.CtaMayor != null && p.CtaMayor.Equals(a.v_NroCuenta));
                        if (fila != null)
                        {
                            var naturaleza = a.v_Naturaleza;
                            if (naturaleza.Equals("D"))
                            {
                                fila.AjusteDebe = a.d_Importe ?? 0;
                                fila.NroAsientoD = a.NroAsiento;
                            }
                            else
                            {
                                fila.AjusteHaber = a.d_Importe ?? 0;
                                fila.NroAsientoH = a.NroAsiento;
                            }
                        }
                    }
                    pobjOperationResult.Success = 1;
                    return data;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.AplicarAsientoAjustes()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public Dictionary<string, Tuple<int, string>> ObtenerRelaciones(string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data = dbContext.flujoefectivoconfiguracion.Where(p => p.v_Periodo.Equals(periodo)).ToList();
                    return data.ToDictionary(k => k.v_NroCuenta,
                        o => new Tuple<int, string>(o.i_IdTipoActividad ?? -1, null));
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void GeneraAsientosAutomaticos(ref OperationResult pobjOperationResult, string periodo, List<CalculoFlujoEfectivoDto> data)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var r = dbContext.flujoefectivoconfiguracion.Where(p =>
                            p.v_Periodo.Equals(periodo) && p.v_NroCuentaPasivos != null &&
                            p.v_NroCuentaPasivos.Trim() != "").ToList();

                        if (!r.Any()) return;
                        {
                            var nroAsiento = 1;
                            var objBl = new AsientoAjusteBl();
                            var dataD = data.Where(p => p.CtaMayor != null)
                                .ToDictionary(k => k.CtaMayor, o => o.BalancePeriodoActual);

                            foreach (var conf in r)
                            {
                                var ctaD = conf.v_NroCuenta;
                                var ctaH = conf.v_NroCuentaPasivos;
                                decimal importeBalance;
                                if (!dataD.TryGetValue(ctaD, out importeBalance)) continue;
                                var cabecera = new flujoefectivoasientoajusteDto
                                {
                                    d_TipoCambio = 3,
                                    i_IdMoneda = 1,
                                    i_NroAsiento = nroAsiento,
                                    v_Glosa = string.Format("ASIENTO AUTOMÁTICO CTA. {0}", ctaD)
                                };

                                var detalle = new List<flujoefectivoasientoajustedetalleDto>
                                {
                                    new flujoefectivoasientoajustedetalleDto
                                    {
                                        d_Importe = importeBalance,
                                        v_Naturaleza = "D",
                                        v_NroCuenta = ctaD
                                    },
                                    new flujoefectivoasientoajustedetalleDto
                                    {
                                        d_Importe = importeBalance,
                                        v_Naturaleza = "H",
                                        v_NroCuenta = ctaH
                                    }
                                };

                                objBl.ActualizarAsiento(ref pobjOperationResult, cabecera, detalle, periodo);
                                if (pobjOperationResult.Success == 0) return;
                                nroAsiento++;
                            }
                            ts.Complete();
                            pobjOperationResult.Success = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.GeneraAsientosAutomaticos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public List<FlujoEfectivoReporte> ObtenerReporte(ref OperationResult pobjOperationResult, List<CalculoFlujoEfectivoDto> flujoData, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var rep = new FlujoEfectivoReporte();
                    var conceptos = dbContext.flujoefectivoconceptos.Where(p => p.i_EsSumatoria == null || p.i_EsSumatoria != 100).OrderBy(o => o.i_Orden).ToList();
                    var detalles = dbContext.flujoefectivoconceptosdetalles.ToList();

                    foreach (var flujoefectivoconceptose in conceptos)
                    {
                        if (flujoefectivoconceptose.i_EsSumatoria == 0)
                        {
                            var detalle = detalles.Where(p => p.i_IdConceptoFlujo == flujoefectivoconceptose.i_IdConceptoFlujo).ToList();
                            if (!detalle.Any()) continue;
                            var ctasDetalle = detalle.Select(p => p.v_NroCuenta);

                            if (flujoefectivoconceptose.i_IdTipoAccion == 1) //Operacion
                                flujoefectivoconceptose.d_Importe =
                                    flujoData.Where(p => ctasDetalle.Contains(p.CtaMayor)).Sum(s => s.Operacion);
                            
                            if (flujoefectivoconceptose.i_IdTipoAccion == 2) //Inversion
                                flujoefectivoconceptose.d_Importe =
                                    flujoData.Where(p => ctasDetalle.Contains(p.CtaMayor)).Sum(s => s.Inversion);
                            
                            if (flujoefectivoconceptose.i_IdTipoAccion == 3) //Financiamiento
                                flujoefectivoconceptose.d_Importe =
                                    flujoData.Where(p => ctasDetalle.Contains(p.CtaMayor)).Sum(s => s.Financiamiento);

                            if (flujoefectivoconceptose.i_IdTipoAccion == 4) //Cta 10 al inicio del ejercicio
                                flujoefectivoconceptose.d_Importe =
                                    flujoData.Where(p => ctasDetalle.Contains(p.CtaMayor)).Sum(s => s.BalancePeriodoAnterior);

                            if (flujoefectivoconceptose.i_IdTipoAccion == 5) // Conciliación
                                flujoefectivoconceptose.d_Importe =
                                    flujoData.Where(p => ctasDetalle.Contains(p.CtaMayor)).Sum(s => s.MetodoDirecto);
                        }
                        else
                        {
                            if (flujoefectivoconceptose.i_EsSumatoria > 0 && flujoefectivoconceptose.i_EsSumatoria < 100)
                                flujoefectivoconceptose.d_Importe = conceptos.Where(p => p.i_IdTipoAccion == flujoefectivoconceptose.i_EsSumatoria).Sum(s => s.d_Importe);

                            if (flujoefectivoconceptose.i_EsSumatoria == -1)
                                flujoefectivoconceptose.d_Importe = conceptos.Where(p => p.i_IdTipoAccion == flujoefectivoconceptose.i_EsSumatoria).Sum(s => s.d_Importe);
                        }

                        if (flujoefectivoconceptose.i_Flag == 2)
                            flujoefectivoconceptose.d_Importe = conceptos.Where(p => p.i_Flag == 1).Sum(s => s.d_Importe);
                    }

                    for (int i = 1; i <= 52; i++)
                    {
                        var c = conceptos[i - 1];
                        if (c != null)
                        {
                            rep[i, true] = c.v_Descripcion;
                            rep[i, false] = c.d_Importe ?? 0m;
                        }
                    }

                    return new List<FlujoEfectivoReporte> { rep };
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerReporte()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }
    }

    public class AsientoAjusteBl
    {
        public int ObtenerNroAsiento(ref OperationResult pobjOperationResult, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientos = (from n in dbContext.flujoefectivoasientoajuste
                                    join c in dbContext.flujoefectivocabecera on n.i_IdFlujoEfectivoCabecera equals
                                                                                c.i_IdFlujoEfectivoCabecera into cJoin
                                    from c in cJoin.DefaultIfEmpty()
                                    where c.v_PeriodoProceso.Equals(periodo)
                                    orderby n.i_NroAsiento
                                    select n).ToList();

                    if (!asientos.Any()) return 1;
                    var ultimo = asientos.LastOrDefault();
                    return ultimo == null ? 1 : (ultimo.i_NroAsiento ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientoAjusteBL.ObtenerNroAsiento()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return 0;
            }
        }

        public flujoefectivoasientoajusteDto ObtenerCabecera(ref OperationResult pobjOperationResult, int id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var c = dbContext.flujoefectivoasientoajuste.FirstOrDefault(p => p.i_IdAsientoAjuste == id);
                    pobjOperationResult.Success = 1;
                    return c.ToDTO();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerCabecera()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public BindingList<flujoefectivoasientoajustedetalleDto> ObtenerDetalle(ref OperationResult pobjOperationResult, int id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var c = dbContext.flujoefectivoasientoajuste.FirstOrDefault(p => p.i_IdAsientoAjuste == id);
                    pobjOperationResult.Success = 1;
                    return c == null ? null :
                        new BindingList<flujoefectivoasientoajustedetalleDto>(c.flujoefectivoasientoajustedetalle.ToDTOs());
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FlujoEfectivoBL.ObtenerCabecera()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public void ActualizarAsiento(ref OperationResult pobjOperationResult, flujoefectivoasientoajusteDto cabecera,
            List<flujoefectivoasientoajustedetalleDto> detalle, string periodo)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var flujocabecera = dbContext.flujoefectivocabecera.First(p => p.v_PeriodoProceso.Equals(periodo));
                        if (flujocabecera == null) throw new Exception("No se guardo ningun flujo para el presente año.");

                        var nroAsiento = cabecera.i_NroAsiento;
                        var idCabecera = 0;
                        var c = dbContext.flujoefectivoasientoajuste.FirstOrDefault(
                                p => p.i_NroAsiento == nroAsiento);
                        if (c == null)
                        {
                            cabecera.i_IdFlujoEfectivoCabecera = flujocabecera.i_IdFlujoEfectivoCabecera;
                            dbContext.AddToflujoefectivoasientoajuste(cabecera.ToEntity());
                            dbContext.SaveChanges();
                            c = dbContext.flujoefectivoasientoajuste.FirstOrDefault(
                                p => p.i_NroAsiento == nroAsiento);
                            if (c != null) idCabecera = c.i_IdAsientoAjuste;
                        }
                        else
                        {
                            idCabecera = cabecera.i_IdAsientoAjuste;
                            dbContext.flujoefectivoasientoajuste.ApplyCurrentValues(cabecera.ToEntity());
                        }

                        if (idCabecera > 0)
                        {
                            var detalleAnterior = dbContext.flujoefectivoasientoajustedetalle.
                                Where(p => p.i_IdAsientoAjuste == idCabecera).ToList();

                            detalleAnterior.ForEach(p => dbContext.DeleteObject(p));

                            detalle.ForEach(p =>
                            {
                                p.i_IdAsientoAjuste = idCabecera;
                                dbContext.AddToflujoefectivoasientoajustedetalle(p.ToEntity());
                            });
                            dbContext.SaveChanges();
                        }
                        else
                            throw new Exception("No se encontró la cabecera del asiento.");

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientoAjusteBL.ActualizarAsiento()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public IEnumerable<flujoefectivoasientoajusteDto> ObtenerListadoAsientos(
            ref OperationResult pobjOperationResult, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var r = (from n in dbContext.flujoefectivoasientoajuste
                             join j1 in dbContext.flujoefectivocabecera on n.i_IdFlujoEfectivoCabecera equals
                                 j1.i_IdFlujoEfectivoCabecera into j1Join
                             from j1 in j1Join.DefaultIfEmpty()
                             where j1.v_PeriodoProceso.Equals(periodo)
                             orderby n.i_NroAsiento
                             select n).ToDTOs();
                    pobjOperationResult.Success = 1;
                    return r;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientoAjusteBL.ObtenerListadoAsientos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public void EliminarAsientoAjuste(ref OperationResult pobjOperationResult, int idAsiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var detalles = dbContext.flujoefectivoasientoajustedetalle.Where(p => p.i_IdAsientoAjuste == idAsiento).ToList();
                        var ajuste = dbContext.flujoefectivoasientoajuste.FirstOrDefault(p => p.i_IdAsientoAjuste == idAsiento);

                        detalles.ForEach(r => dbContext.DeleteObject(r));
                        dbContext.SaveChanges();
                        if (ajuste != null)
                        {
                            dbContext.DeleteObject(ajuste);
                            dbContext.SaveChanges();
                        }

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientoAjusteBL.EliminarAsientoAjuste()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public bool CuentaUsada(string periodo, string cta, string naturaleza = null)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    naturaleza = string.IsNullOrEmpty(naturaleza) ? null : naturaleza;
                    var ee = (from n in dbContext.flujoefectivoasientoajustedetalle
                              join a in dbContext.flujoefectivoasientoajuste on n.i_IdAsientoAjuste equals a.i_IdAsientoAjuste
                                  into aJoin
                              from a in aJoin.DefaultIfEmpty()
                              join b in dbContext.flujoefectivocabecera on a.i_IdFlujoEfectivoCabecera equals
                                  b.i_IdFlujoEfectivoCabecera into bJoin
                              from b in bJoin.DefaultIfEmpty()
                              where b.v_PeriodoProceso.Equals(periodo)
                              select n).ToList();

                    var e = ee.Any(p => (naturaleza == null || p.v_Naturaleza.Equals(naturaleza)) && p.v_NroCuenta.Equals(cta));
                    return e;
                }
            }
            catch
            {
                return true;
            }
        }
    }

    public class ConceptoFlujoBl
    {
        public List<flujoefectivoconceptosDto> ObtenerConceptos(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from p in dbContext.flujoefectivoconceptos
                                  where p.i_EsSumatoria == 0 || p.i_EsSumatoria == 100
                                  orderby p.i_Orden
                                  select new flujoefectivoconceptosDto
                                  {
                                      i_EsSumatoria = p.i_EsSumatoria,
                                      i_IdConceptoFlujo = p.i_IdConceptoFlujo,
                                      i_Orden = p.i_Orden,
                                      d_Importe = p.d_Importe,
                                      v_Descripcion = p.v_Descripcion,
                                      CtasRelacionadas = (from n in dbContext.flujoefectivoconceptosdetalles
                                                          where n.i_IdConceptoFlujo == p.i_IdConceptoFlujo
                                                          select n.v_NroCuenta).AsEnumerable(),
                                      i_Flag = p.i_Flag,
                                      i_IdTipoAccion = p.i_IdTipoAccion
                                  }).ToList();
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoFlujoBl.ObtenerConceptos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public List<flujoefectivoconceptosdetallesDto> ObtenerConceptoDetalles(ref OperationResult pobjOperationResult, int idConcepto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodo = Globals.ClientSession.i_Periodo.ToString();

                    var result = (from n in dbContext.flujoefectivoconceptosdetalles
                                  join c in dbContext.asientocontable on new { cta = n.v_NroCuenta, p = periodo }
                                                                    equals new { cta = c.v_NroCuenta, p = c.v_Periodo } into cJoin
                                  from c in cJoin.DefaultIfEmpty()
                                  where n.i_IdConceptoFlujo == idConcepto
                                  select new flujoefectivoconceptosdetallesDto
                                  {
                                      v_NroCuenta = n.v_NroCuenta,
                                      DenominacionCuenta = c.v_NombreCuenta,
                                      i_Id = n.i_Id,
                                      i_IdConceptoFlujo = n.i_IdConceptoFlujo
                                  }).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoFlujoBl.ObtenerConceptos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public void ActualizarConcepto(ref OperationResult pobjOperationResult, flujoefectivoconceptosDto pobjEntidad)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entidad = dbContext.flujoefectivoconceptos.FirstOrDefault(p => p.i_IdConceptoFlujo == pobjEntidad.i_IdConceptoFlujo);
                    if (entidad == null) throw new Exception("No se encontró el concepto.");
                    entidad = pobjEntidad.ToEntity();
                    dbContext.flujoefectivoconceptos.ApplyCurrentValues(entidad);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoFlujoBl.ActualizarConcepto()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public void IngresarDetalles(ref OperationResult pobjOperationResult, flujoefectivoconceptosdetallesDto detalleDto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var ctaExiste = dbContext.flujoefectivoconceptosdetalles.Any(p => p.v_NroCuenta.Equals(detalleDto.v_NroCuenta) && p.i_IdConceptoFlujo == detalleDto.i_IdConceptoFlujo);
                    if (!ctaExiste)
                    {
                        dbContext.flujoefectivoconceptosdetalles.AddObject(detalleDto.ToEntity());
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                    else
                        throw new Exception("La cuenta ya existe en el detalle!");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoFlujoBl.IngresarDetalles()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public void EliminarDetalles(ref OperationResult pobjOperationResult, flujoefectivoconceptosdetallesDto detalleDto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = dbContext.flujoefectivoconceptosdetalles.FirstOrDefault(p => p.i_Id == detalleDto.i_Id);
                    if (entity == null) return;
                    dbContext.flujoefectivoconceptosdetalles.DeleteObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoFlujoBl.EliminarDetalles()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public asientocontableDto ObtenerCuentaMayor(string nroCuenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var r = dbContext.asientocontable.FirstOrDefault(p => p.v_NroCuenta.Equals(nroCuenta) && p.i_LongitudJerarquica == 2 && p.i_Eliminado == 0);

                    return r.ToDTO();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
