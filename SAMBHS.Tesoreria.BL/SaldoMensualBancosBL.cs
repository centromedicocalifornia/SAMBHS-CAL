using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using System.Linq.Dynamic;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Tesoreria.BL
{
    public class SaldoMensualBancosBL
    {

        public List<saldomensualbancosDto> ObtenerListadoSaldoMensualBancos(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.saldomensualbancos
                            where A.i_Eliminado == 0
                            select new saldomensualbancosDto
                            {
                                v_NroCuenta = A.v_NroCuenta,
                                v_Mes = A.v_Mes,
                                v_IdSaldoMensualB = A.v_IdSaldoMensualB,
                                v_Anio = A.v_Anio,
                                d_SaldoDolares = A.d_SaldoDolares,
                                d_SaldoSoles = A.d_SaldoSoles,
                                i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                t_ActualizaFecha = A.t_ActualizaFecha,
                                t_InsertaFecha = A.t_InsertaFecha

                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<saldomensualbancosDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }

        }
        public string ObtenerNombreCuenta(string pstrNumeroCuenta)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            try
            {

                var AsientoContable = (from n in dbContext.asientocontable

                                       where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.i_Imputable == 1
                                       select new { n.v_NombreCuenta }).FirstOrDefault();

                if (AsientoContable != null)
                {
                    return AsientoContable.v_NombreCuenta;

                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception e)
            {
                throw e;

            }
        }

        public string InsertarSaldoMensualBanco(ref OperationResult pobjOperationResult, saldomensualbancosDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    saldomensualbancos objEntity = saldomensualbancosAssembler.ToEntity(pobjDtoEntity);

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 63);
                    var newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "YA");
                    objEntity.v_IdSaldoMensualB = newId;
                    dbContext.AddTosaldomensualbancos(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    return newId; 
                }
            }
            catch (Exception ex)
            {
                

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoMensualBancosBL.InsertarSaldoMensualBanco()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null  ;
            }
        }

        public string ActualizarSaldoMensualBanco(ref OperationResult pobjOperationResult, saldomensualbancosDto pobjDtoEntity, List<string> ClientSession)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.saldomensualbancos
                                       where a.v_IdSaldoMensualB == pobjDtoEntity.v_IdSaldoMensualB
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                saldomensualbancos objEntity = saldomensualbancosAssembler.ToEntity(pobjDtoEntity);
                dbContext.saldomensualbancos.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return pobjDtoEntity.v_IdSaldoMensualB;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return string.Empty;
            }
        }

        public saldomensualbancosDto ObtenerSaldoMensualBancos(ref OperationResult pobjOperationResult, string pstrIdSaldoMensualBanco)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                saldomensualbancosDto objDtoEntity = null;
                var objEntity = (from A in dbContext.saldomensualbancos
                                 where A.v_IdSaldoMensualB == pstrIdSaldoMensualBanco
                                 select A
                                 ).FirstOrDefault();
                if (objEntity != null)
                    objDtoEntity = saldomensualbancosAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;
                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public void EliminarSaldoMensualBanco(ref OperationResult pobjOperationResult, string pstrIdSaldoMensualBanco, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.saldomensualbancos
                                       where a.v_IdSaldoMensualB == pstrIdSaldoMensualBanco
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;


                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public void GenerarSaldoMesualBancos(ref OperationResult pobOperationResult, int mes, string anioActual)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            DateTime fechaInicio = DateTime.Parse("01/" + mes + "/" + anioActual);
            DateTime fechaFin = DateTime.Parse(DateTime.DaysInMonth(int.Parse(anioActual), mes).ToString() + "/" + mes + "/" + anioActual + " 23:59");
            saldomensualbancosDto objSaldoMensualBancoDto = new saldomensualbancosDto();
            string anio = string.Empty, mesAnterior = string.Empty;
            OperationResult objOperationResult = new OperationResult();


            if (mes != (int)Mes.Enero)
            {
                anio = anioActual;
                mesAnterior = (mes - 1).ToString("00");

            }
            else
            {
                anio = (int.Parse(anioActual) - 1).ToString();
                mesAnterior = ((int)Mes.Diciembre).ToString("00");
            }


            var SaldoMensualBancoMesAnterior = (from n in dbContext.saldomensualbancos

                                                where n.v_Anio == anio && n.v_Mes == mesAnterior && n.v_NroCuenta.StartsWith("10") && n.i_Eliminado == 0

                                                select n).ToList();


            var HaberesMes = (from n in dbContext.tesoreria

                              join a in dbContext.tesoreriadetalle on new { t = n.v_IdTesoreria, eliminado = 0 } equals new { t = a.v_IdTesoreria, eliminado = a.i_Eliminado.Value } into a_join

                              from a in a_join.DefaultIfEmpty()

                              join b in dbContext.documento on new { doc = n.i_IdTipoDocumento.Value } equals new { doc = b.i_CodigoDocumento } into b_join

                              from b in b_join.DefaultIfEmpty()

                              where n.i_Eliminado == 0

                              && n.t_FechaRegistro >= fechaInicio && n.t_FechaRegistro <= fechaFin

                              && n.i_IdEstado == 1 && a.v_Naturaleza == "H" && a.v_NroCuenta.StartsWith("10")

                              select new CierreMensualBancos
                             {
                                 v_NroCuenta = a.v_NroCuenta,
                                 d_Importe = n.i_IdMoneda == 1 ? a.d_Importe.Value : a.d_Cambio.Value,
                                 d_Cambio = n.i_IdMoneda == 1 ? a.d_Cambio.Value : a.d_Importe.Value,

                             }).ToList();

            var DebeMes = (from n in dbContext.tesoreria

                           join a in dbContext.tesoreriadetalle on new { t = n.v_IdTesoreria, eliminado = 0 } equals new { t = a.v_IdTesoreria, eliminado = a.i_Eliminado.Value } into a_join

                           from a in a_join.DefaultIfEmpty()

                           join b in dbContext.documento on new { doc = n.i_IdTipoDocumento.Value } equals new { doc = b.i_CodigoDocumento } into b_join

                           from b in b_join.DefaultIfEmpty()

                           where n.i_Eliminado == 0

                        
                           && n.t_FechaRegistro >= fechaInicio && n.t_FechaRegistro <= fechaFin

                           && n.i_IdEstado == 1 && a.v_Naturaleza == "D" && a.v_NroCuenta.StartsWith("10")

                           select new CierreMensualBancos
                           {

                               v_NroCuenta = a.v_NroCuenta,
                               d_Importe = n.i_IdMoneda == 1 ? a.d_Importe.Value : a.d_Cambio.Value,
                               d_Cambio = n.i_IdMoneda == 1 ? a.d_Cambio.Value : a.d_Importe.Value,

                           }).ToList();


            if (SaldoMensualBancoMesAnterior.Count() != 0)
            {

                List<string> Cuentas = new List<string>();
                List<string> CuentasMesAnterior = new List<string>();

                var ListaCuenta = HaberesMes.Union(DebeMes).ToList();
                Cuentas = ListaCuenta.Select(o => o.v_NroCuenta).Distinct().ToList();
                CuentasMesAnterior = SaldoMensualBancoMesAnterior.Select(o => o.v_NroCuenta).Distinct().ToList();

                var CuentasFinal = Cuentas.Union(CuentasMesAnterior).ToList();
                foreach (var Cuenta in CuentasFinal)
                {

                    if (Cuenta == "1041104")
                    {

                        string x = "";
                    }
                    var Debe = DebeMes.Where(o => o.v_NroCuenta == Cuenta).ToList();
                    var Haber = HaberesMes.Where(x => x.v_NroCuenta == Cuenta).ToList();

                    decimal TotalDebeSoles = Debe.Sum(x => x.d_Importe);
                    decimal TotalDebeDolares = Debe.Sum(y => y.d_Cambio);
                    decimal TotalHaberSoles = Haber.Sum(x => x.d_Importe);
                    decimal TotalHaberDolares = Haber.Sum(y => y.d_Cambio);

                    objSaldoMensualBancoDto.v_Anio = anioActual;
                    objSaldoMensualBancoDto.v_Mes = mes.ToString("00");
                    objSaldoMensualBancoDto.v_NroCuenta = Cuenta;
                    objSaldoMensualBancoDto.d_SaldoSoles = SaldoMensualBancoMesAnterior.Where(x => x.v_NroCuenta == Cuenta).Select(y => y.d_SaldoSoles.Value).FirstOrDefault() == null ? 0 + TotalDebeSoles - TotalHaberSoles : SaldoMensualBancoMesAnterior.Where(x => x.v_NroCuenta == Cuenta).Select(y => y.d_SaldoSoles.Value).FirstOrDefault() + TotalDebeSoles - TotalHaberSoles;
                    objSaldoMensualBancoDto.d_SaldoDolares = SaldoMensualBancoMesAnterior.Where(x => x.v_NroCuenta == Cuenta).Select(y => y.d_SaldoDolares).FirstOrDefault() == null ? 0 + TotalDebeDolares - TotalHaberDolares : SaldoMensualBancoMesAnterior.Where(x => x.v_NroCuenta == Cuenta).Select(y => y.d_SaldoDolares).FirstOrDefault() + TotalDebeDolares - TotalHaberDolares;
                    objSaldoMensualBancoDto.d_SaldoSoles = Utils.Windows.DevuelveValorRedondeado(objSaldoMensualBancoDto.d_SaldoSoles.Value, 2);
                    objSaldoMensualBancoDto.d_SaldoDolares = Utils.Windows.DevuelveValorRedondeado(objSaldoMensualBancoDto.d_SaldoDolares.Value, 2);

                    InsertarSaldoMensualBanco(ref objOperationResult, objSaldoMensualBancoDto, Globals.ClientSession.GetAsList());
                }
            }
            else
            {
                List<string> Cuentas = new List<string>();
                var ListaCuenta = HaberesMes.Union(DebeMes).ToList();
                Cuentas = ListaCuenta.Select(o => o.v_NroCuenta).Distinct().ToList();
                foreach (string Cuenta in Cuentas)
                {
                    objSaldoMensualBancoDto = new saldomensualbancosDto();
                    decimal TotalHaberSoles = 0, TotalHaberDolares = 0, TotalDebeSoles = 0, TotalDebeDolares = 0;

                    foreach (var Haber in HaberesMes.Where(p => p.v_NroCuenta == Cuenta))
                    {

                        TotalHaberSoles = Haber.d_Importe + TotalHaberSoles;
                        TotalHaberDolares = Haber.d_Cambio + TotalHaberDolares;
                    }

                    foreach (var Debe in DebeMes.Where(q => q.v_NroCuenta == Cuenta))
                    {
                        TotalDebeSoles = Debe.d_Importe + TotalDebeSoles;
                        TotalDebeDolares = Debe.d_Cambio + TotalDebeDolares;
                    }

                    objSaldoMensualBancoDto.v_Anio = anioActual;
                    objSaldoMensualBancoDto.v_Mes = mes.ToString("00");
                    objSaldoMensualBancoDto.v_NroCuenta = Cuenta;
                    objSaldoMensualBancoDto.d_SaldoSoles = TotalDebeSoles - TotalHaberSoles;
                    objSaldoMensualBancoDto.d_SaldoDolares = TotalDebeDolares - TotalHaberDolares;
                    InsertarSaldoMensualBanco(ref objOperationResult, objSaldoMensualBancoDto, Globals.ClientSession.GetAsList());
                }

            }

            pobOperationResult.Success = 1;


        }





       


        public bool SaldoMensualMesAnterior(string pstranio, int pstrMes)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            string mes = String.Empty, anio = string.Empty;

            if (pstrMes == (int)Mes.Enero)
            {
                mes = "12";
                anio = (int.Parse(pstranio) - 1).ToString();
            }
            else
            {
                mes = (pstrMes - 1).ToString("00");
                anio = pstranio;
            }

            var SaldoMensualBancoMesAnterior = (from n in dbContext.saldomensualbancos

                                                where n.v_Anio == anio && n.v_Mes == mes && n.v_NroCuenta.StartsWith("10") && n.i_Eliminado == 0

                                                select n).ToList();

            if (!SaldoMensualBancoMesAnterior.Any())
            {

                return false;

            }
            else
            {
                return true;
            }

        }

        public bool ExistenciaSaldoMensualBanco(string pstrMes, string pstrAnio)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var ExistenciaSaldoMensualBanco = (from n in dbContext.saldomensualbancos

                                               where n.i_Eliminado == 0 && n.v_Anio == pstrAnio && n.v_Mes == pstrMes 
                                               select n).ToList();
            if (ExistenciaSaldoMensualBanco.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            };
        }

        public void EliminarSaldoMensualBancoporAnioMes(string pstrAnio, string pstrMes)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var saldomensualbanco = (from n in dbContext.saldomensualbancos

                                     where n.i_Eliminado == 0 && n.v_Anio == pstrAnio && n.v_Mes == pstrMes

                                     select n).ToList();

            if (saldomensualbanco.Count() != 0)
            {
                foreach (var Fila in saldomensualbanco)
                {
                    dbContext.saldomensualbancos.DeleteObject(Fila);

                }
                dbContext.SaveChanges();
            }
        }

        public decimal[] ObtenerValoresSaldoMensualBanco(string pstrNumeroCuenta, string pstrAnio, string pstrMes)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                decimal[] Lista = new decimal[3];
                var query = (from n in dbContext.saldomensualbancos

                             where n.v_NroCuenta == pstrNumeroCuenta && n.v_Anio == pstrAnio && n.v_Mes == pstrMes && n.i_Eliminado == 0

                             select new { n.d_SaldoDolares, n.d_SaldoSoles, n.v_IdSaldoMensualB }).FirstOrDefault();

                if (query != null)
                {

                    Lista[0] = query.d_SaldoSoles.Value;
                    Lista[1] = query.d_SaldoDolares.Value;

                }
                else
                {
                    Lista[0] = 0;
                    Lista[1] = 0;

                }
                return Lista;
            }
            catch (Exception e)
            {

                throw e;
            }


        }

        public string ObtenerIdSaldoMensualBanco(string pstrNumeroCuenta, string pstrMes, string pstrAnio)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var IdSaldoMensualBanco = (from n in dbContext.saldomensualbancos

                                       where n.i_Eliminado == 0 && n.v_Anio == pstrAnio && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumeroCuenta

                                       select new { n.v_IdSaldoMensualB }).FirstOrDefault();
            if (IdSaldoMensualBanco != null)
            {
                return IdSaldoMensualBanco.v_IdSaldoMensualB;
            }
            else
            {
                return null;
            }

        }


    }
}
