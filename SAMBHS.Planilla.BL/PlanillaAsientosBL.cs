using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Tesoreria.BL;

namespace SAMBHS.Planilla.BL
{
    /// <summary>
    /// Clase en la que se almacena la lógica del negocio para el enlace del modulo de planilla con el modulo de contabilidad.
    /// EQC - nov2015
    /// </summary>
    public static class PlanillaAsientosBL
    {
        /// <summary>
        /// Consulta que trae las planillas del mes seleccionado en el formulario de Generación de Asientos.
        /// Revisa si la planilla tiene un asiento o no y lo plasma como booleano en el campo _Check
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintMes"></param>
        /// <param name="pstrPeriodo"></param>
        /// <returns></returns>
        public static List<PlanillaAsientoConsulta> ObtenerPlanillasPorMes(ref OperationResult pobjOperationResult,
            int pintMes, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var mes = pintMes.ToString("00");
                    var consulta = dbContext.planillanumeracion.Where(p => p.v_Periodo == pstrPeriodo && p.v_Mes == mes && p.i_Eliminado == 0)
                                    .Select(n => new PlanillaAsientoConsulta
                                    {
                                        IdPlanilla = n.i_Id,
                                        CodigoPlanilla = n.v_Numero,
                                        NombrePlanilla = n.v_Observaciones
                                    }).ToList();

                    consulta.ForEach(p =>
                    {
                        p.NroTrabajadores = dbContext.planillavariablestrabajador.Count(o => o.i_IdPlanillaNumeracion == p.IdPlanilla && o.i_Eliminado == 0);
                        p._Check = dbContext.diario.Any(o => o.i_IdPlanillaNumeracion == p.IdPlanilla && o.i_Eliminado == 0);
                    });
                    pobjOperationResult.Success = 1;

                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaAsientosBL.ObtenerPlanillasPorMes()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Método privado anexo a la generación de Planilla. Sirve para indicar los Conceptos que forman parte de los calculos de las AFP.
        /// Busca en la formula colocada en el datahierarchy para indicar la colección de conceptos.
        /// </summary>
        /// <param name="pintIdAfp"></param>
        /// <returns></returns>
        private static List<string> ConceptosDeAfp(int pintIdAfp)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var conceptos = new List<string>();
                var afp = dbContext.datahierarchy.FirstOrDefault(o => o.i_GroupId == 125 && o.i_ItemId == pintIdAfp);
                if (afp != null)
                {
                    var conceptosAsociadosAfp = !string.IsNullOrWhiteSpace(afp.v_Field) ? afp.v_Field.Split(';').ToList() : null;

                    if (conceptosAsociadosAfp != null)
                    {
                        foreach (var formula in conceptosAsociadosAfp)
                        {
                            var codigo = formula.Split('|')[0];
                            var regimen = formula.Split('|')[1];
                            if (regimen == "AFP")
                            {
                                conceptos.Add(dbContext.planillaconceptos.FirstOrDefault(p => p.v_Codigo.Trim() == codigo.Trim() && p.i_Eliminado == 0).v_IdConceptoPlanilla);
                            }
                        }
                    }
                }

                return conceptos;
            }
        }

        /// <summary>
        /// Método principal que sirve para Generar los Asientos en contabilidad de la planilla seleccionada.
        /// Relaciona todos los conceptos generados en el cálculo de planilla con cuentas contables de acuerdo a las relaciones indicadas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla"></param>
        /// <param name="pdateFecha"></param>
        /// <param name="pdecTipoCambio"></param>
        public static void GenerarAsientoPlanilla(ref OperationResult pobjOperationResult, int pintIdPlanilla, DateTime pdateFecha, decimal pdecTipoCambio)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Eimina el diario si ya existe
                        string[] regDiarioAnterior = new string[3];
                        DiarioBL _objDiarioBL = new DiarioBL();

                        if (dbContext.diario.Any(p => p.i_IdPlanillaNumeracion == pintIdPlanilla && p.i_Eliminado == 0))
                        {
                            regDiarioAnterior = _objDiarioBL.EliminarDiarioXIdPlanilla(ref pobjOperationResult,
                                pintIdPlanilla, Globals.ClientSession.GetAsList(), false);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Valida que los datos necesarios no sean nulos

                        List<ConceptosCuentasCalculo> planillaCalculoCuentasConceptos =
                            new List<ConceptosCuentasCalculo>();
                        //Coleccion principal donde se almacenan todo lo procesado de la planilla con cuentas
                        List<string[]> conceptosNoRegistradosCuentas = new List<string[]>();   
                        var planillaEntity = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == pintIdPlanilla);  
                        if (planillaEntity == null) throw new ArgumentNullException("Planilla no existe!");
                        var periodo = planillaEntity.v_Periodo;
                        var relacionNeto =
                            dbContext.planillarelacionesnetopagar.FirstOrDefault(
                                p => p.i_IdTipoPlanilla == planillaEntity.i_IdTipoPlanilla && p.v_Periodo.Equals(periodo));

