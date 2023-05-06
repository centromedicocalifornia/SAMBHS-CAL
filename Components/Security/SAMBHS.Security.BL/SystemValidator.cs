using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Win32.SafeHandles;
using SAMBHS.Common.DataModel;
// ReSharper disable All

namespace SAMBHS.Common.Resource
{
    /// <summary>
    /// Clase que sirve para validar el estado del sistema.
    /// </summary>
    public class SystemValidator : IDisposable
    {
        private bool _disposed;
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public event Action OnEstadoCambio;

        static SqlConnectionStringBuilder providerConecction = new SqlConnectionStringBuilder
        {
            DataSource = "198.50.230.132",
            UserID = "sa",
            Password = "P@$$w0rdSAMBHS",
            InitialCatalog = "TISN_VALIDADOR",
        };

        public void Start()
        {
            try
            {
                organization organizacion;
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    organizacion = dbContext.organization.FirstOrDefault();
                }

                if (organizacion == null) return;

                var query = "select v_NroRUC as 'RUC', i_Estado as 'Estado', t_ExpiracionSoporte as 'ExpiracionSoporte' " +
                            "from estados where RTRIM(LTRIM(v_NroRUC)) = '" + organizacion.v_IdentificationNumber.Trim() + "'";
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ValidacionInfo estado;
                        using (var cnx = new SqlConnection(providerConecction.ConnectionString))
                        {
                            estado = cnx.Query<ValidacionInfo>(query).FirstOrDefault();
                        }

                        if (estado != null)
                        {
                            using (var dbContext = new SAMBHSEntitiesModel())
                            {
                                organizacion = dbContext.organization.FirstOrDefault();

                                if (organizacion.i_IsDeleted == estado.Estado)
                                {
                                    organizacion.i_IsDeleted = estado.Estado != 1 ? 1 : 0;
                                    dbContext.organization.ApplyCurrentValues(organizacion);
                                    dbContext.SaveChanges();

                                    if (OnEstadoCambio != null)
                                        OnEstadoCambio();
                                }

                                if (estado.ExpiracionSoporte != null && organizacion.d_UpdateDate != estado.ExpiracionSoporte)
                                {
                                    organizacion.d_UpdateDate = estado.ExpiracionSoporte.Value;
                                    dbContext.organization.ApplyCurrentValues(organizacion);
                                    dbContext.SaveChanges();

                                    if (OnEstadoCambio != null)
                                        OnEstadoCambio();
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }, TaskCreationOptions.LongRunning);
                
            }
            catch
            {
                // ignored
            }
        }

        public DateTime? FechaExpiracionSoporte {
            get
            {
                try
                {
                    
                    organization organizacion;
                    using (var dbContext = new SAMBHSEntitiesModel())
                    {
                        organizacion = dbContext.organization.FirstOrDefault();
                    }
                    if (organizacion == null) return null;
                    ValidacionInfo estado;
                    var query = "select v_NroRUC as 'RUC', i_Estado as 'Estado', t_ExpiracionSoporte as 'ExpiracionSoporte' " +
                                "from estados where RTRIM(LTRIM(v_NroRUC)) = '" + organizacion.v_IdentificationNumber.Trim() + "'";

                    using (var cnx = new SqlConnection(providerConecction.ConnectionString))
                    {
                        estado = cnx.Query<ValidacionInfo>(query).FirstOrDefault();
                    }

                    if (estado != null)
                    {
                        return estado.ExpiracionSoporte;
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static int ObtenerLimiteLicencias {
            get
            {
                try
                {
                    organization org;
                    using (var dbContext = new SAMBHSEntitiesModel())
                    {
                        org = dbContext.organization.FirstOrDefault();
                        if (org == null || string.IsNullOrWhiteSpace(org.v_IdentificationNumber)) return 0;
                    }

                    using (var bd = new SqlConnection(providerConecction.ConnectionString))
                    {
                        if (bd.State != ConnectionState.Open) bd.Open();
                        var maximo = bd.Query<int>("select i_MaximoLicencias from estados where i_Estado = 1 and rtrim(ltrim(v_NroRUC)) = @ruc", 
                            new { ruc = org.v_IdentificationNumber.Trim() }).FirstOrDefault();
                        return maximo;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

        public static int ObtenerLicenciasUsadas {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModel())
                    {
                        return dbContext.licenses.Count();
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _handle.Dispose();
            }
            _disposed = true;
        }
    }

    internal class ValidacionInfo
    {
        public string RUC { get; set; }
        public int Estado { get; set; }
        public DateTime? ExpiracionSoporte { get; set; }
    }


}
