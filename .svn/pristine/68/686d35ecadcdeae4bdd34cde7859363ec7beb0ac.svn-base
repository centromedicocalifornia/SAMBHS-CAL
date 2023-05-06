using SAMBHS.Common.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Planilla.BL
{
    public static class AsistenciaBl
    {
        private static TimeSpan ObtenerHorasMensuales(IEnumerable<planillaturnosdetalle> turnoDetalle)
        {
            try
            {
                return turnoDetalle.Aggregate(TimeSpan.Zero, (current, planillaturnosdetalleDto) => current + ObtenerHorasDiarias(planillaturnosdetalleDto));
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        private static TimeSpan ObtenerHorasDiarias(planillaturnosdetalle turnoDetalle)
        {
            try
            {
                var primerTurno = turnoDetalle.t_SalidaI - turnoDetalle.t_IngresoI;
                var segundoTurno = turnoDetalle.t_SalidaII - turnoDetalle.t_IngresoII;
                var r = (primerTurno ?? TimeSpan.Zero) + (segundoTurno ?? TimeSpan.Zero);
                return r;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        public static List<planillaasistenciaDto> ObtenerAsistencia(ref OperationResult pobjOperationResult, int idSemana,
            DateTime fecha)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.trabajador
                                    join c in dbContext.cliente on new { id = n.v_IdCliente, e = 0 }   
                                                            equals new { id =  c.v_IdCliente, e = c.i_Eliminado.Value}  into cJoin
                                    from c in cJoin.DefaultIfEmpty()
                                    join l in dbContext.areaslaboratrabajador on new { id = n.v_IdTrabajador, e = 0, v = 1 }  
                                                        equals new { id =  l.v_IdTrabajador, e = l.i_Eliminado.Value, v = l.i_AreaVigente.Value}  into lJoin
                                    from l in lJoin.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 
                                    select new planillaasistenciaDto
                                    {
                                        v_IdTrabajador = n.v_IdTrabajador,
                                        Trabajador = c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_SegundoNombre,
                                        NroDocIdentidad = c.v_NroDocIdentificacion,
                                        i_IdEstado = 1,
                                        IdTurno = l != null ? l.i_IdTurno ?? -1 : -1,
                                        FechaInicioAreaLaboral = l != null ? l.v_FechaInicio ?? DateTime.Today :  DateTime.Today
                                    })
                        .OrderBy(o => o.Trabajador)
                        .ToList();

                    //elimina los trabajadores sin turno y con fecha de inicio de area laboral superior a la fecha de asistencia
                    consulta.RemoveAll(p => p.IdTurno == -1 || p.FechaInicioAreaLaboral > fecha);

                    var turnos = dbContext.planillaturnosdetalle.ToList();

                    foreach (var trabajador in consulta)
                    {
                        var entidadOriginal = dbContext.planillaasistencia
                            .FirstOrDefault(p => p.v_IdTrabajador
                                .Equals(trabajador.v_IdTrabajador) && p.t_Fecha == fecha);

                        if (entidadOriginal == null)
                        {
                            trabajador.i_IdSemana = idSemana;
                            trabajador.t_Fecha = fecha;
                            trabajador.t_Ingreso_I = DateTime.MinValue;
                            trabajador.t_Salida_I = DateTime.MinValue;
                            trabajador.t_Ingreso_II = DateTime.MinValue;
                            trabajador.t_Salida_II = DateTime.MinValue;
                            trabajador.t_Ingreso_I_Turno = DateTime.MinValue;
                            trabajador.t_Ingreso_II_Turno = DateTime.MinValue;
                            trabajador.t_Salida_I_Turno = DateTime.MinValue;
                            trabajador.t_Salida_II_Turno = DateTime.MinValue;

                            if (trabajador.IdTurno > 0 )
                            {
                                var diaSemana = (int) fecha.DayOfWeek;
                                var turno = turnos.Where(p => p.i_IdTurno == trabajador.IdTurno).ToList();
                                if (turno.Any())
                                {
                                    trabajador.HorasMensuales = ObtenerHorasMensuales(turno);
                                    var diaLaboral = turno.FirstOrDefault(p => p.i_IdDia == diaSemana);
                                    if (diaLaboral != null && diaLaboral.i_Asiste == 1)
                                    {
                                        trabajador.HorasCorrespondidas = ObtenerHorasDiarias(diaLaboral);
                                        trabajador.t_Ingreso_I_Turno = trabajador.t_Ingreso_I = diaLaboral.t_IngresoI;
                                        trabajador.t_Salida_I_Turno = trabajador.t_Salida_I = diaLaboral.t_SalidaI;
                                        trabajador.t_Ingreso_II_Turno = trabajador.t_Ingreso_II = diaLaboral.t_IngresoII;
                                        trabajador.t_Salida_II_Turno = trabajador.t_Salida_II = diaLaboral.t_SalidaII;
                                    }
                                    else
                                    {
                                        //verificar que dia tiene vacaciones
                                        trabajador.i_IdEstado = 6;
                                    }
                                }
                            }
                        }
                        else
                        {
                            trabajador.HorasExtrasAutorizadas = (entidadOriginal.i_HorasExtrasAutorizadas ?? 0) == 1;
                            trabajador.i_IdAsistencia = entidadOriginal.i_IdAsistencia;
                            trabajador.i_IdSemana = idSemana;
                            trabajador.t_Fecha = fecha;
                            trabajador.i_IdEstado = entidadOriginal.i_IdEstado;
                            trabajador.t_Ingreso_I = entidadOriginal.t_Ingreso_I;
                            trabajador.t_Salida_I = entidadOriginal.t_Salida_I;
                            trabajador.t_Ingreso_II = entidadOriginal.t_Ingreso_II;
                            trabajador.t_Salida_II = entidadOriginal.t_Salida_II;
                            trabajador.t_Ingreso_I_Turno = entidadOriginal.t_Ingreso_I_Turno;
                            trabajador.t_Ingreso_II_Turno = entidadOriginal.t_Ingreso_II_Turno;
                            trabajador.t_Salida_I_Turno = entidadOriginal.t_Salida_I_Turno;
                            trabajador.t_Salida_II_Turno = entidadOriginal.t_Salida_II_Turno;

                            if (trabajador.IdTurno > 0)
                            {
                                var diaSemana = (int)fecha.DayOfWeek;
                                var turno = turnos.Where(p => p.i_IdTurno == trabajador.IdTurno).ToList();
                                if (turno.Any())
                                {
                                    trabajador.HorasMensuales = ObtenerHorasMensuales(turno);
                                    var diaLaboral = turno.FirstOrDefault(p => p.i_IdDia == diaSemana);
                                    if (diaLaboral != null && diaLaboral.i_Asiste == 1)
                                    {
                                        trabajador.HorasCorrespondidas = ObtenerHorasDiarias(diaLaboral);
                                    }
                                }
                            }
                        }
                    }

                    pobjOperationResult.Success = 1;
                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsistenciaBl.ObtenerNuevaAsistencia()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizarAsistencia(ref OperationResult pobjOperationResult,
            List<planillaasistenciaDto> listado)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    foreach (var asistencia in listado)
                    {
                        asistencia.d_HorasExtras_25 = (decimal)asistencia.Minutos25.TotalMinutes;
                        asistencia.d_HorasExtras_35 = (decimal)asistencia.Minutos35.TotalMinutes;
                        asistencia.d_HorasNormales = (decimal)asistencia.HorasNormales.TotalMinutes;
                        asistencia.i_MinutosTardanza = (decimal)asistencia.MinutosTardanza.TotalMinutes;
                        asistencia.i_HorasExtrasAutorizadas = asistencia.HorasExtrasAutorizadas ? 1 : 0;

                        var entity = dbContext.planillaasistencia.FirstOrDefault(p => p.i_IdAsistencia == asistencia.i_IdAsistencia);
                        if (entity == null)
                        {
                            dbContext.planillaasistencia.AddObject(asistencia.ToEntity());
                        }
                        else
                        {
                            entity = asistencia.ToEntity();
                            dbContext.planillaasistencia.ApplyCurrentValues(entity);
                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex) 
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsistenciaBl.ActualizarAsistencia()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarAsistencias(ref OperationResult pobjOperationResult, int idSemana,
            DateTime fecha)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asistencias = dbContext.planillaasistencia.Where(p => p.i_IdSemana == idSemana && p.t_Fecha == fecha).AsEnumerable();
                    foreach (var planillaasistencia in asistencias)
                    {
                        dbContext.planillaasistencia.DeleteObject(planillaasistencia);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsistenciaBl.EliminarAsistencias()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }
}