                        if (relacionNeto == null)
                        {
                            var tipoPlanilla =
                                dbContext.datahierarchy.FirstOrDefault(
                                    p => p.i_GroupId == 112 && p.i_ItemId == planillaEntity.i_IdTipoPlanilla);

                            if (tipoPlanilla != null)
                            {
                                throw new ArgumentNullException("No existe una cuenta en relaciones para el Neto A Pagar.! - " +
                                    tipoPlanilla.v_Value1);
                            }
                            throw new ArgumentNullException(@"No existe una cuenta en relaciones para el Neto A Pagar.!");
                        }             

                        var dsCalculoPlanilla = dbContext.planillacalculo.Where(p => p.i_IdPlanillaNumeracion == pintIdPlanilla).ToList();
                        if (!dsCalculoPlanilla.Any()) throw new ArgumentNullException("Planilla no fue calculada!");
                        #endregion

                        #region Recopila las relaciones de todos los conceptos de acuerdo al tipo de planilla seleccionada
                        
                        var dsCtaNeto = relacionNeto.v_NroCuenta;

                        var ingresos =
                            dbContext.planillarelacioningresos.Where(
                                p =>
                                    p.v_Periodo == periodo && p.i_IdTipoPlanilla == planillaEntity.i_IdTipoPlanilla &&
                                    p.i_Eliminado == 0)
                                .Select(p => new ConceptosCuentasCalculo
                                {
                                    Cuenta = p.v_NroCuenta,
                                    IdConcepto = p.v_IdConceptoPlanilla,
                                    IdCentroCosto = p.i_IdCentroCosto,
                                    Tipo = "I"
                                }).ToList();

                        var descuentos =
                            dbContext.planillarelaciondescuentos.Where(
                                p =>
                                    p.v_Periodo == periodo && p.i_IdTipoPlanilla == planillaEntity.i_IdTipoPlanilla &&
                                    p.i_Eliminado == 0)
                                .Select(p => new ConceptosCuentasCalculo
                                {
                                    Cuenta = p.v_NroCuenta,
                                    IdConcepto = p.v_IdConceptoPlanilla,
                                    Tipo = "D",
                                    IdRegimen = -1
                                }).ToList();

                        var dsConceptos = ingresos.Concat(descuentos).ToList();

                        var dsRegimenes =
                            dbContext.planillarelacionesdescuentosafp.Where(
                                p => p.v_Periodo == periodo && p.i_Eliminado == 0)
                                .Select(p => new ConceptosCuentasCalculo
                                {
                                    Cuenta = p.v_NroCuenta,
                                    IdConcepto = p.v_IdConceptoPlanilla,
                                    IdRegimen = p.i_IdRegimenPensionario,
                                    Tipo = "D"
                                }).Distinct();

                        var dsAportaciones =
                            dbContext.planillarelacionesaportaciones.Where(
                                p =>
                                    p.v_Periodo == periodo && p.i_IdTipoPlanilla == planillaEntity.i_IdTipoPlanilla &&
                                    p.i_Eliminado == 0)
                                .Select(p => new ConceptosCuentasCalculo
                                {
                                    Cuenta = p.v_NroCuenta_A,
                                    Cuenta2 = p.v_NroCuenta_B,
                                    IdConcepto = p.v_IdConceptoPlanilla,
                                    IdCentroCosto = p.i_IdCentroCosto,
                                    Tipo = "A"
                                }).Distinct();

                        dsConceptos = dsConceptos.Where(p => !dsRegimenes.Any(o => o.IdConcepto == p.IdConcepto)).ToList();
                        #endregion

                        #region Revisa que existan todas los conceptos del cálculo en las relaciones
                        var conceptosCalculo = dsCalculoPlanilla.Select(p =>
                        {
                            var cc = p.trabajador.areaslaboratrabajador.FirstOrDefault(o => o.i_AreaVigente == 1);
                            if (cc == null)
                                throw new ArgumentNullException("El trabajador" + p.trabajador.v_CodInterno +
                                                                "no tiene areas vigentes");
                            return new
                            {
                                IdConcepto = p.v_IdConceptoPlanilla,
                                IdCentroCosto = cc != null ? cc.i_IdCentroCosto ?? 0 : 0,
                                p.Tipo,
                                Importe = p.d_Importe ?? 0,
                                IdRegimen = p.i_IdAfp,
                                EsAfp = ConceptosDeAfp(p.i_IdAfp ?? 0).Contains(p.v_IdConceptoPlanilla),
                                IdTrabajador = p.trabajador.cliente.v_IdCliente
                            };
                        }).ToList();

