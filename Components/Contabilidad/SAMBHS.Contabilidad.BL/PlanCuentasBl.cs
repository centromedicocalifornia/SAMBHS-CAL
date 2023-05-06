using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Devart.Data.PostgreSql;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Contabilidad.BL
{
    public class PlanCuentasBl
    {
        public TipoMotorBD TipoMotor { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Database { get; set; }

        private static string Periodo
        {
            get { return Globals.ClientSession.i_Periodo.ToString(); }
        }

        private string ConnectionString
        {
            get
            {
                switch (TipoMotor)
                {
                    case TipoMotorBD.MSSQLServer:
                        return "Data Source=" + Host + ";Initial Catalog=" + Database + ";Integrated Security=False;Persist Security Info=True;User ID=" + Username + ";Password=" + Crypto.DecryptStringAES(Password, "TiSolUciOnEs");

                    case TipoMotorBD.PostgreSQL:
                        return "User Id=" + Username + "; password=" + Crypto.DecryptStringAES(Password, "TiSolUciOnEs") + ";Host=" + Host + ";Database=" + Database + ";Initial Schema=public";
                }
                return string.Empty;
            }
        }

        private List<asientocontableDto> ObtenerCuentasModelo
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(ConnectionString)) return null;

                    switch (TipoMotor)
                    {
                        case TipoMotorBD.MSSQLServer:
                            using (var db = new SqlConnection(ConnectionString))
                            {
                                if (db.State != ConnectionState.Open) db.Open();
                                return db
                                    .Query<asientocontableDto>("select * from asientocontable where v_Periodo = '" + Periodo + "' and i_Eliminado = 0")
                                    .ToList();
                            }

                        case TipoMotorBD.PostgreSQL:
                            using (var db = new PgSqlConnection(ConnectionString))
                            {
                                if (db.State != ConnectionState.Open) db.Open();
                                return db
                                    .Query<asientocontableDto>("select * from asientocontable where \"v_Periodo\" = '" + Periodo + "' and \"i_Eliminado\" = 0")
                                    .ToList();
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static bool SonIguales(asientocontableDto local, asientocontableDto modelo)
        {
            var ctas = local.v_NroCuenta == modelo.v_NroCuenta;
            var nombre = local.v_NombreCuenta == modelo.v_NombreCuenta;
            var mon = local.i_IdMoneda == modelo.i_IdMoneda;
            var nat = local.i_Naturaleza == modelo.i_Naturaleza;
            var detalle = local.i_Detalle == modelo.i_Detalle;
            var tipoE = local.v_TipoExistencia == modelo.v_TipoExistencia;

            return ctas && nombre && mon && nat && detalle && tipoE;
        }

        public void CopiaParcial(out int modificados, out int insertados)
        {
            modificados = 0;
            insertados = 0;
            try
            {
                var ctasModelos = ObtenerCuentasModelo;
                if (ctasModelos == null) throw new Exception("No se pudo obtener las cuentas de la empresa modelo");
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var ctasLocal = dbContext.asientocontable.Where(p => p.i_Eliminado == 0 && p.v_Periodo == Periodo).ToList();

                    foreach (var ctaModelo in ctasModelos)
                    {
                        var ctaModificar = ctasLocal.FirstOrDefault(p => p.v_NroCuenta.Equals(ctaModelo.v_NroCuenta));
                        if (ctaModificar != null)
                        {
                            if (SonIguales(ctaModelo, ctaModificar.ToDTO())) continue;

                            var id = ctaModificar.i_IdCuenta;
                            ctaModificar = ctaModelo.ToEntity();
                            ctaModificar.i_IdCuenta = id;
                            ctaModificar.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                            ctaModificar.t_ActualizaFecha = DateTime.Now;
                            dbContext.asientocontable.ApplyCurrentValues(ctaModificar);
                            modificados++;
                        }
                        else
                        {
                            ctaModelo.i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId;
                            ctaModelo.t_InsertaFecha = DateTime.Now;
                            dbContext.asientocontable.AddObject(ctaModelo.ToEntity());
                            insertados++;
                        }
                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public void CopiaCompleta(out int modificados, out int insertados, out int eliminados)
        {
            try
            {
                modificados = 0;
                insertados = 0;
                eliminados = 0;
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    CopiaParcial(out modificados, out insertados);
                    var ctasModeloDto = ObtenerCuentasModelo;
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var ctasModList = ctasModeloDto.Select(p => p.v_NroCuenta).Distinct().ToList();
                        var ctasLocalesPorEliminarDtos = dbContext.asientocontable.Where(p => !ctasModList.Contains(p.v_NroCuenta) && p.v_Periodo == Periodo && p.i_Eliminado == 0).ToList();
                        var ctasEliminarList = ctasLocalesPorEliminarDtos.Select(p => p.v_NroCuenta).Distinct().ToList();
                        
                        var hayUsadasDiario = (from n in dbContext.diariodetalle
                            join d in dbContext.diario on n.v_IdDiario equals d.v_IdDiario into dJoin
                            from d in dJoin.DefaultIfEmpty()
                            where n.i_Eliminado == 0 && d.v_Periodo == Periodo
                                  && ctasEliminarList.Contains(n.v_NroCuenta)
                            select n.v_NroCuenta);

                        var hayUsadasTesoreria = (from n in dbContext.tesoreriadetalle
                            join d in dbContext.tesoreria on n.v_IdTesoreria equals d.v_IdTesoreria into dJoin
                            from d in dJoin.DefaultIfEmpty()
                            where n.i_Eliminado == 0 && d.v_Periodo == Periodo
                                  && ctasEliminarList.Contains(n.v_NroCuenta)
                            select n.v_NroCuenta);

                        if (hayUsadasDiario.Any() || hayUsadasTesoreria.Any()) 
                            throw new Exception("Hay cuentas en la empresa local que no existen en la empresa modelo que se están usando, " +
                                                "no se puede terminar el proceso!\nCUENTAS USADAS EN LOCAL:\n" + 
                                                string.Join(", ", hayUsadasDiario.Concat(hayUsadasTesoreria).Distinct().OrderBy(o => o)));

                        foreach (var ctaEliminarDto in ctasLocalesPorEliminarDtos)
                        {
                            ctaEliminarDto.i_Eliminado = 1;
                            ctaEliminarDto.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                            ctaEliminarDto.t_ActualizaFecha = DateTime.Now;
                            dbContext.asientocontable.ApplyCurrentValues(ctaEliminarDto);
                            eliminados++;
                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}