                        foreach (var p in conceptosCalculo)
                        {
                            switch (p.Tipo)
                            {
                                case "I":
                                    if (!ingresos.Any(z => z.IdConcepto == p.IdConcepto && z.IdCentroCosto == p.IdCentroCosto))
                                        conceptosNoRegistradosCuentas.Add(new[] { p.IdConcepto, p.IdCentroCosto.ToString(), "I" });
                                    else
                                        planillaCalculoCuentasConceptos.Add(new ConceptosCuentasCalculo
                                        {
                                            IdCentroCosto = p.IdCentroCosto,
                                            Tipo = p.Tipo,
                                            IdConcepto = p.IdConcepto,
                                            IdRegimen = p.EsAfp ? p.IdRegimen : -1,
                                            Cuenta = ingresos.FirstOrDefault(z => z.IdConcepto == p.IdConcepto && z.IdCentroCosto == p.IdCentroCosto).Cuenta,
                                            Importe = p.Importe,
                                            NecesitaDetalle = Utils.Windows.CuentaRequiereDetalle(ingresos.FirstOrDefault(z => z.IdConcepto == p.IdConcepto && z.IdCentroCosto == p.IdCentroCosto).Cuenta),
                                            EsAfp = p.EsAfp,
                                            IdTrabajador = p.IdTrabajador
                                        });
                                    break;

                                case "D":
                                    if (p.EsAfp)
                                    {
                                        if (!dsRegimenes.Any(z => z.IdConcepto == p.IdConcepto && z.IdRegimen == p.IdRegimen))
                                            conceptosNoRegistradosCuentas.Add(new[] { p.IdConcepto, p.IdRegimen.ToString(), "D" });
                                        else
                                            planillaCalculoCuentasConceptos.Add(new ConceptosCuentasCalculo
                                            {
                                                Tipo = p.Tipo,
                                                IdConcepto = p.IdConcepto,
                                                IdRegimen = p.EsAfp ? p.IdRegimen : -1,
                                                Cuenta = dsRegimenes.FirstOrDefault(z => z.IdConcepto == p.IdConcepto && z.IdRegimen == p.IdRegimen).Cuenta,
                                                Importe = p.Importe,
                                                NecesitaDetalle = Utils.Windows.CuentaRequiereDetalle(dsRegimenes.FirstOrDefault(z => z.IdConcepto == p.IdConcepto && z.IdRegimen == p.IdRegimen).Cuenta),
                                                EsAfp = p.EsAfp,
                                                IdTrabajador = p.IdTrabajador
                                            });
                                    }
                                    else
                                    {
                                        if (dsConceptos.Where(x => x.Tipo == "D").All(z => z.IdConcepto != p.IdConcepto))
                                            conceptosNoRegistradosCuentas.Add(new[] { p.IdConcepto, null, "D" });
                                        else
                                            planillaCalculoCuentasConceptos.Add(new ConceptosCuentasCalculo
                                            {
                                                Tipo = p.Tipo,
                                                IdConcepto = p.IdConcepto,
                                                IdRegimen = p.EsAfp ? p.IdRegimen : -1,
                                                Cuenta = dsConceptos.Where(x => x.Tipo == "D").FirstOrDefault(z => z.IdConcepto == p.IdConcepto).Cuenta,
                                                Importe = p.Importe,
                                                NecesitaDetalle = Utils.Windows.CuentaRequiereDetalle(dsConceptos.Where(x => x.Tipo == "D").FirstOrDefault(z => z.IdConcepto == p.IdConcepto).Cuenta),
                                                EsAfp = p.EsAfp,
                                                IdTrabajador = p.IdTrabajador
                                            });
                                    }

                                    break;

                                case "A":
                                    if (!dsAportaciones.Any(z => z.IdConcepto == p.IdConcepto && z.IdCentroCosto == p.IdCentroCosto))
                                        conceptosNoRegistradosCuentas.Add(new[] { p.IdConcepto, p.IdCentroCosto.ToString(), "A" });
                                    else
                                        planillaCalculoCuentasConceptos.Add(new ConceptosCuentasCalculo
                                        {
                                            IdCentroCosto = p.IdCentroCosto,
                                            Tipo = p.Tipo,
                                            IdConcepto = p.IdConcepto,
                                            IdRegimen = p.EsAfp ? p.IdRegimen : -1,
                                            Cuenta = dsAportaciones.FirstOrDefault(z => z.IdConcepto == p.IdConcepto && z.IdCentroCosto == p.IdCentroCosto).Cuenta,
                                            Cuenta2 = dsAportaciones.FirstOrDefault(z => z.IdConcepto == p.IdConcepto && z.IdCentroCosto == p.IdCentroCosto).Cuenta2,
                                            Importe = p.Importe,
                                            NecesitaDetalle = Utils.Windows.CuentaRequiereDetalle(dsAportaciones.FirstOrDefault(z => z.IdConcepto == p.IdConcepto).Cuenta),
                                            EsAfp = p.EsAfp,
                                            IdTrabajador = p.IdTrabajador
                                        });
                                    break;
                            }
                        }

                        if (conceptosNoRegistradosCuentas.Any())
                        {
                            var mensaje = new StringBuilder();
                            conceptosNoRegistradosCuentas.ForEach(o =>
                            {
                                var idcc = !string.IsNullOrEmpty(o[1]) ? int.Parse(o[1]) : -1;
                                var idconcepto = o[0];
                                var concepto = dbContext.planillaconceptos.FirstOrDefault(p => p.v_IdConceptoPlanilla == idconcepto);
                                var centrocosto = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 31 && p.i_ItemId == idcc);
                                var nombrecentrocosto = centrocosto != null ? centrocosto.v_Value1 : "Sin Centro Costo";
                                if (concepto != null)
                                {
                                    string nombreConcepto;
                                    if (idcc != -1 && o[2] != "D")
                                        nombreConcepto = "- " + concepto.v_Codigo + "\t" + concepto.v_Nombre + "\t" + "Centro Costo: " + nombrecentrocosto;
                                    else
                                    {
                                        if (idcc == -1)
                                            nombreConcepto = "- " + concepto.v_Codigo + "\t" + concepto.v_Nombre;
                                        else
                                        {
                                            var regimen = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 125 && p.i_ItemId == idcc);
                                            nombreConcepto = "- " + concepto.v_Codigo + "\t" + concepto.v_Nombre + "\t" + "Régimen: " + regimen != null ? regimen.v_Value1 : "No Registrado";
                                        }
                                    }

                                    if (!mensaje.ToString().Contains(nombreConcepto)) mensaje.AppendLine(nombreConcepto);
                                }
                            });

                            mensaje.AppendLine("\nTipo Planilla: " +
                                               dbContext.datahierarchy.FirstOrDefault(
                                                   p =>
                                                       p.i_GroupId == 112 &&
                                                       p.i_ItemId == planillaEntity.i_IdTipoPlanilla.Value).v_Value1
                                                       + " | " + planillaEntity.v_Observaciones);
                            throw new ArgumentNullException("Los Siguientes Conceptos no fueron registrados en las Relaciones \n" + mensaje);
                        }
                        #endregion

                        #region Genera el Asiento
                        var mes = planillaEntity.v_Mes;
                        diarioDto _diarioDto = new diarioDto();
                        List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                        List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                        #region Diario Cabecera
                        var nombreAsiento = "ASIENTO DE PLANILLA MES: " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(mes)).ToUpper();
                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, periodo, mes, 405);

                        if (regDiarioAnterior[2] == null)
                        {
                            var maxMovimiento = _ListadoDiarios.Any() ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count - 1].Value1) : 0;
                            maxMovimiento++;
                            _diarioDto.v_Periodo = periodo;
                            _diarioDto.v_Mes = mes;
                            _diarioDto.v_Correlativo = maxMovimiento.ToString("00000000");
                        }
                        else
                        {
                            _diarioDto.v_Periodo = regDiarioAnterior[0];
                            _diarioDto.v_Mes = mes;
                            _diarioDto.v_Correlativo = regDiarioAnterior[2];
                        }

                        _diarioDto.i_IdPlanillaNumeracion = pintIdPlanilla;
                        _diarioDto.v_Nombre = nombreAsiento;
                        _diarioDto.v_Glosa = nombreAsiento;
                        _diarioDto.d_TipoCambio = pdecTipoCambio;
                        _diarioDto.i_IdMoneda = 1;
                        _diarioDto.i_IdTipoDocumento = 405; // PLA = Diario de Planilla
                        _diarioDto.t_Fecha = pdateFecha;
                        _diarioDto.i_IdTipoComprobante = 2;
                        #endregion

                        #region Detalles del diario

                        #region Reunion de detalles de cuentas sin detalle
                        var cuentasSinDetalle = planillaCalculoCuentasConceptos.Where(p => !p.NecesitaDetalle).ToList();
                        var centroCostos =
                            dbContext.datahierarchy.Where(p => p.i_GroupId == 31 && p.i_IsDeleted == 0).Distinct()
                                .ToDictionary(o => o.i_ItemId, j => j.v_Value2);
                        string outStringCc;

                        var detallesDiarioSinDetalle = cuentasSinDetalle.Where(o => o.Tipo != "A" && !o.EsAfp)
                            .GroupBy(p => new { p.IdCentroCosto, p.Cuenta, p.Tipo })
                            .Select(grupo => new diariodetalleDto
                            {
                                d_Importe = grupo.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupo.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = null,
                                v_Naturaleza = grupo.Key.Tipo == "I" ? "D" : "H",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupo.Key.Cuenta
                            });

                        TempXInsertar = TempXInsertar.Concat(detallesDiarioSinDetalle).ToList();

                        var detallesDiarioAfpSinDetalle = cuentasSinDetalle.Where(o => o.Tipo != "A" && o.EsAfp)
                            .GroupBy(p => new { p.IdCentroCosto, p.Cuenta, p.Tipo, p.IdRegimen })
                            .Select(grupo => new diariodetalleDto
                            {
                                d_Importe = grupo.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupo.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = null,
                                v_Naturaleza = grupo.Key.Tipo == "I" ? "D" : "H",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupo.Key.Cuenta
                            });

                        TempXInsertar = TempXInsertar.Concat(detallesDiarioAfpSinDetalle).ToList();
                        #endregion

                        #region Reunion de detalles de cuentas con detalle
                        var cuentasConDetalle = planillaCalculoCuentasConceptos.Where(p => p.NecesitaDetalle).ToList();

                        var detallesDiarioConDetalle = cuentasConDetalle.Where(o => o.Tipo != "A" && !o.EsAfp)
                            .GroupBy(p => new { p.IdCentroCosto, p.Cuenta, p.Tipo, p.IdTrabajador })
                            .Select(grupo => new diariodetalleDto
                            {
                                d_Importe = grupo.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupo.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = grupo.Key.IdTrabajador,
                                v_Naturaleza = grupo.Key.Tipo == "I" ? "D" : "H",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupo.Key.Cuenta
                            });

                        TempXInsertar = TempXInsertar.Concat(detallesDiarioConDetalle).ToList();

                        var detallesDiarioAfpConDetalle = cuentasConDetalle.Where(o => o.Tipo != "A" && o.EsAfp)
                            .GroupBy(p => new { p.IdCentroCosto, p.Cuenta, p.Tipo, p.IdRegimen, p.IdTrabajador })
                            .Select(grupo => new diariodetalleDto
                            {
                                d_Importe = grupo.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupo.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupo.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = grupo.Key.IdTrabajador,
                                v_Naturaleza = grupo.Key.Tipo == "I" ? "D" : "H",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupo.Key.Cuenta
                            });

                        TempXInsertar = TempXInsertar.Concat(detallesDiarioAfpConDetalle).ToList();
                        #endregion

                        #region Reunion de detalles de las aportaciones
                        var cuentasAportacionSinDetalle = cuentasSinDetalle.Where(o => o.Tipo == "A" && !o.EsAfp)
                                        .GroupBy(p => new { p.IdCentroCosto, p.Cuenta, p.Cuenta2 });

                        foreach (var grupoAportacion in cuentasAportacionSinDetalle)
                        {
                            var detalleDiario_D = new diariodetalleDto
                            {
                                d_Importe = grupoAportacion.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupoAportacion.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = null,
                                v_Naturaleza = "D",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupoAportacion.Key.Cuenta
                            };
                            TempXInsertar.Add(detalleDiario_D);

                            var detalleDiario_H = new diariodetalleDto
                            {
                                d_Importe = grupoAportacion.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupoAportacion.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = null,
                                v_Naturaleza = "H",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupoAportacion.Key.Cuenta2
                            };
                            TempXInsertar.Add(detalleDiario_H);
                        }

                        var cuentasAportacionConDetalle = cuentasConDetalle.Where(o => o.Tipo == "A" && !o.EsAfp)
                                        .GroupBy(p => new { p.IdCentroCosto, p.Cuenta, p.Cuenta2, p.IdTrabajador });

                        foreach (var grupoAportacion in cuentasAportacionConDetalle)
                        {
                            var detalleDiario_D = new diariodetalleDto
                            {
                                d_Importe = grupoAportacion.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupoAportacion.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = grupoAportacion.FirstOrDefault().IdTrabajador,
                                v_Naturaleza = "D",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupoAportacion.FirstOrDefault().Cuenta
                            };
                            TempXInsertar.Add(detalleDiario_D);

                            var detalleDiario_H = new diariodetalleDto
                            {
                                d_Importe = grupoAportacion.Sum(s => s.Importe),
                                d_Cambio =
                                    _diarioDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) / _diarioDto.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            grupoAportacion.Sum(s => s.Importe) * _diarioDto.d_TipoCambio.Value, 2),
                                i_IdCentroCostos = centroCostos.TryGetValue(grupoAportacion.Key.IdCentroCosto ?? -1, out  outStringCc) ? outStringCc : string.Empty,
                                i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value,
                                t_Fecha = _diarioDto.t_Fecha.Value,
                                v_IdCliente = grupoAportacion.Key.IdTrabajador,
                                v_Naturaleza = "H",
                                v_NroDocumento = planillaEntity.v_Numero,
                                v_NroCuenta = grupoAportacion.Key.Cuenta2
                            };
                            TempXInsertar.Add(detalleDiario_H);
                        }
                        #endregion

                        #region Detalle de Neto a Pagar

                        var agrupadoNetoPagar = planillaCalculoCuentasConceptos.GroupBy(group => group.IdTrabajador);

                        foreach (var netoPagar in agrupadoNetoPagar)
                        {
                            var detalleDiario_Neto = new diariodetalleDto();

                            detalleDiario_Neto.d_Importe = netoPagar.Where(p => p.Tipo == "I").Sum(s => s.Importe) -
                                                           netoPagar.Where(p => p.Tipo == "D").Sum(s => s.Importe);
                            detalleDiario_Neto.d_Cambio = _diarioDto.i_IdMoneda.Value == 1
                                ? Utils.Windows.DevuelveValorRedondeado(
                                    detalleDiario_Neto.d_Importe.Value / _diarioDto.d_TipoCambio.Value, 2)
                                : Utils.Windows.DevuelveValorRedondeado(
                                    detalleDiario_Neto.d_Importe.Value * _diarioDto.d_TipoCambio.Value, 2);
                            detalleDiario_Neto.i_IdCentroCostos = "";
                            detalleDiario_Neto.i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento.Value;
                            detalleDiario_Neto.t_Fecha = _diarioDto.t_Fecha.Value;
                            detalleDiario_Neto.v_IdCliente = netoPagar.Key;
                            detalleDiario_Neto.v_Naturaleza = "H";
                            detalleDiario_Neto.v_NroDocumento = planillaEntity.v_Numero;
                            detalleDiario_Neto.v_NroCuenta = dsCtaNeto;

                            TempXInsertar.Add(detalleDiario_Neto);
                        }

                        #endregion

                        #endregion

                        var TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        var TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        var TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        var TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);

                        _diarioDto.d_TotalDebe = decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        _diarioDto.d_TotalHaber = decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        _diarioDto.d_TotalDebeCambio = decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        _diarioDto.d_TotalHaberCambio = decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        _diarioDto.d_DiferenciaDebe = decimal.Parse(Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        _diarioDto.d_DiferenciaHaber = decimal.Parse(Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));

                        _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempXInsertar, (int)TipoMovimientoTesoreria.Ingreso);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaAsientosBL.GenerarAsientoPlanilla()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Método usado en la interfaz de la generación de asientos para consultar el diario generado por la planilla después de que el cálculo ha terminado.
        /// </summary>
        /// <param name="pintIdPlanilla"></param>
        /// <returns></returns>
        public static string RetornarIdDiarioPorPlanilla(int pintIdPlanilla)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                if (dbContext.diario.Any(p => p.i_IdPlanillaNumeracion == pintIdPlanilla && p.i_Eliminado == 0))
                {
                    var diario =
                        dbContext.diario.FirstOrDefault(
                            p => p.i_IdPlanillaNumeracion == pintIdPlanilla && p.i_Eliminado == 0);

                    if (diario != null)
                        return diario.v_IdDiario;

                    return null;
                }
                return null;
            }
        }
    }

    class ConceptosCuentasCalculo
    {
        public int? IdRegimen { get; set; }
        public string IdConcepto { get; set; }
        public string Cuenta { get; set; }
        public string Cuenta2 { get; set; }
        public int? IdCentroCosto { get; set; }
        public string Tipo { get; set; }
        public decimal Importe { get; set; }
        public bool NecesitaDetalle { get; set; }
        public bool EsAfp { get; set; }
        public string IdTrabajador { get; set; }
    }

    public static class PlanillaRelacionesBL
    {
        #region Ingresos
        public static List<planillarelacioningresosDto> ObtenerListadoRelacionesIngreso(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.planillarelacioningresos

                                  join J1 in dbContext.datahierarchy on new { grupo = 112, id = n.i_IdTipoPlanilla.Value }
                                                                     equals new { grupo = J1.i_GroupId, id = J1.i_ItemId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  join J2 in dbContext.datahierarchy on new { grupo = 31, id = n.i_IdCentroCosto.Value }
                                                                     equals new { grupo = J2.i_GroupId, id = J2.i_ItemId } into J2_join
                                  from J2 in J2_join.DefaultIfEmpty()

                                  join J3 in dbContext.asientocontable on new { cta = n.v_NroCuenta , periodo = pstrPeriodo, eliminado = 0} 
                                                                                equals new { cta = J3.v_NroCuenta, periodo = J3.v_Periodo, eliminado = J3.i_Eliminado.Value } into J3_join
                                  from J3 in J3_join.DefaultIfEmpty()

                                  where n.v_Periodo == pstrPeriodo && n.i_Eliminado == 0

                                  select new planillarelacioningresosDto
                                  {
                                      NombreConcepto = n.planillaconceptos.v_Nombre,
                                      v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      i_Eliminado = n.i_Eliminado,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      i_Id = n.i_Id,
                                      v_Periodo = n.v_Periodo,
                                      i_IdTipoPlanilla = n.i_IdTipoPlanilla,
                                      TipoPlanilla = J1.v_Value1,
                                      v_NroCuenta = n.v_NroCuenta,
                                      i_IdCentroCosto = n.i_IdCentroCosto,
                                      CentroCosto = J2 != null ? J2.v_Value1 : string.Empty,
                                      _Cuenta = J3.v_NombreCuenta
                                  }
                                  ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ObtenerListadoRelacionesIngreso()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarRelacionIngreso(ref OperationResult pobjOperationResult,
            planillarelacioningresosDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = pobjEntityDto.ToEntity();
                    entity.i_Eliminado = 0;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_InsertaFecha = DateTime.Now;

                    dbContext.planillarelacioningresos.AddObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.InsertarRelacionIngreso()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarRelacionIngreso(ref OperationResult pobjOperationResult,
            planillarelacioningresosDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacioningresos.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;

                    dbContext.planillarelacioningresos.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ActualizarRelacionIngreso()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarRelacionIngreso(ref OperationResult pobjOperationResult,
            planillarelacioningresosDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacioningresos.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_Eliminado = 1;

                    dbContext.planillarelacioningresos.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.EliminarRelacionIngreso()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Descuentos Afp
        public static List<planillarelacionesdescuentosafpDto> ObtenerListadoRelacionesDescuentosAfp(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.planillarelacionesdescuentosafp

                                  join J1 in dbContext.datahierarchy on new { grupo = 125, id = n.i_IdRegimenPensionario.Value }
                                                                     equals new { grupo = J1.i_GroupId, id = J1.i_ItemId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  join J3 in dbContext.asientocontable on new { cta = n.v_NroCuenta, periodo = pstrPeriodo, eliminado = 0 }
                                                                                equals new { cta = J3.v_NroCuenta, periodo = J3.v_Periodo, eliminado = J3.i_Eliminado.Value } into J3_join
                                  from J3 in J3_join.DefaultIfEmpty()

                                  where n.v_Periodo == pstrPeriodo && n.i_Eliminado == 0

                                  select new planillarelacionesdescuentosafpDto
                                  {
                                      NombreConcepto = n.planillaconceptos.v_Nombre,
                                      v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      i_Eliminado = n.i_Eliminado,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      i_Id = n.i_Id,
                                      v_Periodo = n.v_Periodo,
                                      v_NroCuenta = n.v_NroCuenta,
                                      i_IdRegimenPensionario = n.i_IdRegimenPensionario,
                                      RegimenLaboral = J1.v_Value1,
                                      _Cuenta = J3.v_NombreCuenta
                                  }
                                  ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ObtenerListadoRelacionesIngreso()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarRelacionDescuentosAfp(ref OperationResult pobjOperationResult,
            planillarelacionesdescuentosafpDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = pobjEntityDto.ToEntity();
                    entity.i_Eliminado = 0;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_InsertaFecha = DateTime.Now;

                    dbContext.planillarelacionesdescuentosafp.AddObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.InsertarRelacionDescuentosAfp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarRelacionDescuentosAfp(ref OperationResult pobjOperationResult,
            planillarelacionesdescuentosafpDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacionesdescuentosafp.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;

                    dbContext.planillarelacionesdescuentosafp.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ActualizarRelacionDescuentosAfp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarRelacionDescuentosAfp(ref OperationResult pobjOperationResult,
            planillarelacionesdescuentosafpDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacionesdescuentosafp.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_Eliminado = 1;

                    dbContext.planillarelacionesdescuentosafp.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.EliminarRelacionDescuentosAfp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Otros Descuentos
        public static List<planillarelaciondescuentosDto> ObtenerListadoRelacionesDescuentos(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.planillarelaciondescuentos

                                  join J1 in dbContext.datahierarchy on new { grupo = 112, id = n.i_IdTipoPlanilla.Value }
                                                                     equals new { grupo = J1.i_GroupId, id = J1.i_ItemId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  join J3 in dbContext.asientocontable on new { cta = n.v_NroCuenta, periodo = pstrPeriodo, eliminado = 0 }
                                                                                equals new { cta = J3.v_NroCuenta, periodo = J3.v_Periodo, eliminado = J3.i_Eliminado.Value } into J3_join
                                  from J3 in J3_join.DefaultIfEmpty()

                                  where n.v_Periodo == pstrPeriodo && n.i_Eliminado == 0

                                  select new planillarelaciondescuentosDto
                                  {
                                      NombreConcepto = n.planillaconceptos.v_Nombre,
                                      v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      i_Eliminado = n.i_Eliminado,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      i_Id = n.i_Id,
                                      v_Periodo = n.v_Periodo,
                                      i_IdTipoPlanilla = n.i_IdTipoPlanilla,
                                      v_NroCuenta = n.v_NroCuenta,
                                      TipoPlanilla = J1.v_Value1,
                                      _Cuenta = J3.v_NombreCuenta
                                  }
                                  ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ObtenerListadoRelacionesDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarRelacionDescuentos(ref OperationResult pobjOperationResult,
            planillarelaciondescuentosDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = pobjEntityDto.ToEntity();
                    entity.i_Eliminado = 0;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_InsertaFecha = DateTime.Now;

                    dbContext.planillarelaciondescuentos.AddObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.InsertarRelacionDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarRelacionDescuentos(ref OperationResult pobjOperationResult,
            planillarelaciondescuentosDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelaciondescuentos.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;

                    dbContext.planillarelaciondescuentos.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ActualizarRelacionDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarRelacionDescuentos(ref OperationResult pobjOperationResult,
            planillarelaciondescuentosDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelaciondescuentos.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_Eliminado = 1;

                    dbContext.planillarelaciondescuentos.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.EliminarRelacionDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Neto Pagar
        public static List<planillarelacionesnetopagarDto> ObtenerListadoRelacionesNetoPagar(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.planillarelacionesnetopagar

                                  join J1 in dbContext.datahierarchy on new { grupo = 112, id = n.i_IdTipoPlanilla.Value }
                                                                     equals new { grupo = J1.i_GroupId, id = J1.i_ItemId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  join J3 in dbContext.asientocontable on new { cta = n.v_NroCuenta, periodo = pstrPeriodo, eliminado = 0 }
                                                                                equals new { cta = J3.v_NroCuenta, periodo = J3.v_Periodo, eliminado = J3.i_Eliminado.Value } into J3_join
                                  from J3 in J3_join.DefaultIfEmpty()

                                  where n.v_Periodo == pstrPeriodo && n.i_Eliminado == 0

                                  select new planillarelacionesnetopagarDto
                                  {
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      i_Eliminado = n.i_Eliminado,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      i_Id = n.i_Id,
                                      v_Periodo = n.v_Periodo,
                                      i_IdTipoPlanilla = n.i_IdTipoPlanilla,
                                      TipoPlanilla = J1.v_Value1,
                                      v_NroCuenta = n.v_NroCuenta,
                                      _Cuenta = J3.v_NombreCuenta
                                  }
                                  ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ObtenerListadoRelacionesOtrosDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarRelacionNetoPagar(ref OperationResult pobjOperationResult,
            planillarelacionesnetopagarDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = pobjEntityDto.ToEntity();
                    entity.i_Eliminado = 0;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_InsertaFecha = DateTime.Now;

                    dbContext.planillarelacionesnetopagar.AddObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.InsertarRelacionNetoPagar()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarRelacionNetoPagar(ref OperationResult pobjOperationResult,
            planillarelacionesnetopagarDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacionesnetopagar.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;

                    dbContext.planillarelacionesnetopagar.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ActualizarRelacionNetoPagar()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarRelacionNetoPagar(ref OperationResult pobjOperationResult,
            planillarelacionesnetopagarDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacionesnetopagar.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_Eliminado = 1;

                    dbContext.planillarelacionesnetopagar.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.EliminarRelacionNetoPagar()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Aportaciones
        public static List<planillarelacionesaportacionesDto> ObtenerListadoRelacionesAportaciones(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.planillarelacionesaportaciones

                                  join J1 in dbContext.datahierarchy on new { grupo = 112, id = n.i_IdTipoPlanilla.Value }
                                                                     equals new { grupo = J1.i_GroupId, id = J1.i_ItemId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  join J2 in dbContext.datahierarchy on new { grupo = 31, id = n.i_IdCentroCosto.Value }
                                                                     equals new { grupo = J2.i_GroupId, id = J2.i_ItemId } into J2_join
                                  from J2 in J2_join.DefaultIfEmpty()

                                  join J3 in dbContext.asientocontable on new { cta = n.v_NroCuenta_A, periodo = pstrPeriodo, eliminado = 0 }
                                                                                 equals new { cta = J3.v_NroCuenta, periodo = J3.v_Periodo, eliminado = J3.i_Eliminado.Value } into J3_join
                                  from J3 in J3_join.DefaultIfEmpty()

                                  join J4 in dbContext.asientocontable on new { cta = n.v_NroCuenta_B, periodo = pstrPeriodo, eliminado = 0 }
                                                                                equals new { cta = J4.v_NroCuenta, periodo = J4.v_Periodo, eliminado = J4.i_Eliminado.Value } into J4_join
                                  from J4 in J4_join.DefaultIfEmpty()

                                  where n.v_Periodo == pstrPeriodo && n.i_Eliminado == 0

                                  select new planillarelacionesaportacionesDto
                                  {
                                      NombreConcepto = n.planillaconceptos.v_Nombre,
                                      v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      i_Eliminado = n.i_Eliminado,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      i_Id = n.i_Id,
                                      v_Periodo = n.v_Periodo,
                                      i_IdTipoPlanilla = n.i_IdTipoPlanilla,
                                      TipoPlanilla = J1.v_Value1,
                                      v_NroCuenta_A = n.v_NroCuenta_A,
                                      v_NroCuenta_B = n.v_NroCuenta_B,
                                      i_IdCentroCosto = n.i_IdCentroCosto,
                                      CentroCosto = J2.v_Value1,
                                      _CuentaA = J3.v_NombreCuenta,
                                      _CuentaB = J4.v_NombreCuenta
                                  }
                                  ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ObtenerListadoRelacionesIngreso()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarRelacionAportaciones(ref OperationResult pobjOperationResult,
            planillarelacionesaportacionesDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = pobjEntityDto.ToEntity();
                    entity.i_Eliminado = 0;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_InsertaFecha = DateTime.Now;

                    dbContext.planillarelacionesaportaciones.AddObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.InsertarRelacionAportaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarRelacionAportaciones(ref OperationResult pobjOperationResult,
            planillarelacionesaportacionesDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacionesaportaciones.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;

                    dbContext.planillarelacionesaportaciones.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.ActualizarRelacionAportaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarRelacionAportaciones(ref OperationResult pobjOperationResult,
            planillarelacionesaportacionesDto pobjEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource =
                        dbContext.planillarelacionesaportaciones.FirstOrDefault(p => p.i_Id == pobjEntityDto.i_Id);

                    var entity = pobjEntityDto.ToEntity();
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_Eliminado = 1;

                    dbContext.planillarelacionesaportaciones.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaRelacionesBL.EliminarRelacionAportaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

    }
}
