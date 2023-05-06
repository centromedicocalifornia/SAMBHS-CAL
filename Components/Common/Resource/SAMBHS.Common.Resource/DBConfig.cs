using SAMBHS.Common.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.BE;

namespace SAMBHS.Common.Resource
{
    /// <summary>
    /// Clase para mantener los esquemas de las bases de datos de los clientes siempre al día.
    /// EQC-13-02-2016
    /// </summary>
    public class DbConfig
    {
        public delegate void Estado(string estado);
        public event Estado EstadoEvent;

        /// <summary>
        /// Revisa si la versión de la base de datos conicide con la requerida por el aplicativo, 
        /// si ésta es menor se procede a pasar por cada versión hasta que ambas versiones queden iguales.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="actualVersion">Versión actual del aplicativo</param>
        /// <param name="_TipoMotorBD"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public bool BaseDatosActualizador(ref OperationResult pobjOperationResult, int actualVersion, TipoMotorBD _TipoMotorBD,
            string connectionString, string ReplicationId, string RucEmpresa)
        {
            try
            {
                Globals.TipoMotor = _TipoMotorBD;
                Globals.CadenaConexion = connectionString;
                int dbVersion;
                if (ReplicationId != "N") return true; //sólo se pueden actualizar desde una pc que esta conectada directa al servidor publicador, no a una que este conectada al suscriptor
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var conf = dbContext.dbconfig.FirstOrDefault();
                    if (conf == null)
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "La tabla dbconfig está vacía.";
                        return false;
                    }

                    dbVersion = conf.version;
                }

                while (dbVersion < actualVersion)
                {
                    dbVersion++;

                    if (EstadoEvent != null)
                        EstadoEvent(string.Format("Actualizando base de datos ver. {0}", dbVersion));

                    ActualizaEsquemaBd(ref pobjOperationResult, dbVersion, _TipoMotorBD, connectionString, RucEmpresa);
                    if (pobjOperationResult.Success == 0) return false;

                    #region Actualiza nueva versión a la BD
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var dbc = dbContext.dbconfig.FirstOrDefault();
                        dbc.version = dbVersion;
                        dbContext.dbconfig.ApplyCurrentValues(dbc);
                        dbContext.SaveChanges();
                    }
                    #endregion
                }

                pobjOperationResult.Success = 1;
                return true;
            }
            catch (Exception ex)
            {
                #region Manejo de excepcion
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DBConfig.BaseDatosActualizador()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = "No se pudo actualizar el esquema de la Base de Datos: " + ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return false;
                #endregion
            }
        }

        /// <summary>
        /// Según el tipo de motor de bd que se esta usando se almacena el historial de 
        /// cambios en querys para ser ejecutados segun la versión  de la BD y actualizarla.
        /// </summary>
        /// <param name="pobjOperationResult">Clase utilitaria para contener los errores.</param>
        /// <param name="version">Versión que se desea actualizar.</param>
        /// <param name="tipoMotorBd">El tipo de motor de bd que se está usando.</param>
        /// <param name="connectionString">La candena de conexión del proveedor que se está usando.</param>
        /// <returns></returns>
        private static void ActualizaEsquemaBd(ref OperationResult pobjOperationResult, int version, TipoMotorBD tipoMotorBd,
            string connectionString, string RucEmpresa)
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();
                var listaQuerys = new List<string>();
                switch (tipoMotorBd)
                {
                    #region SQL Server
                    case TipoMotorBD.MSSQLServer:
                        switch (version)
                        {
                            #region Actualizacion 2
                            case 2:
                                listaQuerys.Add("ALTER TABLE nbs_ventakardex ADD d_Monto decimal(18,4);");
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD i_FacturadoContabilidad int;");
                                listaQuerys.Add("ALTER TABLE saldoscontables ADD v_ReplicationId character(1);");
                                listaQuerys.Add("update saldoscontables set v_Replicationid = 'N';");
                                break;
                            #endregion

                            #region Actualizacion 3
                            case 3:
                                listaQuerys.Add("DELETE from datahierarchy where i_GroupId = 146 and  i_ItemId>5");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 6,'ESTADOS UNIDOS MEXICANOS',5,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 7,'REPÚBLICA DE COREA',6,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 8,'CONFEDERACIÓN DE SUIZA',7,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 9,'PORTUGAL',8,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 10,'OTROS',9,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                break;
                            #endregion

                            #region Actualización 4:
                            case 4:
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD v_Descuento  nvarchar(50);");
                                break;
                            #endregion

                            #region Actualizacion 5:
                            case 5:

                                listaQuerys.Add("ALTER TABLE guiaremision ADD i_IdTipoGuia int;");
                                listaQuerys.Add("ALTER TABLE documento ADD i_DescontarStock integer;");
                                break;
                            #endregion

                            #region Actualizador 6:
                            case 6:
                                listaQuerys.Add("ALTER TABLE guiaremision DROP CONSTRAINT FK_guiaremision_documento_i_IdTipoDocumento");
                                break;

                            #endregion

                            #region Actualizacion 7
                            case 7:
                                listaQuerys.Add("update guiaremision  set i_IdTipoGuia = 9 where i_IdTipoGuia is null");
                                break;
                            #endregion

                            #region Actualizacion 8
                            case 8:
                                listaQuerys.Add("DELETE from  administracionconceptos WHERE v_Codigo ='20'");
                                listaQuerys.Add("INSERT INTO administracionconceptos (v_IdAdministracionConceptos,v_Codigo,v_Nombre,v_CuentaPVenta,v_CuentaIGV,v_CuentaDetraccion,i_Eliminado,i_InsertaIdUsuario,t_InsertaFecha,i_ActualizaIdUsuario,t_ActualizaFecha) values ('N002-ZV000000031',	'20'	,'PROVEEDOR IGV','4212102','','',0,	1,'2014-11-25',	null,	null)");
                                listaQuerys.Add("INSERT INTO concepto (v_IdConcepto,v_Codigo,v_Nombre,i_IdArea,i_Eliminado,i_InsertaIdUsuario,t_InsertaFecha,i_ActualizaIdUsuario,t_ActualizaFecha) values ('N002-ZT000000031','50','PERCEPCION',2,0,1,'2014-11-25',NULL,NULL)");
                                listaQuerys.Add("INSERT INTO administracionconceptos(v_IdAdministracionConceptos,v_Codigo,v_Nombre,v_CuentaPVenta,v_CuentaIGV,v_CuentaDetraccion,i_Eliminado,i_InsertaIdUsuario,t_InsertaFecha,i_ActualizaIdUsuario,t_ActualizaFecha) values ('N002-ZV000000032',	'50'	,'PERCEPCION','','','',0,	1,'2014-11-25',	null,	null)");
                                listaQuerys.Add("INSERT INTO concepto(v_IdConcepto,v_Codigo,v_Nombre,i_IdArea,i_Eliminado,i_InsertaIdUsuario,t_InsertaFecha,i_ActualizaIdUsuario,t_ActualizaFecha) values ('N002-ZT000000032','51','PROVEEDOR PERCEPCION',2,0,1,'2014-11-25',NULL,NULL)");
                                listaQuerys.Add("INSERT INTO administracionconceptos (v_IdAdministracionConceptos,v_Codigo,v_Nombre,v_CuentaPVenta,v_CuentaIGV,v_CuentaDetraccion,i_Eliminado,i_InsertaIdUsuario,t_InsertaFecha,i_ActualizaIdUsuario,t_ActualizaFecha) values ('N002-ZV000000033',	'51'	,'PROVEEDOR PERCEPCION','','','',0,	1,'2014-11-25',	null,	null)");

                                break;

                            #endregion

                            #region Actualizacion 9
                            case 9:

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirTransportistaGuiaRemision int");
                                listaQuerys.Add("ALTER TABLE guiaremision ADD  i_AfectoIgv int");
                                listaQuerys.Add("ALTER TABLE guiaremision  ADD  i_PrecionInclIgv int");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD  d_Valor decimal(18,4)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD  v_Descuento nvarchar(50)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD  d_Descuento decimal(18,4)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD d_ValorVenta decimal(18,4)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD  d_Igv  decimal(18,4)");

                                break;
                            #endregion

                            #region Actualizacion 10
                            case 10:
                                listaQuerys.Add("ALTER TABLE guiaremision ADD i_IdIgv int");
                                break;
                            #endregion

                            #region Actualizacion 11
                            case 11:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_ActualizarCostoProductos int;");
                                break;
                            #endregion

                            #region Actualizacion 12
                            case 12:

                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD i_UsadoVenta int;");
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajodetalle ADD  v_DescripcionTemporal  nvarchar(100)");
                                break;
                            #endregion

                            #region Actualizacion 13

                            case 13:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajodetalle ADD d_ImporteRegistral decimal(18,4)");
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD d_ImporteRegistral decimal(18,4)");

                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajo ADD  d_TotalRegistral decimal(18,4)");
                                break;
                            #endregion

                            #region Actualizacion 14
                            case 14:
                                listaQuerys.Add("ALTER TABLE linea ALTER COLUMN v_Nombre varchar(50)");
                                listaQuerys.Add("ALTER TABLE marca ADD v_Sigla varchar(3)");
                                listaQuerys.Add("ALTER TABLE producto ADD v_IdColor VARCHAR(16)");
                                listaQuerys.Add("ALTER TABLE producto ADD v_IdTalla VARCHAR(16)");
                                listaQuerys.Add("ALTER TABLE producto ADD  b_Foto IMAGE");
                                break;
                            #endregion

                            #region Actualizacion 15
                            case 15:
                                listaQuerys.Add("ALTER TABLE producto ADD v_Modelo varchar(100)");
                                listaQuerys.Add(@"create table productoreceta
												(
													i_IdReceta int identity primary key,
													v_IdProdTerminado varchar(16),
													v_IdProdInsumo varchar(16),
													v_Observacion varchar(250),
													d_Cantidad decimal(16, 4),
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													CONSTRAINT fk_productodetalle_v_IdProdTerminado FOREIGN KEY (v_IdProdTerminado) REFERENCES productodetalle (v_IdProductoDetalle),
													CONSTRAINT fk_productodetalle_v_IdProdInsumo FOREIGN KEY (v_IdProdInsumo) REFERENCES productodetalle (v_IdProductoDetalle),
												);");
                                break;
                            #endregion

                            #region Actualizacion 16
                            case 16:
                                listaQuerys.Add("ALTER TABLE compra ALTER COLUMN v_GuiaRemisionCorrelativo nvarchar(100);");
                                break;
                            #endregion

                            #region Actualizacion17
                            case 17:
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD  i_UsadoIrpe int");
                                break;
                            #endregion

                            #region Actualizacion 18
                            case 18:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD v_IdRecetaFinal character varying(16);");
                                listaQuerys.Add(@"CREATE TABLE movimientodetallerecetafinal (
												  v_IdRecetaFinal VARCHAR(16) COLLATE Modern_Spanish_CI_AS NOT NULL,
												  v_IdMovimientoDetalle VARCHAR(16) COLLATE Modern_Spanish_CI_AS NULL,
												  v_IdProdTerminado VARCHAR(16) COLLATE Modern_Spanish_CI_AS NULL,
												  v_IdProdInsumo VARCHAR(16) COLLATE Modern_Spanish_CI_AS NULL,
												  d_Cantidad DECIMAL(10, 4) NULL,
												  i_Eliminado INT NULL,
												  i_InsertaIdUsuario INT NULL,
												  t_InsertaFecha DATETIME NULL,
												  i_ActualizaIdUsuario INT NULL,
												  t_ActualizaFecha DATETIME NULL,
												  CONSTRAINT PK_movimientodetallerecetafinal PRIMARY KEY CLUSTERED (v_IdRecetaFinal),
												  CONSTRAINT FK_movimientodetalle_movimientodetallerecetafinal_id FOREIGN KEY (v_IdMovimientoDetalle) REFERENCES movimientodetalle (v_IdMovimientoDetalle),
												  CONSTRAINT FK_productodetalle_movimientodetallerecetafinal FOREIGN KEY (v_IdProdTerminado) REFERENCES productodetalle (v_IdProductoDetalle),
												  CONSTRAINT FK_productodetalle_movimientodetallerecetafinalInsumo FOREIGN KEY (v_IdProdInsumo) REFERENCES productodetalle (v_IdProductoDetalle)
												);");
                                break;
                            #endregion

                            #region Actualizacion 19
                            case 19:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD i_EsProductoFinal int;");
                                break;
                            #endregion

                            #region Actualizacion 20
                            case 20:
                                listaQuerys.Add("ALTER TABLE movimiento ADD v_IdMovimientoOrigen varchar(16);");
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal DROP CONSTRAINT FK_movimientodetalle_movimientodetallerecetafinal_id;");
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal DROP COLUMN v_IdMovimientoDetalle;");
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal ADD v_IdMovimiento varchar(16);");

                                break;
                            #endregion

                            #region Actualizacion 21
                            case 21:
                                listaQuerys.Add("ALTER TABLE linea ADD b_Foto image;");
                                listaQuerys.Add("ALTER TABLE linea ADD i_Header int;");
                                break;
                            #endregion

                            #region Actualizacion 22
                            case 22:
                                listaQuerys.Add(@"EXEC sp_rename 'FK_establecimientoalmacen_establecimiento_IdEstablecimientoAlmacen_i_IdAlmacen', 
																 'FK_establecimientoalmacen_establecimiento_IdEstablecimientoAlma'");
                                break;
                            #endregion

                            #region Actualizacion 23
                            case 23:
                                listaQuerys.Add("ALTER TABLE establecimientodetalle ADD  i_NumeroItems int;");
                                listaQuerys.Add("ALTER TABLE movimiento ADD  i_GenerarGuia int;");
                                break;
                            #endregion

                            #region Actualizacion 24
                            case 24:
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD  v_NroPedido varchar(50);");
                                break;
                            #endregion

                            #region Actualizacion 25
                            case 25:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirPedidoExportacionCompraVenta int");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirLotesCompraVenta int");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirNingunoCompraVenta int");
                                listaQuerys.Add("update configuracionempresa set i_IncluirNingunoCompraVenta = 1;");
                                listaQuerys.Add("update configuracionempresa set i_IncluirLotesCompraVenta = 0;");
                                listaQuerys.Add("update configuracionempresa set i_IncluirPedidoExportacionCompraVenta= 0;");

                                break;
                            #endregion

                            #region Actualizacion 26
                            case 26:
                                listaQuerys.Add("ALTER TABLE productoreceta ADD i_IdAlmacen integer;");
                                listaQuerys.Add(@"ALTER TABLE productoreceta ADD CONSTRAINT fk_productoreceta_almacen_i_IdAlmacen " +
                                                "FOREIGN KEY (i_IdAlmacen) REFERENCES almacen (i_IdAlmacen);");
                                break;
                            #endregion

                            #region Actualizacion 27
                            case 27:
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal ADD i_IdAlmacen integer;");
                                break;
                            #endregion

                            #region Actualizacion 28
                            case 28:
                                listaQuerys.Add("ALTER TABLE documento ADD i_OperacionTransitoria integer;");
                                listaQuerys.Add("ALTER TABLE documento ADD v_NroContraCuenta varchar(50);");
                                break;
                            #endregion

                            #region Actualizacion 29
                            case 29:
                                listaQuerys.Add("ALTER TABLE venta ADD i_EstadoSunat smallint;");
                                listaQuerys.Add(@"CREATE TABLE ventahomolagacion
												(
													i_IdVentaHomologacion int IDENTITY(1,1) NOT NULL,
													b_FileXml varbinary(max) NULL,
													v_IdVenta nchar(16) NULL,
													CONSTRAINT PK_ventahomolagacion PRIMARY KEY CLUSTERED (i_IdVentaHomologacion),
													CONSTRAINT Fk_ventahomologacion_venta_v_IdVenta FOREIGN KEY (v_IdVenta) REFERENCES venta (v_IdVenta),
												)");
                                break;
                            #endregion

                            #region Actualizacion 30
                            case 30:
                                listaQuerys.Add("update datahierarchy set  v_Field='FECHA,NROCOMPROBANTE' where i_GroupId=98 and i_ItemId=1");
                                break;

                            #endregion

                            #region Actualizacion 31
                            case 31:
                                listaQuerys.Add("ALTER TABLE ventahomolagacion ADD v_Ticket nchar(16);");
                                listaQuerys.Add("ALTER TABLE ventahomolagacion ADD b_ResponseTicket varbinary(MAX);");
                                break;
                            #endregion

                            #region Actualizacion 32
                            case 32:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdDepartamento int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdProvincia int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdDistrito int;");
                                break;
                            #endregion

                            #region Actualizacion 33
                            //actualizacion 33 solo es para postgres.
                            case 33:
                                break;
                            #endregion

                            #region Actualizacion 34
                            case 34:
                                listaQuerys.Add(@"CREATE TABLE configuracionfacturacion
											(
												i_Idconfiguracionfacturacion int IDENTITY(1,1) NOT NULL,
												v_Ruc nchar(11) NULL,
												v_Usuario nchar(30) NULL,
												v_Clave nchar(30) NULL,
												v_RazonSocial nchar(100) NULL,
												v_NombreComercial nchar(100) NULL,
												v_Domicilio nchar(100) NULL,
												v_Urbanizacion nchar(25) NULL,
												v_Ubigueo nchar(6) NULL,
												v_Departamento nchar(30) NULL,
												v_Provincia nchar(30) NULL,
												v_Distrito nchar(30) NULL,
												b_FileCertificado varbinary(MAX) NULL,
												v_ClaveCertificado nchar(30) NULL,
												i_EsEmisor smallint NULL,
												CONSTRAINT PK_i_Idconfiguracionfacturacion PRIMARY KEY CLUSTERED (i_Idconfiguracionfacturacion)
											)");
                                listaQuerys.Add("ALTER TABLE configuracionempresa DROP COLUMN i_IdDepartamento");
                                listaQuerys.Add("ALTER TABLE configuracionempresa DROP COLUMN i_IdProvincia");
                                listaQuerys.Add("ALTER TABLE configuracionempresa DROP COLUMN i_IdDistrito");
                                listaQuerys.Add(@"CREATE TABLE ventaresumenhomologacion
											(
												i_Idventaresumen int IDENTITY(1,1) NOT NULL,
												v_Ticket nchar(16) NULL,
												b_FileZip varbinary(MAX) NULL,
												t_FechaResumen date NULL,
												i_InsertaIdUsuario int NULL,
												t_InsertaFecha datetime NULL,
												CONSTRAINT PK_ventaresumenhomologacion PRIMARY KEY CLUSTERED (i_Idventaresumen)
											)");
                                listaQuerys.Add(@"CREATE TABLE ventaelectronicasecuential
											(
											  i_Id int IDENTITY(1,1) NOT NULL,
											  i_IdTipoOperacion int NULL,
											  i_NroCorrelativo int NULL,
											  CONSTRAINT PK_ventaelectronicasecuential PRIMARY KEY CLUSTERED (i_Id)
											)");
                                break;
                            #endregion

                            #region Actualizacion 35
                            case 35:
                                listaQuerys.Add("ALTER TABLE venta ADD i_IdTipoNota int NULL;");

                                listaQuerys.Add("UPDATE venta SET i_IdTipoNota = -1;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Clave varchar(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_ClaveCertificado varchar(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Usuario varchar(30);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_RazonSocial varchar(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_NombreComercial varchar(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Domicilio varchar(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Urbanizacion varchar(25);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Departamento varchar(30);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Provincia varchar(30);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  v_Distrito varchar(30);");
                                break;
                            #endregion

                            #region Actualizacion 36
                            case 36:
                                listaQuerys.Add("ALTER TABLE importacion  ALTER COLUMN d_ValorFob decimal(18,6);");
                                break;
                            #endregion

                            #region Actualizacion 37
                            case 37:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdMonedaImportacion int");
                                listaQuerys.Add("update configuracionempresa set i_IdMonedaImportacion = 2;");
                                break;
                            #endregion

                            #region Actualizacion 38
                            case 38:
                                listaQuerys.Add("ALTER TABLE ventaresumenhomologacion ADD i_Estado smallint NULL;");
                                break;
                            #endregion

                            #region Actualizacion 39
                            case 39:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD i_GroupUndInter int null;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD i_GroupNCR int null;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD i_GroupNDB int null;");
                                break;
                            #endregion

                            #region Actualizacion 40
                            case 40:
                                listaQuerys.Add("ALTER TABLE letrasdetalle ADD v_NroUnico varchar(50);");
                                break;
                            #endregion

                            #region Actualizacion 41
                            case 41:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD i_TipoServicio smallint NULL;");
                                break;
                            #endregion

                            #region Actualizacion 42

                            case 42:
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD  t_FechaRegistro datetime;");
                                listaQuerys.Add("update importacionDetalleGastos set \"t_FechaRegistro\" =\"t_FechaEmision\";");
                                break;
                            #endregion

                            #region Actualizacion 43
                            case 43:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD b_Logo VARBINARY(MAX) NULL;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD v_Web VARCHAR(150) NULL;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD v_Resolucion VARCHAR(50) NULL;");
                                break;
                            #endregion

                            #region Actualizacion 44
                            case 44:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaISC varchar(20);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaPercepcion varchar(20);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaOtrosConsumos varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 45
                            case 45:
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD d_GastosFinancieros decimal(18,2);");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD d_IngresosFinancieros decimal(18,2);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaGastosFinancierosCobranza varchar(20);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaIngresosFinancierosCobranza varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 46
                            case 46:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD d_PrecioCambio decimal(12,6);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD  d_TotalCambio   decimal(10,3);");
                                listaQuerys.Add("ALTER TABLE importacionDetalleProducto ADD  d_CostoUnitarioCambio   decimal(18,6);");
                                break;
                            #endregion

                            #region Actualizacion 47
                            case 47:

                                listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (80, 3,'TIPO DOCUMENTO','IDTIPODOCUMENTO','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                break;
                            #endregion

                            #region Actualizacion 48

                            case 48:
                                listaQuerys.Add(@"create table documentorol
												(
													i_IdDocumentoRol int identity primary key,
													i_CodigoEnum int,
													i_IdTipoDocumento int,
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													CONSTRAINT fk_documento_i_IdTipoDocumento FOREIGN KEY (i_IdTipoDocumento) REFERENCES documento (i_CodigoDocumento),
												);");
                                break;
                            #endregion

                            #region Actualizacion 49
                            case 49:
                                listaQuerys.Add("ALTER TABLE  documentorol DROP CONSTRAINT fk_documento_i_IdTipoDocumento;");
                                listaQuerys.Add("ALTER TABLE movimiento ADD v_NroOrdenCompra  nvarchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 50
                            case 50:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_CambiarUnidadMedidaVentaPedido int");
                                break;
                            #endregion

                            #region Actualizacion 51
                            case 51:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirSEUOImpresionDocumentos int");

                                listaQuerys.Add("update configuracionempresa set i_IncluirSEUOImpresionDocumentos = 1;");
                                break;
                            #endregion

                            #region Actualizacion 52


                            case 52:

                                listaQuerys.Add("update datahierarchy set  v_Value2='idTipoDocumento',v_Value1='TIPO DOCUMENTO' where i_GroupId=81 and i_ItemId=1");
                                break;
                            #endregion

                            #region Actualizacion 53
                            case 53:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdDocumentoContableLEC int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdDocumentoContableLEP int;");
                                break;
                            #endregion

                            #region Actualizacion 54
                            case 54:
                                listaQuerys.Add(@"CREATE TABLE productoisc
								(
									i_IdProductoIsc INT IDENTITY(1,1) NOT NULL,
									v_IdProducto varchar(16),
									i_IdSistemaIsc INT,
									d_Porcentaje DECIMAL(4,3),
									d_Monto DECIMAL(7,4),
									v_Periodo nchar(4),
									i_InsertaIdUsuario INT,
									t_InsertaFecha DATETIME,
									i_ActualizaIdUsuario INT,
									t_ActualizaFecha DATETIME,
									CONSTRAINT Pk_Id_ProductoIsc PRIMARY KEY(i_IdProductoIsc),
									CONSTRAINT Fk_Id_Producto FOREIGN KEY(v_IdProducto) REFERENCES producto(v_IdProducto)
								);");
                                listaQuerys.Add("ALTER TABLE producto ADD i_EsAfectoIsc SMALLINT;");
                                break;
                            #endregion

                            #region Actualizacion 55
                            case 55:
                                listaQuerys.Add("update datahierarchy set  v_Field='BLS' where i_GroupId=17 and i_ItemId=1");
                                listaQuerys.Add("update datahierarchy set  v_Field='CTO' where i_GroupId=17 and i_ItemId=2");
                                listaQuerys.Add("update datahierarchy set  v_Field='GAL' where i_GroupId=17 and i_ItemId=3");
                                listaQuerys.Add("update datahierarchy set  v_Field='JGO' where i_GroupId=17 and i_ItemId=4");
                                listaQuerys.Add("update datahierarchy set  v_Field='KG' where i_GroupId=17 and i_ItemId=5");
                                listaQuerys.Add("update datahierarchy set  v_Field='LAM' where i_GroupId=17 and i_ItemId=6");
                                listaQuerys.Add("update datahierarchy set  v_Field='L' where i_GroupId=17 and i_ItemId=7");
                                listaQuerys.Add("update datahierarchy set  v_Field='M' where i_GroupId=17 and i_ItemId=8");
                                listaQuerys.Add("update datahierarchy set  v_Field='PCK' where i_GroupId=17 and i_ItemId=9");
                                listaQuerys.Add("update datahierarchy set  v_Field='PQ' where i_GroupId=17 and i_ItemId=10");
                                listaQuerys.Add("update datahierarchy set  v_Field='PAR' where i_GroupId=17 and i_ItemId=11");
                                listaQuerys.Add("update datahierarchy set  v_Field='PL' where i_GroupId=17 and i_ItemId=12");
                                listaQuerys.Add("update datahierarchy set  v_Field='PLI' where i_GroupId=17 and i_ItemId=13");
                                listaQuerys.Add("update datahierarchy set  v_Field='RLL' where i_GroupId=17 and i_ItemId=14");
                                listaQuerys.Add("update datahierarchy set  v_Field='UND' where i_GroupId=17 and i_ItemId=15");
                                listaQuerys.Add("update datahierarchy set  v_Field='VAR' where i_GroupId=17 and i_ItemId=16");
                                listaQuerys.Add("update datahierarchy set  v_Field='1/2 PL' where i_GroupId=17 and i_ItemId=17");
                                listaQuerys.Add("update datahierarchy set  v_Field='MIL' where i_GroupId=17 and i_ItemId=18");
                                listaQuerys.Add("update datahierarchy set  v_Field='CJA' where i_GroupId=17 and i_ItemId=19");
                                listaQuerys.Add("update datahierarchy set  v_Field='DOC' where i_GroupId=17 and i_ItemId=20");
                                break;

                            #endregion

                            #region Actualizacion 56
                            case 56:
                                listaQuerys.Add("ALTER TABLE movimientoestadobancario ALTER COLUMN v_Concepto nvarchar(250);");
                                break;
                            #endregion

                            #region Actualizacion 57
                            case 57:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (0, 151, 'TIPOS DE SISTEMA DE CALCULO DE ISC', 0, 1, '2016-07-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (151, 1, 'SISTEMA AL VALOR', 0, 1, '2016-07-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (151, 2, 'APLICACION DEL MONTO FIJO', 0, 1, '2016-07-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (151, 3, 'SISTEMA DE PRECIOS DE VENTA AL PUBLICO', 0, 1, '2016-07-27');");

                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle DROP CONSTRAINT FK_documentoretenciondetalle_documentoretenciondetalle_v_IdDocumentoRetencion;");
                                listaQuerys.Add(@"ALTER TABLE documentoretenciondetalle ADD CONSTRAINT FK_documentoretenciondetalle_v_IdDocumentoRetencion 
								FOREIGN KEY(v_IdDocumentoRetencion) REFERENCES documentoretencion(v_IdDocumentoRetencion);");

                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (0, 152,'TIPO REGIMEN EMPRESAS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (152, 1,'RÉGIMEN GENERAL','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (152, 2,'RER','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (152, 3,'NUEVOS RUS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_TipoRegimenEmpresa int");

                                break;
                            #endregion

                            #region Actualizacion 58
                            case 58:
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD i_EstadoSunat smallint;");
                                listaQuerys.Add(@"CREATE TABLE documentoretencionhomologacion
												(
													i_Idhomologacion int IDENTITY(1,1) NOT NULL,
													v_IdDocumentoRetencion nchar(16) NULL, 
													b_FileXml varbinary(max) NULL,
													v_Ticket nchar(16),   
													b_ResponseTicket varbinary(MAX),
													CONSTRAINT PK_documentoretencionhomologacion PRIMARY KEY CLUSTERED (i_Idhomologacion),
													CONSTRAINT Fk_retencionhomologacion_documentoretencion FOREIGN KEY (v_IdDocumentoRetencion) REFERENCES documentoretencion(v_IdDocumentoRetencion),
												)");
                                break;
                            #endregion

                            #region Actualizacion 59
                            case 59:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaObligacionesFinancierosCobranza varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 60
                            case 60:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_ImprimirDniPNaturalesLetras int;");
                                break;
                            #endregion

                            #region Actualizacion 61
                            case 61:
                                listaQuerys.Add(@"CREATE TABLE [letrasdescuentomantenimiento](
												[v_IdLetraDescuentoCancelacion] [varchar](16) NOT NULL,
												[v_IdLetrasDetalle] [varchar](16) NULL,
												[t_FechaCancelacion] [datetime] NULL,
												[i_Eliminado] [int] NULL,
												[i_InsertaIdUsuario] [int] NULL,
												[t_InsertaFecha] [datetime] NULL,
												[i_ActualizaIdUsuario] [int] NULL,
												[t_ActualizaFecha] [datetime] NULL,
												[i_Estado] [int] NULL,
												CONSTRAINT [PK_LetrasDescuentos] PRIMARY KEY CLUSTERED (v_IdLetraDescuentoCancelacion),
												CONSTRAINT [FK_letrasdescuentomantenimiento_letrasdetalle_v_IdLetrasDetalle] 
												FOREIGN KEY (v_IdLetrasDetalle) REFERENCES letrasdetalle(v_IdLetrasDetalle))");
                                break;
                            #endregion

                            #region Actualizacion 62
                            case 62:
                                listaQuerys.Add("update datahierarchy set  v_Value2='IDTIPODOCUMENTO,NRODOCUMENTO' where i_GroupId=80 and i_ItemId=3");
                                break;

                            #endregion

                            #region Actualizacion 63
                            case 63:
                                listaQuerys.Add("ALTER TABLE venta ADD i_EsGratuito SMALLINT;");
                                break;
                            #endregion

                            #region Actualizacion 64


                            case 64:


                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (0, 153,'TIPOS DE KARDEX NBS','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (153, 1,'KARDEX','K','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (153, 2,'COPIAS CERTIFICADAS','C','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (153, 3,'PEDIDOS','P','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (153, 4,'DECLARATORIAS','D','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (153, 5,'LEGALIZACIÓN','L','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (153, 6,'EXPEDIENTES','E','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values  (153, 7,'VARIOS','V','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values  (153, 8,'PRESENCIAL NOTARIAL','N','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (153, 9,'VEHICULAR','H','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values  (153, 10,'GARANTIAS MOBILIARIAS','M','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values  (153, 11,'TESTAMENTO','T','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                break;

                            #endregion

                            #region Actualizacion 65
                            case 65:
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD i_EsAbonoLetraDescuento int;");
                                break;
                            #endregion

                            #region Actualizacion 66
                            case 66:
                                listaQuerys.Add("update datahierarchy set  v_Field='FECHA,NROCOMPROBANTE' where i_GroupId=98 and i_ItemId=1");
                                listaQuerys.Add("update datahierarchy  set v_Value1= 'ENTRADA POR DEVOLUCIÓN DE PRODUCCIÓN' where i_ItemId=8 and i_GroupId=19");
                                listaQuerys.Add("update datahierarchy  set v_Value2= '21' where i_ItemId=15 and i_GroupId =19");
                                listaQuerys.Add("update datahierarchy  set v_Value2 ='11' where i_ItemId=16 and i_GroupId=20");


                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ADD d_PrecioVenta DECIMAL(18,4);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_UsaListaPrecios SMALLINT");
                                listaQuerys.Add("UPDATE configuracionempresa SET i_UsaListaPrecios = 1");


                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 17,'BONIFICACION','07','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 18,'PREMIO','08','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 19,'DONACIÓN','09','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 20,'DESMEDROS','14','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 21,'DESTRUCCIÓN','15','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 22,'EXPORTACIÓN','17','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 23,'MUESTRAS MÉDICAS','33','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 24,'PUBLICIDAD','34','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (20, 25,'GASTOS DE REPRESENTACIÓN','35','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");


                                break;
                            #endregion

                            #region Actualizacion 67
                            case 67:
                                listaQuerys.Add("ALTER TABLE letrasmantenimientodetalle ADD d_GastosAdministrativos DECIMAL(18,4);");
                                break;
                            #endregion

                            #region Actaulizacion 68
                            case 68:

                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (79, 4,'TIPO DOC. VENTA','GROUPHEADERSECTION1,GROUPFOOTERSECTION1','TIPODOCUMENTOVENTA',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                break;
                            #endregion

                            #region Actaulizacion 69
                            case 69:

                                listaQuerys.Add("ALTER TABLE dbo.movimiento ADD i_IdTipoDocumento INT");
                                listaQuerys.Add("ALTER TABLE dbo.movimiento ADD v_SerieDocumento NCHAR(4)");
                                listaQuerys.Add("ALTER TABLE dbo.movimiento ADD v_CorrelativoDocumento NVARCHAR(8);");
                                break;
                            #endregion

                            #region Actualizacion 70
                            case 70:
                                listaQuerys.Add("ALTER TABLE pedido ADD  t_FechaDespacho date null ;");
                                listaQuerys.Add("ALTER TABLE pedido ADD v_IdAgenciaTransporte  varchar(16) ;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirAgenciaTransportePedido int;");

                                break;
                            #endregion

                            #region Actualizacion 71
                            case 71:
                                listaQuerys.Add("ALTER TABLE letrasdescuentomantenimiento ADD d_Saldo decimal(18,3)");
                                listaQuerys.Add("ALTER TABLE letrasdescuentomantenimiento ADD d_Acuenta decimal(18,3)");
                                listaQuerys.Add("ALTER TABLE agenciatransporte ALTER COLUMN v_Telefono NVARCHAR(120)");

                                break;
                            #endregion

                            #region Actualizacion 72
                            case 72:
                                listaQuerys.Add("UPDATE dbo.datahierarchy SET v_Value1 ='Gravado - Operación Onerosa', v_Value2 = '10', v_Field = '1' WHERE i_GroupId = 35 AND i_ItemId = 1");
                                listaQuerys.Add("UPDATE dbo.datahierarchy SET v_Value1 ='Exonerado - Operación Onerosa', v_Value2 = '20', v_Field = '1' WHERE i_GroupId = 35 AND i_ItemId = 2");
                                listaQuerys.Add("UPDATE dbo.datahierarchy SET v_Value1 ='Inafecto - Operación Onerosa', v_Value2 = '30', v_Field = '1' WHERE i_GroupId = 35 AND i_ItemId = 3");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
																VALUES (35, 4, 'Exportación', '40', '1', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 5, 'Mixto', '50', '1', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 11, 'Gravado – Retiro por premio', '11', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 12, 'Gravado – Retiro por donación', '12', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 13, 'Gravado – Retiro', '13', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 14, 'Gravado – Retiro por publicidad', '14', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 15, 'Gravado – Bonificaciones', '15', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 16, 'Gravado – Retiro por entrega a trabajadores', '16', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 17, 'Gravado – IVAP', '17', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 21, 'Exonerado – Transferencia Gratuita', '21', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 31, 'Inafecto – Retiro por Bonificación', '31', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 32, 'Inafecto – Retiro', '32', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 33, 'Inafecto – Retiro por Muestras Médicas', '33', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 34, 'Inafecto -  Retiro por Convenio Colectivo', '34', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 35, 'Inafecto – Retiro por premio', '35', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
															   VALUES (35, 36, 'Inafecto -  Retiro por publicidad', '36', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("UPDATE cliente SET i_IdTipoIdentificacion = 1, v_NroDocIdentificacion = '00000000' WHERE v_IdCliente LIKE '%-CL000000000'");
                                break;
                            #endregion

                            #region Actualizacion 73
                            case 73:
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN  i_IdTipoOperacion int not null ;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  i_EditarPrecioVentaPedido int;");
                                listaQuerys.Add("update configuracionempresa set i_EditarPrecioVentaPedido = 0;");
                                break;
                            #endregion

                            #region Actualizacion 74
                            case 74:
                                listaQuerys.Add("ALTER TABLE pedido ADD i_IdTipoOperacion INT DEFAULT 1 NOT NULL");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD i_IdTipoOperacion INT NULL");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_GlosaTicket NVARCHAR(600) NULL");
                                break;
                            #endregion

                            #region Actualizacion 75
                            case 75:
                                listaQuerys.Add(@"CREATE TABLE productoinventario
								(
								  i_IdProductoInventario INT IDENTITY(1,1) NOT NULL,
								  i_IdEstablecimiento INT,
								  i_IdAlmacen INT,
								  v_IdProducto VARCHAR(16),
								  d_Cantidad DECIMAL(12,2),
								  t_Fecha DATE,
								  CONSTRAINT Pk_IdProductoInventario PRIMARY KEY (i_IdProductoInventario),
								  CONSTRAINT Fk_IdEstablecimiento FOREIGN KEY (i_IdEstablecimiento) REFERENCES dbo.establecimiento(i_IdEstablecimiento),
								  CONSTRAINT Fk_IdAlmacen FOREIGN KEY (i_IdAlmacen) REFERENCES dbo.almacen(i_IdAlmacen),
								  CONSTRAINT Fk_IdProducto FOREIGN KEY (v_IdProducto) REFERENCES dbo.producto(v_IdProducto)
								);");
                                break;
                            #endregion

                            #region  Actualizacion 76
                            case 76:

                                listaQuerys.Add("ALTER TABLE datahierarchy ADD v_Value4 varchar(200);");
                                listaQuerys.Add("update datahierarchy set  v_Value4='01' where i_GroupId=17 and i_ItemId=5");
                                listaQuerys.Add("update datahierarchy set  v_Value4='07' where i_GroupId=17 and i_ItemId=15");
                                listaQuerys.Add("update datahierarchy set  v_Value4='08' where i_GroupId=17 and i_ItemId=7");
                                listaQuerys.Add("update datahierarchy set  v_Value4='09' where i_GroupId=17 and i_ItemId=3");
                                listaQuerys.Add("update datahierarchy set  v_Value4='12' where i_GroupId=17 and i_ItemId=19");
                                listaQuerys.Add("update datahierarchy set  v_Value4='13' where i_GroupId=17 and i_ItemId=18");
                                listaQuerys.Add("update datahierarchy set  v_Value4='15' where i_GroupId=17 and i_ItemId=8");

                                listaQuerys.Add("update datahierarchy set  v_Value2='02' where i_GroupId=6 and i_ItemId=1");
                                listaQuerys.Add("update datahierarchy set  v_Value2='03' where i_GroupId=6 and i_ItemId=2");

                                listaQuerys.Add("ALTER TABLE transportista ALTER COLUMN v_Telefono varchar(200)");
                                break;

                            #endregion

                            #region Actualizacion 77
                            case 77:
                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var TipoProductos = dbContext.datahierarchy.Where(l => l.i_GroupId == 6 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = dbContext.datahierarchy.Where(l => l.i_GroupId == 6).Max(l => l.i_ItemId);
                                    var Mercaderia = TipoProductos.Where(l => l.v_Value1.Contains("MERCA")).ToList();
                                    if (Mercaderia.Any())
                                    {


                                        listaQuerys.Add("update datahierarchy set  v_Value2='01' where i_GroupId=6 and i_ItemId= " + Mercaderia.FirstOrDefault().i_ItemId);
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'MERCADERIA','01','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                    var ProductoTerminado = TipoProductos.Where(l => l.v_Value1.Contains("TERMINAD")).ToList();

                                    if (ProductoTerminado.Any())
                                    {


                                        listaQuerys.Add("update datahierarchy set  v_Value2='02' where i_GroupId=6 and i_ItemId= " + ProductoTerminado.FirstOrDefault().i_ItemId);
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'PRODUCTO TERMINADO','02','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }

                                    var MateriasPrimas = TipoProductos.Where(l => l.v_Value1.Contains("PRIMA")).ToList();
                                    if (MateriasPrimas.Any())
                                    {
                                        foreach (var item in MateriasPrimas)
                                        {

                                            listaQuerys.Add("update datahierarchy set  v_Value2='03' where i_GroupId=6 and i_ItemId= " + item.i_ItemId);
                                        }
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'MATERIAS PRIMAS Y AUXILIARES-MAT','03','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");

                                    }

                                    var Envases = TipoProductos.Where(l => l.v_Value1.Contains("ENVASES")).ToList();
                                    if (Envases.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy set  v_Value2='04' where i_GroupId=6 and i_ItemId= " + Envases.FirstOrDefault().i_ItemId);

                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'ENVASES Y EMBALAJES','04','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }


                                    var SuministrosDiversos = TipoProductos.Where(l => l.v_Value1.Contains("SUMINISTROS")).ToList();
                                    if (SuministrosDiversos.Any())
                                    {

                                        listaQuerys.Add("update datahierarchy set  v_Value2='05' where i_GroupId=6 and i_ItemId= " + SuministrosDiversos.FirstOrDefault().i_ItemId);
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'SUMINISTROS DIVERSOS','05','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }

                                    var Otros = TipoProductos.Where(l => l.v_Value1.Contains("OTROS")).ToList();
                                    if (Otros.Any())
                                    {

                                        listaQuerys.Add("update datahierarchy set  v_Value2='99' where i_GroupId=6 and i_ItemId= " + Otros.FirstOrDefault().i_ItemId);
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'OTROS','99','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");

                                    }
                                    var Etiquetas = TipoProductos.Where(l => l.v_Value1.Contains("ETIQUETA")).ToList();

                                    if (Etiquetas.Any())
                                    {

                                        listaQuerys.Add("update datahierarchy set  v_Value2='07' where i_GroupId=6 and i_ItemId= " + Etiquetas.FirstOrDefault().i_ItemId);
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'ETIQUETAS','07','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");

                                    }

                                    var ProdProceso = TipoProductos.Where(l => l.v_Value1.Contains("PROCESO")).ToList();
                                    if (ProdProceso.Any())
                                    {

                                        listaQuerys.Add("update datahierarchy set  v_Value2='06' where i_GroupId=6 and i_ItemId= " + ProdProceso.FirstOrDefault().i_ItemId);
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (6," + MaxItemId + ",'PRODUCTOS EN PROCESO','06','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");

                                    }

                                }
                                break;
                            #endregion

                            #region Actualizacion 78
                            case 78:
                                listaQuerys.Add("ALTER TABLE letrascanje ADD i_EsAdelanto int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaAdelanto varchar(20);");
                                listaQuerys.Add("ALTER TABLE adelanto ADD i_IdDocumentoCaja integer; ");
                                break;
                            #endregion

                            #region Actualizacion 79
                            case 79:
                                listaQuerys.Add("ALTER TABLE letrascanje ADD v_IdAdelanto nvarchar(16);");
                                listaQuerys.Add("ALTER TABLE letrascanje ADD CONSTRAINT FK_letrascanje_adelanto_v_IdAdelanto " +
                                                "FOREIGN KEY(v_IdAdelanto) REFERENCES adelanto(v_IdAdelanto);");
                                break;
                            #endregion

                            #region Actualizacion 80
                            case 80:
                                listaQuerys.Add("UPDATE dbo.datahierarchy SET v_Field = '1' WHERE i_GroupId = 35 AND i_ItemId = 4");
                                break;
                            #endregion

                            #region Actualizacion 81
                            case 81:
                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ADD d_CantidadCancelada DECIMAL(18,4)");
                                break;
                            #endregion

                            #region Actualizacion 82
                            case 82:

                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (0, 154,'CODIGOS PLAN CONTABLE','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (154, 1,'PLAN CONTABLE GENERAL EMPRESARIAL','01','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (154, 2,'PLAN CONTABLE GENERAL REVISADO','02','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (154, 3,'PLAN DE CUENTAS PARA EMPRESAS DEL SISTEMA FINANCIERO SUPERVISADO POR SBS','03','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (154, 4,'PLAN DE CUENTAS PAR ENTIDADES PRESTADORAS DE SALUD,SUPERVISADAS POR SBS','04','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (154, 5,'PLAN DE CUENTAS PARA EMPRESAS DEL SISTEMA ASEGURADOR,SUPERVISADAS POR SBS','05','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (154, 6,'PLAN DE CUENTAS DE DE LAS ADMINISTRADORAS PRIVADAS DE FONDOS DE PENSIONES ,SUPERVISADAS POR SBS','06','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values  (154, 7,'PLAN CONTABLE GUBERNAMENTAL','07','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values  (154, 99,'OTROS','99','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_CodigoPlanContable int");
                                break;

                            #endregion

                            #region Actualizacion 83
                            case 83:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER column v_DescripcionProducto nvarchar(2000);");
                                break;
                            #endregion

                            #region Actualizacion 84
                            case 84:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_TckUseInfo smallint;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_TckRuc VARCHAR(11);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_TckRzs VARCHAR(200);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_TckDireccion VARCHAR(250);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_TckExt VARCHAR(100)");
                                break;
                            #endregion

                            #region Actualizacion 85
                            case 85:
                                listaQuerys.Add("ALTER TABLE cliente ADD v_Password VARCHAR(100)");
                                break;
                            #endregion

                            #region Actualizacion 86
                            case 86:
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate) values    (41, 6,'LETRA DE CAMBIO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate) values    (43, 3,'ANULADO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                break;
                            #endregion

                            #region Actualizacion 87
                            case 87:
                                listaQuerys.Add("ALTER TABLE establecimientodetalle ALTER COLUMN v_NombreImpresora varchar(500)");
                                break;
                            #endregion

                            #region Actualizacion 88
                            case 88:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajodetalle ALTER column v_DescripcionTemporal nvarchar(2000);");
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD v_DescripcionTemporal nvarchar(2000);");
                                break;
                            #endregion

                            #region Actualizacion 89
                            case 89:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajo ADD  i_IdEstado int;");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (0, 155,'ESTADO-ORDEN TRABAJO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (155, 1,'ACTIVO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (155, 2,'ANULADO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("update datahierarchy set  v_Value2='GROUPHEADERSECTION1,GROUPFOOTERSECTION1' where i_GroupId=78 and i_ItemId=2");
                                listaQuerys.Add("update datahierarchy set  v_Value2='GROUPHEADERSECTION1,GROUPFOOTERSECTION1' where i_GroupId=78 and i_ItemId=3");
                                break;
                            #endregion

                            #region Actualizacion 90
                            case 90:
                                listaQuerys.Add("ALTER TABLE marca DROP CONSTRAINT Fk_Linea_Marca_v_IdLinea");
                                break;
                            #endregion

                            #region Actualizacion 91
                            case 91:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (0, 156, 'TIPOS DE NOTA CREDITO', 0, 1, '2016-10-20');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 1, 'ANULACION DE LA OPERACIÓN', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 2, 'ANULACION POR ERROR EN EL RUC', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 3, 'CORRECION POR ERROR EN LA DESCRIPCION', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 4, 'DESCUENTO GLOBAL', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 5, 'DESCUENTO POR ITEM', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 6, 'DEVOLUCION TOTAL', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 7, 'DEVOLUCION POR ITEM', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 8, 'BONIFICACION', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (156, 9, 'DISMINUCION EN EL VALOR', 0, 1, '2016-10-20');");


                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (0, 157, 'TIPOS DE NOTA DEBITO', 0, 1, '2016-10-20');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (157, 1, 'INTERESES POR MORA', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (157, 2, 'AUMENTO EN EL VALOR', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (157, 3, 'PENALIDADES / OTROS CONCEPTOS', 0, 1, '2016-10-20');");
                                break;
                            #endregion

                            #region Actualizacion 92
                            case 92:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_ValidarStockMinimoProducto int");
                                break;
                            #endregion

                            #region Actualizacion 93
                            case 93:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCtaRetenciones varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 94
                            case 94:
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturacion ALTER COLUMN d_Total decimal(18, 2)");
                                break;

                            #endregion

                            #region Actualizacion 95
                            case 95:
                                listaQuerys.Add("ALTER TABLE producto ADD i_CantidadFabricacionMensual int");
                                break;
                            #endregion

                            #region Actualizacion 96
                            case 96:
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD i_AplicaRetencion int;");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD d_MontoRetencion decimal(18,4);");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD v_NroRetencion varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 97
                            case 97:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (0, 158, 'ESTADO GUIA REMISION', 0, 1, '2016-10-31');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (158, 0, 'ANULADO', 0, 1, '2016-10-31');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (158, 1, 'ACTIVO', 0, 1, '2016-10-31');");
                                break;
                            #endregion

                            #region Actualizacion98

                            case 98:
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD i_EsDetraccion int;");
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD i_CodigoDetraccion int;");

                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD v_NroDetraccion nvarchar(20);");
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD d_PorcentajeDetraccion decimal(18,4);");
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD  t_FechaDetraccion datetime;");
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD d_ValorSolesDetraccion decimal(18,4);");
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD d_ValorDolaresDetraccion decimal(18,4);");

                                break;
                            #endregion
                            #region Actualizacion99
                            case 99:


                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD d_ValorSolesDetraccionNoAfecto decimal(18,4);");
                                listaQuerys.Add("ALTER TABLE importacionDetalleGastos ADD d_ValorDolaresDetraccionNoAfecto decimal(18,4);");

                                break;
                            #endregion

                            #region Actualizacion 100
                            case 100:


                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, v_Field, i_Header, i_Sort, i_IsDeleted, i_InsertUserId, d_InsertDate)
																VALUES (6, 10, 'PRODUCTOS DE SEGUNDA', '02', '0', 0, 0 , 0, 1, '2016-06-24')");
                                break;
                            #endregion

                            #region Actualizacion101
                            case 101:

                                listaQuerys.Add("ALTER TABLE  almacen ADD i_ValidarStockAlmacen int;");
                                break;
                            #endregion


                            #region Actualizacion 102
                            case 102:

                                listaQuerys.Add("ALTER TABLE guiaremisioncompra  ADD v_NroOrdenCompra nvarchar(25);");
                                break;

                            #endregion

                            #region Actualizacion 103
                            case 103:
                                listaQuerys.Add("update asientocontable set \"v_Periodo\" = '2016'");
                                break;
                            #endregion

                            #region Actulizacion 104
                            case 104:
                                listaQuerys.Add("ALTER TABLE planillavariablestrabajador ADD  i_TieneVacaciones int;");
                                break;
                            #endregion

                            #region Actualizacion 105
                            case 105:
                                listaQuerys.Add("update asientocontable set \"v_Periodo\" = '2016'");
                                break;
                            #endregion

                            #region Actualizacion 106
                            case 106:
                                listaQuerys.Add("update linea set \"v_Periodo\" = '2016'");
                                break;
                            #endregion

                            #region Actualizacion 107
                            case 107:
                                listaQuerys.Add(@"CREATE TABLE  [lineacuenta](
												[i_IdLineaCuenta] [int] IDENTITY(1,1) NOT NULL,
												[v_IdLinea] [varchar](16) NULL,
												[v_NroCuentaVenta] [varchar](20) NULL,
												[v_NroCuentaCompra] [varchar](20) NULL,
												[v_NroCuentaDConsumo] [varchar](20) NULL,
												[v_NroCuentaHConsumo] [varchar](20) NULL,
												[v_Periodo] [varchar](4) NULL,
												[i_Eliminado] [int] NULL,
												[i_InsertaIdUsuario] [int] NULL,
												[t_InsertaFecha] [datetime] NULL,
												[i_ActualizaIdUsuario] [int] NULL,
												[t_ActualizaFecha] [datetime] NULL,
														 CONSTRAINT [PK_lineacuenta] PRIMARY KEY CLUSTERED 
														(
															[i_IdLineaCuenta] ASC
														)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
														) ON [PRIMARY]
												ALTER TABLE  [lineacuenta]  WITH CHECK ADD  CONSTRAINT [lineacuenta_linea_v_IdLinea] FOREIGN KEY([v_IdLinea])
												REFERENCES  [linea] ([v_IdLinea]);");

                                listaQuerys.Add("ALTER TABLE  [lineacuenta] CHECK CONSTRAINT [lineacuenta_linea_v_IdLinea]");
                                break;
                            #endregion

                            #region Actualizacion 108
                            case 108:
                                listaQuerys.Add("ALTER TABLE compradetalle ADD d_DescuentoItem varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 109
                            case 109:
                                listaQuerys.Add("ALTER TABLE  [compradetalle] DROP COLUMN [d_DescuentoItem]");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD d_DescuentoItem decimal(18,2);");
                                break;
                            #endregion

                            #region Actualizacion 110
                            case 110:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_CostoListaPreciosDiferentesxAlmacen int");
                                break;
                            #endregion

                            #region Actualizacion 111


                            case 111:
                                listaQuerys.Add("ALTER TABLE listapreciodetalle ADD v_IdProductoDetalle VARCHAR(16) NULL");
                                listaQuerys.Add("ALTER TABLE listapreciodetalle ADD i_IdAlmacen INT NULL");
                                listaQuerys.Add("ALTER TABLE listapreciodetalle add  d_Costo decimal (18,4)");
                                listaQuerys.Add(@"ALTER TABLE listapreciodetalle ADD CONSTRAINT fk_listapreciodetalle_lista_v_productodetalle " +
                                  "FOREIGN KEY (v_IdProductoDetalle) REFERENCES productodetalle (v_IdProductoDetalle)");

                                break;
                            #endregion


                            #region Actualizacion 112
                            case 112:

                                listaQuerys.Add("UPDATE listapreciodetalle SET v_IdProductoDetalle = p.v_ProductoDetalleId,i_IdAlmacen  = p.i_IdAlmacen , d_Costo=prod.d_PrecioCosto FROM listapreciodetalle l JOIN productoalmacen p ON l.v_IdProductoAlmacen = p.v_IdProductoAlmacen   JOIN productodetalle pd ON p.v_ProductoDetalleId = pd.v_IdProductoDetalle  JOIN producto prod ON pd.v_IdProducto =prod.v_IdProducto  WHERE v_idListaPrecioDetalle = l.v_idListaPrecioDetalle");


                                break;
                            #endregion

                            #region Actualizacion 113

                            case 113:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IncluirAlmacenDestinoGuiaRemision int");
                                listaQuerys.Add("ALTER TABLE guiaremision ADD i_IdAlmacenDestino int");
                                listaQuerys.Add("ALTER TABLE movimiento ADD v_NroGuiaVenta VARCHAR(20) NULL");
                                break;
                            #endregion

                            #region Actualizacion 114
                            case 114:
                                var or = new OperationResult();
                                Utils.AperturaData.InicializaLineas(ref or, "2016");
                                break;
                            #endregion

                            #region Actualizacion 115
                            case 115:
                                listaQuerys.Add("ALTER TABLE concepto ADD v_Periodo varchar(4);");
                                listaQuerys.Add("ALTER TABLE administracionconceptos ADD v_Periodo varchar(4);");
                                listaQuerys.Add("update concepto set v_Periodo = '2016' where i_Eliminado = 0;");
                                listaQuerys.Add("update administracionconceptos set v_Periodo = '2016' where i_Eliminado = 0;");
                                break;
                            #endregion

                            #region Actualizacion 116

                            case 116:
                                listaQuerys.Add("ALTER TABLE cliente ADD v_Alias varchar(200);");
                                listaQuerys.Add("ALTER TABLE producto ADD v_NroPartidaArancelaria varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 117
                            case 117:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD i_Automatic smallint NULL");
                                listaQuerys.Add("UPDATE configuracionfacturacion SET i_Automatic = 1");
                                break;
                            #endregion

                            #region Actualizacion 118
                            case 118:
                                listaQuerys.Add("ALTER TABLE vendedor ADD i_PermiteAnularVentas integer;");
                                listaQuerys.Add("ALTER TABLE vendedor ADD i_PermiteEliminarVentas integer;");
                                listaQuerys.Add("UPDATE vendedor SET i_PermiteAnularVentas = 1");
                                listaQuerys.Add("UPDATE vendedor SET i_PermiteEliminarVentas = 1");
                                break;
                            #endregion

                            #region Actualizacion 119
                            case 119:
                                listaQuerys.Add("ALTER TABLE compradetalle ADD v_DescuentoItem varchar(20);");
                                break;
                            #endregion

                            #region Actualizacion 120
                            case 120:
                                listaQuerys.Add("ALTER TABLE compradetalle ALTER column d_DescuentoItem decimal(18,4);");
                                break;
                            #endregion


                            #region Actualizacion 121
                            case 121:

                                listaQuerys.Add(@"create table ventadetalleanexo
												(
													i_IdVentaDetalleAnexo int identity primary key,
													v_Anexo varchar(5000),
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													
												);");

                                listaQuerys.Add("ALTER TABLE ventadetalle ADD i_IdVentaDetalleAnexo int;");
                                break;
                            #endregion

                            #region Actualizacion 122
                            case 122:
                                //listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate) values (20, 26,'TRASLADO DE BIENES PARA TRANSFORMACIÓN','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (20, 26,'TRASLADO DE BIENES PARA TRANSFORMACIÓN','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                break;
                            #endregion

                            #region Actualizacion 123
                            case 123:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER COLUMN d_Total decimal(18, 3)");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER COLUMN d_TotalCambio decimal(18, 3)");
                                break;
                            #endregion

                            #region Actualizacion 124
                            case 124:
                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var RegimenLaboral = dbContext.datahierarchy.Where(l => l.i_GroupId == 115 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = RegimenLaboral.Max(l => l.i_ItemId);
                                    var PequeñaEmpresa = RegimenLaboral.Where(l => l.v_Value1.Contains("PEQUE")).ToList();
                                    if (PequeñaEmpresa.Any())
                                    {
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (115," + MaxItemId + ",'PEQUEÑA EMPRESA D. LEY 2086','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                    var Agraria = RegimenLaboral.Where(l => l.v_Value1.Contains("AGRARIA")).ToList();
                                    if (Agraria.Any())
                                    { }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (115," + MaxItemId + ",'AGRARIA LEY 23760','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                    var Mineros = RegimenLaboral.Where(l => l.v_Value1.Contains("MINEROS")).ToList();
                                    if (Mineros.Any())
                                    { }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (115," + MaxItemId + ",'MINEROS','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                }

                                break;
                            #endregion

                            #region Actualizacion 125
                            case 125:
                                listaQuerys.Add(@"CREATE TABLE  [planillaafectacionesgenerales](
													[i_Id] [int] IDENTITY(1,1) NOT NULL,
													[v_Periodo] [nchar](4) NULL,
													[v_Mes] [nchar](2) NULL,
													[v_IdConceptoPlanilla] [varchar](16) NULL,
													[i_Leyes_Trab_ONP] [int] NULL,
													[i_Leyes_Trab_Senati] [int] NULL,
													[i_Leyes_Trab_SCTR] [int] NULL,
													[i_Leyes_Emp_Essalud] [int] NULL,
													[i_Leyes_Emp_SCTR] [int] NULL,
													[i_AFP_Afecto] [int] NULL,
													[i_Rent5ta_Afecto] [int] NULL,
													[i_Eliminado] [int] NULL,
													[t_InsertaFecha] [datetime] NULL,
													[i_InsertaIdUsuario] [int] NULL,
													[t_ActualizaFecha] [datetime] NULL,
													[i_ActualizaIdUsuario] [int] NULL,
												 CONSTRAINT [PK_planillaafectacionesgenerales] PRIMARY KEY CLUSTERED 
												(
													[i_Id] ASC
												)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
												) ON [PRIMARY]");
                                listaQuerys.Add(@"ALTER TABLE  [planillaafectacionesgenerales]  WITH CHECK ADD  CONSTRAINT [FK_planillaafectacionesgenerales_planillaconceptos] FOREIGN KEY([v_IdConceptoPlanilla])
													REFERENCES  [planillaconceptos] ([v_IdConceptoPlanilla]);");
                                listaQuerys.Add(@"ALTER TABLE  [planillaafectacionesgenerales] CHECK CONSTRAINT [FK_planillaafectacionesgenerales_planillaconceptos];");
                                break;
                            #endregion

                            #region Actualizacion 126
                            case 126:
                                //ignorado
                                break;
                            #endregion

                            #region Actualizacion 127
                            case 127:
                                var listaInsertar = new List<planillaafectacionesgeneralesDto>();

                                using (var dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var limpiarAfectacion = dbContext.planillaafectacionesgenerales.ToList();
                                    limpiarAfectacion.ForEach(o => dbContext.DeleteObject(o));
                                    dbContext.SaveChanges();

                                    var leyestrab = dbContext.planillaafecleyestrab.Where(p => p.i_Eliminado == 0).ToList();
                                    var leyesemp = dbContext.planillaafecleyesemp.Where(p => p.i_Eliminado == 0).ToList();
                                    var afectosafp = dbContext.planillaafecafp.Where(p => p.i_Eliminado == 0).ToList();
                                    var conceptosExistentes = leyestrab.Select(p => new { idConcepto = p.v_IdConceptoPlanilla, mes = p.v_Mes, periodo = p.v_Periodo, tipo = "O" });
                                    conceptosExistentes = conceptosExistentes.Concat(
                                        leyesemp.Select(p => new { idConcepto = p.v_IdConceptoPlanilla, mes = p.v_Mes, periodo = p.v_Periodo, tipo = "E" }));
                                    conceptosExistentes = conceptosExistentes.Concat(
                                        afectosafp.Select(p => new { idConcepto = p.v_IdConceptoPlanilla, mes = p.v_Mes, periodo = p.v_Periodo, tipo = "A" }));
                                    conceptosExistentes = conceptosExistentes.Distinct();

                                    foreach (var agrupadoMesPeriodo in conceptosExistentes.GroupBy(g => new { p = g.periodo, m = g.mes, i = g.idConcepto }))
                                    {
                                        listaInsertar.Add(
                                                new planillaafectacionesgeneralesDto
                                                {
                                                    v_IdConceptoPlanilla = agrupadoMesPeriodo.Key.i,
                                                    i_Leyes_Trab_ONP = agrupadoMesPeriodo.Any(i => i.tipo.Equals("O")) ? 1 : 0,
                                                    i_Leyes_Emp_Essalud = agrupadoMesPeriodo.Any(i => i.tipo.Equals("E")) ? 1 : 0,
                                                    i_AFP_Afecto = agrupadoMesPeriodo.Any(i => i.tipo.Equals("A")) ? 1 : 0,
                                                    i_Leyes_Emp_SCTR = 0,
                                                    i_Leyes_Trab_SCTR = 0,
                                                    i_Leyes_Trab_Senati = 0,
                                                    i_Rent5ta_Afecto = 0,
                                                    v_Mes = agrupadoMesPeriodo.Key.m,
                                                    v_Periodo = agrupadoMesPeriodo.Key.p
                                                });
                                    }

                                    var conceptosEssalud =
                                           dbContext.planillaconceptos.Where(p => p.v_ColumnaAfectaciones.Equals("i_Essalud")).ToList();
                                    foreach (var planillaconceptose in conceptosEssalud)
                                    {
                                        planillaconceptose.v_ColumnaAfectaciones = "i_Leyes_Emp_Essalud";
                                        dbContext.planillaconceptos.ApplyCurrentValues(planillaconceptose);
                                    }

                                    foreach (var objEntity in listaInsertar.Select(entityDto => entityDto.ToEntity()))
                                    {
                                        objEntity.i_InsertaIdUsuario = 1;
                                        objEntity.i_Eliminado = 0;
                                        dbContext.AddToplanillaafectacionesgenerales(objEntity);
                                    }

                                    pobjOperationResult.Success = 1;
                                    dbContext.SaveChanges();
                                }
                                break;
                            #endregion

                            #region Actualizacion 128
                            case 128:
                                listaQuerys.Add("ALTER TABLE dbo.trabajador ADD b_HojaVida varbinary(MAX) NULL");
                                break;
                            #endregion

                            #region Actualizacion 129
                            case 129:

                                listaQuerys.Add(@"CREATE TABLE [cajachica](
												[v_IdCajaChica] [varchar](16) NOT NULL,
												[v_Periodo] [varchar](4) NULL,
												[v_Mes] [varchar](2) NULL,
												[v_Correlativo] [varchar](8) NULL,
												[t_FechaRegistro] [datetime] NULL,
												[d_TotalIngresos] DECIMAL(12,2),
												[d_TotalGastos] DECIMAL(12,2),
												[d_CajaSaldo] DECIMAL(12,2),
												[i_Eliminado] [int] NULL,
												[i_InsertaIdUsuario] [int] NULL,
												[t_InsertaFecha] [datetime] NULL,
												[i_ActualizaIdUsuario] [int] NULL,
												[t_ActualizaFecha] [datetime] NULL
												
												CONSTRAINT [PK_IdCajaChica] PRIMARY KEY CLUSTERED (v_IdCajaChica))");


                                listaQuerys.Add(@"CREATE TABLE conceptoscajachica
								(
								  i_IdConceptosCajaChica INT IDENTITY(1,1) NOT NULL,
								  v_NombreConceptoCajaChica varchar(100),
								  v_NroCuenta varchar (10),
								   d_Cantidad decimal(12,2),
								  [i_Eliminado] [int] NULL,
								  [i_InsertaIdUsuario] [int] NULL,
								  [t_InsertaFecha] [datetime] NULL,
								  [i_ActualizaIdUsuario] [int] NULL,
								 [t_ActualizaFecha] [datetime] NULL,

								  CONSTRAINT Pk_i_IdConceptosCajaChica PRIMARY KEY (i_IdConceptosCajaChica)
								  
								);");


                                listaQuerys.Add(@"CREATE TABLE [cajachicadetalle](
												[v_IdCajaChicaDetalle] [varchar](16) NOT NULL,
												[v_IdCajaChica] [varchar](16) NULL,
												[i_Motivo] [int] NULL,
												[d_Importe] DECIMAL(12,2),
												[v_Usuario] [varchar](100) NULL,
												[v_NombreConceptoCajaChica] [varchar](100) NULL,
												[v_Observacion] [varchar](100) NULL,
												[i_IdConceptosCajaChica] int ,
												[i_Eliminado] [int] NULL,
												[i_InsertaIdUsuario] [int] NULL,
												[t_InsertaFecha] [datetime] NULL,
												[i_ActualizaIdUsuario] [int] NULL,
												[t_ActualizaFecha] [datetime] NULL,
												
												CONSTRAINT [PK_IdCajaChicaDetalle] PRIMARY KEY CLUSTERED (v_IdCajaChicaDetalle),
												FOREIGN KEY (v_IdCajaChica) REFERENCES dbo.cajachica(v_IdCajaChica),
												FOREIGN KEY (i_IdConceptosCajaChica) REFERENCES conceptoscajachica(i_IdConceptosCajaChica))");





                                break;


                            #endregion

                            #region Actualizacion 130
                            case 130:
                                listaQuerys.Add("ALTER TABLE dbo.cajachica ADD i_IdTipoDocumento int");
                                listaQuerys.Add("ALTER TABLE dbo.cajachicadetalle ADD i_IdTipoDocumento int");
                                listaQuerys.Add("ALTER TABLE dbo.cajachicadetalle ADD v_NroDocumento varchar (20)");
                                break;

                            #endregion

                            #region 131
                            case 131:
                                listaQuerys.Add("ALTER TABLE dbo.cajachica ADD i_IdEstado int");
                                break;
                            #endregion

                            #region 132
                            case 132:

                                break;
                            #endregion


                            #region Actualizacion 133
                            case 133:
                                listaQuerys.Add("ALTER TABLE dbo.cajachica ADD d_TipoCambio decimal(18,4)");
                                listaQuerys.Add("ALTER TABLE dbo.cajachica ADD i_IdMoneda int");
                                break;

                            #endregion

                            #region Actualizacion 134
                            case 134:

                                listaQuerys.Add(@"delete from conceptoscajachica");
                                listaQuerys.Add(@"Insert into conceptoscajachica(v_NombreConceptoCajaChica,v_NroCuenta,i_Eliminado,i_InsertaIdUsuario,t_InsertaFecha)values ('APERTURA','',0,1,'2017-01-24')");

                                break;
                            #endregion

                            #region 135
                            case 135:
                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var UnidadMedida = dbContext.datahierarchy.Where(l => l.i_GroupId == 17 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = dbContext.datahierarchy.Where(l => l.i_GroupId == 17).Max(l => l.i_ItemId);
                                    var Toneladas = UnidadMedida.Where(l => l.v_Value1.Contains("TONE")).ToList();
                                    if (!Toneladas.Any())
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (17," + MaxItemId + ",'TONELADA METRICA','1','TM',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                    break;
                                }
                            #endregion
                            #region Actualizacion 136
                            case 136:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (0, 159, 'VENTA-TIPO BULTO', 0, 1, '2016-10-31');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (159, 0, 'CAJAS', 0, 1, '2016-10-31');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (159, 1, 'SACOS DE PAPEL', 0, 1, '2016-10-31');");

                                listaQuerys.Add("ALTER TABLE venta ADD i_IdTipoBulto int");
                                break;
                            #endregion


                            #region Actualizacion 137
                            case 137:
                                listaQuerys.Add("ALTER TABLE dbo.conceptoscajachica ADD i_RequiereTipoDocumento int");
                                listaQuerys.Add("ALTER TABLE dbo.conceptoscajachica ADD i_RequiereNumeroDocumento int");
                                listaQuerys.Add("ALTER TABLE dbo.conceptoscajachica ADD i_RequiereAnexo int");
                                listaQuerys.Add("ALTER TABLE dbo.cajachicadetalle ADD v_IdCliente varchar (20)");
                                break;
                            #endregion

                            #region Actualizacion 138
                            case 138:

                                if (RucEmpresa == Constants.RucDetec)
                                {
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (0, 160, 'VENTA-TIPO BULTO', 0, 1, '2016-10-31');");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (160, 0, 'CAJAS', 0, 1, '2016-10-31');");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (160, 1, 'SACOS DE PAPEL', 0, 1, '2016-10-31');");




                                }
                                else
                                {
                                    listaQuerys.Add("DELETE from datahierarchy where i_GroupId = 159");
                                    listaQuerys.Add("DELETE from datahierarchy where i_ItemId = 159");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) VALUES (0, 159, 'UNIDADES DE MEDIDA INTER.', 0, 1, '2016-10-31');");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 1,'bag','BG',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 2,'hundred','CEN',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 3,'gal','A76',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 4,'gallon (US)','GLL',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate) values (159, 5,'kilogram','KGM',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 6,'sheet','ST',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)   values (159, 7,'slipsheet','SL',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 8,'cubic centimetre','CMQ',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 9,'centimetre','CMT',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 10,'cubic millimetre','MMQ',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 11,'millimetre','MMT',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 12,'metre','MTR',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 13,'ton (US shipping)','L86',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 14,'bulk pack','AB',0,1, '2015-12-03 14:45:50.0092387' )");

                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 15,'bar [unit of packaging]','BR',0,1,'2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate) values (159, 16,'packet','PA',0,1,'2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 17,'pack','PK',0,1,'2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 18,'number of pair','NPR',0,1,'2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 19,'pair','PR',0,1,'2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 20,'number of international units','NIU',0,1,'2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 21,'thousand','MIL',0,1,'2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 22,'thousand bag','T4',0,1,'2015-12-03 14:45:50.0092387')");

                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 23,'box','BX',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 24,'hundred boxes','HBX',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 25,'dozen','DZN',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 26,'dozen pack','DZP',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 27,'number of rolls','NRL',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 28,'roll','RO',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 29,'rod','RD',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 30,'thousand sheet','TW',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 31,'set','SET',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, v_Value2, i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 32,'litre','LTR',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 33,'millilitre','MLT',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1,  v_Value2,i_IsDeleted, i_InsertUserId, d_InsertDate)  values (159, 34,'otro','ZZ',0,1, '2015-12-03 14:45:50.0092387' )");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (0, 160, 'VENTA-TIPO BULTO', 0, 1, '2016-10-31');");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (160, 0, 'CAJAS', 0, 1, '2016-10-31');");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate) 
								VALUES (160, 1, 'SACOS DE PAPEL', 0, 1, '2016-10-31');");




                                }
                                break;
                            #endregion

                            #region Actualizacion 139
                            case 139:
                                listaQuerys.Add("update datahierarchy set  v_Value2='9' where i_GroupId=29 and i_ItemId=1");
                                listaQuerys.Add("update datahierarchy set  v_Value2='3.85' where i_GroupId=29 and i_ItemId=2");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=3");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=4");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=5");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=6");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=7");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=8");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=9");
                                listaQuerys.Add("update datahierarchy set  v_Value2='15' where i_GroupId=29 and i_ItemId=10");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=11");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=12");
                                listaQuerys.Add("update datahierarchy set  v_Value2='9' where i_GroupId=29 and i_ItemId=13");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=14");
                                listaQuerys.Add("update datahierarchy set  v_Value2='9' where i_GroupId=29 and i_ItemId=15");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=16");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=17");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=18");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=19");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=20");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=21");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=22");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=22");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=23");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=24");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=25");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=26");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=27");
                                listaQuerys.Add("update datahierarchy set  v_Value2='9' where i_GroupId=29 and i_ItemId=28");
                                listaQuerys.Add("update datahierarchy set  v_Value2='9' where i_GroupId=29 and i_ItemId=29");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=30");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=31");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=32");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=33");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=34");
                                listaQuerys.Add("update datahierarchy set  v_Value2='1.5' where i_GroupId=29 and i_ItemId=35");
                                listaQuerys.Add("update datahierarchy set  v_Value2='1.5' where i_GroupId=29 and i_ItemId=36");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=37");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=38");
                                listaQuerys.Add("update datahierarchy set  v_Value2='10' where i_GroupId=29 and i_ItemId=39");
                                listaQuerys.Add("update datahierarchy set  v_Value2='4' where i_GroupId=29 and i_ItemId=40");
                                listaQuerys.Add("update datahierarchy set  v_Value2='' where i_GroupId=29 and i_ItemId=41");


                                break;
                            #endregion

                            #region Actualización 140
                            case 140:
                                listaQuerys.Add("ALTER TABLE dbo.producto ADD i_IndicaFormaParteOtrosTributos int");
                                break;
                            #endregion

                            #region Actualización 141
                            case 141:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE compradetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE compradetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE  importacionDetalleProducto  ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE  importacionDetalleProducto  ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE  temporalVentaDetalle  ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE  temporalVentaDetalle  ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ALTER COLUMN  d_Cantidad  DECIMAL(21,7);");
                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ALTER COLUMN  d_CantidadEmpaque  DECIMAL(21,7);");
                                break;
                            #endregion

                            #region Actualzacion 142
                            case 142:
                                listaQuerys.Add("delete from saldoscontables");
                                break;
                            #endregion

                            #region Actualizacion 143
                            case 143:
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD \"v_IdCompra\" varchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 144
                            case 144:
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD i_Eliminado int;");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD i_ActualizaUsuario int;");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD t_ActualizaFecha datetime;;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD i_Eliminado int;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD i_ActualizaUsuario int;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD t_ActualizaFecha datetime;;");
                                break;
                            #endregion

                            #region Actualizacion 145
                            case 145:
                                listaQuerys.Add("ALTER TABLE tesoreriadetalle ADD v_IdDocumentoRetencionDetalle nchar(16);");
                                listaQuerys.Add("ALTER TABLE tesoreriadetalle ADD CONSTRAINT " +
                                                "FK_tesoreriadetalle_documentoretenciondetalle_v_IdDocumentoRetencionDetalle " +
                                                "FOREIGN KEY (v_IdDocumentoRetencionDetalle) " +
                                                "REFERENCES documentoretenciondetalle (v_IdDocumentoRetencionDetalle);");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD i_IdMoneda integer;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD d_Cambio decimal(18,2);");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD d_TipoCambio decimal(18,3);");
                                break;
                            #endregion

                            #region Actualizacion 146
                            case 146:
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD i_Procesado integer;");
                                listaQuerys.Add("ALTER TABLE activofijo ADD v_PeriodoAnterior nchar(5);");
                                break;
                            #endregion

                            #region Actualizacion 147
                            case 147:
                                listaQuerys.Add("ALTER TABLE tesoreria ADD v_IdDocumentoRetencion varchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 148
                            case 148:
                                listaQuerys.Add("ALTER TABLE producto ADD v_NroParte VARCHAR(30)");
                                break;
                            #endregion

                            #region Actualizacion 149
                            case 149:
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD i_Independiente integer;");
                                break;
                            #endregion

                            #region Actualizacion 150
                            case 150:
                                listaQuerys.Add("DROP TABLE saldoscontables;");
                                break;

                            #endregion

                            #region Actualizacion 151
                            case 151:
                                listaQuerys.Add("ALTER TABLE destino ADD v_Periodo varchar(4);");
                                listaQuerys.Add("update destino set \"v_Periodo\" = '2017' where \"v_IdDestino\" in (" +
                                                "select \"v_IdDestino\" from destino s " +
                                                "join asientocontable a on s.\"v_CuentaOrigen\" = a.\"v_NroCuenta\" " +
                                                "where a.\"v_Periodo\" = '2017' and s.\"i_Eliminado\" = 0 and a.\"i_Eliminado\" = 0)");
                                listaQuerys.Add(
                                    "update destino set \"v_Periodo\" = '2016' where \"v_Periodo\" is null and \"i_Eliminado\" = 0");
                                break;
                            #endregion

                            #region Actualizacion 152
                            case 152:
                                var or2 = new OperationResult();
                                Utils.AperturaData.InicializarDestinos(ref or2, "2017", "2016");
                                break;
                            #endregion
                            #region Actualizacion 153

                            case 153:
                                listaQuerys.Add(@"create table clientedirecciones
												(
													i_IdDireccionCliente int identity primary key,
													v_Direccion varchar(200),
													v_IdCliente varchar(16),
													i_IdDepartamento int,
													i_IdProvincia int,
													i_IdZona int,
													i_IdDistrito int,
													i_Eliminado int,
													i_EsDireccionPredeterminada int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,

													CONSTRAINT fk_clientedireccion_i_IdDireccion FOREIGN KEY (v_IdCliente) REFERENCES cliente (v_IdCliente),
												);");
                                listaQuerys.Add("ALTER TABLE pedido ADD i_IdDireccionCliente int");


                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (0, 161, 'ZONA DIRECCIONES', 0, 1, '2016-07-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (161, 1, 'HUAROCHIRI', 0, 1, '2016-07-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (161, 2, 'S.J.L.', 0, 1, '2016-07-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (161, 3, 'CANTO REY', 0, 1, '2016-07-27');");






                                break;

                            #endregion

                            #region Acttualizacion 154

                            case 154:
                                listaQuerys.Add("ALTER TABLE movimiento ADD i_IdDireccionCliente int");
                                listaQuerys.Add("ALTER TABLE venta ADD i_IdDireccionCliente int");
                                listaQuerys.Add("ALTER TABLE guiaremision ADD i_IdDireccionCliente int");
                                Utils.AperturaData.IniciarDirecionesClientes(ref objOperationResult, TipoMotorBD.MSSQLServer);
                                if (objOperationResult.Success == 0)
                                {
                                    return;
                                }
                                break;

                            #endregion

                            #region Actualizacion 155

                            case 155:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_VisualizarColumnasBasicasPedido int");
                                break;
                            #endregion

                            #region Actualizacion 156
                            case 156:
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN v_DireccionClienteTemporal varchar(2150);");

                                break;
                            #endregion

                            #region Actualizacion 157
                            case 157:
                                listaQuerys.Add(@"CREATE TABLE  [planillavaloresrentaquinta](
									[i_Id] [int] IDENTITY(1,1) NOT NULL,
									[v_Periodo] [nvarchar](4) NULL,
									[i_MaxUIT] [int] NULL,
									[d_Porcentaje] [decimal](7, 2) NULL,
									[i_Eliminado] [int] NULL,
									[i_InsertaIdUsuario] [int] NULL,
									[t_InsertaFecha] [datetime] NULL,
									[i_ActualizaIdUsuario] [int] NULL,
									[t_ActualizaFecha] [datetime] NULL,
								 CONSTRAINT [PK_planillavaloresrentaquinta] PRIMARY KEY CLUSTERED 
								(
									[i_Id] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"CREATE TABLE  [planillavaloresuit](
									[i_Id] [int] IDENTITY(1,1) NOT NULL,
									[v_Periodo] [nchar](4) NULL,
									[d_ValorUIT] [decimal](7, 2) NULL,
									[i_FactorUIT] [int] NULL,
									[i_Eliminado] [int] NULL,
									[i_InsertaIdUsuario] [int] NULL,
									[t_InsertaFecha] [datetime] NULL,
									[i_ActualizaIdUsuario] [int] NULL,
									[t_ActualizaFecha] [datetime] NULL,
								 CONSTRAINT [PK_planillavaloresuit] PRIMARY KEY CLUSTERED 
								(
									[i_Id] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                break;
                            #endregion

                            #region Actualizacion 158
                            case 158:
                                listaQuerys.Add("ALTER TABLE diariodetalle ALTER column v_Analisis varchar(1150)");
                                listaQuerys.Add("drop table planillavaloresrentaquinta");
                                listaQuerys.Add(@"CREATE TABLE  [planillavaloresrentaquinta](
										[i_Id] [int] IDENTITY(1,1) NOT NULL,
										[v_Periodo] [nvarchar](4) NULL,
										[i_Tope1] [int] NULL,
										[d_Porcentaje1] [decimal](7, 2) NULL,
										[i_Tope2] [int] NULL,
										[d_Porcentaje2] [decimal](7, 2) NULL,
										[i_Tope3] [int] NULL,
										[d_Porcentaje3] [decimal](7, 2) NULL,
										[i_Tope4] [int] NULL,
										[d_Porcentaje4] [decimal](7, 2) NULL,
										[d_Porcentaje4Superior] [decimal](7, 2) NULL,
										[i_Eliminado] [int] NULL,
										[i_InsertaIdUsuario] [int] NULL,
										[t_InsertaFecha] [datetime] NULL,
										[i_ActualizaIdUsuario] [int] NULL,
										[t_ActualizaFecha] [datetime] NULL,
									 CONSTRAINT [PK_planillavaloresrentaquinta] PRIMARY KEY CLUSTERED 
									(
										[i_Id] ASC
									)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF,
										   ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
									) ON [PRIMARY]");

                                listaQuerys.Add(@"CREATE TABLE  [planillavaloresproyeccionquinta](
									[i_Id] [int] IDENTITY(1,1) NOT NULL,
									[v_Periodo] [nchar](4) NULL,
									[i_IdMes] [int] NULL,
									[i_MesesProyectados] [int] NULL,
									[i_GratificacionesProyectadas] [int] NULL,  
									[i_Fraccionamiento] [int] NULL,
									[i_Eliminado] [int] NULL,
									CONSTRAINT [PK_planillavaloresproyeccionquinta] PRIMARY KEY CLUSTERED 
								(
									[i_Id] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, 
									IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");
                                break;
                            #endregion

                            #region Actualizacion 159
                            case 159:
                                listaQuerys.Add(@"ALTER TABLE dbo.planillavaloresrentaquinta ADD
												v_IdConceptoPlanillaRenta5T varchar(16) NULL,
												v_IdConceptoPlanillaGratificacion varchar(16) NULL");

                                listaQuerys.Add(@"ALTER TABLE dbo.planillavaloresrentaquinta ADD CONSTRAINT
																fk_valoresrentaquinta_renta5t FOREIGN KEY
																(
																v_IdConceptoPlanillaRenta5T
																) REFERENCES dbo.planillaconceptos
																(
																v_IdConceptoPlanilla
																) ON UPDATE  NO ACTION 
																 ON DELETE  NO ACTION ");

                                listaQuerys.Add(@"ALTER TABLE dbo.planillavaloresrentaquinta ADD CONSTRAINT
												fk_planillavaloresrentaquinta_gratificaciones FOREIGN KEY
												(
												v_IdConceptoPlanillaGratificacion
												) REFERENCES dbo.planillaconceptos
												(
												v_IdConceptoPlanilla
												) ON UPDATE  NO ACTION 
												 ON DELETE  NO ACTION ");

                                break;
                            #endregion

                            #region Actualizacion 160
                            case 160:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_IdPlanillaConceptoTardanzas varchar(16);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_IdPlanillaConceptoFaltas varchar(16);");

                                break;
                            #endregion

                            #region Actualizacion 161
                            case 161:
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId , i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (0, 162, N'HORAS EXTRAS PARAMETROS', N'', NULL, NULL, NULL, 0, 0, 1, N'2017-04-03 13:08:15.585', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (162, 1, N'25%', N'1.25', N'', NULL, 0, 0, 0, 1, N'2017-04-03 13:10:11.540', NULL, NULL, NULL, NULL, N'');");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												 VALUES (162, 2, N'35%', N'1.35', N'', NULL, 0, 0, 0, 1, N'2017-04-03 13:10:23.663', NULL, NULL, NULL, NULL, N'');");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (162, 3, N'50%', N'1.50', N'', NULL, 0, 0, 0, 1, N'2017-04-03 13:10:49.444', NULL, NULL, NULL, NULL, N'');");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (162, 4, N'200%', N'2.00', N'', NULL, 0, 0, 0, 1, N'2017-04-03 13:11:24.067', NULL, NULL, NULL, NULL, N'');");

                                listaQuerys.Add(@"ALTER TABLE planillavariableshorasextras DROP COLUMN i_IdPlanillaHorasExtras;");

                                listaQuerys.Add(@"ALTER TABLE planillavariableshorasextras ADD i_IdTipoHorasExtras int;");
                                break;
                            #endregion

                            #region Actualizacion 163
                            case 163:
                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (0, 163, N'HORAS EXTRAS TIPOS', N'', NULL, NULL, NULL, 0, 0, 1, N'2017-04-03 14:46:09.402', 1, N'2017-04-03 14:46:24.762', NULL, NULL, NULL);");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (163, 1, N'DIURNO', N'0', N'', NULL, 0, 0, 0, 1, N'2017-04-03 14:46:39.465', NULL, NULL, NULL, NULL, N'');");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (163, 2, N'NOCTURNO', N'0.35', N'', NULL, 0, 0, 0, 1, N'2017-04-03 15:22:46.561', NULL, NULL, NULL, NULL, N'');");

                                listaQuerys.Add(@"INSERT INTO dbo.datahierarchy ( i_GroupId ,  i_ItemId ,  v_Value1 ,  v_Value2 ,  v_Field ,  i_ParentItemId ,  i_Header ,  i_Sort ,  i_IsDeleted ,  i_InsertUserId ,  d_InsertDate ,  i_UpdateUserId ,  d_UpdateDate ,  i_SyncStatusId ,  i_RecordStatusId ,  v_Value4 )
												VALUES (163, 3, N'DOMINGO/FERIADO', N'2.0', N'', NULL, 0, 0, 0, 1, N'2017-04-03 15:23:01.562', NULL, NULL, NULL, NULL, N'');");
                                break;
                            #endregion

                            #region Actualizacion 164
                            case 164:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_IdRepresentanteLegal nvarchar(16);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroLeyCuartaCategoria nvarchar(100);");
                                break;
                            #endregion

                            #region Actualizacion 165
                            case 165:
                                listaQuerys.Add(@"delete from datahierarchy where i_GroupId = 162;");
                                listaQuerys.Add(@"delete from datahierarchy where i_GroupId = 163;");
                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
											  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 1, N'25% DIURNO', N'1.25', N'', NULL, 1, 0, 0, 1, '20170403 13:10:11.540', 1, '20170403 17:49:17.231', NULL, NULL, N'N001-PN000000064')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
												  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
												VALUES (162, 2, N'35% DIURNO', N'1.35', N'', NULL, 1, 0, 0, 1, '20170403 13:10:23.663', 1, '20170403 17:59:37.821', NULL, NULL, N'N001-PN000000062')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
											  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (163, 3, N'NOLAB/DIURNO', N'2.0', N'', NULL, 0, 0, 0, 1, '20170403 15:23:01.562', 1, '20170404 12:07:38.517', NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
											  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (163, 4, N'NOLAB/NOCTURNO', N'2.7', N'', NULL, 0, 0, 0, 1, '20170404 12:07:49.998', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
												([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 13, N'25% NOLAB/DIURNO', N'1.25', NULL, NULL, 3, NULL, 0, 1, '20170404 13:46:54.728', NULL, NULL, NULL, NULL, N'N001-PN000000064')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
											  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (163, 1, N'DIURNO', N'1', N'', NULL, 0, 0, 0, 1, '20170403 14:46:39.465', 1, '20170404 11:51:53.355', NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
												  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
												VALUES (163, 2, N'NOCTURNO', N'1.35', N'', NULL, 0, 0, 0, 1, '20170403 15:22:46.561', 1, '20170404 11:51:57.641', NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
											  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 9, N'25% NOLAB/NOCTURNO', N'1.25', NULL, NULL, 4, NULL, 0, 1, '20170404 12:50:59.108', NULL, NULL, NULL, NULL, N'N001-PN000000064')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
												([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 10, N'25% NOCTURNO', N'1.25', NULL, NULL, 2, NULL, 0, 1, '20170404 13:44:15.008', NULL, NULL, NULL, NULL, N'N001-PN000000064')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
												([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 11, N'35% NOCTURNO', N'1.35', NULL, NULL, 2, NULL, 0, 1, '20170404 13:44:38.632', NULL, NULL, NULL, NULL, N'N001-PN000000062')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
											  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 12, N'35% NOLAB/NOCTURNO', N'1.35', NULL, NULL, 4, NULL, 0, 1, '20170404 13:45:15.682', NULL, NULL, NULL, NULL, N'N001-PN000000062')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
												([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
											VALUES (162, 14, N'35% NOLAB/DIURNO', N'1.35', NULL, NULL, 3, NULL, 0, 1, '20170404 13:47:25.215', NULL, NULL, NULL, NULL, N'N001-PN000000062')");

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_UsaDominicalCalculoDescuento int;");
                                break;
                            #endregion

                            #region Actualizacion 166
                            case 166:
                                listaQuerys.Add("ALTER TABLE producto ADD v_NroOrdenProduccion nvarchar(16);");
                                listaQuerys.Add("update configuracionempresa  set v_NroLeyCuartaCategoria = '(Articulo 45° del D.S N° 122-94-EF, Reglamento de la Ley del IR)'");

                                break;
                            #endregion

                            #region Actualizacion 167
                            case 167:
                                listaQuerys.Add(@"CREATE TABLE  [flujoefectivoconceptos](
								   [i_IdConceptoFlujo] [int] IDENTITY(1,1) NOT NULL,
								   [v_Descripcion] [nvarchar](150) NULL,
								 CONSTRAINT [PK_flujoefectivoconceptos] PRIMARY KEY CLUSTERED 
								(
								   [i_IdConceptoFlujo] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"CREATE TABLE  [flujoefectivo](
									[i_IdFlujoEfectivo] [int] IDENTITY(1,1) NOT NULL,
									[v_PeriodoProceso] [nvarchar](4) NULL,
									[v_CtaMayor] [nvarchar](2) NULL,
									[v_DescripcionCuenta] [nvarchar](120) NULL,
									[d_BalancePeriodoActual] [decimal](15, 2) NULL,
									[d_BalancePeriodoAnterior] [decimal](15, 2) NULL,
									[d_Aumento] [decimal](15, 2) NULL,
									[d_Disminucion] [decimal](15, 2) NULL,
									[d_AjusteDebe] [decimal](15, 2) NULL,
									[d_AjusteHaber] [decimal](15, 2) NULL,
									[d_Operacion] [decimal](15, 2) NULL,
									[d_Inversion] [decimal](15, 2) NULL,
									[d_Financiamiento] [decimal](15, 2) NULL,
									[d_MetodoDirecto] [decimal](15, 2) NULL,
									[i_NaturalezaCuenta] [int] NULL,
									[i_NroAsiento] [int] NULL,
									CONSTRAINT [PK_flujoefectivo] PRIMARY KEY CLUSTERED 
								(
									[i_IdFlujoEfectivo] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"CREATE TABLE  [flujoefectivoasientoajuste](
									[i_IdAsientoAjuste] [int] IDENTITY(1,1) NOT NULL,
									[i_IdFlujoEfectivo] [int] NULL,
									[i_NroAsiento] [int] NULL,
									[i_IdMoneda] [int] NULL,
									[d_TipoCambio] [decimal](10, 3) NULL,
									[v_Glosa] [nvarchar](150) NULL,
									CONSTRAINT [PK_flujoefectivoasientoajuste] PRIMARY KEY CLUSTERED 
								(
									[i_IdAsientoAjuste] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"CREATE TABLE  [flujoefectivoconfiguracion](
									[i_Id] [int] IDENTITY(1,1) NOT NULL,
									[i_IdConceptoFlujo] [int] NULL,
									[v_Periodo] [nvarchar](4) NULL,
									[v_NroCuenta] [nvarchar](20) NULL,
									[v_NroCuentaPasivos] [nvarchar](20) NULL,
									[i_IdTipoCuenta] [int] NULL,
									[i_IdTipoActividad] [int] NULL,
									[i_EsCuentaActivo] [int] NULL,
									CONSTRAINT [PK_flujoefectivoconfiguracion] PRIMARY KEY CLUSTERED 
								(
									[i_Id] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"CREATE TABLE  [flujoefectivoasientoajustedetalle](
									[i_IdAsientoAjusteDetalle] [int] IDENTITY(1,1) NOT NULL,
									[i_IdAsientoAjuste] [int] NULL,
									[v_NroCuenta] [nvarchar](2) NULL,
									[v_Naturaleza] [nvarchar](1) NULL,
									[d_Importe] [decimal](15, 2) NULL,
									[d_Cambio] [decimal](15, 2) NULL,
									CONSTRAINT [PK_flujoefectivoasientoajustedetalle] PRIMARY KEY CLUSTERED 
								(
									[i_IdAsientoAjusteDetalle] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"ALTER TABLE  [flujoefectivoasientoajuste]  WITH CHECK ADD  CONSTRAINT [FK_flujoefectivoasientoajuste_flujoefectivo] FOREIGN KEY([i_IdFlujoEfectivo])
									REFERENCES  [flujoefectivo] ([i_IdFlujoEfectivo])");

                                listaQuerys.Add(@"ALTER TABLE  [flujoefectivoasientoajuste] CHECK CONSTRAINT [FK_flujoefectivoasientoajuste_flujoefectivo]");

                                listaQuerys.Add(@"ALTER TABLE  [flujoefectivoasientoajustedetalle]  WITH CHECK ADD  CONSTRAINT [FK_flujoefectivoasientoajustedetalle_flujoefectivoasientoajuste] FOREIGN KEY([i_IdAsientoAjuste])
									REFERENCES  [flujoefectivoasientoajuste] ([i_IdAsientoAjuste])");

                                listaQuerys.Add(@"ALTER TABLE  [flujoefectivoasientoajustedetalle] CHECK CONSTRAINT [FK_flujoefectivoasientoajustedetalle_flujoefectivoasientoajuste]");

                                listaQuerys.Add(@"ALTER TABLE  [flujoefectivoconfiguracion]  WITH CHECK ADD  CONSTRAINT [FK_flujoefectivoconfiguracion_flujoefectivoconceptos] FOREIGN KEY([i_IdConceptoFlujo])
									REFERENCES  [flujoefectivoconceptos] ([i_IdConceptoFlujo])");

                                listaQuerys.Add(@"ALTER TABLE  [flujoefectivoconfiguracion] CHECK CONSTRAINT [FK_flujoefectivoconfiguracion_flujoefectivoconceptos]");

                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (0, 164, N'TIPO DE CUENTA', N'', NULL, NULL, NULL, 0, 0, 1, '20170418 13:56:26.359', NULL, NULL, NULL, NULL, NULL)");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
									([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (0, 165, N'ACTIVIDAD CUENTA', N'', NULL, NULL, NULL, 0, 0, 1, '20170418 13:56:42.139', NULL, NULL, NULL, NULL, NULL)");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
									  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
									VALUES (164, 1, N'INVENTARIO', N'', N'', NULL, 0, 0, 0, 1, '20170418 13:56:52.664', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (165, 1, N'ACTIVIDAD DE OPERACIÓN', N'', N'', NULL, 0, 0, 0, 1, '20170418 13:57:09.250', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (165, 2, N'ACTIVIDAD DE INVERSIÓN', N'', N'', NULL, 0, 0, 0, 1, '20170418 13:57:15.805', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO  [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (165, 3, N'ACTIVIDAD DE FINANCIAMIENTO', N'', N'', NULL, 0, 0, 0, 1, '20170418 13:57:28.341', NULL, NULL, NULL, NULL, N'')");
                                break;
                            #endregion

                            #region Actualizacion 168
                            case 168:


                                listaQuerys.Add(@"create table productorecetasalida
												(
													i_IdRecetaSalida int identity primary key,
													v_IdProductoTributo varchar(16),
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													CONSTRAINT fk_productodetalle_v_IdProductoTributo FOREIGN KEY (v_IdProductoTributo) REFERENCES productodetalle (v_IdProductoDetalle),
													
												);");


                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (0, 166, 'TIPO TRIBUTO- VENTAS EXPORTACION', 0, 1, '2017-04-21');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (166, 1, 'FLETE', 0, 1, '2017-04-21');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (166, 2, 'SEGURO', 0, 1, '2017-04-21');");

                                listaQuerys.Add("ALTER TABLE venta ADD   d_SeguroTotal  decimal(20,5);");
                                listaQuerys.Add("ALTER TABLE venta ADD   d_FleteTotal  decimal(20,5);");
                                listaQuerys.Add("ALTER TABLE venta ADD   d_CantidaTotal  decimal(20,5);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD   d_PrecioPactado  decimal(20,5);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD   d_SeguroXProducto  decimal(20,5);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD   d_FleteXProducto  decimal(20,5);");
                                listaQuerys.Add("ALTER TABLE producto ADD i_IdTipoTributo int;");
                                listaQuerys.Add("ALTER TABLE productorecetasalida ADD   v_IdProductoExportacion  varchar(16);");

                                break;
                            #endregion

                            #region Actualizacion 169

                            case 169:
                                listaQuerys.Add(@"create table relaciontributosexportacion
												(
													i_Id  [int] IDENTITY(1,1) primary key,
													i_IdTipoPagoExportacion int,
													i_IdRecetaExportacion int													
												);");

                                listaQuerys.Add(@"INSERT INTO [dbo].[relaciontributosexportacion]
													([i_IdTipoPagoExportacion], [i_IdRecetaExportacion])
												VALUES (2, 1)");

                                listaQuerys.Add(@"INSERT INTO [dbo].[relaciontributosexportacion]
													([i_IdTipoPagoExportacion], [i_IdRecetaExportacion])
												VALUES (3, 1)");

                                listaQuerys.Add(@"INSERT INTO [dbo].[relaciontributosexportacion]
													([i_IdTipoPagoExportacion], [i_IdRecetaExportacion])
												VALUES (3, 2)");
                                break;

                            #endregion

                            #region Actualizacion 170
                            case 170:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_IdProductoDetalleFlete varchar(16);");

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_IdProductoDetalleSeguro varchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 171
                            case 171:

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_GenerarNotaSalidaDesdeVentaUltimoDiaMes int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_TipoVentaVentas int;");
                                listaQuerys.Add("ALTER TABLE venta ADD v_NroBL varchar(40);");
                                listaQuerys.Add("ALTER TABLE venta ADD t_FechaPagoBL datetime");
                                listaQuerys.Add("ALTER TABLE venta ADD v_Contenedor varchar(40)");
                                listaQuerys.Add("ALTER TABLE venta ADD v_Banco varchar(40)");
                                listaQuerys.Add("ALTER TABLE venta ADD v_Naviera varchar(40)");
                                break;
                            #endregion

                            #region Actualizacion 172
                            case 172:
                                listaQuerys.Add(@"CREATE TABLE [dbo].[flujoefectivocabecera](
								   [i_IdFlujoEfectivoCabecera] [int] IDENTITY(1,1) NOT NULL,
								   [v_PeriodoProceso] [nchar](4) NULL,
								 CONSTRAINT [PK_flujoefectivocabecera] PRIMARY KEY CLUSTERED 
								(
								   [i_IdFlujoEfectivoCabecera] ASC
								)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
								) ON [PRIMARY]");

                                listaQuerys.Add(@"ALTER TABLE flujoefectivo ADD i_IdFlujoEfectivoCabecera integer;");
                                listaQuerys.Add(@"ALTER TABLE flujoefectivo ADD CONSTRAINT FK_flujoefectivo_flujoefectivocabecera FOREIGN KEY (i_IdFlujoEfectivoCabecera) REFERENCES flujoefectivocabecera (i_IdFlujoEfectivoCabecera) ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                listaQuerys.Add(@"ALTER TABLE flujoefectivoasientoajuste DROP CONSTRAINT FK_flujoefectivoasientoajuste_flujoefectivo;");
                                listaQuerys.Add(@"EXEC sp_rename 'flujoefectivoasientoajuste.i_IdFlujoEfectivo', 'i_IdFlujoEfectivoCabecera', 'COLUMN'");
                                listaQuerys.Add(@"ALTER TABLE flujoefectivoasientoajuste ADD CONSTRAINT fk_flujoefectivoasientoajuste_flujoefectivo FOREIGN KEY (i_IdFlujoEfectivoCabecera) REFERENCES flujoefectivocabecera (i_IdFlujoEfectivoCabecera) ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                break;
                            #endregion

                            #region Actualizacion 173
                            case 173:
                                listaQuerys.Add("ALTER TABLE flujoefectivo DROP COLUMN v_PeriodoProceso;");
                                break;
                            #endregion

                            #region Actualizacion 174
                            case 174:
                                listaQuerys.Add(@"create table relacionusuariocliente
												(
													i_IdRelacionusuariocliente int identity primary key,
													i_SystemUser int,
													v_IdCliente varchar(16),
													i_IdDireccionCliente int,
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,

													CONSTRAINT fk_relacionusuariocliente_v_IdCliente FOREIGN KEY (v_IdCliente) REFERENCES cliente (v_IdCliente),
												   
												);");
                                break;
                            #endregion

                            #region Actualizacion 175
                            case 175:
                                listaQuerys.Add("ALTER TABLE flujoefectivo DROP COLUMN i_NroAsiento;");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD i_NroAsientoD INT;");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD i_NroAsientoH INT;");
                                break;
                            #endregion

                            #region Actualizacion 176
                            case 176:
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD d_Origen decimal(20,2);");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD d_Aplicacion decimal(20,2);");
                                break;
                            #endregion

                            #region Actualizacion 177
                            case 177:
                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de venta de bienes o servicios e ingresos operacionales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de regalías, honorarios, comisiones y otros');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de intereses y dividendos recibidos');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros cobros de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pago a proveedores de bienes y servicios');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pago de remuneraciones y beneficios sociales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pago de tributos');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pago de intereses y rendimientos');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros pagos de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Operación');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de venta de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de venta de inmuebles, maquinaria y equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de venta de activos intangibles');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros cobros de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pagos por compra de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pagos por compra de inmuebles, maquinaria y equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pagos por compra de activos intangibles');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros pagos de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Inversión');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de emisión de acciones o nuevos aportes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cobranza de recursos obtenidos por emisión de valores u otras obligaciones de largo plazo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros cobros de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pagos de amortización o cancelación de valores u otras obligaciones de largo plazo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pago de dividendos y otras distribuciones');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros pagos de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Financiamiento');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Saldo Efectivo y Equivalente de Efectivo al Inicio del Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Saldo Efectivo y Equivalente de Efectivo al Finalizar el Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Conciliación del Resultado Neto con el Efectivo y Equivalente de Efectivo proveniente de las Actividades de  Operación');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Ajustes a la Utilidad (Pérdida) del Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Depreciación y amortización del período');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Provisión Beneficios Sociales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Provisiones Diversas');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pérdida en venta de inmuebles, maquinaria y Equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pérdida en venta de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Pérdida por activos monetarios no corrientes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Otros');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Ajustes a la Utilidad (Pérdida) del Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Utilidad en venta de inmuebles, maquinaria y equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Utilidad en venta de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Ganancia por pasivos monetarios no corrientes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Cargos y Abonos por cambios netos en el Activo y Pasivo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'(Aumento) Disminución de Cuentas por Cobrar Comerciales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'(Aumento) Disminución de Cuentas por Cobrar Vinculadas');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'(Aumento) Disminución de Otras Cuentas por Cobrar');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'(Aumento) Disminución en Existencias');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'(Aumento) Disminución en Gastos Pagados por Anticipado');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) de Cuentas por Pagar Comerciales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) de Cuentas por Pagar Vinculadas');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) de Otras Cuentas por Pagar');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de la Actividad de Operación');");
                                break;
                            #endregion

                            #region Actualizacion  178
                            case 178:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdCondicionPagoPedido int");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_GlosaPedido varchar(200)");
                                break;
                            #endregion

                            #region Actualizacion  179
                            case 179:
                                listaQuerys.Add(@"ALTER TABLE compra ADD i_AplicaRetencion int;");
                                listaQuerys.Add(@"ALTER TABLE compra ADD v_DocumentoPercepcion varchar(16);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD v_SeriePercepcion varchar(4);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD v_CorrelativoPercepcion varchar(8);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD t_FechaPercepcion datetime;");
                                listaQuerys.Add(@"ALTER TABLE compra ADD d_ImporteCalculoPercepcion decimal(20,2);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD d_PorcentajePercepcion decimal(5,2);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD d_Percepcion decimal(20,2);");
                                listaQuerys.Add("INSERT INTO documento (\"i_CodigoDocumento\", \"v_Nombre\", \"v_Siglas\", \"i_UsadoDocumentoContable\", \"i_UsadoDocumentoInterno\", \"i_UsadoDocumentoInverso\", \"i_UsadoCompras\", \"i_UsadoContabilidad\", \"i_UsadoLibroDiario\", \"i_UsadoTesoreria\", \"i_UsadoVentas\", \"i_RequiereSerieNumero\", \"i_UsadoRendicionCuentas\", \"i_Naturaleza\", \"i_IdFormaPago\", \"v_NroCuenta\", \"v_provimp_3i\", \"i_Destino\", \"i_UsadoPedidoCotizacion\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_DescontarStock\", \"i_OperacionTransitoria\", \"v_NroContraCuenta\") " +
                                    "VALUES (40, N'DOCUMENTO PERCEPCIÓN', N'RTC', 1, 0, NULL, 0, 1, 1, 0, 0, 1, 0, 2, NULL, N'4011401', N'', NULL, 0, 0, 1, N'2017-05-22 15:09:46.638', NULL, NULL, 0, 0, NULL);");
                                break;
                            #endregion

                            #region Actualizacion  180
                            case 180:
                                listaQuerys.Add("ALTER TABLE compra ADD i_IdTipoDocumentoPercepcion int;");
                                break;
                            #endregion

                            #region Actualizacion  181
                            case 181:
                                listaQuerys.Add("delete from documento where \"i_CodigoDocumento\" = 40;");
                                listaQuerys.Add("INSERT INTO documento (\"i_CodigoDocumento\", \"v_Nombre\", \"v_Siglas\", \"i_UsadoDocumentoContable\", \"i_UsadoDocumentoInterno\", \"i_UsadoDocumentoInverso\", \"i_UsadoCompras\", \"i_UsadoContabilidad\", \"i_UsadoLibroDiario\", \"i_UsadoTesoreria\", \"i_UsadoVentas\", \"i_RequiereSerieNumero\", \"i_UsadoRendicionCuentas\", \"i_Naturaleza\", \"i_IdFormaPago\", \"v_NroCuenta\", \"v_provimp_3i\", \"i_Destino\", \"i_UsadoPedidoCotizacion\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_DescontarStock\", \"i_OperacionTransitoria\", \"v_NroContraCuenta\") " +
                                "VALUES (40, N'DOCUMENTO PERCEPCIÓN', N'RTC', 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 2, NULL, N'4011301', N'', NULL, 0, 0, 1, N'2017-05-22 15:09:46.638', NULL, NULL, 0, 0, NULL);");
                                break;
                            #endregion

                            #region Actualizacion  182
                            case 182:
                                listaQuerys.Add("update documento set \"i_UsadoContabilidad\" = 0, \"i_UsadoLibroDiario\" = 0, \"i_UsadoTesoreria\" = 0 where \"i_CodigoDocumento\" = 8");
                                break;
                            #endregion

                            #region Actualizacion 183
                            case 183:
                                listaQuerys.Add("ALTER TABLE producto ADD d_Utilidad decimal(18,4);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_VisualizarBusquedaProductos int");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_PermiteIncluirPreciosCeroPedido int");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_SeUsaraSoloUnaListaPrecioEmpresa int");
                                break;
                            #endregion

                            #region Actualizacion 184
                            case 184:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (0, 167, 'CODIGO ETIQUETAS AVIOS', 0, 1, '2017-05-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (i_GroupId, i_ItemId, v_Value1, i_IsDeleted, i_InsertUserId, d_InsertDate)
												VALUES (0, 168, 'CODIGO HANTAG AVIOS', 0, 1, '2017-05-27');");
                                break;
                            #endregion

                            #region Actualizacion 185
                            case 185:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_IdCondicionPagoVenta int");
                                break;
                            #endregion

                            #region Actualizacion 186
                            case 186:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaCobranzaRedondeoPerdida varchar(16)");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD v_NroCuentaCobranzaRedondeoGanancia varchar(16)");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD d_Redondeo  decimal (18,2)");



                                break;

                            #endregion

                            #region Actualizacion 187
                            case 187:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD i_IdCentroCosto varchar(4)");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_CambiarAlmacenVentasDesdeVendedor int");
                                listaQuerys.Add("ALTER TABLE vendedor ADD i_IdAlmacen int");

                                break;
                            #endregion

                            #region Actualizacion 188
                            case 188:
                                listaQuerys.Add("ALTER TABLE venta ADD i_AplicaPercepcion int;");
                                listaQuerys.Add("ALTER TABLE venta ADD i_ClienteEsAgente int;");
                                listaQuerys.Add("ALTER TABLE venta ADD d_PorcentajePercepcion int;");
                                break;
                            #endregion

                            #region Actualizacion 189
                            case 189:
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (0, 169,'TIPO VERIFICACION PEDIDO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (169, 1,'POR ATENDER','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (169, 2,'ATENDIDO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate ) values (169, 3,'REDIRECCIONADO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("ALTER TABLE pedido ADD   i_IdTipoVerificacion int;");

                                break;

                            #endregion

                            #region Actualizacion 190
                            case 190:


                                listaQuerys.Add(@"create table configuracionbalances
												(
													i_IdConfiguracionBalance int identity primary key,
													v_TipoBalance varchar(4),
													v_Codigo varchar(10),
													v_Nombre varchar(200),
													i_IdTipoElemento int,
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime
												);");

                                listaQuerys.Add("ALTER TABLE venta ADD i_ItemsAfectosPercepcion INT;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_EmpresaAfectaPercepcionVenta INT;");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD v_CodigoBalanceFuncion varchar(10);");
                                break;
                            #endregion

                            #region Actualizacion 191
                            case 191:
                                listaQuerys.Add(@"ALTER TABLE venta ALTER column d_PorcentajePercepcion decimal(16,2)");
                                break;
                            #endregion

                            #region Actualizacion 192
                            case 192:
                                listaQuerys.Add(@"ALTER TABLE configuracionbalances ADD i_TipoOperacion int");
                                listaQuerys.Add(@"ALTER TABLE configuracionbalances ADD  v_NombreGrupo varchar(200)");
                                break;

                            #endregion

                            #region Actualizacion 193

                            case 193:

                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G2','UTILIDAD OPERATIVA',null,0,1,'2017-06-30',null,null,-1,'UTILIDAD OPERATIVA')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G3','RESULTADO ANTES DEL IMP. RENTA',null,0,1,'2017-06-30',null,null,-1,'RESULTADO ANTES DEL IMP. RENTA')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G4','UTILIDAD (PERDIDA) NETA  DE ACT. NOT.',null,0,1,'2017-06-30',null,null ,-1,'UTILIDAD (PERDIDA) NETA  DE ACT. NOT.')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G5','UTLIDAD (PERDIDA) DEL EJERCICIO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G11','INGRESOS OPERACIONALES',null,0,1,'2017-06-30',null,null,-1,'TOTAL INGRESOS BRUTOS')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G112','OTROS INGRESOS OPERACIONALES',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G111','VENTAS NETAS  (INGRESOS OPERACIONALES)',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G122','OTROS COSTOS OPERACIONALES',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G21','UTILIDAD OPERATIVA',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G213','GANANCIA (PERDIDA) DE VENTA ACT.',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G214','OTROS INGRESOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G215','OTROS GASTOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G31','RESULTADO ANTES DEL IMP. RENTA',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G312','GASTOS FINANCIEROS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G313','PARTICIPACIÓN EN LOS RESULTADOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G314','GANANCIA (PERDIDA) POR INST. FINANCIERA',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G41','UTILIDAD (PERDIDA) NETA  DE ACT. NOT.',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G411','PARTICIPACION DE LOS TRABAJADORES',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G412','IMPUESTO A LA RENTA',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G51','UTLIDAD (PERDIDA) DEL EJERCICIO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G511','INGRESO (GASTO ) DE OPERACIONES DISCONTINUAS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G12','COSTO DE VENTAS',null,0,1,'2017-06-30',null,null,-1,'TOTAL COSTOS OPERACIONALES')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G311','INGRESOS FINANCIEROS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G1','UTILIDAD BRUTA',null,0,1,'2017-06-30',null,null,-1,'UTILIDAD BRUTA')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G121','COSTO DE VENTAS (OPERACIONALES)',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G211','GASTOS DE VENTAS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('1','G212','GASTOS DE ADMINISTRACIÓN',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G121','INVERSIONES EN VALORES',null,0,1,'2017-07-01',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G1','ACTIVO',null,0,1,'2017-07-01',1,'2017-07-01',null,'TOTAL ACTIVO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G2','CUENTAS ORDEN',null,0,1,'2017-07-01',1,'2017-07-01',null,'CUENTAS ORDEN')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G111','CAJA Y BANCOS',null,0,1,'2017-07-01',1,'2017-07-01',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G112','CUENTAS POR COBRAR COMERCIALES',null,0,1,'2017-07-01',1,'2017-07-01',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G31','PASIVO CORRIENTE',null,0,1,'2017-07-01',null,null,null,'TOTAL PASIVO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G4','PATRIMONIO',null,0,1,'2017-07-01',null,null,null,'TOTAL PATRIMONIO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G41','PATRIMONIO',null,0,1,'2017-07-01',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G122','INMUEBLE MAQUINARIA Y EQUIPO',null,0,1,'2017-07-01',1,'2017-07-01',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G3','PASIVO ',null,0,1,'2017-07-01',1,'2017-07-01',null,'TOTAL PASIVO ')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G32','PASIVO NO CORRIENTE',null,0,1,'2017-07-01',1,'2017-07-01',null,'TOTAL PASIVO NO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G11','ACTIVO CORRIENTE',null,0,1,'2017-07-01',1,'2017-07-01 ',null,'TOTAL ACTIVO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G12','ACTIVO NO CORRIENTE',null,0,1,'2017-07-01',1,'2017-07-01',null,'TOTAL ACTIVO NO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G311','SOBRECARGOS BANCARIOS',null,0,1,'2017-07-01',1,'2017-07-01',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G411','CAPITAL SOCIAL',null,0,1,'2017-07-01',1,'2017-07-01',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G321','CUENTAS POR PAGAR CORRIENTES',null,0,1,'2017-07-01',1,'2017-07-01',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G113','CUENTAS POR COBRAR A ACCIONISTA Y PERSONAL',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G114','OTRAS CUENTAS POR COBRAR',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G115','EXISTENCIAS',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G116','CARGAS DIFERIDAS',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G123','INTANGIBLES',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G124','DEPRECIACIÓN ACUMULADA',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G125','OTRAS CUENTAS DEL ACTIVO',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G312','TRIBUTOS POR PAGAR',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G313','CUENTAS POR PAGAR COMERCIALES',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G314','OTRAS CUENTAS POR PAGAR DIVERSAS',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G322','COMPENSACIÓN POR TIEMPO DE SERVICIO',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G323','OTRAS CUENTAS POR PAGAR',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G412','CAPITAL ADICIONAL',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G413','EXCEDENTE DE REEVALUACIÓN',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G414','RESERVA  LEGAL',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G415','RESULTADOS ACUMULADOS',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('2','G416','RESULTADOS DEL EJERCICIO',null,0,1,'2017-07-02',null,null,null,null)");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD  v_CodigoSituacionFinaciera varchar(10);");
                                break;

                            #endregion

                            #region Actualizacion 194
                            case 194:
                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (0, 170, N'TIPO DE ANEXO CONTABLE', N'', NULL, NULL, NULL, 0, 0, 1, '20170703 13:13:58.793', NULL, NULL, NULL, NULL, NULL)");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 1, N'ACCIONISTAS', N'A', N'', NULL, 0, 0, 0, 1, '20170703 13:14:28.272', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 2, N'ENTIDAD FINANCIERA', N'B', N'', NULL, 0, 0, 0, 1, '20170703 13:20:50.937', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 3, N'FONDOS FIJOS', N'F', N'', NULL, 0, 0, 0, 1, '20170703 13:21:01.643', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 4, N'CONCEPTOS PATRIMONIALES', N'P', N'', NULL, 0, 0, 0, 1, '20170703 13:21:13.950', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
									([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 5, N'VARIOS', N'V', N'', NULL, 0, 0, 0, 1, '20170703 13:21:21.366', NULL, NULL, NULL, NULL, N'')");

                                break;

                            #endregion

                            #region Actualizacion 195
                            case 195:
                                listaQuerys.Add(@"DELETE FROM datahierarchy
								where i_GroupId = 170");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 1, N'ACCIONISTAS', N'A', N'', NULL, 0, 0, 0, 1, '20170703 13:14:28.272', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 2, N'ENTIDAD FINANCIERA', N'B', N'', NULL, 0, 0, 0, 1, '20170703 13:20:50.937', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
									([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 3, N'FONDOS FIJOS', N'F', N'', NULL, 0, 0, 0, 1, '20170703 13:21:01.643', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 4, N'CONCEPTOS PATRIMONIALES', N'P', N'', NULL, 0, 0, 0, 1, '20170703 13:21:13.950', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 6, N'VENDEDORES', N'E', N'', NULL, 0, 0, 0, 1, '20170704 13:15:25.474', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 7, N'PROVEEDORES', N'V', N'', NULL, 0, 0, 0, 1, '20170704 13:15:44.343', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 5, N'VARIOS', N'S', N'', NULL, 0, 0, 0, 1, '20170703 13:21:21.366', 1, '20170704 13:16:04.763', NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
									([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 8, N'CLIENTES', N'C', N'', NULL, 0, 0, 0, 1, '20170704 13:16:45.965', NULL, NULL, NULL, NULL, N'')");


                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
								  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
								VALUES (170, 9, N'TRABAJADORES', N'T', N'', NULL, 0, 0, 0, 1, '20170704 13:16:55.389', NULL, NULL, NULL, NULL, N'')");
                                break;

                            #endregion

                            #region Actualizacion 196
                            case 196:
                                listaQuerys.Add(@"ALTER TABLE compra ADD i_AplicaRectificacion int;");
                                listaQuerys.Add(@"ALTER TABLE compra ADD v_RectificacionPeriodo varchar(4);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD v_RectificacionMes varchar(4);");
                                listaQuerys.Add(@"ALTER TABLE compra ADD v_RectificacionCorrelativo varchar(8);");


                                break;


                            #endregion

                            #region Actualizacion 197
                            case 197:
                                listaQuerys.Add("ALTER TABLE compra DROP COLUMN v_RectificacionPeriodo");
                                listaQuerys.Add("ALTER TABLE compra DROP COLUMN v_RectificacionMes");
                                listaQuerys.Add("ALTER TABLE compra DROP COLUMN v_RectificacionCorrelativo");
                                listaQuerys.Add("ALTER TABLE compra ADD  t_FechaCorreccionPle datetime;");
                                break;
                            #endregion

                            #region Actualizacion 198

                            case 198:

                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G1','PRODUCCIÓN TOTAL ',null,0,1,'2017-06-30',null,null,-1,'TOTAL PRODUCCIÓN TOTAL')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G11','MARGEN COMERCIAL',null,0,1,'2017-06-30',null,null,-1,'TOTAL MARGEN COMERCIAL')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G111','VENTAS NETAS  DE MERCADERIAS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G112','COMPRAS DE MERCADERIAS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G113','VARIACIÓN DE MERCADERIAS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G12','PRODUCCION DEL EJERCICIO',null,0,1,'2017-06-30',null,null,-1,'PRODUCCION DEL EJERCICIO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G121','VENTAS NETAS DE PRODUCTOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G122','PRODUCCIÓN ALMACENADA',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G123','PRODUCCIÓN INMOVILIZADA',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G2','CONSUMO',null,0,1,'2017-06-30',null,null,-1,'TOTAL CONSUMO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G21','CONSUMO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G211','COMPRA MATERIA PRIMA Y AUX',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G212','COMPRA SUMINISTROS DIVERSOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G213','VARIACIÓN DE MATERIAS PRIMAS Y AUX',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G214','VARIACIÓN EXISTENCIAS SUM. DIVERSOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G3','VALOR AGREGADO',null,0,1,'2017-06-30',null,null,-1,'TOTAL VALOR AGREGADO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G31','VALOR AGREGADO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G311','VALOR AGREGADO',null,0,1,'2017-06-30 ',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G312','SERVICIOS PRESTADOS POR TERCEROS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G4','EXCEDENTE BRUTO DE EXPLOTACIÓN',null,0,1,'2017-06-30',null,null,-1,'TOTAL EXCEDENTE BRUTO DE EXPLOTACIÓN')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G41','EXCEDENTE BRUTO DE EXPLOTACIÓN',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G411','CARGAS DE PERSONAL',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G412','TRIBUTOS',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G5','RESULTADO DE EXPLOTACIÓN',null,0,1,'2017-06-30',null,null,-1,'RESULTADO DE EXPLOTACIÓN')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G51','RESULTADO DE EXPLOTACIÓN',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G511','CARGAS DIVERSAS DE GESTION',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G512','PROVISIONES DEL EJERCICIO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G512','INGRESOS DIVERSOS (INC. DESC.)',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G6','RESULTADO DEL EJERCICIO',null,0,1,'2017-06-30',null,null,-1,'RESULTADO DEL EJERCICIO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G61','RESULTADO DEL EJERCICIO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G611','REI DE EJERCICIO',null,0,1,'2017-06-30',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (v_TipoBalance, v_Codigo, v_Nombre, i_IdTipoElemento, i_Eliminado, i_InsertaIdUsuario, t_InsertaFecha, i_ActualizaIdUsuario, t_ActualizaFecha, i_TipoOperacion, v_NombreGrupo)  values ('3','G612','IMPUESTO A LA RENTA',null,0,1,'2017-06-30',null,null,-1,null)");

                                listaQuerys.Add(@"ALTER TABLE flujoefectivoconceptos ADD i_EsSumatoria int;");
                                listaQuerys.Add(@"ALTER TABLE flujoefectivoconceptos ADD i_IdTipoAccion int;");

                                listaQuerys.Add(@"CREATE TABLE flujoefectivoconceptosdetalles
								 (
								 i_Id int identity, 
								 i_IdConceptoFlujo int, 
								 v_NroCuenta varchar(12), 
								PRIMARY KEY (i_Id), 
							   CONSTRAINT fk_flujoconcepto_flujoconceptodetalle FOREIGN KEY (i_IdConceptoFlujo) 
							   REFERENCES flujoefectivoconceptos (i_IdConceptoFlujo) ON UPDATE NO ACTION ON DELETE NO ACTION)");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD v_CodigoBalanceNaturaleza varchar(10);");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD v_CodigoPatrimonioNeto varchar(10);");


                                break;
                            #endregion

                            #region Actulizacion 199

                            case 199:
                                listaQuerys.Add("ALTER TABLE asientocontable ADD i_UsaraPatrimonioNeto int;");
                                listaQuerys.Add("ALTER TABLE diariodetalle ADD i_IdPatrimonioNeto varchar(4);");
                                listaQuerys.Add("ALTER TABLE tesoreriadetalle ADD i_IdPatrimonioNeto varchar(4);");

                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (0, 171,'NOTAS - PATRIMONIO NETO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,1,'EFECTO ACUMULADO DE LOS CAMBIOS EN LAS POLÍTICAS CONTABLES Y LA CORRECCIÓN DE ERRORES SUSTANCIALES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (171,2,'DISTRIBUCIONES O ASIGNACIONES DE UTILIDADES EFECTUADAS EN EL PERÍODO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,3,'DIVIDENDOS Y PARTICIPACIONES ACORDADOS DURANTE EL PERÍODO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,4,'NUEVOS APORTES DE ACCIONISTAS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (171,5,'MOVIMIENTO DE  PRIMA EN LA COLOCACIÓN DE APORTES Y DONACIONES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,6,'INCREMENTOS O DISMINUCIONES POR FUSIONES O ESCISIONES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,7,'REVALUACIÓN DE ACTIVOS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,8,'CAPITALIZACIÓN DE PARTIDAS PATRIMONIALES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (171,9,'REDENCIÓN DE ACCIONES DE INVERSIÓN O REDUCCIÓN DE CAPITAL','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (171,10,'UTILIDAD (PÉRDIDA) NETA DEL EJERCICIO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (171,11,'OTROS INCREMENTOS O DISMINUCIONES DE LAS PARTIDAS PATRIMONIALES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");

                                break;
                            #endregion

                            #region Actualizacion 200
                            case 200:
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD \"i_Flag\" int;");
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD \"i_Orden\" int;");
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD \"d_Importe\" decimal(20,2);");

                                listaQuerys.Add("delete from flujoefectivoconceptos;");

                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de venta de bienes o servicios e ingresos operacionales', 0, 1, 0, 1, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de regalías, honorarios, comisiones y otros', 0, 1, 0, 2, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de intereses y dividendos recibidos', 0, 1, 0, 3, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros cobros de efectivo relativos a la actividad', 0, 1, 0, 4, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de emisión de acciones o nuevos aportes', 0, 3, 0, 20, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Ganancia por pasivos monetarios no corrientes', 0, 5, 0, 42, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pago a proveedores de bienes y servicios', 0, 1, 0, 5, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pago de remuneraciones y beneficios sociales', 0, 1, 0, 6, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pago de tributos', 0, 1, 0, 7, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pago de intereses y rendimientos', 0, 1, 0, 8, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros pagos de efectivo relativos a la actividad', 0, 1, 0, 9, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Operación', 1, -1, 0, 10, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de venta de valores e inversiones permanentes', 0, 2, 0, 11, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de venta de inmuebles, maquinaria y equipo', 0, 2, 0, 12, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de venta de activos intangibles', 0, 2, 0, 13, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros cobros de efectivo relativos a la actividad', 0, 2, 0, 14, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pagos por compra de valores e inversiones permanentes', 0, 2, 0, 15, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pagos por compra de inmuebles, maquinaria y equipo', 0, 2, 0, 16, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pagos por compra de activos intangibles', 0, 2, 0, 17, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros pagos de efectivo relativos a la actividad', 0, 2, 0, 18, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Inversión', 2, -1, 0, 19, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cobranza de recursos obtenidos por emisión de valores u otras obligaciones de largo plazo', 0, 3, 0, 21, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) Neto de efectivo y Equivalente de Efectivo', -1, 0, 1, 27, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Saldo Efectivo y Equivalente de Efectivo al Inicio del Ejercicio', 0, 4, 1, 28, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Saldo Efectivo y Equivalente de Efectivo al Finalizar el Ejercicio', NULL, NULL, 2, 29, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Utilidad (Pérdida) neta del Ejercicio', 0, 5, 0, 30, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Ajustes a la Utilidad (Pérdida) del Ejercicio', 0, 5, 0, 31, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Depreciación y amortización del período', 0, 5, 0, 32, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Provisión Beneficios Sociales', 0, 5, 0, 33, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Provisiones Diversas', 0, 5, 0, 34, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pérdida en venta de inmuebles, maquinaria y Equipo', 0, 5, 0, 35, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pérdida en venta de valores e inversiones permanentes', 0, 5, 0, 36, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pérdida por activos monetarios no corrientes', 0, 5, 0, 37, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros', 0, 5, 0, 38, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Ajustes a la Utilidad (Pérdida) del Ejercicio', 0, 5, 0, 39, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Utilidad en venta de inmuebles, maquinaria y equipo', 0, 5, 0, 40, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Utilidad en venta de valores e inversiones permanentes', 0, 5, 0, 41, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Cargos y Abonos por cambios netos en el Activo y Pasivo', 0, 5, 0, 43, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'(Aumento) Disminución de Cuentas por Cobrar Comerciales', 0, 5, 0, 44, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'(Aumento) Disminución de Cuentas por Cobrar Vinculadas', 0, 5, 0, 45, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'(Aumento) Disminución de Otras Cuentas por Cobrar', 0, 5, 0, 46, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'(Aumento) Disminución en Existencias', 0, 5, 0, 47, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'(Aumento) Disminución en Gastos Pagados por Anticipado', 0, 5, 0, 48, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) de Cuentas por Pagar Comerciales', 0, 5, 0, 49, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) de Cuentas por Pagar Vinculadas', 0, 5, 0, 50, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) de Otras Cuentas por Pagar', 0, 5, 0, 51, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de la Actividad de Operación', 5, 0, 0, 52, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pagos de amortización o cancelación de valores u otras obligaciones de largo plazo', 0, 3, 0, 23, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Pago de dividendos y otras distribuciones', 0, 3, 0, 24, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros pagos de efectivo relativos a la actividad', 0, 3, 0, 25, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Otros cobros de efectivo relativos a la actividad', 0, 3, 0, 22, NULL)");


                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Financiamiento', 3, -1, 0, 26, NULL)");

                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'- ACTIVIDADES DE INVERSIÓN', 100, NULL, NULL, 10, NULL)");

                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'- ACTIVIDADES DE OPERACIÓN', 100, NULL, NULL, 0, NULL)");

                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'- ACTIVIDADES DE FINANCIAMIENTO', 100, NULL, NULL, 20, NULL)");

                                listaQuerys.Add(@"INSERT INTO [flujoefectivoconceptos]
								  ([v_Descripcion], [i_EsSumatoria], [i_IdTipoAccion], [i_Flag], [i_Orden], [d_Importe])
								VALUES (N'- CONCILIACIÓN', 100, NULL, NULL, 30, NULL)");


                                break;
                            #endregion

                            #region Actualizacion 201
                            case 201:
                                listaQuerys.Add("ALTER TABLE configuracionbalances DROP COLUMN i_IdTipoElemento");
                                listaQuerys.Add("ALTER TABLE configuracionbalances DROP COLUMN i_TipoOperacion");
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD i_TipoNota int");



                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (0, 172,'NOTAS - ESTADO SITUACIÓN FINANCIERA','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,1,'NOTA 1','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (172,2,'NOTA 2','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,3,'NOTA 3','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,4,'NOTA 4','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (172,5,'NOTA 5','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,6,'NOTA 6','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,7,'NOTA 7','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,8,'NOTA 8','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,9,'NOTA 9','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (172,10,'NOTA 10','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (172,11,'NOTA 11','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (172,12,'NOTA 12','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,13,'NOTA 13','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,14,'NOTA 14','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (172,15,'NOTA 15','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");


                                break;
                            #endregion

                            #region Actualizacion 202
                            case 202:
                                listaQuerys.Add(@"ALTER TABLE pedido ALTER column v_DireccionClienteTemporal varchar(200)");
                                break;
                            #endregion

                            #region Actualizacion 203
                            case 203:
                                listaQuerys.Add("ALTER TABLE cobranza ADD v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE pago ADD v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE venta ADD v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE compra ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE pedido ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE movimiento ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE letras ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE diario ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE tesoreria ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE cliente ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE documento ADD  v_MotivoEliminacion varchar(200);");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD  v_MotivoEliminacion varchar(200);");
                                break;
                            #endregion

                            #region Actualizacion 204

                            case 204:

                                listaQuerys.Add("ALTER TABLE activofijo ADD  b_Foto IMAGE");

                                break;
                            #endregion

                            #region Actualizacion 205
                            case 205:
                                listaQuerys.Add("ALTER TABLE activofijo ADD  i_IdTipoDocumento int");
                                listaQuerys.Add("update activofijo set i_IdTipoDocumento = 1;");
                                break;
                            #endregion

                            #region Actualizacion 206
                            case 206:
                                listaQuerys.Add(@"create table activofijoanexo
												(
													i_IdActivoFijoAnexo int identity primary key,
													v_IdActivoFijo varchar(16),
													i_IdTipoDocumento int,
													v_NroDocumento varchar(15),
													t_FechaEmision datetime,
													b_Foto IMAGE,
													v_Observaciones varchar(200),       
													v_UbicacionFoto varchar(200),
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													
													CONSTRAINT fk_activofijoanexo_v_IdActivoFijo FOREIGN KEY (v_IdActivoFijo) REFERENCES activofijo (v_IdActivoFijo),
												);");

                                listaQuerys.Add("ALTER TABLE activofijo ADD  v_UbicacionFoto varchar(200)");
                                break;
                            #endregion

                            #region Actualizacion 207
                            case 207:
                                listaQuerys.Add("ALTER TABLE guiaremision ADD v_UbigueoLlegada varchar(10), v_UbigueoPartida varchar(10), d_TotalPeso decimal(18,4), i_EstadoSunat smallint, i_Modalidad smallint");
                                listaQuerys.Add("ALTER TABLE transportistachofer ADD i_IdTipoIdentificacion int, v_NroDocIdentificacion varchar(20)");
                                listaQuerys.Add("ALTER TABLE almacen ADD v_Ubigueo varchar(10)");
                                listaQuerys.Add(@"CREATE TABLE guiaremisionhomologacion
								(
									i_Idhomologacion int IDENTITY(1,1) NOT NULL,
									v_IdGuiaRemision varchar(16) NULL, 
									b_FileXml varbinary(max) NULL,
									CONSTRAINT PK_guiaremisionhomologacion PRIMARY KEY CLUSTERED (i_Idhomologacion),
									CONSTRAINT Fk_remisionhomologacion_guiaremision FOREIGN KEY (v_IdGuiaRemision) REFERENCES guiaremision(v_IdGuiaRemision),
								)
								");
                                break;

                            #endregion

                            #region Actualizacion 208

                            case 208:

                                listaQuerys.Add("ALTER TABLE activofijo ADD i_IdSituacionActivoFijo int");
                                listaQuerys.Add("ALTER TABLE activofijo ADD i_IdClaseActivoFijo int");
                                listaQuerys.Add("ALTER TABLE activofijo ADD v_CodigoOriginal varchar(20)");
                                listaQuerys.Add("ALTER TABLE activofijo ADD v_CodigoBarras varchar(20)");
                                listaQuerys.Add("ALTER TABLE activofijo ADD v_AnioFabricacion varchar(4)");

                                listaQuerys.Add("ALTER TABLE importacionDetalleProducto ALTER COLUMN v_NroFactura varchar(30)");




                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (0, 173,'SITUACIÓN DEL ACTIVO FIJO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (173,1,'VIGENTE','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (173,2,'TOTALMENTE DEPRECIADO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (173,3,'VENDIDO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");

                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (0, 174,'CLASE DEL ACTIVO FIJO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (174,1,'NORMAL','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (174,2,'COMPONENTE','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");



                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var Depreciacion = dbContext.datahierarchy.Where(l => l.i_GroupId == 109 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = dbContext.datahierarchy.Where(l => l.i_GroupId == 109).Max(l => l.i_ItemId);
                                    var Depreciacion240 = Depreciacion.Where(l => l.v_Value1.Contains("240")).ToList();
                                    if (!Depreciacion240.Any())
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (109," + MaxItemId + ",'240','5','',NULL,0,0,0,1,'2017-07-19 14:45:50.0092387',NULL ,NULL )");



                                    }
                                }


                                break;

                            #endregion

                            #region Actualizacion 209
                            case 209:
                                listaQuerys.Add("ALTER TABLE activofijo ALTER COLUMN v_NumeroFactura varchar(30)");
                                break;

                            #endregion

                            #region Actualizacion 210
                            case 210:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD SmtpHost varchar(80),SmtpPort integer, SmtpEmail varchar(80), SmtpPassword varchar(100), SmtpSsl bit;");
                                break;
                            #endregion

                            #region Actualizacion 211
                            case 211:
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN v_NroBulto varchar(150)");

                                break;
                            #endregion

                            #region Actualizacion 212
                            case 212:
                                listaQuerys.Add("ALTER TABLE cliente ALTER COLUMN v_PrimerNombre varchar(150)");
                                break;
                            #endregion

                            #region Actualizacion 213
                            case 213:


                                listaQuerys.Add("ALTER TABLE cliente ADD  \"i_IdTipoAccionesSocio\" integer");
                                listaQuerys.Add("ALTER TABLE cliente ADD  \"i_NumeroAccionesSuscritas\" decimal (20,2)");
                                listaQuerys.Add("ALTER TABLE cliente ADD \"i_NumeroAccionesPagadas\" decimal (20,2)");



                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (0, 175,'TIPO ACCIONES SOCIOS - BALANCE 50','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )   values (175,1,'NOMINATIVAS','','',NULL,0,0,0,1, '2017-08-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate )  values (175,2,'COMUNES','','',NULL,0,0,0,1, '2017-08-03 14:45:50.0092387')");

                                break;
                            #endregion

                            #region Actualizacion  214

                            case 214:
                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var RegimenLaboral = dbContext.datahierarchy.Where(l => l.i_GroupId == 38 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = RegimenLaboral.Max(l => l.i_ItemId);

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'HAMBURG – GERMANY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'JEDDAH – SAUDI ARABIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ALGER- ALGERIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");




                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ROSARIO – ARGENTINA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BUENOS AIRES – ARGENTINA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SIDNEY – AUSTRALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'MELBOURNE – AUSTRALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'POTI  – GEORGIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BAHRAIN – BAHRAIN','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BEIRUT – LEBANON','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ANTWERP – BELGICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'VARNA – BULGARIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'PRAIA– REP. DE CABO VERDE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'MINDELO– REP. DE CABO VERDE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'TORONTO – CANADA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'MONTREAL – CANADA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'VALPARAISO – CHILE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LIMASSOL – CYPRUS','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BARRANQUILLA – COLOMBIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'PUERTO CALDERA- COSTA RICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'PLOCE – CROACIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'PIRAEUS- GRECIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'THESSALONIKI- GRECIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ALEXANDRIA- EGYPT','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'JEBEL ALI – UNITED ARAB EMIRATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'VALENCIA – ESPAÑA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BARCELONA – ESPAÑA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ALGECIRAS  - ESPAÑA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'FOS SUR MER- FRANCIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LE HAVRE- FRANCIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ROTTERDAM – NETHERLANDS','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'HONG KONG- HONG KONG','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'NAVA SHEVA– INDIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'TILBURY – ENGLAND','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LONDON GATEWAY – ENGLAND','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'FELIXSTOWE – ENGLAND','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'HAIFA – ISRAEL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ASHDOD – ISRAEL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'VENECIA- ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'GENOVA- ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SALERNO ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'NAPLES- ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'KINGSTON– JAMAICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'YOKOHAMA – JAPON','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'AL ‘AQABAH PORT– JORDANIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SEOUL – KOREA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SHUWAIKH – KUWAIT','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'KLAIPEDA- LITUANIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'KUALA LUMPUR- MALAYSIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                }
                                break;
                            #endregion

                            #region Actualizacion  215

                            case 215:
                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var RegimenLaboral = dbContext.datahierarchy.Where(l => l.i_GroupId == 38 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = RegimenLaboral.Max(l => l.i_ItemId);

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'VERACRUZ – MEXICO','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'MANZANILLO – MEXICO','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BALBOA – PANAMA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'WARSZAWA – POLONIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'GDYNIA – POLONIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SINES/BOBADELA – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SINES – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LISBOA – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LEIXOES – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'HAMAD – QATAR','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'BREMERHAVEN -  GERMANY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'STARE MESTO -  REPUBLICA CHECA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'CONSTANZA PORT- RUMANIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ST. PETERSBURG – RUSIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'DURBAN- SOUTH AFRICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'KEELUNG – TAIWAN','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'KAOHSIUNG- TAIWAN','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'PORT OF SPAIN/TRINIDAD Y TOBAGO','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ISTANBUL – TURKEY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'IZMIR – TURKEY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'MERSIN – TURKEY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'ODESSA- UKRAINE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'MONTEVIDEO – URUGUAY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LONG BEACH – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LOS ANGELES – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'NEW YORK – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'HOUSTON – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'HOUSTON – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'SEATTLE – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'OAKLAND – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'LA GUAIRA/VENEZUELA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy  (i_GroupId,i_ItemId,v_Value1,v_Value2,v_Field,i_ParentItemId,i_Header,i_Sort,i_IsDeleted,i_InsertUserId,d_InsertDate,i_UpdateUserId,d_UpdateDate)  values    (38," + MaxItemId + ",'PUERTO CABELLO/VENEZUELA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    listaQuerys.Add("ALTER TABLE  venta ADD  \"v_NroBultoIngles\" varchar(150)");
                                    listaQuerys.Add("ALTER TABLE venta ALTER COLUMN v_Contenedor varchar(150)");
                                }
                                break;
                            #endregion

                            #region Actualizacion 216
                            case 216:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_TipoDepreciacionActivoFijo int");
                                listaQuerys.Add("update configuracionempresa set i_TipoDepreciacionActivoFijo = 1;");
                                break;

                            #endregion

                            #region Actualizacion 217
                            case 217:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER COLUMN v_Observaciones varchar(500)");
                                break;

                            #endregion

                            #region Actualizacion 218
                            case 218:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER COLUMN v_Observaciones varchar(2000);");
                                listaQuerys.Add("ALTER TABLE venta ADD i_AfectaDetraccion INT;");
                                listaQuerys.Add("ALTER TABLE venta ADD d_TasaDetraccion DECIMAL(10,2);");
                                listaQuerys.Add("ALTER TABLE diario ADD i_AfectaDetraccion INT;");
                                listaQuerys.Add("ALTER TABLE diario ADD d_TasaDetraccion DECIMAL(10,2);");
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN v_SerieDocumento varchar(20);");
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN v_SerieDocumentoRef varchar(20);");
                                listaQuerys.Add("ALTER TABLE compra ALTER COLUMN v_SerieDocumento varchar(20);");
                                listaQuerys.Add("ALTER TABLE compra ALTER COLUMN v_SerieDocumentoRef varchar(20);");
                                listaQuerys.Add("ALTER TABLE establecimientodetalle ALTER COLUMN v_Serie varchar(20);");
                                break;

                            #endregion

                            #region Actualizacion 219
                            case 219:
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  t_FechaLiberacion datetime;");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  i_LiberacionUsuario int;");

                                break;
                            #endregion

                            #region Actualizacion 220
                            case 220:
                                listaQuerys.Add("ALTER TABLE compradetalle ADD v_IdAnexo varchar(16);");
                                break;
                            #endregion


                            #region Actualizacion 221

                            case 221:
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD  v_Mes varchar(2);");
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD  v_Periodo varchar(4);");
                                listaQuerys.Add("update  configuracionbalances  set  v_Mes='1',v_Periodo='2017' where v_TipoBalance='2';");


                                break;
                            #endregion


                            #region Actualiazacion 223
                            case 223:
                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {

                                    var ListaSituacionFinanciera = (from a in dbContext.configuracionbalances
                                                                    where a.v_TipoBalance == "2" && a.i_Eliminado == 0
                                                                    select a).ToList();
                                    int Mes = 2;
                                    int MesFin = DateTime.Now.Month;

                                    while (Mes <= MesFin)
                                    {
                                        foreach (var lsf in ListaSituacionFinanciera)
                                        {
                                            var inserta = lsf.t_InsertaFecha ?? DateTime.Now;
                                            listaQuerys.Add("INSERT INTO configuracionbalances (v_TipoBalance, v_Codigo,v_Nombre,i_Eliminado,v_NombreGrupo,i_InsertaIdUsuario,t_InsertaFecha,v_Mes,v_Periodo) " +
                                                                                               "VALUES (" + lsf.v_TipoBalance + ",'" + lsf.v_Codigo + "','" + lsf.v_Nombre + "'," + lsf.i_Eliminado + ",'" + lsf.v_NombreGrupo + "'," + lsf.i_InsertaIdUsuario + ",'" + inserta.ToString("yyyy-MM-dd") + "' ,'" + Mes + "'," + "'2017');");


                                        }
                                        Mes = Mes + 1;
                                    }




                                }
                                break;
                            #endregion

                            #region Actualizacion 224
                            case 224:
                                listaQuerys.Add("ALTER TABLE producto ADD d_PrecioMayorista decimal(18, 7)");
                                break;
                            #endregion

                            #region Actualizacion 225
                            case 225:
                                listaQuerys.Add("ALTER TABLE documento ADD i_BancoDetraccion int;");
                                break;
                            #endregion

                            #region Actualizacion 226
                            case 226:
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (0, 176, N'VENTAS - PERFILES DETRACCION', N'', NULL, NULL, NULL, 0, 0, 1, N'2017-08-31 11:34:02.229', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (176, 1, N'PERFIL 01', N'10', N'700', NULL, 0, 0, 0, 1, N'2017-08-31 11:34:31.872', 1, N'2017-08-31 11:35:04.637', NULL, NULL, N'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (176, 2, N'PERFIL 02', N'10', N'400', NULL, 0, 0, 0, 1, N'2017-08-31 11:34:39.843', 1, N'2017-08-31 11:35:13.576', NULL, NULL, N'');");

                                listaQuerys.Add("ALTER TABLE producto ADD i_IdPerfilDetraccion int;");
                                break;
                            #endregion

                            #region Actualizacion 227
                            case 227:

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD i_PermiteEditarPedidoFacturado int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  b_LogoEmpresa IMAGE");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  b_FirmaDigitalEmpresa IMAGE");
                                break;
                            #endregion

                            #region Actualiazion 228
                            case 228:
                                listaQuerys.Add("ALTER TABLE producto ADD i_EsProductoPerecible int;");
                                listaQuerys.Add("ALTER TABLE pedido ADD  i_TieneGuia int");
                                break;

                            #endregion

                            #region Actualizacion  229
                            case 229:
                                listaQuerys.Add("ALTER TABLE producto DROP  COLUMN i_EsProductoPerecible;");
                                listaQuerys.Add("ALTER TABLE pedido DROP COLUMN  i_TieneGuia");
                                break;
                            #endregion

                            #region Actualizacion  230
                            case 230:
                                listaQuerys.Add(@"CREATE TABLE planilladiasnolaborables
                                (
                                    i_IdDiaNoLaborable int IDENTITY(1,1) NOT NULL,
                                    t_DiaNoLaborable datetime,
                                    CONSTRAINT PK_planilladiasnolaborables PRIMARY KEY (i_IdDiaNoLaborable)
                                )");

                                listaQuerys.Add(@"CREATE TABLE planilladatoslaborables
                                (
                                  i_Id int IDENTITY(1,1) NOT NULL,
                                  v_RangoSemana character varying(10),
                                  v_RangoHoras character varying(10),
                                  i_Vigente integer,
                                  CONSTRAINT PK_planilladatoslaborables PRIMARY KEY (i_Id)
                                )");

                                listaQuerys.Add(@"CREATE TABLE planillasemanasperiodo
                                (
                                    i_IdSemana int IDENTITY(1,1) NOT NULL,
                                    i_NroSemana integer,
                                    t_FechaInicio datetime,
                                    t_FechaFin datetime,
                                    CONSTRAINT PK_planillasemanasperiodo PRIMARY KEY (i_IdSemana)
                                )");

                                listaQuerys.Add(@"CREATE TABLE planillaasistencia
                                (
                                  i_IdAsistencia int IDENTITY(1,1) NOT NULL,
                                  v_IdTrabajador character varying(16),
                                  i_IdSemana integer,
                                  t_Fecha datetime,
                                  t_Ingreso_I datetime,
                                  t_Salida_I datetime,
                                  t_Ingreso_II datetime,
                                  t_Salida_II datetime,
                                  d_HorasNormales numeric(20,3),
                                  d_HorasExtras numeric(20,3),
                                  CONSTRAINT PK_planillaasistencia PRIMARY KEY (i_IdAsistencia),
                                  CONSTRAINT FK_planillasemanasperiodo FOREIGN KEY (i_IdSemana)
                                      REFERENCES planillasemanasperiodo (i_IdSemana),
                                  CONSTRAINT Fk_trabajador FOREIGN KEY (v_IdTrabajador)
                                      REFERENCES trabajador (v_IdTrabajador)
                                )");

                                listaQuerys.Add(@"CREATE TABLE planillaturnos
                                (
                                    i_IdTurno int IDENTITY(1,1) NOT NULL,
                                    v_Descripcion character varying(100),
                                    i_IdSemana integer,
                                    CONSTRAINT PK_planillaturnos PRIMARY KEY (i_IdTurno),
                                    CONSTRAINT Fk_planillasemanasperiodo_planillaturnos FOREIGN KEY (i_IdSemana)
                                        REFERENCES planillasemanasperiodo (i_IdSemana) 
                                )");

                                listaQuerys.Add(@"CREATE TABLE planillaturnosdetalle
                                (
                                  i_IdTurnoDetalle int IDENTITY(1,1) NOT NULL,
                                  i_IdTurno integer,
                                  v_DiaLunes character varying(50),
                                  v_DiaMartes character varying(50),
                                  v_DiaMiercoles character varying(50),
                                  v_DiaJueves character varying(50),
                                  v_DiaViernes character varying(50),
                                  v_DiaSabado character varying(50),
                                  v_DiaDomingo character varying(50),
                                  CONSTRAINT PK_planillaturnosdetalle PRIMARY KEY (i_IdTurnoDetalle),
                                  CONSTRAINT FK_planillaturnos FOREIGN KEY (i_IdTurno)
                                      REFERENCES planillaturnos (i_IdTurno) 
                                )");

                                listaQuerys.Add("ALTER TABLE planilladiasnolaborables ADD  \"v_Descripcion\" character varying(100);");
                                break;
                            #endregion

                            #region Actualizacion 231
                            case 231:
                                listaQuerys.Add("ALTER TABLE venta ADD i_IdCodigoDetraccion int;");
                                listaQuerys.Add("ALTER TABLE venta ADD i_IdTipoOperacionDetraccion int;");
                                listaQuerys.Add("ALTER TABLE diario ADD v_CodigoDetraccion varchar(5);");

                                listaQuerys.Add("ALTER TABLE diario ADD i_IdTipoOperacionDetraccion int;");

                                listaQuerys.Add("ALTER TABLE datahierarchy ALTER COLUMN v_Value1 varchar(400)");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               "VALUES (0, 177, 'TIPOS OPERACION DETRACCION','', NULL, NULL, NULL, 0, 0, 1,'2017-09-12 11:34:02.229', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (177, 1,'VENTA DE BIENES MUEBLES O INMUEBLES, PRESTACIÓN DE SERVICIOS O CONTRATOS DE CONSTRUCCIÓN GRAVADOS CON EL IGV','','', NULL, 0, 0, 0, 1,'2017-09-12 11:34:02.229', 1,NULL, NULL, NULL,'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (177, 2,'RETIRO DE BIENES GRAVADOS CON EL IGV', '', '', NULL, 0, 0, 0, 1,'2017-09-12 11:34:02.229', 1,'2017-09-12 11:34:02.229', NULL, NULL,'');");


                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               "VALUES (177, 3,'TRASLADO DE BIENES FUERA DEL CENTRO DE PRODUCCIÓN, ASÍ COMO DESDE CUALQUIER ZONA GEOGRÁFICA QUE GOCE DE BENEFICIOS TRIBUTARIOS HACIA EL RESTO DEL PAÍS, CUANDO DICHO TRASLADO NO SE ORIGINE EN UNA OPERACIÓN DE VENTA','','', NULL, 0, 0, 0, 1,'2017-09-12 11:34:02.229', 1,'2017-09-12 11:35:04.637', NULL, NULL,'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (177, 4,'VENTA DE BIENES GRAVADA CON EL IGV REALIZADA A TRAVÉS DE LA BOLSA DE PRODUCTOS', '', '', NULL, 0, 0, 0, 1,'2017-09-12 11:34:02.229', 1,'2017-09-12 11:34:02.229', NULL, NULL,'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               "VALUES (177, 5,'VENTA  DE BIENES EXONERADA DEL IGV', '', '', NULL, 0, 0, 0, 1,'2017-09-12 11:34:02.229', 1,'2017-09-12 11:34:02.229', NULL, NULL,'');");


                                break;
                            #endregion

                            #region Actualizacion 232
                            case 232:
                                listaQuerys.Add("ALTER TABLE diario DROP  COLUMN v_CodigoDetraccion;");
                                listaQuerys.Add("ALTER TABLE diario ADD i_IdCodigoDetraccion int;");
                                break;
                            #endregion

                            #region Actualizacion  233
                            case 233:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajo ADD v_IdResponsable varchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 234
                            case 234:
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaLunes;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaMartes;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaMiercoles;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaJueves;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaViernes;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaSabado;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN v_DiaDomingo;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD t_IngresoI datetime;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD t_SalidaI datetime;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD t_IngresoII datetime;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD t_SalidaII datetime;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD d_HorasSemanales decimal(15,3);");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD i_IdDia INT;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD i_Asiste INT;");
                                break;
                            #endregion

                            #region Actualizacion  235
                            case 235:
                                listaQuerys.Add("ALTER TABLE areaslaboratrabajador ADD i_IdTurno int;");
                                break;
                            #endregion

                            #region Actualizacion  236
                            case 236:
                                listaQuerys.Add("ALTER TABLE planillaasistencia DROP COLUMN d_HorasExtras;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD d_HorasExtras_25 decimal(15,2);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD d_HorasExtras_35 decimal(15,2);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD i_IdEstado int;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD i_MinutosTardanza int;");
                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (0, 178, N'PLANILLA - ESTADO ASISTENCIAS', N'', NULL, NULL, NULL, 0, 0, 1, '20170918 11:15:53.415', NULL, NULL, NULL, NULL, NULL)");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (178, 1, N'ASISTENCIA', N'A', N'', NULL, 0, 0, 0, 1, '20170918 11:16:55.396', 1, '20170918 11:17:25.212', NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (178, 2, N'FALTA JUSTIFICADA', N'F', N'', NULL, 0, 0, 0, 1, '20170918 11:17:00.941', 1, '20170918 11:17:28.894', NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (178, 3, N'FALTA INJUSTIFICADA', N'I', N'', NULL, 0, 0, 0, 1, '20170918 11:17:34.925', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (178, 4, N'SUBSIDIO', N'S', N'', NULL, 0, 0, 0, 1, '20170918 11:17:56.092', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (178, 5, N'VACACIONES', N'V', N'', NULL, 0, 0, 0, 1, '20170918 11:18:00.844', NULL, NULL, NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (178, 6, N'DESCANSO', N'D', N'', NULL, 0, 0, 0, 1, '20170918 11:18:00.844', NULL, NULL, NULL, NULL, N'')");
                                break;
                            #endregion

                            #region Actualizacion  237

                            case 237:
                                listaQuerys.Add("ALTER TABLE diario ADD t_FechaVencimiento datetime;");
                                listaQuerys.Add("ALTER TABLE diario ADD t_FechaEmision datetime;");
                                break;

                            #endregion

                            #region Actualizacion  238

                            case 238:
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN d_HorasNormales INT;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN d_HorasExtras_25 INT;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN d_HorasExtras_35 INT;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD t_Ingreso_I_Turno datetime;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD t_Ingreso_II_Turno datetime;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD t_Salida_I_Turno datetime;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD t_Salida_II_Turno datetime;");
                                break;
                            #endregion

                            #region Actualizacion  239
                            case 239:
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN d_HorasNormales decimal(15,6);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN d_HorasExtras_25 decimal(15,6);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN d_HorasExtras_35 decimal(15,6);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER COLUMN i_MinutosTardanza decimal(15,6);");
                                break;
                            #endregion

                            #region Actualizacion 240
                            case 240:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD v_FeEndpoint VARCHAR(200), v_FePassword VARCHAR(100)");
                                listaQuerys.Add("ALTER TABLE venta ADD i_Publish smallint");
                                break;

                            #endregion

                            #region Actualizacion 241
                            case 241:
                                listaQuerys.Add("ALTER TABLE ordendecompra ADD  i_IdTipoOrdenCompra int;");





                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (0, 179, N'TIPOS - ORDEN COMPRA', N'', NULL, NULL, NULL, 0, 0, 1, '20170922 11:15:53.415', NULL, NULL, NULL, NULL, NULL)");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (179, 1, N'NACIONAL', N'', N'', NULL, 0, 0, 0, 1, '20170922 11:16:55.396', 1, '20170922 11:17:25.212', NULL, NULL, N'')");

                                listaQuerys.Add(@"INSERT INTO [datahierarchy]
                                  ([i_GroupId], [i_ItemId], [v_Value1], [v_Value2], [v_Field], [i_ParentItemId], [i_Header], [i_Sort], [i_IsDeleted], [i_InsertUserId], [d_InsertDate], [i_UpdateUserId], [d_UpdateDate], [i_SyncStatusId], [i_RecordStatusId], [v_Value4])
                                VALUES (179, 2, N'IMPORTACIÓN', N'', N'', NULL, 0, 0, 0, 1, '20170922 11:17:00.941', 1, '20170922 11:17:28.894', NULL, NULL, N'')");



                                listaQuerys.Add("update ordendecompra set i_IdTipoOrdenCompra = 1;");


                                break;
                            #endregion

                            #region Actualizacion 242
                            case 242:

                                listaQuerys.Add("ALTER TABLE importacion ADD i_IdTipoDocRerefencia int;");
                                listaQuerys.Add("ALTER TABLE importacion ADD v_NumeroDocRerefencia varchar(30);");
                                listaQuerys.Add("ALTER TABLE importacion ADD v_IdDocumentoReferencia varchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 243
                            case 243:
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD i_HorasExtrasAutorizadas int;");
                                break;
                            #endregion

                            #region Actualizacion 244
                            case 244:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD t_FechaCaducidad datetime;");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD t_FechaFabricacion datetime;");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD v_NroLote varchar(30);");

                                listaQuerys.Add("ALTER TABLE compradetalle ADD t_FechaCaducidad datetime;");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD t_FechaFabricacion datetime;");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD v_NroLote varchar(30);");

                                listaQuerys.Add("ALTER TABLE ventadetalle ADD t_FechaCaducidad datetime;");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD t_FechaFabricacion datetime;");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD v_NroLote varchar(30);");

                                listaQuerys.Add("ALTER TABLE producto ADD i_SolicitarNroSerie int;");
                                listaQuerys.Add("ALTER TABLE producto ADD i_SolicitarNroLote int;");
                                break;

                            #endregion

                            #region Actualizacion 245
                            case 245:
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD t_FechaCaducidad datetime;");
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD v_NroLote varchar(30);");
                                break;
                            #endregion

                            #region Actualizacion 246
                            case 246:
                                listaQuerys.Add("ALTER TABLE planillaconceptos ADD v_Siglas varchar(10);");
                                listaQuerys.Add("ALTER TABLE planillaconceptos ADD v_Formula varchar(1000);");
                                listaQuerys.Add("ALTER TABLE planillaconceptos ADD i_IdTipo int;");
                                break;
                            #endregion

                            #region Actualizacion 247
                            case 247:
                                Utils.Windows.RenombrarCodigosConceptos();
                                break;
                            #endregion

                            #region Actualizacion 248
                            case 248:
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD t_FechaCaducidad datetime;");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  v_NroLote varchar(30);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  v_NroPedido varchar(20);");

                                listaQuerys.Add("ALTER TABLE separacionproducto ADD v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE separacionproducto ADD v_NroLote varchar(30);");
                                listaQuerys.Add("ALTER TABLE separacionproducto ADD  v_NroPedido varchar(20);");



                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ADD v_NroSerie varchar(30);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ADD v_NroLote varchar(30);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ADD t_FechaCaducidad datetime;");
                                listaQuerys.Add("ALTER TABLE vendedor ADD i_EsActivo  int;");
                                listaQuerys.Add("update vendedor set i_EsActivo = 1;");

                                break;
                            #endregion

                            #region Actualizacion 249
                            case 249:
                                //Sin Acción
                                break;
                            #endregion

                            #region Actualizacion 250
                            case 250:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD v_NroOrdenProduccion varchar(30);");
                                listaQuerys.Add("ALTER TABLE producto ADD i_SolicitaOrdenProduccion int;");

                                listaQuerys.Add(@"create table ordenproduccion
												(
													i_IdOrdenProduccion int identity primary key,
													v_IdProductoDetalle varchar(16),
													v_Mes varchar(2),
                                                    v_Correlativo varchar(8),
													v_Periodo  varchar(4),
                                                    t_FechaRegistro datetime,
                                                    v_Observacion varchar(200),
                                                    t_FechaInicio datetime,
                                                    t_FechaTermino  datetime,
                                                    d_Cantidad decimal(16, 4),
                                                    d_CantidadUnidadMedida decimal(16, 4),
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													CONSTRAINT fk_productodetalle_v_IdProductoDetalle FOREIGN KEY (v_IdProductoDetalle) REFERENCES productodetalle (v_IdProductoDetalle),
													
												);");
                                break;
                            #endregion

                            #region Actualizacion 251
                            case 251:
                                listaQuerys.Add(@"CREATE TABLE ventaimportaciondataconfig
                                    (
                                      i_Id int identity,
                                      v_CtaVenta varchar(12),
                                      v_CtaEfectivo varchar(20),
                                      v_CtaVisa varchar(20),
                                      v_CtaMastercard varchar(20),
                                      v_CtaAmericanExpress varchar(20),
                                      CONSTRAINT Pk PRIMARY KEY (i_Id)
                                    )");
                                break;
                            #endregion

                            #region Actualizacion 252
                            case 252:
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD \"i_IdDocumentoEfectivo\" int;");
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD \"i_IdDocumentoVisa\" int;");
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD \"i_IdDocumentoMastercard\" int;");
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD \"i_IdDocumentoAmericanExpress\" int;");
                                break;
                            #endregion

                            #region Actulizacion 253
                            case 253:
                                listaQuerys.Add("ALTER TABLE pedido ALTER COLUMN v_Glosa varchar(200);");
                                break;
                            #endregion

                            #region Actualizacion 254
                            case 254:


                                listaQuerys.Add("ALTER TABLE asientocontable ALTER  COLUMN\"v_CodigoSituacionFinaciera\"  varchar(200);");

                                listaQuerys.Add("ALTER TABLE producto DROP COLUMN i_SolicitarNroSerie;");
                                listaQuerys.Add("ALTER TABLE producto DROP COLUMN i_SolicitarNroLote;");
                                listaQuerys.Add("ALTER TABLE producto DROP COLUMN i_SolicitaOrdenProduccion;");
                                listaQuerys.Add("ALTER TABLE producto ADD  \"i_SolicitarNroSerieIngreso\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD  \"i_SolicitarNroLoteIngreso\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD  \"i_SolicitaOrdenProduccionIngreso\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD  \"i_SolicitarNroSerieSalida\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD  \"i_SolicitarNroLoteSalida\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD  \"i_SolicitaOrdenProduccionSalida\" integer;");




                                break;
                            #endregion

                            #region Actualizacion 255
                            case 255:


                                listaQuerys.Add(@"create table ordenproducciondocumentos
												(
													i_IdOrdenProduccionDocumentos int identity primary key,
                                                    i_IdOrdenProduccion int,
                                                    i_IdTipoDocumento  int,
													v_SerieDocumento varchar(4),
													v_CorrelativoDocumento varchar(8),
													i_Eliminado int,
													i_InsertaIdUsuario int,
													t_InsertaFecha datetime,
													i_ActualizaIdUsuario int,
													t_ActualizaFecha datetime,
													CONSTRAINT fk_ordenproduccion_i_IdOrdenProduccion FOREIGN KEY (i_IdOrdenProduccion) REFERENCES ordenproduccion (i_IdOrdenProduccion),
													
												);");

                                break;
                            #endregion

                            #region Actualizacion 256
                            case 256:
                                listaQuerys.Add("ALTER TABLE venta ADD v_IdDocAnticipo nchar(16)");
                                listaQuerys.Add("ALTER TABLE venta ADD i_EsAnticipo INT");
                                break;
                            #endregion

                            #region Actualizacion 257
                            case 257:
                                listaQuerys.Add("alter table venta add v_SunatResponseCode char(2);");
                                listaQuerys.Add("alter table venta add v_CadenaCodigoQr varchar(MAX);");
                                listaQuerys.Add("alter table venta add v_Hash varchar(2000);");
                                listaQuerys.Add("alter table venta add v_CodigoBarras varchar(max);");
                                listaQuerys.Add("alter table venta add v_KeySunat varchar(2000);");
                                listaQuerys.Add("alter table venta add v_EnlacePdf varchar(2000);");
                                listaQuerys.Add("alter table venta add v_EnlaceXml varchar(2000);");
                                listaQuerys.Add("alter table venta add v_EnlaceCdr varchar(2000);");
                                break;
                            #endregion

                            #region Actualizacion 258
                            case 258:
                                listaQuerys.Add("alter table venta add SunatDescription varchar(max);");
                                listaQuerys.Add("alter table venta add SunatNote varchar(max);");
                                break;
                            #endregion

                            #region Actualizacion 259
                            case 259:
                                listaQuerys.Add("alter table venta drop column v_EnlacePdf;");
                                listaQuerys.Add("alter table venta drop column v_EnlaceCdr;");
                                listaQuerys.Add("alter table venta drop column v_EnlaceXml;");
                                listaQuerys.Add("alter table venta add v_EnlaceEnvio nvarchar(2000);");
                                listaQuerys.Add("alter table venta add v_EnlaceBaja nvarchar(2000);");
                                break;
                            #endregion

                            #region Actualizacion 260
                            case 260:
                                listaQuerys.Add("alter table venta alter column v_NroGuiaRemisionCorrelativo nvarchar(300);");
                                break;
                            #endregion
                        }
                        break;
                    #endregion

                    #region PostgreSQL
                    case TipoMotorBD.PostgreSQL:
                        switch (version)
                        {
                            #region Actualización 2
                            case 2:
                                listaQuerys.Add("ALTER TABLE nbs_ventakardex ADD COLUMN " + "\"" + "d_Monto" + "\"" + " numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD COLUMN " + "\"" + "i_FacturadoContabilidad" + "\"" + " integer;");
                                listaQuerys.Add("ALTER TABLE saldoscontables ADD COLUMN " + "\"" + "v_ReplicationId" + "\"" + " character(1);");
                                listaQuerys.Add("update saldoscontables set " + "\"" + "v_ReplicationId" + "\"" + " = 'N';");
                                break;
                            #endregion

                            #region Actualización 3
                            case 3:
                                listaQuerys.Add("DELETE from datahierarchy where " + "\"" + "i_GroupId" + "\"" + "=146" + " and " + "\"" + "i_ItemId" + "\"" + ">5;");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 6,'ESTADOS UNIDOS MEXICANOS',5,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 7,'REPÚBLICA DE COREA',6,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 8,'CONFEDERACIÓN DE SUIZA',7,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 9,'PORTUGAL',8,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy values (146, 10,'OTROS',9,'',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                break;

                            #endregion

                            #region Actualización 4
                            case 4:
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD COLUMN " + "\"" + "v_Descuento" + "\"" + " character varying(16);");
                                break;
                            #endregion

                            #region Actualizaion 5:
                            case 5:
                                listaQuerys.Add("ALTER TABLE guiaremision ADD COLUMN " + "\"" + "i_IdTipoGuia" + "\"" + " integer;");

                                listaQuerys.Add("ALTER TABLE documento ADD COLUMN " + "\"" + "i_DescontarStock" + "\"" + " integer;");

                                break;

                            #endregion

                            #region Actualizacion 6:
                            case 6:
                                listaQuerys.Add("ALTER TABLE guiaremision DROP CONSTRAINT " + "\"" + "FK_guiaremision_documento_i_IdTipoDocumento" + "\"");

                                break;
                            #endregion

                            #region Actualizacion 7:
                            case 7:

                                listaQuerys.Add("update guiaremision  set " + "\"" + "i_IdTipoGuia" + "\"" + " = 9 where " + "\"" + "i_IdTipoGuia" + "\"" + "is null");

                                break;
                            #endregion

                            #region Actualizacion 8:
                            case 8:
                                listaQuerys.Add("DELETE from administracionconceptos WHERE " + "\"" + "v_Codigo" + "\"" + "='20'");
                                listaQuerys.Add("INSERT INTO administracionconceptos values ('N002-ZV000000031',	'20'	,'PROVEEDOR IGV','4212102','','',0,	1,'2014-11-25',	null,	null)");
                                listaQuerys.Add("INSERT INTO concepto values ('N002-ZT000000031','50','PERCEPCION',2,0,1,'2014-11-25',NULL,NULL)");
                                listaQuerys.Add("INSERT INTO administracionconceptos values ('N002-ZV000000032',	'50'	,'PERCEPCION','','','',0,	1,'2014-11-25',	null,	null)");
                                listaQuerys.Add("INSERT INTO concepto values ('N002-ZT000000032','51','PROVEEDOR PERCEPCION',2,0,1,'2014-11-25',NULL,NULL)");
                                listaQuerys.Add("INSERT INTO administracionconceptos values ('N002-ZV000000033',	'51'	,'PROVEEDOR PERCEPCION','','','',0,	1,'2014-11-25',	null)");

                                break;
                            #endregion

                            #region Actualizacion 9:
                            case 9:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirTransportistaGuiaRemision" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE guiaremision ADD COLUMN " + "\"" + "i_AfectoIgv" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE guiaremision  ADD COLUMN " + "\"" + "i_PrecionInclIgv" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD COLUMN " + "\"" + "d_Valor" + "\"" + " numeric(18,4)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD COLUMN " + "\"" + "v_Descuento" + "\"" + " character varying(16)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD COLUMN " + "\"" + "d_Descuento" + "\"" + " numeric(18,4)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD COLUMN " + "\"" + "d_ValorVenta" + "\"" + " numeric(18,4)");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ADD COLUMN " + "\"" + "d_Igv" + "\"" + " numeric(18,4)");

                                break;

                            #endregion

                            #region Actualizacion 10
                            case 10:
                                listaQuerys.Add("ALTER TABLE guiaremision  ADD COLUMN " + "\"" + "i_IdIgv" + "\"" + " integer");
                                break;
                            #endregion

                            #region Actualizacion 11
                            case 11:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_ActualizarCostoProductos" + "\"" + "integer;");
                                break;
                            #endregion

                            #region Actualizacion 12
                            case 12:
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD COLUMN " + "\"" + "i_UsadoVenta" + "\"" + "integer;");
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajodetalle ADD COLUMN " + "\"" + "v_DescripcionTemporal" + "\"" + "character(100);");
                                break;
                            #endregion

                            #region Actualizacion 13

                            case 13:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajodetalle ADD COLUMN " + "\"" + "d_ImporteRegistral" + "\"" + "numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD  COLUMN " + "\"" + "d_ImporteRegistral" + "\"" + "numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajo ADD COLUMN " + "\"" + "d_TotalRegistral" + "\"" + "numeric(18,4);");
                                break;
                            #endregion

                            #region Actualizacion 14
                            case 14:
                                listaQuerys.Add("ALTER TABLE linea ALTER COLUMN \"v_Nombre\" type character varying(50)");
                                listaQuerys.Add("ALTER TABLE marca ADD COLUMN \"v_Sigla\" character varying(3)");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"v_IdColor\" character varying(16)");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"v_IdTalla\" character varying(16)");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"b_Foto\" bytea");
                                break;
                            #endregion

                            #region Actualizacion 15
                            case 15:
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"v_Modelo\" character varying(100)");
                                listaQuerys.Add("CREATE TABLE productoreceta" +
                                              "(\"i_IdReceta\" serial NOT NULL," +
                                              "\"v_IdProdTerminado\" character varying(16)," +
                                              "\"v_IdProdInsumo\" character varying(16)," +
                                              "\"v_Observacion\" character varying(250)," +
                                              "\"d_Cantidad\" numeric(16,4)," +
                                              "\"i_Eliminado\" integer," +
                                              "\"i_InsertaIdUsuario\" integer," +
                                              "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                              "\"i_ActualizaIdUsuario\" integer," +
                                              "\"t_ActualizaFecha\" timestamp(6) without time zone," +
                                              "CONSTRAINT productoreceta_pkey PRIMARY KEY (\"i_IdReceta\")," +
                                              "CONSTRAINT \"fk_productodetalle_v_IdProdInsumo\" FOREIGN KEY (\"v_IdProdInsumo\")" +
                                                  "REFERENCES productodetalle (\"v_IdProductoDetalle\") MATCH SIMPLE " +
                                                  "ON UPDATE RESTRICT ON DELETE RESTRICT," +
                                              "CONSTRAINT \"fk_productodetalle_v_IdProdTerminado\" FOREIGN KEY (\"v_IdProdTerminado\")" +
                                                  "REFERENCES productodetalle (\"v_IdProductoDetalle\") MATCH SIMPLE " +
                                                  @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");

                                listaQuerys.Add("ALTER TABLE productoreceta OWNER TO postgres;");
                                break;
                            #endregion

                            #region Actualizacion 16
                            case 16:
                                listaQuerys.Add("ALTER TABLE compra ALTER \"v_GuiaRemisionCorrelativo\" TYPE character(100);");
                                break;
                            #endregion

                            #region Actualizacion 17
                            case 17:
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD COLUMN" + "\"" + "i_UsadoIrpe" + "\"" + " integer");
                                break;
                            #endregion

                            #region Actualizacion 18
                            case 18:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"v_IdRecetaFinal\" character varying(16);");
                                listaQuerys.Add(@"CREATE TABLE movimientodetallerecetafinal(" +
                                              "\"v_IdRecetaFinal\" character varying(16) NOT NULL, " +
                                              "\"v_IdMovimientoDetalle\" character varying(16), " +
                                              "\"v_IdProdTerminado\" character varying(16), " +
                                              "\"v_IdProdInsumo\" character varying(16), " +
                                              "\"d_Cantidad\" numeric(10,4), " +
                                              "\"i_Eliminado\" integer, " +
                                              "\"i_InsertaIdUsuario\" integer, " +
                                              "\"i_ActualizaIdUsuario\" integer, " +
                                              "\"t_ActualizaFecha\" timestamp without time zone, " +
                                              "\"t_InsertaFecha\" timestamp without time zone, " +
                                              "CONSTRAINT \"PK_RecetaFinal\" PRIMARY KEY (\"v_IdRecetaFinal\"), " +
                                              "CONSTRAINT \"FK_productodetalle_movimientodetallerecetafinal\" FOREIGN KEY (\"v_IdProdTerminado\") " +
                                                  "REFERENCES productodetalle (\"v_IdProductoDetalle\") MATCH SIMPLE " +
                                                  "ON UPDATE RESTRICT ON DELETE RESTRICT, " +
                                              "CONSTRAINT \"FK_productodetalle_movimientodetallerecetafinalInsumo\" FOREIGN KEY (\"v_IdProdInsumo\") " +
                                                  "REFERENCES productodetalle (\"v_IdProductoDetalle\") MATCH SIMPLE " +
                                                  "ON UPDATE NO ACTION ON DELETE NO ACTION) WITH (OIDS=FALSE);");

                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal OWNER TO postgres;");

                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal ADD CONSTRAINT \"FK_movimientodetalle_movimientodetallerecetafinal_id\" " +
                                                "FOREIGN KEY (\"v_IdMovimientoDetalle\") REFERENCES movimientodetalle (\"v_IdMovimientoDetalle\") " +
                                                "ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                break;
                            #endregion

                            #region Actualizacion 19
                            case 19:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"i_EsProductoFinal\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 20
                            case 20:
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"v_IdMovimientoOrigen\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal DROP CONSTRAINT \"FK_movimientodetalle_movimientodetallerecetafinal_id\";");
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal DROP COLUMN \"v_IdMovimientoDetalle\";");
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal ADD COLUMN \"v_IdMovimiento\" character varying(16);");

                                break;
                            #endregion

                            #region Actualizacion 21
                            case 21:
                                listaQuerys.Add("ALTER TABLE linea ADD COLUMN \"b_Foto\" bytea;");
                                listaQuerys.Add("ALTER TABLE linea ADD COLUMN \"i_Header\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 22
                            case 22:
                                break;
                            #endregion

                            #region  Actualizacion 23
                            case 23:
                                listaQuerys.Add("ALTER TABLE establecimientodetalle ADD COLUMN \"i_NumeroItems\" integer;");
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"i_GenerarGuia\" integer;");
                                break;

                            #endregion

                            #region Actualizacion 24
                            case 24:
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD COLUMN \"v_NroPedido\" character varying (50);");
                                break;
                            #endregion

                            #region Actualizacion 25
                            case 25:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirPedidoExportacionCompraVenta" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirLotesCompraVenta" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirNingunoCompraVenta" + "\"" + " integer");
                                listaQuerys.Add("update configuracionempresa set " + "\"" + "i_IncluirNingunoCompraVenta" + "\"" + " = 1;");
                                listaQuerys.Add("update configuracionempresa set " + "\"" + "i_IncluirLotesCompraVenta" + "\"" + " = 0;");
                                listaQuerys.Add("update configuracionempresa set " + "\"" + "i_IncluirPedidoExportacionCompraVenta" + "\"" + " = 0;");

                                break;
                            #endregion

                            #region Actualizacion 26
                            case 26:
                                listaQuerys.Add("ALTER TABLE productoreceta ADD COLUMN \"i_IdAlmacen\" integer;");
                                listaQuerys.Add("ALTER TABLE productoreceta ADD CONSTRAINT \"fk_productoreceta_almacen_i_IdAlmacen\" " +
                                                "FOREIGN KEY (\"i_IdAlmacen\") REFERENCES almacen (\"i_IdAlmacen\") " +
                                                "ON UPDATE NO ACTION ON DELETE NO ACTION;");

                                break;
                            #endregion

                            #region Actualizacion 27
                            case 27:
                                listaQuerys.Add("ALTER TABLE movimientodetallerecetafinal ADD COLUMN \"i_IdAlmacen\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 28
                            case 28:
                                listaQuerys.Add("ALTER TABLE documento ADD COLUMN \"i_OperacionTransitoria\" integer;");
                                listaQuerys.Add("ALTER TABLE documento ADD COLUMN \"v_NroContraCuenta\" character varying(50);");
                                break;
                            #endregion

                            #region Actualizacion 29
                            case 29:
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_EstadoSunat\" smallint;");
                                listaQuerys.Add(@"CREATE TABLE ventahomolagacion(" +
                                                "\"i_IdVentaHomologacion\" serial NOT NULL," +
                                                "\"b_FileXml\" bytea," +
                                                "\"v_IdVenta\" character(16)," +
                                                "CONSTRAINT \"PK_ventahomolagacion\" PRIMARY KEY (\"i_IdVentaHomologacion\")," +
                                                "CONSTRAINT \"Fk_ventahomologacion_venta_v_IdVenta\" FOREIGN KEY (\"v_IdVenta\")" +
                                                "REFERENCES venta (\"v_IdVenta\") MATCH SIMPLE " +
                                                "ON UPDATE NO ACTION ON DELETE NO ACTION ) WITH ( OIDS=FALSE);");
                                listaQuerys.Add("ALTER TABLE ventahomolagacion OWNER TO postgres;");
                                break;
                            #endregion

                            #region Actualizacion 30
                            case 30:
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'FECHA,NROCOMPROBANTE' where " + "\"" + "i_ItemId" + "\"" + "=1 and " + "\"" + "i_GroupId" + "\"" + "=98");
                                break;
                            #endregion

                            #region Actualizacion 31
                            case 31:
                                listaQuerys.Add("ALTER TABLE ventahomolagacion ADD COLUMN \"v_Ticket\" character(16);");
                                listaQuerys.Add("ALTER TABLE ventahomolagacion ADD COLUMN \"b_ResponseTicket\" bytea;");
                                break;
                            #endregion

                            #region Actualizacion 32
                            case 32:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_IdDepartamento\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_IdProvincia\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_IdDistrito\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 33
                            case 33:
                                listaQuerys.Add("update datahierarchy set \"v_Value2\" = LPAD(NULLIF(\"v_Value2\", '')::int::text, 3, '0') where \"i_GroupId\" = 44;");
                                break;
                            #endregion

                            #region Actualizacion 34
                            case 34:
                                listaQuerys.Add(string.Concat("CREATE TABLE configuracionfacturacion(",
                                              "\"i_Idconfiguracionfacturacion\" serial NOT NULL,",
                                              "\"v_Ruc\" character(11),",
                                              "\"v_Usuario\" character(30),",
                                              "\"v_Clave\" character(30),",
                                              "\"v_RazonSocial\" character(100),",
                                              "\"v_NombreComercial\" character(100),",
                                              "\"v_Domicilio\" character(100),",
                                              "\"v_Urbanizacion\" character(25),",
                                              "\"v_Ubigueo\" character(6),",
                                              "\"v_Departamento\" character(30),",
                                              "\"v_Provincia\" character(30),",
                                              "\"v_Distrito\" character(30),",
                                              "\"v_ClaveCertificado\" character(30),",
                                              "\"b_FileCertificado\" bytea,",
                                              "\"i_EsEmisor\" smallint,",
                                              "CONSTRAINT \"PK_i_Idconfiguracionfacturacion\" PRIMARY KEY (\"i_Idconfiguracionfacturacion\") ) WITH (OIDS=FALSE);"));
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion OWNER TO postgres;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa DROP COLUMN \"i_IdDepartamento\"");
                                listaQuerys.Add("ALTER TABLE configuracionempresa DROP COLUMN \"i_IdProvincia\"");
                                listaQuerys.Add("ALTER TABLE configuracionempresa DROP COLUMN \"i_IdDistrito\"");

                                listaQuerys.Add(string.Concat("CREATE TABLE ventaresumenhomologacion(",
                                              "\"i_Idventaresumen\" serial NOT NULL,",
                                              "\"v_Ticket\" character(16),",
                                              "\"b_FileZip\" bytea,",
                                              "\"t_FechaResumen\" date,",
                                              "\"i_InsertaIdUsuario\" integer,",
                                              "\"t_InsertaFecha\" timestamp(6) without time zone,",
                                              "CONSTRAINT \"PK_ventaresumenhomologacion\" PRIMARY KEY (\"i_Idventaresumen\") ) WITH (OIDS=FALSE);"));
                                listaQuerys.Add("ALTER TABLE ventaresumenhomologacion OWNER TO postgres;");

                                listaQuerys.Add(string.Concat("CREATE TABLE ventaelectronicasecuential(",
                                              "\"i_Id\" serial NOT NULL,",
                                              "\"i_IdTipoOperacion\" integer,",
                                              "\"i_NroCorrelativo\" integer,",
                                              "CONSTRAINT \"PK_ventaelectronicasecuential\" PRIMARY KEY (\"i_Id\"))WITH ( OIDS=FALSE);"));
                                listaQuerys.Add("ALTER TABLE ventaelectronicasecuential OWNER TO postgres;");
                                break;
                            #endregion

                            #region Actualizacion 35
                            case 35:
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_IdTipoNota\" integer NULL;");
                                listaQuerys.Add("UPDATE venta SET \"i_IdTipoNota\" = -1;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Clave\" TYPE character varying(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_ClaveCertificado\" TYPE character varying(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Usuario\" TYPE character varying(30);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_RazonSocial\" TYPE character varying(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_NombreComercial\" TYPE character varying(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Domicilio\" TYPE character varying(100);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Urbanizacion\" TYPE character varying(25);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Departamento\" TYPE character varying(30);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Provincia\" TYPE character varying(30);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ALTER COLUMN  \"v_Distrito\" TYPE character varying(30);");
                                break;
                            #endregion

                            #region Actualizacion 36
                            case 36:
                                listaQuerys.Add("ALTER TABLE importacion  ALTER COLUMN \"d_ValorFob\" TYPE numeric(18,6);");
                                break;
                            #endregion

                            #region Actualizacion 37
                            case 37:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IdMonedaImportacion" + "\"" + " integer");
                                listaQuerys.Add("update configuracionempresa set " + "\"" + "i_IdMonedaImportacion" + "\"" + " = 2;");
                                break;
                            #endregion

                            #region Actualizacion 38
                            case 38:
                                listaQuerys.Add("ALTER TABLE ventaresumenhomologacion ADD COLUMN \"i_Estado\" smallint;");
                                break;
                            #endregion

                            #region Actualizacion 39
                            case 39:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"i_GroupUndInter\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"i_GroupNCR\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"i_GroupNDB\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 40
                            case 40:
                                listaQuerys.Add("ALTER TABLE letrasdetalle ADD COLUMN \"v_NroUnico\" character varying(50);");
                                break;
                            #endregion

                            #region Actualizacion 41
                            case 41:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"i_TipoServicio\" smallint;");
                                break;
                            #endregion

                            #region Actualizacion 42
                            case 42:
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD  COLUMN \"t_FechaRegistro\" timestamp(6) without time zone");
                                listaQuerys.Add("update \"importacionDetalleGastos\" set " + "\"" + "t_FechaRegistro" + "\"" + " = " + "\"" + "t_FechaEmision" + "\";");
                                break;

                            #endregion

                            #region Actualizacion 43
                            case 43:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"b_Logo\" bytea;");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"v_Web\" character varying(150);");
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"v_Resolucion\" character varying(50);");
                                break;
                            #endregion

                            #region Actualizacion 44
                            case 44:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaISC\" character varying(20);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaPercepcion\" character varying(20);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaOtrosConsumos\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 45
                            case 45:
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"d_GastosFinancieros\" numeric(18,2);");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"d_IngresosFinancieros\" numeric(18,2);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaGastosFinancierosCobranza\" character varying(20);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaIngresosFinancierosCobranza\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 46
                            case 46:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"d_PrecioCambio\" numeric(12,6);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"d_TotalCambio\"  numeric(10,3);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleProducto\" ADD COLUMN  \"d_CostoUnitarioCambio\" numeric(18,6);");

                                break;
                            #endregion

                            #region Actualizacion 47
                            case 47:
                                listaQuerys.Add("INSERT INTO datahierarchy values (80, 3,'TIPO DOCUMENTO','IDTIPODOCUMENTO','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                break;
                            #endregion

                            #region Actualizacion 48

                            case 48:
                                listaQuerys.Add("CREATE TABLE documentorol" +
                                             "(\"i_IdDocumentoRol\" serial NOT NULL," +
                                             "\"i_CodigoEnum\" integer," +
                                             "\"i_IdTipoDocumento\" integer," +
                                             "\"i_Eliminado\" integer," +
                                             "\"i_InsertaIdUsuario\" integer," +
                                             "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                             "\"i_ActualizaIdUsuario\" integer," +
                                             "\"t_ActualizaFecha\" timestamp(6) without time zone," +
                                             "CONSTRAINT documentorol_pkey PRIMARY KEY (\"i_IdDocumentoRol\")," +
                                             "CONSTRAINT \"fk_documento_i_IdTipoDocumento\" FOREIGN KEY (\"i_IdTipoDocumento\")" +
                                                 "REFERENCES documento (\"i_CodigoDocumento\") MATCH SIMPLE " +
                                                  @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");

                                break;

                            #endregion

                            #region Actualizacion 49
                            case 49:
                                listaQuerys.Add("ALTER TABLE  documentorol DROP  CONSTRAINT \"fk_documento_i_IdTipoDocumento\";");
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"v_NroOrdenCompra\" character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 50
                            case 50:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_CambiarUnidadMedidaVentaPedido" + "\"" + " integer");
                                break;
                            #endregion

                            #region Actualizacion 51
                            case 51:

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirSEUOImpresionDocumentos" + "\"" + " integer");
                                listaQuerys.Add("update configuracionempresa  set " + "\"" + "i_IncluirSEUOImpresionDocumentos" + "\"" + " = 1;");
                                break;
                            #endregion

                            #region  Actualizacion 52

                            case 52:
                                listaQuerys.Add("update datahierarchy set \"v_Value2\" = 'IDTIPODOCUMENTO' , \"v_Value1\" = 'TIPO DOCUMENTO' where \"i_GroupId\" = 81 AND \"i_ItemId\"=1;");
                                break;
                            #endregion

                            #region  Actualizacion 53
                            case 53:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_IdDocumentoContableLEC\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_IdDocumentoContableLEP\" integer;");
                                listaQuerys.Add("update configuracionempresa set \"i_IdDocumentoContableLEC\" = 335;");
                                listaQuerys.Add("update configuracionempresa set \"i_IdDocumentoContableLEP\" = 335;");
                                break;
                            #endregion

                            #region Actualizacion 54
                            case 54:
                                listaQuerys.Add(@"CREATE TABLE productoisc(" +
                                    "\"i_IdProductoIsc\" serial NOT NULL," +
                                    "\"v_IdProducto\" character varying(16)," +
                                    "\"i_IdSistemaIsc\" integer," +
                                    "\"d_Porcentaje\" numeric(4,3)," +
                                    "\"d_Monto\" numeric(7,4)," +
                                    "\"v_Periodo\" character(4)," +
                                    "\"i_InsertaIdUsuario\" integer," +
                                    "\"t_InsertaFecha\" timestamp(6) with time zone," +
                                    "\"i_ActualizaIdUsuario\" integer," +
                                    "\"t_ActualizaFecha\" timestamp(6) with time zone," +
                                    "CONSTRAINT \"Pk_Id_ProductoIsc\" PRIMARY KEY (\"i_IdProductoIsc\")," +
                                    "CONSTRAINT \"Fk_Id_Producto\" FOREIGN KEY (\"v_IdProducto\") " +
                                    "REFERENCES producto (\"v_IdProducto\") MATCH SIMPLE " +
                                    @" ON UPDATE NO ACTION ON DELETE NO ACTION )
									WITH (OIDS=FALSE);");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_EsAfectoIsc\" SMALLINT;");
                                break;
                            #endregion

                            #region Actualizacion 55

                            case 55:
                                // listaQuerys.Add("update datahierarchy set  v_Field='FECHA,NROCOMPROBANTE' where i_GroupId=17 and i_ItemId=1");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'BLS' where " + "\"" + "i_ItemId" + "\"" + "=1 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'CTO' where " + "\"" + "i_ItemId" + "\"" + "=2 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'GAL' where " + "\"" + "i_ItemId" + "\"" + "=3 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'JGO' where " + "\"" + "i_ItemId" + "\"" + "=4 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'KG' where " + "\"" + "i_ItemId" + "\"" + "=5 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'LAM' where " + "\"" + "i_ItemId" + "\"" + "=6 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'L' where " + "\"" + "i_ItemId" + "\"" + "=7 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'M' where " + "\"" + "i_ItemId" + "\"" + "=8 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'PCK' where " + "\"" + "i_ItemId" + "\"" + "=9 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'PQ' where " + "\"" + "i_ItemId" + "\"" + "=10 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'PAR' where " + "\"" + "i_ItemId" + "\"" + "=11 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'PL' where " + "\"" + "i_ItemId" + "\"" + "=12 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'PLI' where " + "\"" + "i_ItemId" + "\"" + "=13 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'RLL' where " + "\"" + "i_ItemId" + "\"" + "=14 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'UND' where " + "\"" + "i_ItemId" + "\"" + "=15 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'VAR' where " + "\"" + "i_ItemId" + "\"" + "=16 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = '1/2 PL' where " + "\"" + "i_ItemId" + "\"" + "=17 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'MIL' where " + "\"" + "i_ItemId" + "\"" + "=18 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'CJA' where " + "\"" + "i_ItemId" + "\"" + "=19 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Field" + "\"" + " = 'DOC' where " + "\"" + "i_ItemId" + "\"" + "=20 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                break;
                            #endregion

                            #region Actualizacion 56
                            case 56:
                                listaQuerys.Add("ALTER TABLE movimientoestadobancario ALTER \"v_Concepto\" TYPE character(250);");
                                break;
                            #endregion

                            #region Actualizacion 57

                            case 57:

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                               "VALUES (0, 151, 'TIPOS DE SISTEMA DE CALCULO DE ISC', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (151, 1, 'SISTEMA AL VALOR', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (151, 2, 'APLICACION DEL MONTO FIJO', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (151, 3, 'SISTEMA DE PRECIOS DE VENTA AL PUBLICO', 0, 1, '2016-07-27');");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle DROP CONSTRAINT \"FK_documentoretenciondetalle_documentoretenciondetalle_v_IdDocu\";");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD CONSTRAINT \"FK_documentoretenciondetalle_v_IdDocumentoRetencion\" FOREIGN KEY (\"v_IdDocumentoRetencion\")" +
                                " REFERENCES documentoretencion(\"v_IdDocumentoRetencion\") MATCH SIMPLE ON UPDATE RESTRICT ON DELETE RESTRICT;");



                                listaQuerys.Add("INSERT INTO datahierarchy  values (0, 152,'TIPO REGIMEN EMPRESAS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (152, 1,'RÉGIMEN GENERAL','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (152, 2,'RER','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (152, 3,'NUEVOS RUS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_TipoRegimenEmpresa" + "\"" + " integer");

                                break;
                            #endregion

                            #region Actualizacion 58
                            case 58:
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"i_EstadoSunat\" smallint;");
                                listaQuerys.Add(@"CREATE TABLE documentoretencionhomologacion(" +
                                                "\"i_Idhomologacion\" serial NOT NULL," +
                                                "\"v_IdDocumentoRetencion\" character(16)," +
                                                "\"b_FileXml\" bytea," +
                                                "\"v_Ticket\" character(16)," +
                                                "\"b_ResponseTicket\" bytea," +
                                                "CONSTRAINT \"PK_documentoretencionhomologacion\" PRIMARY KEY (\"i_Idhomologacion\")," +
                                                "CONSTRAINT \"Fk_retencionhomologacion_documentoretencion\" FOREIGN KEY (\"v_IdDocumentoRetencion\")" +
                                                "REFERENCES documentoretencion (\"v_IdDocumentoRetencion\") MATCH SIMPLE " +
                                                "ON UPDATE NO ACTION ON DELETE NO ACTION ) WITH ( OIDS=FALSE);");
                                listaQuerys.Add("ALTER TABLE documentoretencionhomologacion OWNER TO postgres;");
                                break;
                            #endregion

                            #region Actualizacion 59
                            case 59:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaObligacionesFinancierosCobranza\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 60
                            case 60:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_ImprimirDniPNaturalesLetras" + "\"" + "integer;");
                                break;
                            #endregion

                            #region Actualizacion 61
                            case 61:
                                listaQuerys.Add(@"CREATE TABLE letrasdescuentomantenimiento (" +
                                                   "\"v_IdLetraDescuentoCancelacion\" character varying(16), " +
                                                   "\"v_IdLetrasDetalle\" character varying(16), " +
                                                   "\"t_FechaCancelacion\" timestamp without time zone, " +
                                                   "\"i_Eliminado\" integer, " +
                                                   "\"i_InsertaIdUsuario\" integer, " +
                                                   "\"t_InsertaFecha\" timestamp without time zone, " +
                                                   "\"i_ActualizaIdUsuario\" integer, " +
                                                   "\"t_ActualizaFecha\" timestamp without time zone, " +
                                                   "\"i_Estado\" integer, " +
                                                   "CONSTRAINT \"PK_LetrasDescuentos\" PRIMARY KEY (\"v_IdLetraDescuentoCancelacion\"), " +
                                                   "CONSTRAINT \"FK_letrasdescuentomantenimiento_letrasdetalle_v_IdLetrasDetalle\" FOREIGN KEY (\"v_IdLetrasDetalle\") " +
                                                   "REFERENCES letrasdetalle (\"v_IdLetrasDetalle\") ON UPDATE NO ACTION ON DELETE NO ACTION) " +
                                                   "WITH (OIDS = FALSE);");
                                break;
                            #endregion

                            #region Actualizacion 62
                            case 62:
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value2" + "\"" + " = 'IDTIPODOCUMENTO,NRODOCUMENTO' where " + "\"" + "i_ItemId" + "\"" + "=3 and " + "\"" + "i_GroupId" + "\"" + "=80");
                                break;
                            #endregion

                            #region Actualizacion 63
                            case 63:
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_EsGratuito\" SMALLINT;");
                                break;
                            #endregion

                            #region Actualizacion 64
                            case 64:
                                listaQuerys.Add("INSERT INTO datahierarchy  values (0, 153,'TIPOS DE KARDEX NBS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 1,'KARDEX','K','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 2,'COPIAS CERTIFICADAS','C','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 3,'PEDIDOS','P','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 4,'DECLARATORIAS','D','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 5,'LEGALIZACIÓN','L','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 6,'EXPEDIENTES','E','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 7,'VARIOS','V','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 8,'PRESENCIAL NOTARIAL','N','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 9,'VEHICULAR','H','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 10,'GARANTIAS MOBILIARIAS','M','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (153, 11,'TESTAMENTO','T','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");

                                break;
                            #endregion

                            #region Actualizacion 65
                            case 65:
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"i_EsAbonoLetraDescuento\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 66
                            case 66:
                                //Ingresos
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value1" + "\"" + " = 'ENTRADA POR DEVOLUCION DE PRODUCCION' where " + "\"" + "i_ItemId" + "\"" + "=8 and " + "\"" + "i_GroupId" + "\"" + "=19");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value2" + "\"" + " = '21' where " + "\"" + "i_ItemId" + "\"" + "=15 and " + "\"" + "i_GroupId" + "\"" + "=19");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value2" + "\"" + " = '11' where " + "\"" + "i_ItemId" + "\"" + "=16 and " + "\"" + "i_GroupId" + "\"" + "=20");

                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ADD COLUMN \"d_PrecioVenta\" numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_UsaListaPrecios\" SMALLINT");
                                listaQuerys.Add("UPDATE configuracionempresa SET \"i_UsaListaPrecios\" = 1");

                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 17,'BONIFICACION','07','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 18,'PREMIO','08','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 19,'DONACION','09','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 20,'DESMEDROS','14','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 21,'DESTRUCCION','15','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 22,'EXPORTACION','17','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 23,'MUESTRAS MEDICAS','33','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 24,'PUBLICIDAD','34','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (20, 25,'GASTOS DE REPRESENTACION','35','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                break;
                            #endregion

                            #region Actualizacion 67
                            case 67:
                                listaQuerys.Add("ALTER TABLE letrasmantenimientodetalle ADD COLUMN \"d_GastosAdministrativos\" numeric(18,3);");
                                break;
                            #endregion

                            #region Actualizacion 68
                            case 68:
                                // listaQuerys.Add("INSERT INTO datahierarchy  values (79, 4,'TIPO DOC. VENTA','GROUPHEADERSECTION1,GROUPFOOTERSECTION1','TIPODOCUMENTOVENTA',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");

                                break;
                            #endregion

                            #region Actaulizacion 69
                            case 69:
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"i_IdTipoDocumento\" INTEGER;");
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"v_SerieDocumento\" CHAR(4);");
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"v_CorrelativoDocumento\" CHARACTER VARYING(8);");
                                break;
                            #endregion

                            #region Actualizacion 70
                            case 70:
                                listaQuerys.Add("ALTER TABLE pedido ADD COLUMN \"t_FechaDespacho\" timestamp(6) without time zone ;");
                                listaQuerys.Add("ALTER TABLE pedido ADD COLUMN \"v_IdAgenciaTransporte\" CHARACTER VARYING(16) NULL;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirAgenciaTransportePedido" + "\"" + " integer");

                                break;
                            #endregion

                            #region Actualizacion 71
                            case 71:
                                listaQuerys.Add("ALTER TABLE letrasdescuentomantenimiento ADD COLUMN \"d_Saldo\" numeric(18,3);");
                                listaQuerys.Add("ALTER TABLE letrasdescuentomantenimiento ADD COLUMN \"d_Acuenta\" numeric(18,3);");
                                listaQuerys.Add("ALTER TABLE agenciatransporte ALTER \"v_Telefono\" TYPE character varying(120);");
                                break;
                            #endregion

                            #region Actualizacion 72
                            case 72:
                                listaQuerys.Add("UPDATE datahierarchy SET \"v_Value1\" ='Gravado - Operación Onerosa', \"v_Value2\" = '10', \"v_Field\" = '1' WHERE \"i_GroupId\" = 35 AND \"i_ItemId\" = 1;");
                                listaQuerys.Add("UPDATE datahierarchy SET \"v_Value1\" ='Exonerado - Operación Onerosa', \"v_Value2\" = '20', \"v_Field\" = '1' WHERE \"i_GroupId\" = 35 AND \"i_ItemId\" = 2;");
                                listaQuerys.Add("UPDATE datahierarchy SET \"v_Value1\" ='Inafecto - Operación Onerosa', \"v_Value2\" = '30', \"v_Field\" = '1' WHERE \"i_GroupId\" = 35 AND \"i_ItemId\" = 3;");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 4, 'Exportación', '40', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 5, 'Mixto', '50', '1', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 11, 'Gravado - Retiro por premio', '11', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 12, 'Gravado - Retiro por donación', '12', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 13, 'Gravado - Retiro', '13', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 14, 'Gravado - Retiro por publicidad', '14', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 15, 'Gravado - Bonificaciones', '15', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 16, 'Gravado - Retiro por entrega a trabajadores', '16', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 17, 'Gravado - IVAP', '17', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 21, 'Exonerado - Transferencia Gratuita', '21', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 31, 'Inafecto - Retiro por Bonificación', '31', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 32, 'Inafecto - Retiro', '32', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 33, 'Inafecto - Retiro por Muestras Médicas', '33', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 34, 'Inafecto -  Retiro por Convenio Colectivo', '34', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 35, 'Inafecto - Retiro por premio', '35', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                               "VALUES (35, 36, 'Inafecto -  Retiro por publicidad', '36', '0', 0, 0 , 0, 1, '2016-06-24')");
                                listaQuerys.Add("UPDATE cliente SET \"i_IdTipoIdentificacion\" = 1, \"v_NroDocIdentificacion\" = '00000000' WHERE \"v_IdCliente\" LIKE '%-CL000000000'");
                                break;
                            #endregion

                            #region Actualizacion 73
                            case 73:
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN  \"i_IdTipoOperacion\" set NOT NULL;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_EditarPrecioVentaPedido\" integer");
                                listaQuerys.Add("update configuracionempresa set \"i_EditarPrecioVentaPedido\" = 0;");
                                break;
                            #endregion

                            #region Actualizacion 74
                            case 74:
                                listaQuerys.Add("ALTER TABLE pedido ADD  COLUMN \"i_IdTipoOperacion\" INT DEFAULT 1 NOT NULL");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  COLUMN \"i_IdTipoOperacion\" INT NULL");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  COLUMN \"v_GlosaTicket\" CHARACTER VARYING(600)");
                                break;
                            #endregion

                            #region Actualizacion 75
                            case 75:
                                listaQuerys.Add(@"CREATE TABLE productoinventario " +
                                "(\"i_IdProductoInventario\" serial NOT NULL," +
                                "  \"i_IdEstablecimiento\" integer," +
                                "  \"i_IdAlmacen\" integer," +
                                "  \"v_IdProducto\" character varying(16)," +
                                "  \"d_Cantidad\" numeric(12,2), " +
                                "  \"t_Fecha\" DATE, " +
                                "  CONSTRAINT \"Pk_IdProductoInventario\" PRIMARY KEY (\"i_IdProductoInventario\"), " +
                                "  CONSTRAINT \"Fk_IdAlmacen\" FOREIGN KEY (\"i_IdAlmacen\") " +
                                "      REFERENCES almacen (\"i_IdAlmacen\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION, " +
                                "  CONSTRAINT \"Fk_IdEstablecimiento\" FOREIGN KEY (\"i_IdEstablecimiento\") " +
                                "      REFERENCES establecimiento (\"i_IdEstablecimiento\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION, " +
                                "  CONSTRAINT \"Fk_IdProducto\" FOREIGN KEY (\"v_IdProducto\") " +
                                "      REFERENCES producto (\"v_IdProducto\") MATCH SIMPLE " +
                                     @" ON UPDATE NO ACTION ON DELETE NO ACTION
								)
								WITH (
								  OIDS=FALSE
								);");
                                listaQuerys.Add("ALTER TABLE productoinventario OWNER TO postgres");
                                break;
                            #endregion

                            #region  Actualizacion 76
                            case 76:
                                // listaQuerys.Add("ALTER TABLE datahierarchy ADD COLUMN " + "\"" + "v_Value4" + "\"" + " character varying (200);");

                                listaQuerys.Add("ALTER TABLE datahierarchy ADD COLUMN \"v_Value4\" character varying(200)");

                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value4" + "\"" + " = '01' where " + "\"" + "i_ItemId" + "\"" + "=5 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy set " + "\"" + "v_Value4" + "\"" + " = '07' where " + "\"" + "i_ItemId" + "\"" + "=15 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value4" + "\"" + " = '08' where " + "\"" + "i_ItemId" + "\"" + "=7 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy set " + "\"" + "v_Value4" + "\"" + " = '09' where " + "\"" + "i_ItemId" + "\"" + "=3 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value4" + "\"" + " = '12' where " + "\"" + "i_ItemId" + "\"" + "=19 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value4" + "\"" + " = '13' where " + "\"" + "i_ItemId" + "\"" + "=18 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value4" + "\"" + " = '15' where " + "\"" + "i_ItemId" + "\"" + "=8 and " + "\"" + "i_GroupId" + "\"" + "=17");
                                listaQuerys.Add("ALTER TABLE transportista ALTER COLUMN \"v_Telefono\" type character varying(200)");

                                listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '02' where " + "\"" + "i_ItemId" + "\"" + "=1 and " + "\"" + "i_GroupId" + "\"" + "=6");
                                listaQuerys.Add("update datahierarchy set " + "\"" + "v_Value2" + "\"" + " = '03' where " + "\"" + "i_ItemId" + "\"" + "=2 and " + "\"" + "i_GroupId" + "\"" + "=6");

                                break;
                            #endregion

                            #region Actualizacion 77
                            case 77:

                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var TipoProductos = dbContext.datahierarchy.Where(l => l.i_GroupId == 6 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = dbContext.datahierarchy.Where(l => l.i_GroupId == 6).Max(l => l.i_ItemId);
                                    var Mercaderia = TipoProductos.Where(l => l.v_Value1.Contains("MERCA")).ToList();
                                    if (Mercaderia.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '01' where " + "\"" + "i_ItemId" + "\"" + "=" + Mercaderia.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6," + MaxItemId + ",'MERCADERIA','01','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }
                                    var ProductoTerminado = TipoProductos.Where(l => l.v_Value1.Contains("TERMINAD")).ToList();

                                    if (ProductoTerminado.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '02' where " + "\"" + "i_ItemId" + "\"" + "=" + ProductoTerminado.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6," + MaxItemId + ",'PRODUCTO TERMINADO','02','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }

                                    var MateriasPrimas = TipoProductos.Where(l => l.v_Value1.Contains("PRIMA")).ToList();
                                    if (MateriasPrimas.Any())
                                    {
                                        foreach (var item in MateriasPrimas)
                                        {
                                            listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '03' where " + "\"" + "i_ItemId" + "\"" + "=" + item.i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                        }
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6," + MaxItemId + ",'MATERIAS PRIMAS Y AUXILIARES-MAT','03','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }

                                    var Envases = TipoProductos.Where(l => l.v_Value1.Contains("ENVASES")).ToList();
                                    if (Envases.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '04' where " + "\"" + "i_ItemId" + "\"" + "=" + Envases.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6," + MaxItemId + ",'ENVASES Y EMBALAJES','04','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }


                                    var SuministrosDiversos = TipoProductos.Where(l => l.v_Value1.Contains("SUMINISTROS")).ToList();
                                    if (SuministrosDiversos.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '05' where " + "\"" + "i_ItemId" + "\"" + "=" + SuministrosDiversos.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6, " + MaxItemId + " ,'SUMINISTROS DIVERSOS','05','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }

                                    var Otros = TipoProductos.Where(l => l.v_Value1.Contains("OTROS")).ToList();
                                    if (Otros.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '99' where " + "\"" + "i_ItemId" + "\"" + "=" + Otros.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6, " + MaxItemId + ",'OTROS','99','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }




                                    var Etiquetas = TipoProductos.Where(l => l.v_Value1.Contains("ETIQUETA")).ToList();
                                    if (Etiquetas.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '07' where " + "\"" + "i_ItemId" + "\"" + "=" + Etiquetas.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6, " + MaxItemId + " ,'ETIQUETAS','07','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }

                                    var ProdProceso = TipoProductos.Where(l => l.v_Value1.Contains("PROCESO")).ToList();
                                    if (ProdProceso.Any())
                                    {
                                        listaQuerys.Add("update datahierarchy  set" + "\"" + "v_Value2" + "\"" + " = '06' where " + "\"" + "i_ItemId" + "\"" + "=" + ProdProceso.FirstOrDefault().i_ItemId.ToString() + " and " + "\"" + "i_GroupId" + "\"" + "=6");
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (6, " + MaxItemId + ",'PRODUCTOS EN PROCESO','06','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }


                                }

                                break;

                            #endregion

                            #region Actualizacion 78
                            case 78:
                                listaQuerys.Add("ALTER TABLE letrascanje ADD COLUMN \"i_EsAdelanto\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaAdelanto\" character varying(20);");
                                listaQuerys.Add("ALTER TABLE adelanto ADD COLUMN \"i_IdDocumentoCaja\" integer; ");
                                break;
                            #endregion

                            #region Actualizacion 79
                            case 79:
                                listaQuerys.Add("ALTER TABLE letrascanje ADD COLUMN \"v_IdAdelanto\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE letrascanje ADD CONSTRAINT \"FK_letrascanje_adelanto_v_IdAdelanto\" FOREIGN KEY (\"v_IdAdelanto\") " +
                                                "REFERENCES adelanto (\"v_IdAdelanto\") ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                break;
                            #endregion

                            #region Actualizacion 80
                            case 80:
                                listaQuerys.Add("UPDATE \"datahierarchy\" SET \"v_Field\" = '1' WHERE \"i_GroupId\" = 35 AND \"i_ItemId\" = 4");
                                break;
                            #endregion

                            #region Actualizacion 81
                            case 81:
                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ADD COLUMN \"d_CantidadCancelada\" NUMERIC(18,4)");
                                break;
                            #endregion

                            #region Actualizacion 82
                            case 82:

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_CodigoPlanContable" + "\"" + " integer");

                                listaQuerys.Add("INSERT INTO datahierarchy  values (0, 154,'CODIGOS PLAN CONTABLE','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 1,'PLAN CONTABLE GENERAL EMPRESARIAL','01','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 2,'PLAN CONTABLE GENERAL REVISADO','02','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 3,'PLAN DE CUENTAS PARA EMPRESAS DEL SISTEMA FINANCIERO SUPERVISADO POR SBS','03','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 4,'PLAN DE CUENTAS PAR ENTIDADES PRESTADORAS DE SALUD,SUPERVISADAS POR SBS','04','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 5,'PLAN DE CUENTAS PARA EMPRESAS DEL SISTEMA ASEGURADOR,SUPERVISADAS POR SBS','05','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 6,'PLAN DE CUENTAS DE DE LAS ADMINISTRADORAS PRIVADAS DE FONDOS DE PENSIONES ,SUPERVISADAS POR SBS','06','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 7,'PLAN CONTABLE GUBERNAMENTAL','07','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (154, 99,'OTROS','99','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");




                                break;
                            #endregion

                            #region Actualizacion 83
                            case 83:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER \"v_DescripcionProducto\" TYPE character varying(2000);");
                                break;
                            #endregion

                            #region Actualizacion 84
                            case 84:
                                listaQuerys.Add(@"ALTER TABLE configuracionempresa ADD COLUMN ""i_TckUseInfo"" smallint;");
                                listaQuerys.Add(@"ALTER TABLE configuracionempresa ADD COLUMN ""v_TckRuc"" character varying(11);");
                                listaQuerys.Add(@"ALTER TABLE configuracionempresa ADD COLUMN ""v_TckRzs"" character varying(200);");
                                listaQuerys.Add(@"ALTER TABLE configuracionempresa ADD COLUMN ""v_TckDireccion"" character varying(250);");
                                listaQuerys.Add(@"ALTER TABLE configuracionempresa ADD COLUMN ""v_TckExt"" character varying(100)");
                                break;
                            #endregion

                            #region Actualizacion 85
                            case 85:
                                listaQuerys.Add(@"ALTER TABLE cliente ADD COLUMN ""v_Password"" character varying(100)");
                                break;
                            #endregion

                            #region Actualizacion 86

                            case 86:
                                listaQuerys.Add("INSERT INTO datahierarchy  values  (41, 6,'LETRA DE CAMBIO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");
                                listaQuerys.Add("INSERT INTO datahierarchy  values  (43, 3,'ANULADO','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387');");

                                break;

                            #endregion

                            #region Actualizacion 87
                            case 87:
                                listaQuerys.Add("ALTER TABLE establecimientodetalle ALTER COLUMN \"v_NombreImpresora\" type character varying(500)");
                                break;
                            #endregion

                            #region Actualizacion 88
                            case 88:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajodetalle ALTER \"v_DescripcionTemporal\" TYPE character varying(2000);");
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturaciondetalle ADD COLUMN \"v_DescripcionTemporal\" character varying(2000);");
                                break;
                            #endregion

                            #region Actualizacion 89
                            case 89:
                                listaQuerys.Add(@"ALTER TABLE nbs_ordentrabajo ADD COLUMN ""i_IdEstado"" integer");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (0, 155,'ESTADO-ORDEN TRABAJO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (155, 1,'ACTIVO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (155, 2,'ANULADO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL)");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value2" + "\"" + " = 'GROUPHEADERSECTION1,GROUPFOOTERSECTION1' where " + "\"" + "i_ItemId" + "\"" + "=2 and " + "\"" + "i_GroupId" + "\"" + "=78");
                                listaQuerys.Add("update datahierarchy  set " + "\"" + "v_Value2" + "\"" + " = 'GROUPHEADERSECTION1,GROUPFOOTERSECTION1' where " + "\"" + "i_ItemId" + "\"" + "=3 and " + "\"" + "i_GroupId" + "\"" + "=78");
                                break;
                            #endregion

                            #region Actualizacion 90
                            case 90:
                                listaQuerys.Add("ALTER TABLE marca DROP CONSTRAINT \"Fk_Linea_Marca_v_IdLinea\"");
                                break;
                            #endregion

                            #region Actualizacion 91
                            case 91:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (0, 156, 'TIPOS DE NOTA CREDITO', 0, 1, '2016-10-20');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 1, 'ANULACION DE LA OPERACIÓN', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 2, 'ANULACION POR ERROR EN EL RUC', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 3, 'CORRECION POR ERROR EN LA DESCRIPCION', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 4, 'DESCUENTO GLOBAL', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 5, 'DESCUENTO POR ITEM', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 6, 'DEVOLUCION TOTAL', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 7, 'DEVOLUCION POR ITEM', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 8, 'BONIFICACION', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (156, 9, 'DISMINUCION EN EL VALOR', 0, 1, '2016-10-20');");


                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (0, 157, 'TIPOS DE NOTA DEBITO', 0, 1, '2016-10-20');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (157, 1, 'INTERESES POR MORA', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (157, 2, 'AUMENTO EN EL VALOR', 0, 1, '2016-10-20');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (157, 3, 'PENALIDADES / OTROS CONCEPTOS', 0, 1, '2016-10-20');");
                                break;
                            #endregion

                            #region Actualizacion 92
                            case 92:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_ValidarStockMinimoProducto" + "\"" + " integer");
                                break;
                            #endregion

                            #region Actualizacion 93
                            case 93:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCtaRetenciones\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 94
                            case 94:
                                listaQuerys.Add("ALTER TABLE nbs_formatounicofacturacion  ALTER COLUMN \"d_Total\" type numeric(18,2)");

                                break;
                            #endregion

                            #region Actualizacion 95
                            case 95:

                                listaQuerys.Add("ALTER TABLE producto  ADD COLUMN \"i_CantidadFabricacionMensual\" integer");
                                break;

                            #endregion

                            #region Actualizacion 96
                            case 96:
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"i_AplicaRetencion\" integer;");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"d_MontoRetencion\" numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"v_NroRetencion\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 97
                            case 97:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (0, 158, 'ESTADO GUIA REMISION', 0, 1, '2016-10-31');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (158, 0, 'ANULADO', 0, 1, '2016-10-31');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (158, 1, 'ACTIVO', 0, 1, '2016-10-31');");
                                break;
                            #endregion

                            #region Actualizacion98

                            case 98:
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN \"i_EsDetraccion\" integer;");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"i_CodigoDetraccion\" integer;");

                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"v_NroDetraccion\" character varying(20);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"d_PorcentajeDetraccion\" numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"t_FechaDetraccion\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"d_ValorSolesDetraccion\" numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"d_ValorDolaresDetraccion\" numeric(18,4);");
                                break;
                            #endregion
                            #region Actualizacion99:

                            case 99:

                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"d_ValorSolesDetraccionNoAfecto\" numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleGastos\" ADD COLUMN  \"d_ValorDolaresDetraccionNoAfecto\" numeric(18,4);");


                                break;

                            #endregion

                            #region Actualizacion 100
                            case 100:
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                            "VALUES (6, 10, 'PRODUCTOS DE SEGUNDA', '02', '0', 0, 0 , 0, 1, '2016-06-24')");
                                break;
                            #endregion

                            #region Actualizacion101
                            case 101:
                                listaQuerys.Add("ALTER TABLE almacen ADD COLUMN  \"i_ValidarStockAlmacen\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 102
                            case 102:
                                listaQuerys.Add("ALTER TABLE guiaremisioncompra  ADD COLUMN  \"v_NroOrdenCompra\" character varying(25);");
                                break;
                            #endregion

                            #region Actualizacion 103
                            case 103:
                                listaQuerys.Add("update asientocontable set \"v_Periodo\" = '2016'");
                                break;
                            #endregion


                            #region Actulizacion 104
                            case 104:
                                listaQuerys.Add("ALTER TABLE planillavariablestrabajador ADD  COLUMN  \"i_TieneVacaciones\"  integer;");

                                break;
                            #endregion

                            #region Actualizacion 105
                            case 105:
                                listaQuerys.Add("update asientocontable set \"v_Periodo\" = '2016'");
                                break;
                            #endregion

                            #region Actualizacion 106
                            case 106:
                                listaQuerys.Add("update linea set \"v_Periodo\" = '2016'");
                                break;
                            #endregion

                            #region Actualizacion 107
                            case 107:
                                listaQuerys.Add(@"CREATE TABLE lineacuenta " +
                                                "(\"i_IdLineaCuenta\" serial NOT NULL, " +
                                                   "\"v_IdLinea\" character varying(16)," +
                                                   "\"v_NroCuentaVenta\" character varying(20), " +
                                                   "\"v_NroCuentaCompra\" character varying(20), " +
                                                   "\"v_NroCuentaDConsumo\" character varying(20),  " +
                                                   "\"v_NroCuentaHConsumo\" character varying(20), " +
                                                   "\"v_Periodo\" character varying(4),  " +
                                                   "\"i_Eliminado\" integer,     " +
                                                   "\"i_InsertaIdUsuario\" integer,           " +
                                                   "\"t_InsertaFecha\" date,   " +
                                                   "\"i_ActualizaIdUsuario\" integer, " +
                                                   "\"t_ActualizaFecha\" date, " +
                                                   "CONSTRAINT \"LineaCuenta_PK\" PRIMARY KEY (\"i_IdLineaCuenta\"), " +
                                                   "CONSTRAINT \"lineacuenta_linea_v_IdLinea\" FOREIGN KEY (\"v_IdLinea\") REFERENCES linea " +
                                                   "(\"v_IdLinea\") ON UPDATE NO ACTION ON DELETE NO ACTION) " +
                                                "WITH (" +
                                                  "OIDS = FALSE " +
                                                    ");");
                                listaQuerys.Add("ALTER TABLE lineacuenta OWNER TO postgres");
                                break;

                            #endregion

                            #region Actualizacion 108
                            case 108:
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"d_DescuentoItem\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 109
                            case 109:
                                listaQuerys.Add("ALTER TABLE compradetalle DROP COLUMN \"d_DescuentoItem\";");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"d_DescuentoItem\" numeric(18,2);");
                                break;
                            #endregion

                            #region Actualizacion 110
                            case 110:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_CostoListaPreciosDiferentesxAlmacen" + "\"" + " integer");
                                break;
                            #endregion


                            #region Actualizacion 111
                            case 111:
                                listaQuerys.Add("ALTER TABLE listapreciodetalle ADD COLUMN \"v_IdProductoDetalle\" CHARACTER VARYING(16) NULL");
                                listaQuerys.Add("ALTER TABLE listapreciodetalle ADD COLUMN \"i_IdAlmacen\" INTEGER NULL");
                                listaQuerys.Add("ALTER TABLE listapreciodetalle add COLUMN \"d_Costo\" numeric (18,4)");
                                listaQuerys.Add("ALTER TABLE listapreciodetalle ADD CONSTRAINT \"fk_listapreciodetalle_lista_v_productodetalle\" " +
                                                 "FOREIGN KEY (\"v_IdProductoDetalle\") REFERENCES productodetalle (\"v_IdProductoDetalle\") " + "ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                break;
                            #endregion


                            #region Actualizacion 112
                            case 112:

                                listaQuerys.Add("UPDATE listapreciodetalle AS li SET  \"v_IdProductoDetalle\" = p.\"v_ProductoDetalleId\",\"i_IdAlmacen\" = p.\"i_IdAlmacen\",\"d_Costo\" =prod.\"d_PrecioCosto\" FROM listapreciodetalle l JOIN productoalmacen p ON l.\"v_IdProductoAlmacen\" = p.\"v_IdProductoAlmacen\"  join productodetalle pd ON p.\"v_ProductoDetalleId\"= pd.\"v_IdProductoDetalle\"  join producto prod on pd.\"v_IdProducto\" = prod.\"v_IdProducto\"  WHERE li.\"v_idListaPrecioDetalle\" = l.\"v_idListaPrecioDetalle\"");


                                break;
                            #endregion

                            #region Actualizacion 113
                            case 113:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_IncluirAlmacenDestinoGuiaRemision" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE guiaremision ADD COLUMN " + "\"" + "i_IdAlmacenDestino" + "\"" + " integer");
                                listaQuerys.Add("ALTER TABLE movimiento ADD \"v_NroGuiaVenta\" CHARACTER VARYING(20) NULL");
                                break;
                            #endregion

                            #region Actualizacion 114
                            case 114:
                                var or = new OperationResult();
                                Utils.AperturaData.InicializaLineas(ref or, "2016");
                                break;
                            #endregion

                            #region Actualizacion 115
                            case 115:
                                listaQuerys.Add("ALTER TABLE concepto ADD COLUMN \"v_Periodo\" character varying(4);");
                                listaQuerys.Add("ALTER TABLE administracionconceptos ADD COLUMN \"v_Periodo\" character varying(4);");
                                listaQuerys.Add("update concepto set \"v_Periodo\" = '2016' where \"i_Eliminado\" = 0;");
                                listaQuerys.Add("update administracionconceptos set \"v_Periodo\" = '2016' where \"i_Eliminado\" = 0;");
                                break;
                            #endregion

                            #region Actualizacion 116
                            case 116:
                                listaQuerys.Add("ALTER TABLE cliente ADD COLUMN \"v_Alias\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"v_NroPartidaArancelaria\" character varying(20);");

                                break;

                            #endregion

                            #region Actualizacion 117
                            case 117:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"i_Automatic\" smallint NULL");
                                listaQuerys.Add("UPDATE configuracionfacturacion SET \"i_Automatic\" = 1");
                                break;
                            #endregion

                            #region Actualizacion 118
                            case 118:
                                listaQuerys.Add("ALTER TABLE vendedor ADD COLUMN \"i_PermiteAnularVentas\" integer;");
                                listaQuerys.Add("ALTER TABLE vendedor ADD COLUMN \"i_PermiteEliminarVentas\" integer;");
                                listaQuerys.Add("UPDATE vendedor SET \"i_PermiteAnularVentas\" = 1");
                                listaQuerys.Add("UPDATE vendedor SET \"i_PermiteEliminarVentas\" = 1");
                                break;
                            #endregion

                            #region Actualizacion 119
                            case 119:
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"v_DescuentoItem\" character varying(20);");
                                break;
                            #endregion

                            #region Actualizacion 120
                            case 120:
                                listaQuerys.Add("ALTER TABLE compradetalle ALTER \"d_DescuentoItem\" TYPE numeric(18,4);");
                                break;
                            #endregion
                            #region Actualizacion 121
                            case 121:

                                listaQuerys.Add("CREATE TABLE ventadetalleanexo" +
                                            "(\"i_IdVentaDetalleAnexo\" serial NOT NULL," +
                                            "\"v_Anexo\" character varying(5000)," +
                                             "\"i_Eliminado\" integer," +
                                              "\"i_InsertaIdUsuario\" integer," +
                                              "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                              "\"i_ActualizaIdUsuario\" integer," +
                                              "\"t_ActualizaFecha\" timestamp(6) without time zone ," +
                                              "CONSTRAINT ventadetalleanexo_pkey PRIMARY KEY (\"i_IdVentaDetalleAnexo\"));");



                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN " + "\"" + "i_IdVentaDetalleAnexo" + "\"" + " integer;");


                                break;
                            #endregion

                            #region Actualizacion 122
                            case 122:
                                listaQuerys.Add("INSERT INTO datahierarchy   values (20, 26,'TRASLADO DE BIENES PARA TRANSFORMACIÓN','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                break;
                            #endregion

                            #region Actualizacion 123
                            case 123:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER COLUMN \"d_Total\" type numeric(18,3)");
                                listaQuerys.Add("ALTER TABLE movimientodetalle  ALTER COLUMN \"d_TotalCambio\" type numeric(18,3)");
                                break;
                            #endregion

                            #region Actualizacion 124


                            case 124:

                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var RegimenLaboral = dbContext.datahierarchy.Where(l => l.i_GroupId == 115 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = RegimenLaboral.Max(l => l.i_ItemId);
                                    var PequeñaEmpresa = RegimenLaboral.Where(l => l.v_Value1.Contains("PEQUE")).ToList();
                                    if (PequeñaEmpresa.Any())
                                    {
                                    }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;

                                        listaQuerys.Add("INSERT INTO datahierarchy values (115," + MaxItemId + ",'PEQUEÑA EMPRESA D. LEY 2086','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }
                                    var Agraria = RegimenLaboral.Where(l => l.v_Value1.Contains("AGRARIA")).ToList();
                                    if (Agraria.Any())
                                    { }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values    (115," + MaxItemId + ",'AGRARIA LEY 23760','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                    var Mineros = RegimenLaboral.Where(l => l.v_Value1.Contains("MINEROS")).ToList();
                                    if (Mineros.Any())
                                    { }
                                    else
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy  values    (115," + MaxItemId + ",'MINEROS','','',NULL,0,0,0,1,'2015-12-03 14:45:50.0092387',NULL ,NULL )");
                                    }
                                }

                                break;

                            #endregion

                            #region Actualizacion 125
                            case 125:
                                listaQuerys.Add(@"CREATE TABLE planillaafectacionesgenerales" +
                                                "(   \"i_Id\" serial NOT NULL, " +
                                                    "\"v_Periodo\" character(4), " +
                                                    "\"v_Mes\" character(2),     " +
                                                    "\"v_IdConceptoPlanilla\" character varying(16),  " +
                                                    "\"i_Leyes_Trab_ONP\" integer,   " +
                                                    "\"i_Leyes_Trab_Senati\" integer,  " +
                                                    "\"i_Leyes_Trab_SCTR\" integer,    " +
                                                    "\"i_Leyes_Emp_Essalud\" integer," +
                                                    "\"i_Leyes_Emp_SCTR\" integer,    " +
                                                    "\"i_AFP_Afecto\" integer,       " +
                                                    "\"i_Rent5ta_Afecto\" integer,   " +
                                                    "\"i_Eliminado\" integer,         " +
                                                    "\"t_InsertaFecha\" time with time zone,   " +
                                                    "\"i_InsertaIdUsuario\" integer,         " +
                                                    "\"t_ActualizaFecha\" time with time zone,    " +
                                                    "\"i_ActualizaIdUsuario\" integer, " +
                                                    "CONSTRAINT \"PK_planillaafectacionesgenerales\" PRIMARY KEY (\"i_Id\")," +
                                                    "CONSTRAINT \"FK_planillaafectacionesgenerales_planillaconceptos\" FOREIGN KEY (\"v_IdConceptoPlanilla\") " +
                                                    "REFERENCES planillaconceptos (\"v_IdConceptoPlanilla\") ON UPDATE NO ACTION ON DELETE NO ACTION" +
                                                @") 
												WITH (
													OIDS = FALSE
												);");
                                break;
                            #endregion

                            #region Actualizacion 126
                            case 126:
                                listaQuerys.Add("ALTER TABLE planillaafectacionesgenerales DROP COLUMN \"t_InsertaFecha\";");
                                listaQuerys.Add("ALTER TABLE planillaafectacionesgenerales DROP COLUMN \"t_ActualizaFecha\";");
                                listaQuerys.Add("ALTER TABLE planillaafectacionesgenerales ADD COLUMN \"t_InsertaFecha\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaafectacionesgenerales ADD COLUMN \"t_ActualizaFecha\" timestamp(6) without time zone;");
                                break;
                            #endregion

                            #region Actualizacion 127
                            case 127:
                                var listaInsertar = new List<planillaafectacionesgeneralesDto>();

                                using (var dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var limpiarAfectacion = dbContext.planillaafectacionesgenerales.ToList();
                                    limpiarAfectacion.ForEach(o => dbContext.DeleteObject(o));
                                    dbContext.SaveChanges();

                                    var leyestrab = dbContext.planillaafecleyestrab.Where(p => p.i_Eliminado == 0).ToList();
                                    var leyesemp = dbContext.planillaafecleyesemp.Where(p => p.i_Eliminado == 0).ToList();
                                    var afectosafp = dbContext.planillaafecafp.Where(p => p.i_Eliminado == 0).ToList();
                                    var conceptosExistentes = leyestrab.Select(p => new
                                    {
                                        idConcepto = p.v_IdConceptoPlanilla,
                                        mes = p.v_Mes,
                                        periodo = p.v_Periodo,
                                        tipo = "O"
                                    });
                                    conceptosExistentes = conceptosExistentes.Concat(
                                        leyesemp.Select(p => new
                                        {
                                            idConcepto = p.v_IdConceptoPlanilla,
                                            mes = p.v_Mes,
                                            periodo = p.v_Periodo,
                                            tipo = "E"
                                        }));
                                    conceptosExistentes = conceptosExistentes.Concat(
                                        afectosafp.Select(p => new
                                        {
                                            idConcepto = p.v_IdConceptoPlanilla,
                                            mes = p.v_Mes,
                                            periodo = p.v_Periodo,
                                            tipo = "A"
                                        }));
                                    conceptosExistentes = conceptosExistentes.Distinct();

                                    foreach (var agrupadoMesPeriodo in conceptosExistentes.
                                        GroupBy(g => new { p = g.periodo, m = g.mes, i = g.idConcepto }))
                                    {
                                        listaInsertar.Add(
                                                new planillaafectacionesgeneralesDto
                                                {
                                                    v_IdConceptoPlanilla = agrupadoMesPeriodo.Key.i,
                                                    i_Leyes_Trab_ONP = agrupadoMesPeriodo.Any(i => i.tipo.Equals("O")) ? 1 : 0,
                                                    i_Leyes_Emp_Essalud = agrupadoMesPeriodo.Any(i => i.tipo.Equals("E")) ? 1 : 0,
                                                    i_AFP_Afecto = agrupadoMesPeriodo.Any(i => i.tipo.Equals("A")) ? 1 : 0,
                                                    i_Leyes_Emp_SCTR = 0,
                                                    i_Leyes_Trab_SCTR = 0,
                                                    i_Leyes_Trab_Senati = 0,
                                                    i_Rent5ta_Afecto = 0,
                                                    v_Mes = agrupadoMesPeriodo.Key.m,
                                                    v_Periodo = agrupadoMesPeriodo.Key.p
                                                });
                                    }

                                    var conceptosEssalud = dbContext.planillaconceptos
                                           .Where(p => p.v_ColumnaAfectaciones.Equals("i_Essalud")).ToList();

                                    foreach (var planillaconceptose in conceptosEssalud)
                                    {
                                        planillaconceptose.v_ColumnaAfectaciones = "i_Leyes_Emp_Essalud";
                                        dbContext.planillaconceptos.ApplyCurrentValues(planillaconceptose);
                                    }

                                    foreach (var objEntity in listaInsertar.Select(entityDto => entityDto.ToEntity()))
                                    {
                                        objEntity.i_InsertaIdUsuario = 1;
                                        objEntity.i_Eliminado = 0;
                                        dbContext.AddToplanillaafectacionesgenerales(objEntity);
                                    }

                                    pobjOperationResult.Success = 1;
                                    dbContext.SaveChanges();
                                }
                                break;
                            #endregion

                            #region Actualizacion 128
                            case 128:
                                listaQuerys.Add("ALTER TABLE trabajador ADD COLUMN \"b_HojaVida\" bytea;");
                                break;
                            #endregion

                            #region Actualizacion 129
                            case 129:

                                listaQuerys.Add(@"CREATE TABLE cajachica " +
                                    "(\"v_IdCajaChica\" character varying(16), " +
                                                   "\"v_Periodo\" character varying(4), " +
                                                    "\"v_Mes\" character varying(2), " +
                                                     "\"v_Correlativo\" character varying(16), " +
                                                   "\"t_FechaRegistro\" timestamp without time zone, " +
                                                     "\"d_TotalIngresos\" NUMERIC(12,2), " +
                                                       "\"d_TotalGastos\" NUMERIC(12,2), " +
                                                        "\"d_CajaSaldo\" NUMERIC(12,2), " +
                                                   "\"i_Eliminado\" integer, " +
                                                   "\"i_InsertaIdUsuario\" integer, " +
                                                   "\"t_InsertaFecha\" timestamp without time zone, " +
                                                   "\"i_ActualizaIdUsuario\" integer, " +
                                                   "\"t_ActualizaFecha\" timestamp without time zone, " +

                                                   "CONSTRAINT \"PK_IdCajaChica\" PRIMARY KEY (\"v_IdCajaChica\"));");

                                listaQuerys.Add(@"CREATE TABLE conceptoscajachica " +
                               "(\"i_IdConceptosCajaChica\" serial NOT NULL," +
                               "\"v_NombreConceptoCajaChica\" character varying(100)," +
                               "\"v_NroCuenta\" character varying(10)," +
                               "\"d_Cantidad\" numeric(12,2), " +
                               "\"i_Eliminado\" integer, " +
                                                "\"i_InsertaIdUsuario\" integer, " +
                                                "\"t_InsertaFecha\" timestamp without time zone, " +
                                                "\"i_ActualizaIdUsuario\" integer, " +
                                                "\"t_ActualizaFecha\" timestamp without time zone, " +
                               "CONSTRAINT \"Pk_i_IdConceptosCajaChica\" PRIMARY KEY (\"i_IdConceptosCajaChica\"));");



                                listaQuerys.Add(@"CREATE TABLE cajachicadetalle " +
                                                "(\"v_IdCajaChicaDetalle\" character varying(16), " +
                                                  "\"v_IdCajaChica\" character varying(16), " +
                                                   "\"i_Motivo\" integer, " +
                                                      "\"v_Usuario\" character varying(100), " +
                                                        "\"v_NombreConceptoCajaChica\" character varying(100), " +
                                                         "\"v_Observacion\" character varying(100), " +
                                                        "\"i_IdConceptosCajaChica\" integer, " +
                                                 "\"i_Eliminado\" integer, " +
                                                 "\"i_InsertaIdUsuario\" integer, " +
                                                 "\"t_InsertaFecha\" timestamp without time zone, " +
                                                 "\"i_ActualizaIdUsuario\" integer, " +
                                                 "\"t_ActualizaFecha\" timestamp without time zone, " +

                                  "CONSTRAINT PK_IdCajaChicaDetalle PRIMARY KEY (\"v_IdCajaChicaDetalle\")," +
                                              "CONSTRAINT \"v_IdCajaChica\" FOREIGN KEY (\"v_IdCajaChica\")" +
                                                  "REFERENCES cajachica (\"v_IdCajaChica\") MATCH SIMPLE " +
                                                  "ON UPDATE RESTRICT ON DELETE RESTRICT," +
                                              "CONSTRAINT \"i_IdConceptosCajaChica\" FOREIGN KEY (\"i_IdConceptosCajaChica\")" +
                                                  "REFERENCES conceptoscajachica (\"i_IdConceptosCajaChica\") MATCH SIMPLE " +
                                                  @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");



                                break;

                            #endregion


                            #region Actualizacion 130

                            case 130:
                                listaQuerys.Add("ALTER TABLE cajachica ADD \"i_IdTipoDocumento\" integer");
                                listaQuerys.Add("ALTER TABLE cajachicadetalle ADD \"i_IdTipoDocumento\" integer");
                                listaQuerys.Add("ALTER TABLE cajachicadetalle ADD \"v_NroDocumento\" character varying(20)");
                                break;


                            #endregion

                            #region Actualizacion 131
                            case 131:

                                listaQuerys.Add("ALTER TABLE cajachica ADD \"i_IdEstado\" integer");
                                break;
                            #endregion


                            #region Actualizacion 132
                            case 132:

                                listaQuerys.Add("ALTER TABLE cajachicadetalle ADD \"d_Importe\" numeric(12,2)");
                                break;
                            #endregion


                            #region Actualizacion 133
                            case 133:
                                listaQuerys.Add("ALTER TABLE cajachica ADD \"d_TipoCambio\" numeric(18,4)");
                                listaQuerys.Add("ALTER TABLE cajachica ADD \"i_IdMoneda\" integer");
                                break;

                            #endregion

                            #region Actualizacion 134
                            case 134:
                                listaQuerys.Add(@"delete from conceptoscajachica");
                                listaQuerys.Add(@"Insert into conceptoscajachica(""i_IdConceptosCajaChica"" ,""v_NombreConceptoCajaChica"",""v_NroCuenta"",""i_Eliminado"",""i_InsertaIdUsuario"",""t_InsertaFecha"")values (1,'APERTURA','',0,1,'2017-01-24')");



                                break;
                            #endregion

                            #region Actualizacion 135


                            case 135:

                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var UnidadMedida = dbContext.datahierarchy.Where(l => l.i_GroupId == 17 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = dbContext.datahierarchy.Where(l => l.i_GroupId == 17).Max(l => l.i_ItemId);
                                    var Toneladas = UnidadMedida.Where(l => l.v_Value1.Contains("MERCA")).ToList();
                                    if (!Toneladas.Any())
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy values (17," + MaxItemId + ",'TONELADA METRICA','1','TM',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                    }


                                }
                                break;
                            #endregion
                            #region Actualizacion 136
                            case 136:


                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (0, 159, 'VENTA-TIPO BULTO', 0, 1, '2016-10-31');");

                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (159, 0, 'CAJAS', 0, 1, '2016-10-31');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (159, 1, 'SACOS DE PAPEL', 0, 1, '2016-10-31');");

                                listaQuerys.Add("ALTER TABLE venta ADD \"i_IdTipoBulto\" integer");

                                break;
                            #endregion


                            #region Actualizacion 137
                            case 137:
                                listaQuerys.Add("ALTER TABLE conceptoscajachica ADD \"i_RequiereTipoDocumento\" integer");
                                listaQuerys.Add("ALTER TABLE conceptoscajachica ADD \"i_RequiereNumeroDocumento\" integer");
                                listaQuerys.Add("ALTER TABLE conceptoscajachica ADD \"i_RequiereAnexo\" integer");
                                listaQuerys.Add("ALTER TABLE cajachicadetalle ADD \"v_IdCliente\" varchar (20)");
                                break;
                            #endregion

                            #region Actualizacion 138
                            case 138:

                                if (RucEmpresa == Constants.RucDetec)
                                {
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (0, 160, 'VENTA-TIPO BULTO', 0, 1, '2016-10-31');");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (160, 0, 'CAJAS', 0, 1, '2016-10-31');");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (160, 1, 'SACOS DE PAPEL', 0, 1, '2016-10-31');");


                                }
                                else
                                {
                                    listaQuerys.Add("DELETE from datahierarchy where " + "\"" + "i_GroupId" + "\"" + "=159");
                                    listaQuerys.Add("DELETE from datahierarchy where " + "\"" + "i_ItemId" + "\"" + "=159");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") VALUES (0, 159, 'UNIDADES DE MEDIDA INTER.', 0, 1, '2016-10-31');");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")  values (159, 1,'bag','BG',0,1, '2015-12-03 14:45:50.0092387')");


                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")  values (159, 2,'hundred','CEN',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")  values (159, 3,'gal','A76',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")  values (159, 4,'gallon (US)','GLL',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 5,'kilogram','KGM',0,1, '2015-12-03 14:45:50.0092387' )");



                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""v_Value2"",""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")  values (159, 6,'sheet','ST',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 7,'slipsheet','SL',0,1, '2015-12-03 14:45:50.0092387' )");


                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 8,'cubic centimetre','CMQ',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 9,'centimetre','CMT',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""v_Value2"",""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 10,'cubic millimetre','MMQ',0,1, '2015-12-03 14:45:50.0092387' )");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""v_Value2"",""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 11,'millimetre','MMT',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 12,'metre','MTR',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 13,'ton (US shipping)','L86',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 14,'bulk pack','AB',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 15,'bar [unit of packaging]','BR',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""v_Value2"",""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 16,'packet','PA',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""v_Value2"",""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 17,'pack','PK',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 18,'number of pair','NPR',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 19,'pair','PR',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 20,'number of international units','NIU',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 21,'thousand','MIL',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 22,'thousand bag','T4',0,1, '2015-12-03 14:45:50.0092387')");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 23,'box','BX',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 24,'hundred boxes','HBX',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"",""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 25,'dozen','DZN',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 26,'dozen pack','DZP',0,1, '2015-12-03 14:45:50.0092387')");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 27,'number of rolls','NRL',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 28,'roll','RO',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 29,'rod','RD',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 30,'thousand sheet','TW',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 31,'set','SET',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 32,'litre','LTR',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 33,'millilitre','MLT',0,1, '2015-12-03 14:45:50.0092387' )");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"",""v_Value2"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") values (159, 34,'otro','ZZ',0,1, '2015-12-03 14:45:50.0092387' )");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (0, 160, 'VENTA-TIPO BULTO', 0, 1, '2016-10-31');");

                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (160, 0, 'CAJAS', 0, 1, '2016-10-31');");
                                    listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"") 
								VALUES (160, 1, 'SACOS DE PAPEL', 0, 1, '2016-10-31');");
                                }
                                break;
                            #endregion


                            #region Actualizacion 139
                            case 139:




                                listaQuerys.Add("update datahierarchy set  \"v_Value2\" ='9' where \"i_GroupId\"=29 and \"i_ItemId\"=1");
                                listaQuerys.Add("update datahierarchy set \"v_Value2\"='3.85' where  \"i_GroupId\"=29 and \"i_ItemId\"=2");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=3");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=4");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=5");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=6");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=7");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=8");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=9");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='15' where  \"i_GroupId\"=29 and \"i_ItemId\"=10");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=11");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=12");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='9' where  \"i_GroupId\"=29 and \"i_ItemId\"=13");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=14");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='9' where  \"i_GroupId\"=29 and \"i_ItemId\"=15");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=16");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=17");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=18");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=19");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=20");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=21");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=22");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=22");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=23");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=24");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=25");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=26");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=27");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='9' where  \"i_GroupId\"=29 and \"i_ItemId\"=28");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='9' where  \"i_GroupId\"=29 and \"i_ItemId\"=29");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=30");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=31");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=32");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=33");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=34");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='1.5' where  \"i_GroupId\"=29 and \"i_ItemId\"=35");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='1.5' where  \"i_GroupId\"=29 and \"i_ItemId\"=36");
                                listaQuerys.Add("update datahierarchy set  \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=37");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=38");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='10' where  \"i_GroupId\"=29 and \"i_ItemId\"=39");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='4' where  \"i_GroupId\"=29 and \"i_ItemId\"=40");
                                listaQuerys.Add("update datahierarchy set   \"v_Value2\"='' where  \"i_GroupId\"=29 and \"i_ItemId\"=41");


                                break;

                            #endregion

                            #region Actualizacion 140
                            case 140:

                                listaQuerys.Add("ALTER TABLE producto ADD \"i_IndicaFormaParteOtrosTributos\" integer");

                                break;
                            #endregion

                            #region Actualizacion 141
                            case 141:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE compradetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE compradetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE guiaremisiondetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleProducto\" ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleProducto\" ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE \"temporalVentaDetalle\" ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE \"temporalVentaDetalle\" ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ALTER \"d_Cantidad\" TYPE numeric(21,7);");
                                listaQuerys.Add("ALTER TABLE ordendecompradetalle ALTER \"d_CantidadEmpaque\" TYPE numeric(21,7);");
                                break;
                            #endregion

                            #region Actualizacion 142
                            case 142:
                                listaQuerys.Add("delete from saldoscontables");
                                break;
                            #endregion

                            #region Actualizacion 143
                            case 143:
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"v_IdCompra\" character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 144
                            case 144:
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"i_Eliminado\" integer;");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"i_ActualizaUsuario\" integer;");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"t_ActualizaFecha\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"i_Eliminado\" integer;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"i_ActualizaUsuario\" integer;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"t_ActualizaFecha\" timestamp(6) without time zone;");
                                break;
                            #endregion

                            #region Actualizacion 145
                            case 145:
                                listaQuerys.Add("ALTER TABLE tesoreriadetalle ADD COLUMN \"v_IdDocumentoRetencionDetalle\" character(16);");
                                listaQuerys.Add("ALTER TABLE tesoreriadetalle ADD " +
                                                "CONSTRAINT \"FK_tesoreriadetalle_documentoretenciondetalle_v_IdDocumentoRetencionDetalle\" " +
                                                "FOREIGN KEY (\"v_IdDocumentoRetencionDetalle\") REFERENCES documentoretenciondetalle " +
                                                "(\"v_IdDocumentoRetencionDetalle\") ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"i_IdMoneda\" integer;");
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"d_Cambio\" numeric(18,2);");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"d_TipoCambio\" numeric(18,3);");
                                break;
                            #endregion

                            #region Actualizacion 146
                            case 146:
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"i_Procesado\" integer;");
                                listaQuerys.Add("ALTER TABLE activofijo ADD \"v_PeriodoAnterior\" character(5);");
                                break;
                            #endregion

                            #region Actualizacion 147
                            case 147:
                                listaQuerys.Add("ALTER TABLE tesoreria ADD COLUMN \"v_IdDocumentoRetencion\" character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 148
                            case 148:
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"v_NroParte\" character varying(30)");
                                break;
                            #endregion

                            #region Actualizacion 149
                            case 149:
                                listaQuerys.Add("ALTER TABLE documentoretenciondetalle ADD COLUMN \"i_Independiente\" integer;");
                                break;
                            #endregion


                            #region Actualizacion 150
                            case 150:
                                listaQuerys.Add("DROP TABLE saldoscontables;");
                                break;

                            #endregion

                            #region Actualizacion 151
                            case 151:
                                listaQuerys.Add("ALTER TABLE destino ADD COLUMN \"v_Periodo\" character varying(4);");
                                listaQuerys.Add("update destino set \"v_Periodo\" = '2017' where \"v_IdDestino\" in (" +
                                                "select \"v_IdDestino\" from destino s " +
                                                "join asientocontable a on s.\"v_CuentaOrigen\" = a.\"v_NroCuenta\" " +
                                                "where a.\"v_Periodo\" = '2017' and s.\"i_Eliminado\" = 0 and a.\"i_Eliminado\" = 0)");
                                listaQuerys.Add(
                                    "update destino set \"v_Periodo\" = '2016' where \"v_Periodo\" is null and \"i_Eliminado\" = 0");
                                break;
                            #endregion

                            #region Actualizacion 152
                            case 152:
                                var or2 = new OperationResult();
                                Utils.AperturaData.InicializarDestinos(ref or2, "2017", "2016");
                                break;
                            #endregion

                            #region Actualizacion 153
                            case 153:
                                listaQuerys.Add("CREATE TABLE clientedirecciones" +
                                          "(\"i_IdDireccionCliente\" serial NOT NULL ," +
                                          "\"v_Direccion\" character varying(200)," +
                                          "\"i_IdZona\" integer," +
                                          "\"v_IdCliente\" character varying(16)," +
                                          "\"i_IdDepartamento\" integer," +
                                          "\"i_IdProvincia\" integer," +
                                          "\"i_IdDistrito\" integer," +
                                            "\"i_EsDireccionPredeterminada\" integer," +
                                           "\"i_Eliminado\" integer," +
                                            "\"i_InsertaIdUsuario\" integer," +
                                            "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                            "\"i_ActualizaIdUsuario\" integer," +
                                            "\"t_ActualizaFecha\" timestamp(6) without time zone ," +
                                            "CONSTRAINT clientedirecciones_pkey PRIMARY KEY (\"i_IdDireccionCliente\")," +
                                             "CONSTRAINT \"fk_clientedireccion_i_IdDireccion\" FOREIGN KEY (\"v_IdCliente\")" +
                                                  "REFERENCES cliente (\"v_IdCliente\") MATCH SIMPLE " +
                                                  @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");

                                listaQuerys.Add("ALTER TABLE pedido ADD \"i_IdDireccionCliente\" int");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                               "VALUES (0, 161, 'ZONAS DIRECCIONES', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 1, 'HUAROCHIRI', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 2, 'S.J.L.', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 3, 'CANTO REY', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                               "VALUES (161, 4, 'SAN CARLOS', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 5, 'CANTO GRANDE', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 6, 'LAS FLORES', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 7, 'APOLO', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 8, 'RIO CHINCHA', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 9, 'SAN JUAN', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 10, 'EL AIRE', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161,11, 'BEINGOLEA', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 12, 'CHANCAS', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 13, 'SANTA ANITA', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161,14, 'METRO', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 15, 'TILDA', 0, 1, '2016-07-27');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (161, 16, 'ALIPIO', 0, 1, '2016-07-27');");
                                break;
                            #endregion

                            #region Actualizacion 154
                            case 154:
                                listaQuerys.Add("ALTER TABLE venta ADD \"i_IdDireccionCliente\" int");
                                listaQuerys.Add("ALTER TABLE movimiento ADD \"i_IdDireccionCliente\" int");
                                listaQuerys.Add("ALTER TABLE guiaremision ADD \"i_IdDireccionCliente\" int");
                                Utils.AperturaData.IniciarDirecionesClientes(ref objOperationResult, TipoMotorBD.PostgreSQL);

                                if (objOperationResult.Success == 0)
                                {
                                    return;
                                }
                                break;
                            #endregion

                            #region Actualizacion 155
                            case 155:
                                listaQuerys.Add("ALTER TABLE configuracionEmpresa ADD \"i_VisualizarColumnasBasicasPedido\" integer");

                                break;
                            #endregion

                            #region Actualizacion 156
                            case 156:
                                listaQuerys.Add("ALTER TABLE venta ALTER \"v_DireccionClienteTemporal\" TYPE character varying(2150);");

                                break;
                            #endregion

                            #region Actualizacion 157
                            case 157:
                                listaQuerys.Add(@"CREATE TABLE planillavaloresrentaquinta " +
                                        "(\"i_Id\" serial, " +
                                            "\"v_Periodo\" character varying(4), " +
                                            "\"i_MaxUIT\" integer, " +
                                            "\"d_Porcentaje\" numeric(7,2), " +
                                            "\"i_Eliminado\" integer, " +
                                            "\"i_InsertaIdUsuario\" integer, " +
                                            "\"t_InsertaFecha\" timestamp(6) without time zone, " +
                                            "\"i_ActualizaIdUsuario\" integer, " +
                                            "\"t_ActualizaFecha\" timestamp(6) without time zone, " +
                                            "CONSTRAINT \"PK_planillavaloresrentaquinta\" PRIMARY KEY (\"i_Id\") USING INDEX TABLESPACE pg_default" +
                                        @") 
										WITH (
											OIDS = FALSE
										)
										;");

                                listaQuerys.Add(@"CREATE TABLE planillavaloresuit " +
                                            "(" +
                                                "\"i_Id\" serial, " +
                                                "\"v_Periodo\" character varying(4), " +
                                                "\"d_ValorUIT\" numeric(7,2), " +
                                                "\"i_FactorUIT\" integer, " +
                                                "\"i_Eliminado\" integer, " +
                                                "\"i_InsertaIdUsuario\" integer, " +
                                                "\"t_InsertaFecha\" timestamp(6) without time zone, " +
                                                "\"i_ActualizaIdUsuario\" integer, " +
                                                "\"t_ActualizaFecha\" timestamp(6) without time zone, " +
                                                "CONSTRAINT \"PK_planillavaloresuit\" PRIMARY KEY (\"i_Id\")" +
                                            @") 
											WITH (
												OIDS = FALSE
											)
											;");
                                break;
                            #endregion

                            #region Actualizacion 158
                            case 158:
                                listaQuerys.Add("ALTER TABLE diariodetalle ALTER \"v_Analisis\" TYPE character varying(1150);");
                                listaQuerys.Add("DROP TABLE planillavaloresrentaquinta;");
                                listaQuerys.Add(@"CREATE TABLE planillavaloresrentaquinta " +
                                                "(\"i_Id\" serial NOT NULL," +
                                                "\"v_Periodo\" character varying(4)," +
                                                "\"i_Tope1\" integer," +
                                                "\"d_Porcentaje1\" numeric(7,2)," +
                                                "\"i_Tope2\" integer," +
                                                "\"d_Porcentaje2\" numeric(7,2)," +
                                                "\"i_Tope3\" integer," +
                                                "\"d_Porcentaje3\" numeric(7,2)," +
                                                "\"i_Tope4\" integer," +
                                                "\"d_Porcentaje4\" numeric(7,2)," +
                                                "\"d_Porcentaje4Superior\" numeric(7,2)," +
                                                "\"i_Eliminado\" integer," +
                                                "\"i_InsertaIdUsuario\" integer, " +
                                                "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                                "\"i_ActualizaIdUsuario\" integer," +
                                                "\"t_ActualizaFecha\" timestamp(6) without time zone, " +
                                                "CONSTRAINT \"PK_planillavaloresrentaquinta\" PRIMARY KEY (\"i_Id\") " +
                                                @")
												WITH (
												  OIDS=FALSE
												);");

                                listaQuerys.Add(@"CREATE TABLE planillavaloresproyeccionquinta " +
                                                "( " +
                                                  "\"v_Periodo\" character varying(4)," +
                                                  "\"i_IdMes\" integer," +
                                                  "\"i_MesesProyectados\" integer," +
                                                  "\"i_GratificacionesProyectadas\" integer," +
                                                  "\"i_Fraccionamiento\" integer," +
                                                  "\"i_Eliminado\" integer," +
                                                  "\"i_Id\" serial NOT NULL," +
                                                  "CONSTRAINT \"PK_planillavaloresproyeccionquinta\" PRIMARY KEY (\"i_Id\")" +
                                                @")
												WITH (
												  OIDS=FALSE
												);");
                                break;
                            #endregion

                            #region Actualizacion 159
                            case 159:
                                listaQuerys.Add("ALTER TABLE planillavaloresrentaquinta ADD COLUMN \"v_IdConceptoPlanillaRenta5T\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE planillavaloresrentaquinta ADD COLUMN \"v_IdConceptoPlanillaGratificacion\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE planillavaloresrentaquinta ADD CONSTRAINT fk_valoresrentaquinta_renta5t FOREIGN KEY (\"v_IdConceptoPlanillaRenta5T\") REFERENCES planillaconceptos (\"v_IdConceptoPlanilla\") ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                listaQuerys.Add("ALTER TABLE planillavaloresrentaquinta ADD CONSTRAINT fk_planillavaloresrentaquinta_gratificaciones FOREIGN KEY (\"v_IdConceptoPlanillaGratificacion\") REFERENCES planillaconceptos (\"v_IdConceptoPlanilla\") ON UPDATE NO ACTION ON DELETE NO ACTION;");

                                break;
                            #endregion

                            #region Actualizacion 160
                            case 160:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_IdPlanillaConceptoTardanzas\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_IdPlanillaConceptoFaltas\" character varying(16);");

                                break;
                            #endregion

                            #region Actualizacion 161
                            case 161:
                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", " +
                                                "\"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", " +
                                                "\"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                                " VALUES (0, 162, E'HORAS EXTRAS PARAMETROS', E'', NULL, NULL, NULL, 0, 0, 1, " +
                                                "E'2017-04-03 13:08:15.585', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", " +
                                                "\"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", " +
                                                "\"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (162, 1, E'25%', E'1.25', E'', NULL, 0, 0, 0, 1, E'2017-04-03 13:10:11.540', " +
                                                "NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", " +
                                                "\"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", " +
                                                "\"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (162, 2, E'35%', E'1.35', E'', NULL, 0, 0, 0, 1, E'2017-04-03 13:10:23.663', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", " +
                                                "\"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\"," +
                                                " \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (162, 3, E'50%', E'1.50', E'', NULL, 0, 0, 0, 1, E'2017-04-03 13:10:49.444', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", " +
                                                "\"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", " +
                                                "\"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (162, 4, E'200%', E'2.00', E'', NULL, 0, 0, 0, 1, E'2017-04-03 13:11:24.067', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("ALTER TABLE planillavariableshorasextras DROP COLUMN \"i_IdPlanillaHorasExtras\";");
                                listaQuerys.Add("ALTER TABLE planillavariableshorasextras ADD COLUMN \"i_IdTipoHorasExtras\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 163
                            case 163:
                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (0, 163, E'HORAS EXTRAS TIPOS', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-04-03 14:46:09.402', 1, E'2017-04-03 14:46:24.762', NULL, NULL, NULL);");
                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (163, 1, E'DIURNO', E'0', E'', NULL, 0, 0, 0, 1, E'2017-04-03 14:46:39.465', NULL, NULL, NULL, NULL, E'');");
                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (163, 2, E'NOCTURNO', E'0.35', E'', NULL, 0, 0, 0, 1, E'2017-04-03 15:22:46.561', NULL, NULL, NULL, NULL, E'');");
                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                                "VALUES (163, 3, E'DOMINGO/FERIADO', E'2.0', E'', NULL, 0, 0, 0, 1, E'2017-04-03 15:23:01.562', NULL, NULL, NULL, NULL, E'');");
                                break;
                            #endregion

                            #region Actualizacion 164

                            case 164:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_IdRepresentanteLegal\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroLeyCuartaCategoria\" character varying(100);");
                                break;
                            #endregion

                            #region Actualizacion 165
                            case 165:
                                listaQuerys.Add("delete from datahierarchy where \"i_GroupId\" = 162;");
                                listaQuerys.Add("delete from datahierarchy where \"i_GroupId\" = 163;");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 1, E'25% DIURNO', E'1.25', E'', NULL, 1, 0, 0, 1, E'2017-04-03 13:10:11.540', 1, E'2017-04-03 17:49:17.231', NULL, NULL, E'N001-PN000000064');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 2, E'35% DIURNO', E'1.35', E'', NULL, 1, 0, 0, 1, E'2017-04-03 13:10:23.663', 1, E'2017-04-03 17:59:37.821', NULL, NULL, E'N001-PN000000062');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (163, 3, E'NOLAB/DIURNO', E'2.0', E'', NULL, 0, 0, 0, 1, E'2017-04-03 15:23:01.562', 1, E'2017-04-04 12:07:38.517', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (163, 4, E'NOLAB/NOCTURNO', E'2.7', E'', NULL, 0, 0, 0, 1, E'2017-04-04 12:07:49.998', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 13, E'25% NOLAB/DIURNO', E'1.25', NULL, NULL, 3, NULL, 0, 1, E'2017-04-04 13:46:54.728', NULL, NULL, NULL, NULL, E'N001-PN000000064');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (163, 1, E'DIURNO', E'1', E'', NULL, 0, 0, 0, 1, E'2017-04-03 14:46:39.465', 1, E'2017-04-04 11:51:53.355', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (163, 2, E'NOCTURNO', E'1.35', E'', NULL, 0, 0, 0, 1, E'2017-04-03 15:22:46.561', 1, E'2017-04-04 11:51:57.641', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 9, E'25% NOLAB/NOCTURNO', E'1.25', NULL, NULL, 4, NULL, 0, 1, E'2017-04-04 12:50:59.108', NULL, NULL, NULL, NULL, E'N001-PN000000064');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 10, E'25% NOCTURNO', E'1.25', NULL, NULL, 2, NULL, 0, 1, E'2017-04-04 13:44:15.008', NULL, NULL, NULL, NULL, E'N001-PN000000064');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 11, E'35% NOCTURNO', E'1.35', NULL, NULL, 2, NULL, 0, 1, E'2017-04-04 13:44:38.632', NULL, NULL, NULL, NULL, E'N001-PN000000062');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 12, E'35% NOLAB/NOCTURNO', E'1.35', NULL, NULL, 4, NULL, 0, 1, E'2017-04-04 13:45:15.682', NULL, NULL, NULL, NULL, E'N001-PN000000062');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (162, 14, E'35% NOLAB/DIURNO', E'1.35', NULL, NULL, 3, NULL, 0, 1, E'2017-04-04 13:47:25.215', NULL, NULL, NULL, NULL, E'N001-PN000000062');");

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_UsaDominicalCalculoDescuento\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 166

                            case 166:
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN  \"v_NroOrdenProduccion\"character varying(16);");
                                listaQuerys.Add("update configuracionempresa  set \"v_NroLeyCuartaCategoria\" = '(Articulo 45° del D.S N° 122-94-EF, Reglamento de la Ley del IR)'");

                                break;


                            #endregion

                            #region Actualizacion 167

                            case 167:
                                listaQuerys.Add(@"CREATE TABLE flujoefectivo " +
                                    "( " +
                                "  \"i_IdFlujoEfectivo\" serial NOT NULL, " +
                                "  \"v_PeriodoProceso\" character varying(4), " +
                                "  \"v_CtaMayor\" character varying(2), " +
                                "  \"v_DescripcionCuenta\" character varying(120), " +
                                "  \"d_BalancePeriodoActual\" numeric(20,2), " +
                                "  \"d_BalancePeriodoAnterior\" numeric(20,2), " +
                                "  \"d_Aumento\" numeric(20,2), " +
                                "  \"d_Disminucion\" numeric(20,2), " +
                                "  \"d_AjusteDebe\" numeric(20,2), " +
                                "  \"d_AjusteHaber\" numeric(20,2), " +
                                "  \"d_Operacion\" numeric(20,2), " +
                                "  \"d_Inversion\" numeric(20,2), " +
                                "  \"d_Financiamiento\" numeric(20,2), " +
                                "  \"d_MetodoDirecto\" numeric(20,2), " +
                                "  \"i_NaturalezaCuenta\" integer, " +
                                "  \"i_NroAsiento\" integer, " +
                                "  CONSTRAINT \"PK_flujoefectivo\" PRIMARY KEY (\"i_IdFlujoEfectivo\") " +
                                "); ");

                                listaQuerys.Add(@"CREATE TABLE flujoefectivoasientoajuste " +
                                "( " +
                                "  \"i_IdAsientoAjuste\" serial NOT NULL, " +
                                "  \"i_IdFlujoEfectivo\" integer, " +
                                "  \"i_NroAsiento\" integer, " +
                                "  \"i_IdMoneda\" integer, " +
                                "  \"d_TipoCambio\" numeric(10,3), " +
                                "  \"v_Glosa\" character varying(150), " +
                                "  CONSTRAINT \"PK_flujoefectivoasientoajuste\" PRIMARY KEY (\"i_IdAsientoAjuste\"), " +
                                "  CONSTRAINT \"FK_flujoefectivoasientoajuste_flujoefectivo_i_IdFlujoEfectivo\" FOREIGN KEY (\"i_IdFlujoEfectivo\") " +
                                "      REFERENCES flujoefectivo (\"i_IdFlujoEfectivo\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                "); ");

                                listaQuerys.Add(@"CREATE TABLE flujoefectivoasientoajustedetalle " +
                                "( " +
                                "  \"i_IdAsientoAjusteDetalle\" serial NOT NULL, " +
                                "  \"i_IdAsientoAjuste\" integer, " +
                                "  \"v_NroCuenta\" character varying(2), " +
                                "  \"v_Naturaleza\" character varying(1), " +
                                "  \"d_Importe\" numeric(20,2), " +
                                "  \"d_Cambio\" numeric(20,2), " +
                                "  CONSTRAINT \"PK_flujoefectivoasientoajustedetalle\" PRIMARY KEY (\"i_IdAsientoAjusteDetalle\"), " +
                                "  CONSTRAINT \"FK_i_IdAsientoAjuste\" FOREIGN KEY (\"i_IdAsientoAjuste\") " +
                                "      REFERENCES flujoefectivoasientoajuste (\"i_IdAsientoAjuste\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                "); ");

                                listaQuerys.Add(@"CREATE TABLE flujoefectivoconceptos " +
                                "( " +
                                "  \"i_IdConceptoFlujo\" serial NOT NULL, " +
                                "  \"v_Descripcion\" character varying(150), " +
                                "  CONSTRAINT \"PK_flujoefectivoconceptos\" PRIMARY KEY (\"i_IdConceptoFlujo\") " +
                                "); ");

                                listaQuerys.Add(@"CREATE TABLE flujoefectivoconfiguracion " +
                                "( " +
                                "  \"i_Id\" serial NOT NULL, " +
                                "  \"i_IdConceptoFlujo\" integer, " +
                                "  \"v_Periodo\" character varying(4), " +
                                "  \"v_NroCuenta\" character varying(20), " +
                                "  \"v_NroCuentaPasivos\" character varying(20), " +
                                "  \"i_IdTipoCuenta\" integer, " +
                                "  \"i_IdTipoActividad\" integer, " +
                                "  \"i_EsCuentaActivo\" integer, " +
                                "  CONSTRAINT \"PK_flujoefectivoconfiguracion\" PRIMARY KEY (\"i_Id\"), " +
                                "  CONSTRAINT \"FK_flujoefectivoconfiguracion_flujoefectivoconceptos_i_IdConcep\" FOREIGN KEY (\"i_IdConceptoFlujo\") " +
                                "      REFERENCES flujoefectivoconceptos (\"i_IdConceptoFlujo\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                "); ");

                                listaQuerys.Add(
                                    "INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\",  " +
                                    " \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\",  " +
                                    " \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                    " VALUES (0, 164, E'TIPO DE CUENTA', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-04-18 13:56:26.359', NULL, NULL, NULL, NULL, NULL); ");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\",  " +
                                  " \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\",  " +
                                  " \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                " VALUES (0, 165, E'ACTIVIDAD CUENTA', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-04-18 13:56:42.139', NULL, NULL, NULL, NULL, NULL); ");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\",  " +
                                  " \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\",  " +
                                 "  \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                " VALUES (164, 1, E'INVENTARIO', E'', E'', NULL, 0, 0, 0, 1, E'2017-04-18 13:56:52.664', NULL, NULL, NULL, NULL, E''); ");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\",  " +
                                  " \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\",  " +
                                  " \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                " VALUES (165, 1, E'ACTIVIDAD DE OPERACIÓN', E'', E'', NULL, 0, 0, 0, 1, E'2017-04-18 13:57:09.250', NULL, NULL, NULL, NULL, E''); ");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\",  " +
                                 " \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\",  " +
                                 " \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               " VALUES (165, 2, E'ACTIVIDAD DE INVERSIÓN', E'', E'', NULL, 0, 0, 0, 1, E'2017-04-18 13:57:15.805', NULL, NULL, NULL, NULL, E''); ");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\",  " +
                                 "  \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\",  " +
                                 "  \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                " VALUES (165, 3, E'ACTIVIDAD DE FINANCIAMIENTO', E'', E'', NULL, 0, 0, 0, 1, E'2017-04-18 13:57:28.341', NULL, NULL, NULL, NULL, E''); ");

                                break;


                            #endregion

                            #region Actualizacion 168
                            case 168:

                                listaQuerys.Add("CREATE TABLE productorecetasalida " +
                                                "( " +
                                                "  \"i_IdRecetaSalida\" serial NOT NULL, " +
                                                "  \"v_IdProductoTributo\" character varying(16), " +
                                                "  \"i_Eliminado\" integer, " +
                                                "  \"i_InsertaIdUsuario\" integer, " +
                                                "  \"t_InsertaFecha\" timestamp(6) without time zone, " +
                                                "  \"i_ActualizaIdUsuario\" integer, " +
                                                "  \"t_ActualizaFecha\" timestamp(6) without time zone, " +
                                                "  CONSTRAINT pk_productorecetasalida PRIMARY KEY (\"i_IdRecetaSalida\"), " +
                                                "  CONSTRAINT \"fk_productodetalle_v_IdProdInsumo\" FOREIGN KEY (\"v_IdProductoTributo\") " +
                                                "      REFERENCES productodetalle (\"v_IdProductoDetalle\") MATCH SIMPLE " +
                                                "      ON UPDATE RESTRICT ON DELETE RESTRICT " +
                                                ") " +
                                                "WITH ( " +
                                                "  OIDS=FALSE " +
                                                "); ");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                              "VALUES (0, 166, 'TIPO TRIBUTO- VENTAS EXPORTACION', 0, 1, '2017-04-21');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (166, 1, 'FLETE', 0, 1, '2017-04-21');");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\") " +
                                                "VALUES (166, 2, 'SEGURO', 0, 1, '2017-04-21');");

                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"d_SeguroTotal\" numeric(20,5);");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"d_FleteTotal\" numeric(20,5);");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"d_CantidaTotal\" numeric(20,5);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"d_PrecioPactado\" numeric(20,5);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"d_SeguroXProducto\" numeric(20,5);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"d_FleteXProducto\" numeric(20,5);");

                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_IdTipoTributo\" integer;");
                                listaQuerys.Add("ALTER TABLE productorecetasalida ADD COLUMN \"v_IdProductoExportacion\"  character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 169

                            case 169:
                                listaQuerys.Add("CREATE TABLE relaciontributosexportacion " +
                                                "( " +
                                                "   \"i_IdTipoPagoExportacion\" integer,  " +
                                                "   \"i_IdRecetaExportacion\" integer,  " +
                                                "   \"i_Id\" serial,  " +
                                                "   CONSTRAINT pk_relaciontributosexportacion PRIMARY KEY (\"i_Id\") " +
                                                ")  " +
                                                "WITH ( " +
                                                "  OIDS = FALSE " +
                                                ") " +
                                                "; "
                                );
                                listaQuerys.Add("INSERT INTO public.relaciontributosexportacion (\"i_IdTipoPagoExportacion\", \"i_IdRecetaExportacion\") " +
                                                "VALUES (2, 1);");

                                listaQuerys.Add("INSERT INTO public.relaciontributosexportacion (\"i_IdTipoPagoExportacion\", \"i_IdRecetaExportacion\") " +
                                                "VALUES (3, 1);");

                                listaQuerys.Add("INSERT INTO public.relaciontributosexportacion (\"i_IdTipoPagoExportacion\", \"i_IdRecetaExportacion\") " +
                                                "VALUES (3, 2);");
                                break;


                            #endregion

                            #region Actualizacion 170
                            case 170:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_IdProductoDetalleFlete\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_IdProductoDetalleSeguro\" character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 171
                            case 171:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_GenerarNotaSalidaDesdeVentaUltimoDiaMes\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_TipoVentaVentas\" integer;");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"v_NroBL\" character varying(40);");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"t_FechaPagoBL\" timestamp(6) without time zone");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"v_Contenedor\" character varying(40)");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"v_Banco\" character varying(40)");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"v_Naviera\" character varying(40)");
                                break;
                            #endregion

                            #region Actualizacion 172
                            case 172:
                                listaQuerys.Add("CREATE TABLE flujoefectivocabecera" +
                                "(" +
                                "   \"i_IdFlujoEfectivoCabecera\" serial, " +
                                "   \"v_PeriodoProceso\" character varying(4), " +
                                "   CONSTRAINT \"PK_flujoefectivocabecera\" PRIMARY KEY (\"i_IdFlujoEfectivoCabecera\")" +
                                ") " +
                                "WITH (" +
                                "  OIDS = FALSE" +
                                ")" +
                                ";");

                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD COLUMN \"i_IdFlujoEfectivoCabecera\" integer;");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD CONSTRAINT \"FK_flujoefectivo_flujoefectivocabecera\" FOREIGN KEY (\"i_IdFlujoEfectivoCabecera\") REFERENCES flujoefectivocabecera (\"i_IdFlujoEfectivoCabecera\") ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                listaQuerys.Add("ALTER TABLE flujoefectivoasientoajuste DROP CONSTRAINT \"FK_flujoefectivoasientoajuste_flujoefectivo_i_IdFlujoEfectivo\";");
                                listaQuerys.Add("ALTER TABLE flujoefectivoasientoajuste RENAME \"i_IdFlujoEfectivo\"  TO \"i_IdFlujoEfectivoCabecera\";");
                                listaQuerys.Add("ALTER TABLE flujoefectivoasientoajuste ADD CONSTRAINT fk_flujoefectivoasientoajuste_flujoefectivo FOREIGN KEY (\"i_IdFlujoEfectivoCabecera\") REFERENCES flujoefectivocabecera (\"i_IdFlujoEfectivoCabecera\") ON UPDATE NO ACTION ON DELETE NO ACTION;");
                                break;
                            #endregion

                            #region Actualizacion 173
                            case 173:
                                listaQuerys.Add("ALTER TABLE flujoefectivo DROP COLUMN \"v_PeriodoProceso\";");
                                break;
                            #endregion

                            #region Actualizacion 174
                            case 174:

                                listaQuerys.Add("CREATE TABLE relacionusuariocliente" +
                                "(\"i_IdRelacionusuariocliente\" serial NOT NULL," +
                                    "\"i_SystemUser\" integer," +
                                    "\"v_IdCliente\" character varying(200)," +
                                    "\"i_IdDireccionCliente\" integer," +
                                    "\"i_Eliminado\" integer," +
                                    "\"i_InsertaIdUsuario\" integer," +
                                    "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                    "\"i_ActualizaIdUsuario\" integer," +
                                    "\"t_ActualizaFecha\" timestamp(6) without time zone," +
                                    "CONSTRAINT i_IdRelacionusuariocliente_pkey PRIMARY KEY (\"i_IdRelacionusuariocliente\")," +
                                    "CONSTRAINT \"fk_relacionusuariocliente_v_IdCliente\" FOREIGN KEY (\"v_IdCliente\")" +
                                    " REFERENCES cliente (\"v_IdCliente\") MATCH SIMPLE " +
                                    @"ON UPDATE RESTRICT ON DELETE RESTRICT
								)
								WITH (
									OIDS=FALSE
								);");
                                break;

                            #endregion

                            #region Actualizacion 175
                            case 175:
                                listaQuerys.Add("ALTER TABLE flujoefectivo DROP COLUMN \"i_NroAsiento\";");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD COLUMN \"i_NroAsientoD\" integer;");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD COLUMN \"i_NroAsientoH\" integer;");
                                break;

                            #endregion

                            #region Actualizacion 176
                            case 176:
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD COLUMN \"d_Aplicacion\" numeric(20,2);");
                                listaQuerys.Add("ALTER TABLE flujoefectivo ADD COLUMN \"d_Origen\" numeric(20,2);");

                                break;

                            #endregion

                            #region Actualizacion 177
                            case 177:
                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de venta de bienes o servicios e ingresos operacionales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de regalías, honorarios, comisiones y otros');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de intereses y dividendos recibidos');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros cobros de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pago a proveedores de bienes y servicios');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pago de remuneraciones y beneficios sociales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pago de tributos');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pago de intereses y rendimientos');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros pagos de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Operación');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de venta de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de venta de inmuebles, maquinaria y equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de venta de activos intangibles');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros cobros de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pagos por compra de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pagos por compra de inmuebles, maquinaria y equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pagos por compra de activos intangibles');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros pagos de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Inversión');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de emisión de acciones o nuevos aportes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cobranza de recursos obtenidos por emisión de valores u otras obligaciones de largo plazo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros cobros de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pagos de amortización o cancelación de valores u otras obligaciones de largo plazo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pago de dividendos y otras distribuciones');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros pagos de efectivo relativos a la actividad');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Financiamiento');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Saldo Efectivo y Equivalente de Efectivo al Inicio del Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Saldo Efectivo y Equivalente de Efectivo al Finalizar el Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Conciliación del Resultado Neto con el Efectivo y Equivalente de Efectivo proveniente de las Actividades de  Operación');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Ajustes a la Utilidad (Pérdida) del Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Depreciación y amortización del período');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Provisión Beneficios Sociales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Provisiones Diversas');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pérdida en venta de inmuebles, maquinaria y Equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pérdida en venta de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Pérdida por activos monetarios no corrientes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Otros');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Ajustes a la Utilidad (Pérdida) del Ejercicio');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Utilidad en venta de inmuebles, maquinaria y equipo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Utilidad en venta de valores e inversiones permanentes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Ganancia por pasivos monetarios no corrientes');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Cargos y Abonos por cambios netos en el Activo y Pasivo');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'(Aumento) Disminución de Cuentas por Cobrar Comerciales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'(Aumento) Disminución de Cuentas por Cobrar Vinculadas');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'(Aumento) Disminución de Otras Cuentas por Cobrar');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'(Aumento) Disminución en Existencias');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'(Aumento) Disminución en Gastos Pagados por Anticipado');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) de Cuentas por Pagar Comerciales');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) de Cuentas por Pagar Vinculadas');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) de Otras Cuentas por Pagar');");

                                listaQuerys.Add("INSERT INTO flujoefectivoconceptos (\"v_Descripcion\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de la Actividad de Operación');");
                                break;

                            #endregion

                            #region Actualizacion 178
                            case 178:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD \"i_IdCondicionPagoPedido\" integer");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD \"v_GlosaPedido\" character varying(200)");
                                break;
                            #endregion

                            #region Actualizacion 179
                            case 179:
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"i_AplicaRetencion\" integer;");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_DocumentoPercepcion\" character varying(16);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_SeriePercepcion\" character varying(4);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_CorrelativoPercepcion\" character varying(8);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"t_FechaPercepcion\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"d_ImporteCalculoPercepcion\" numeric(20,2);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"d_PorcentajePercepcion\" numeric(5,2);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"d_Percepcion\" numeric(20,2);");
                                listaQuerys.Add(
                                    "INSERT INTO documento (\"i_CodigoDocumento\", \"v_Nombre\", \"v_Siglas\", \"i_UsadoDocumentoContable\", \"i_UsadoDocumentoInterno\", \"i_UsadoDocumentoInverso\", \"i_UsadoCompras\", \"i_UsadoContabilidad\", \"i_UsadoLibroDiario\", \"i_UsadoTesoreria\", \"i_UsadoVentas\", \"i_RequiereSerieNumero\", \"i_UsadoRendicionCuentas\", \"i_Naturaleza\", \"i_IdFormaPago\", \"v_NroCuenta\", \"v_provimp_3i\", \"i_Destino\", \"i_UsadoPedidoCotizacion\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_DescontarStock\", \"i_OperacionTransitoria\", \"v_NroContraCuenta\") " +
                                    "VALUES (40, E'DOCUMENTO PERCEPCIÓN', E'RTC', 1, 0, NULL, 0, 1, 1, 0, 0, 1, 0, 2, NULL, E'4011401', E'', NULL, 0, 0, 1, E'2017-05-22 15:09:46.638', NULL, NULL, 0, 0, NULL);");
                                break;



                            #endregion
                            #region Actualizacion  180
                            case 180:
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"i_IdTipoDocumentoPercepcion\" integer;");
                                break;
                            #endregion

                            #region Actualizacion  181
                            case 181:
                                listaQuerys.Add("delete from documento where \"i_CodigoDocumento\" = 40;");
                                listaQuerys.Add("INSERT INTO public.documento (\"i_CodigoDocumento\", \"v_Nombre\", \"v_Siglas\", \"i_UsadoDocumentoContable\", \"i_UsadoDocumentoInterno\", \"i_UsadoDocumentoInverso\", \"i_UsadoCompras\", \"i_UsadoContabilidad\", \"i_UsadoLibroDiario\", \"i_UsadoTesoreria\", \"i_UsadoVentas\", \"i_RequiereSerieNumero\", \"i_UsadoRendicionCuentas\", \"i_Naturaleza\", \"i_IdFormaPago\", \"v_NroCuenta\", \"v_provimp_3i\", \"i_Destino\", \"i_UsadoPedidoCotizacion\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_DescontarStock\", \"i_OperacionTransitoria\", \"v_NroContraCuenta\") " +
                                "VALUES (40, E'DOCUMENTO PERCEPCIÓN', E'RTC', 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 2, NULL, E'4011301', E'', NULL, 0, 0, 1, E'2017-05-22 15:09:46.638', NULL, NULL, 0, 0, NULL);");
                                break;
                            #endregion

                            #region Actualizacion  182
                            case 182:
                                listaQuerys.Add("update documento set \"i_UsadoContabilidad\" = 0, \"i_UsadoLibroDiario\" = 0, \"i_UsadoTesoreria\" = 0 where \"i_CodigoDocumento\" = 8");
                                break;
                            #endregion

                            #region Actualizacion 183
                            case 183:
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"d_Utilidad\" numeric(18,4);");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_VisualizarBusquedaProductos\" integer");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_PermiteIncluirPreciosCeroPedido\" integer");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_SeUsaraSoloUnaListaPrecioEmpresa\" integer");
                                break;
                            #endregion

                            #region Actualizacion 184
                            case 184:
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")
												VALUES (0, 167, 'CODIGO ETIQUETAS AVIOS', 0, 1, '2017-05-27');");
                                listaQuerys.Add(@"INSERT INTO datahierarchy (""i_GroupId"", ""i_ItemId"", ""v_Value1"", ""i_IsDeleted"", ""i_InsertUserId"", ""d_InsertDate"")
												VALUES (0, 168, 'CODIGO HANTAG AVIOS', 0, 1, '2017-05-27');");
                                break;
                            #endregion

                            #region Actualizacion 185
                            case 185:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD \"i_IdCondicionPagoVenta\" integer");
                                break;
                            #endregion

                            #region Actualizacion 186
                            case 186:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaCobranzaRedondeoPerdida\" character varying(16)");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"v_NroCuentaCobranzaRedondeoGanancia\" character varying(16)");
                                listaQuerys.Add("ALTER TABLE cobranzadetalle ADD COLUMN \"d_Redondeo\"  numeric (18,2)");



                                break;

                            #endregion

                            #region Actualizacion 187
                            case 187:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"i_IdCentroCosto\" character varying(4)");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  COLUMN \"i_CambiarAlmacenVentasDesdeVendedor\" integer");
                                listaQuerys.Add("ALTER TABLE vendedor ADD COLUMN \"i_IdAlmacen\" integer");
                                break;

                            #endregion

                            #region Actualizacion 188
                            case 188:
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_AplicaPercepcion\" integer;");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_ClienteEsAgente\" integer;");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"d_PorcentajePercepcion\" integer;");
                                break;

                            #endregion

                            #region Actualizacion 189
                            case 189:

                                listaQuerys.Add("INSERT INTO datahierarchy  values (0, 169,'TIPO VERIFICACION PEDIDO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (169, 1,'POR ATENDER','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (169, 2,'ATENDIDO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (169, 3,'REDIRECCIONADO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("ALTER TABLE pedido ADD COLUMN \"i_IdTipoVerificacion\" integer");
                                break;

                            #endregion

                            #region Actualizacion 190
                            case 190:
                                listaQuerys.Add("CREATE TABLE configuracionbalances" +
                                 "(" +
                                 "\"i_IdConfiguracionBalance\" serial NOT NULL, " +
                                "\"v_TipoBalance\" character varying(4), " +
                                "\"v_Codigo\" character varying(10), " +
                                "\"v_Nombre\" character varying(200), " +
                                 "\"i_IdTipoElemento\" integer , " +
                                  "\"i_Eliminado\" integer," +
                                    "\"i_InsertaIdUsuario\" integer," +
                                    "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                    "\"i_ActualizaIdUsuario\" integer," +
                                    "\"t_ActualizaFecha\" timestamp(6) without time zone " + ") " +
                                     "WITH (" +
                                 "  OIDS=FALSE" +
                                     ")" + ";");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_ItemsAfectosPercepcion\" integer;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN \"i_EmpresaAfectaPercepcionVenta\" integer;");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD COLUMN \"v_CodigoBalanceFuncion\" character  varying(10);");
                                break;
                            #endregion

                            #region Actualizacion 191
                            case 191:

                                listaQuerys.Add("ALTER TABLE venta ALTER \"d_PorcentajePercepcion\" TYPE numeric(16,2);");
                                break;
                            #endregion

                            #region Actualizacion 192
                            case 192:
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD COLUMN \"i_TipoOperacion\" integer");
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD COLUMN \"v_NombreGrupo\"character  varying(200)");
                                break;

                            #endregion

                            #region Actualizacion 193
                            case 193:


                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G2','UTILIDAD OPERATIVA',null,0,1,'2017-06-30 14:38:26.905669',null,null,-1,'UTILIDAD OPERATIVA')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G3','RESULTADO ANTES DEL IMP. RENTA',null,0,1,'2017-06-30 14:38:44.9207',null,null,-1,'RESULTADO ANTES DEL IMP. RENTA')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G4','UTILIDAD (PERDIDA) NETA  DE ACT. NOT.',null,0,1,'2017-06-30 14:39:27.649144',null,null ,-1,'UTILIDAD (PERDIDA) NETA  DE ACT. NOT.')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G5','UTLIDAD (PERDIDA) DEL EJERCICIO',null,0,1,'2017-06-30 14:39:52.664574',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G11','INGRESOS OPERACIONALES',null,0,1,'2017-06-30 14:40:28.871645',null,null,-1,'TOTAL INGRESOS BRUTOS')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G112','OTROS INGRESOS OPERACIONALES',null,0,1,'2017-06-30 14:41:16.833389',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G111','VENTAS NETAS  (INGRESOS OPERACIONALES)',null,0,1,'2017-06-30 14:40:58.959366',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G122','OTROS COSTOS OPERACIONALES',null,0,1,'2017-06-30 14:55:20.697655',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G21','UTILIDAD OPERATIVA',null,0,1,'2017-06-30 14:59:02.624348',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G213','GANANCIA (PERDIDA) DE VENTA ACT.',null,0,1,'2017-06-30 15:00:17.847651',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G214','OTROS INGRESOS',null,0,1,'2017-06-30 15:00:57.495919',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G215','OTROS GASTOS',null,0,1,'2017-06-30 15:01:10.538665',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G31','RESULTADO ANTES DEL IMP. RENTA',null,0,1,'2017-06-30 15:01:43.208533',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G312','GASTOS FINANCIEROS',null,0,1,'2017-06-30 15:02:08.055954',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G313','PARTICIPACIÓN EN LOS RESULTADOS',null,0,1,'2017-06-30 15:03:00.655963',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G314','GANANCIA (PERDIDA) POR INST. FINANCIERA',null,0,1,'2017-06-30 15:04:41.761746',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G41','UTILIDAD (PERDIDA) NETA  DE ACT. NOT.',null,0,1,'2017-06-30 15:05:10.377383',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G411','PARTICIPACION DE LOS TRABAJADORES',null,0,1,'2017-06-30 15:05:31.999619',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G412','IMPUESTO A LA RENTA',null,0,1,'2017-06-30 15:06:59.887646',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G51','UTLIDAD (PERDIDA) DEL EJERCICIO',null,0,1,'2017-06-30 15:08:10.40568',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G511','INGRESO (GASTO ) DE OPERACIONES DISCONTINUAS',null,0,1,'2017-06-30 15:09:11.9522',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G12','COSTO DE VENTAS',null,0,1,'2017-06-30 14:54:15.370918',null,null,-1,'TOTAL COSTOS OPERACIONALES')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G311','INGRESOS FINANCIEROS',null,0,1,'2017-06-30 15:01:56.864314',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G1','UTILIDAD BRUTA',null,0,1,'2017-06-30 14:37:51.719657',null,null,-1,'UTILIDAD BRUTA')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G121','COSTO DE VENTAS (OPERACIONALES)',null,0,1,'2017-06-30 14:55:01.575561',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G211','GASTOS DE VENTAS',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('1','G212','GASTOS DE ADMINISTRACIÓN',null,0,1,'2017-06-30 14:59:40.784531',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G121','INVERSIONES EN VALORES',null,0,1,'2017-07-01 10:28:24.113599',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G1','ACTIVO',null,0,1,'2017-07-01 00:25:07.553164',1,'2017-07-01 10:34:02.187491',null,'TOTAL ACTIVO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G2','CUENTAS ORDEN',null,0,1,'2017-07-01 10:35:07.235323',1,'2017-07-01 10:35:25.286783',null,'CUENTAS ORDEN')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G111','CAJA Y BANCOS',null,0,1,'2017-07-01 00:26:05.675934',1,'2017-07-01 12:03:28.053993',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G112','CUENTAS POR COBRAR COMERCIALES',null,0,1,'2017-07-01 01:03:57.587305',1,'2017-07-01 12:10:49.25712',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G31','PASIVO CORRIENTE',null,0,1,'2017-07-01 13:26:59.068352',null,null,null,'TOTAL PASIVO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G4','PATRIMONIO',null,0,1,'2017-07-01 13:41:57.115426',null,null,null,'TOTAL PATRIMONIO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G41','PATRIMONIO',null,0,1,'2017-07-01 13:42:39.219491',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G122','INMUEBLE MAQUINARIA Y EQUIPO',null,0,1,'2017-07-01 10:32:56.324811',1,'2017-07-01 14:05:40.911962',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G3','PASIVO ',null,0,1,'2017-07-01 13:26:09.369918',1,'2017-07-01 14:07:58.15149',null,'TOTAL PASIVO ')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G32','PASIVO NO CORRIENTE',null,0,1,'2017-07-01 13:40:23.151871',1,'2017-07-01 14:08:32.222043',null,'TOTAL PASIVO NO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G11','ACTIVO CORRIENTE',null,0,1,'2017-07-01 00:25:41.892467',1,'2017-07-01 14:09:00.223433',null,'TOTAL ACTIVO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G12','ACTIVO NO CORRIENTE',null,0,1,'2017-07-01 10:17:53.907571',1,'2017-07-01 14:09:06.694935',null,'TOTAL ACTIVO NO CORRIENTE')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G311','SOBRECARGOS BANCARIOS',null,0,1,'2017-07-01 13:27:38.964495',1,'2017-07-01 14:11:31.631909',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G411','CAPITAL SOCIAL',null,0,1,'2017-07-01 13:42:55.659356',1,'2017-07-01 14:12:30.583277',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G321','CUENTAS POR PAGAR CORRIENTES',null,0,1,'2017-07-01 13:41:02.34123',1,'2017-07-01 14:31:33.025052',null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G113','CUENTAS POR COBRAR A ACCIONISTA Y PERSONAL',null,0,1,'2017-07-02 18:08:37.961475',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G114','OTRAS CUENTAS POR COBRAR',null,0,1,'2017-07-02 18:08:55.161459',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G115','EXISTENCIAS',null,0,1,'2017-07-02 18:09:10.524877',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G116','CARGAS DIFERIDAS',null,0,1,'2017-07-02 18:09:19.207523',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G123','INTANGIBLES',null,0,1,'2017-07-02 18:11:17.460887',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G124','DEPRECIACIÓN ACUMULADA',null,0,1,'2017-07-02 18:11:40.982512',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G125','OTRAS CUENTAS DEL ACTIVO',null,0,1,'2017-07-02 18:12:14.930069',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G312','TRIBUTOS POR PAGAR',null,0,1,'2017-07-02 18:12:56.672643',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G313','CUENTAS POR PAGAR COMERCIALES',null,0,1,'2017-07-02 18:27:28.099963',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G314','OTRAS CUENTAS POR PAGAR DIVERSAS',null,0,1,'2017-07-02 18:27:51.474801',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G322','COMPENSACIÓN POR TIEMPO DE SERVICIO',null,0,1,'2017-07-02 18:31:10.71396',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G323','OTRAS CUENTAS POR PAGAR',null,0,1,'2017-07-02 18:31:49.404587',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G412','CAPITAL ADICIONAL',null,0,1,'2017-07-02 18:32:26.400249',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G413','EXCEDENTE DE REEVALUACIÓN',null,0,1,'2017-07-02 18:32:49.003122',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G414','RESERVA  LEGAL',null,0,1,'2017-07-02 18:33:09.574961',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G415','RESULTADOS ACUMULADOS',null,0,1,'2017-07-02 18:33:23.115414',null,null,null,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('2','G416','RESULTADOS DEL EJERCICIO',null,0,1,'2017-07-02 18:33:47.175958',null,null,null,null)");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD COLUMN \"v_CodigoSituacionFinaciera\" character  varying(10);");



                                break;
                            #endregion

                            #region Actualizacion 194
                            case 194:
                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (0, 170, E'TIPO DE ANEXO CONTABLE', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-07-03 13:13:58.793', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 1, E'ACCIONISTAS', E'A', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:14:28.272', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 2, E'ENTIDAD FINANCIERA', E'B', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:20:50.937', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 3, E'FONDOS FIJOS', E'F', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:21:01.643', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 4, E'CONCEPTOS PATRIMONIALES', E'P', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:21:13.950', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 5, E'VARIOS', E'V', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:21:21.366', NULL, NULL, NULL, NULL, E'');");
                                break;

                            #endregion

                            #region Actualizacion 195
                            case 195:
                                listaQuerys.Add("DELETE FROM datahierarchy " +
                                "WHERE \"i_GroupId\" = 170");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 1, E'ACCIONISTAS', E'A', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:14:28.272', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 2, E'ENTIDAD FINANCIERA', E'B', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:20:50.937', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 3, E'FONDOS FIJOS', E'F', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:21:01.643', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 4, E'CONCEPTOS PATRIMONIALES', E'P', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:21:13.950', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 6, E'VENDEDORES', E'E', E'', NULL, 0, 0, 0, 1, E'2017-07-04 13:15:25.474', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 7, E'PROVEEDORES', E'V', E'', NULL, 0, 0, 0, 1, E'2017-07-04 13:15:44.343', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 5, E'VARIOS', E'S', E'', NULL, 0, 0, 0, 1, E'2017-07-03 13:21:21.366', 1, E'2017-07-04 13:16:04.763', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 8, E'CLIENTES', E'C', E'', NULL, 0, 0, 0, 1, E'2017-07-04 13:16:45.965', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO public.datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (170, 9, E'TRABAJADORES', E'T', E'', NULL, 0, 0, 0, 1, E'2017-07-04 13:16:55.389', NULL, NULL, NULL, NULL, E'');");
                                break;

                            #endregion

                            #region Actualizacion 196

                            case 196:
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"i_AplicaRectificacion\" integer;");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_RectificacionPeriodo\" character varying(4);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_RectificacionMes\" character varying(4);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_RectificacionCorrelativo\" character varying(8);");



                                break;
                            #endregion

                            #region Actualizacion 197
                            case 197:



                                listaQuerys.Add("ALTER TABLE compra DROP COLUMN \"v_RectificacionPeriodo\";");
                                listaQuerys.Add("ALTER TABLE compra DROP COLUMN \"v_RectificacionMes\";");
                                listaQuerys.Add("ALTER TABLE compra DROP COLUMN \"v_RectificacionCorrelativo\";");
                                listaQuerys.Add("ALTER TABLE compra ADD  COLUMN \"t_FechaCorreccionPle\" timestamp(6) without time zone");
                                break;
                            #endregion

                            #region Actualizacion 198


                            case 198:
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G1','PRODUCCIÓN TOTAL ',null,0,1,'2017-06-30 14:37:51.719657',null,null,-1,'TOTAL PRODUCCIÓN TOTAL')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G11','MARGEN COMERCIAL',null,0,1,'2017-06-30 14:40:28.871645',null,null,-1,'TOTAL MARGEN COMERCIAL')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G111','VENTAS NETAS  DE MERCADERIAS',null,0,1,'2017-06-30 14:40:58.959366',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G112','COMPRAS DE MERCADERIAS',null,0,1,'2017-06-30 14:41:16.833389',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G113','VARIACIÓN DE MERCADERIAS',null,0,1,'2017-06-30 14:41:16.833389',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G12','PRODUCCION DEL EJERCICIO',null,0,1,'2017-06-30 14:54:15.370918',null,null,-1,'PRODUCCION DEL EJERCICIO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G121','VENTAS NETAS DE PRODUCTOS',null,0,1,'2017-06-30 14:55:01.575561',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G122','PRODUCCIÓN ALMACENADA',null,0,1,'2017-06-30 14:55:20.697655',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G123','PRODUCCIÓN INMOVILIZADA',null,0,1,'2017-06-30 14:55:20.697655',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G2','CONSUMO',null,0,1,'2017-06-30 14:38:26.905669',null,null,-1,'TOTAL CONSUMO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G21','CONSUMO',null,0,1,'2017-06-30 14:59:02.624348',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G211','COMPRA MATERIA PRIMA Y AUX',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G212','COMPRA SUMINISTROS DIVERSOS',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G213','VARIACIÓN DE MATERIAS PRIMAS Y AUX',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G214','VARIACIÓN EXISTENCIAS SUM. DIVERSOS',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G3','VALOR AGREGADO',null,0,1,'2017-06-30 14:38:26.905669',null,null,-1,'TOTAL VALOR AGREGADO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G31','VALOR AGREGADO',null,0,1,'2017-06-30 14:59:02.624348',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G311','VALOR AGREGADO',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G312','SERVICIOS PRESTADOS POR TERCEROS',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G4','EXCEDENTE BRUTO DE EXPLOTACIÓN',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,'TOTAL EXCEDENTE BRUTO DE EXPLOTACIÓN')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G41','EXCEDENTE BRUTO DE EXPLOTACIÓN',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G411','CARGAS DE PERSONAL',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G412','TRIBUTOS',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G5','RESULTADO DE EXPLOTACIÓN',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,'RESULTADO DE EXPLOTACIÓN')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G51','RESULTADO DE EXPLOTACIÓN',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G511','CARGAS DIVERSAS DE GESTION',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G512','PROVISIONES DEL EJERCICIO',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G512','INGRESOS DIVERSOS (INC. DESC.)',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G6','RESULTADO DEL EJERCICIO',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,'RESULTADO DEL EJERCICIO')");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G61','RESULTADO DEL EJERCICIO',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G611','REI DE EJERCICIO',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");
                                listaQuerys.Add("INSERT INTO configuracionbalances  (\"v_TipoBalance\", \"v_Codigo\", \"v_Nombre\", \"i_IdTipoElemento\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"i_TipoOperacion\", \"v_NombreGrupo\")  values ('3','G612','IMPUESTO A LA RENTA',null,0,1,'2017-06-30 14:59:21.376421',null,null,-1,null)");


                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD COLUMN \"i_EsSumatoria\" integer;");
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD COLUMN \"i_IdTipoAccion\" integer;");


                                listaQuerys.Add("CREATE TABLE flujoefectivoconceptosdetalles " +
                                "( " +
                                "   \"i_Id\" serial,  " +
                                "   \"i_IdConceptoFlujo\" integer,  " +
                                "   \"v_NroCuenta\" character varying(12),  " +
                                "    PRIMARY KEY (\"i_Id\"),  " +
                                "   CONSTRAINT fk_flujoconcepto_flujoconceptodetalle FOREIGN KEY (\"i_IdConceptoFlujo\") REFERENCES flujoefectivoconceptos (\"i_IdConceptoFlujo\") ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                ")  ");


                                listaQuerys.Add("ALTER TABLE asientocontable ADD COLUMN \"v_CodigoBalanceNaturaleza\" character  varying(10);");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD COLUMN \"v_CodigoPatrimonioNeto\" character  varying(10);");

                                break;
                            #endregion

                            #region Actualizar 199
                            case 199:
                                listaQuerys.Add("ALTER TABLE asientocontable ADD COLUMN \"i_UsaraPatrimonioNeto\" integer;");
                                listaQuerys.Add("ALTER TABLE diariodetalle ADD \"i_IdPatrimonioNeto\" varchar(4);");
                                listaQuerys.Add("ALTER TABLE tesoreriadetalle ADD \"i_IdPatrimonioNeto\" varchar(4);");

                                listaQuerys.Add("INSERT INTO datahierarchy values (0, 171,'NOTAS - PATRIMONIO NETO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,1,'EFECTO ACUMULADO DE LOS CAMBIOS EN LAS POLÍTICAS CONTABLES Y LA CORRECCIÓN DE ERRORES SUSTANCIALES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,2,'DISTRIBUCIONES O ASIGNACIONES DE UTILIDADES EFECTUADAS EN EL PERÍODO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,3,'DIVIDENDOS Y PARTICIPACIONES ACORDADOS DURANTE EL PERÍODO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,4,'NUEVOS APORTES DE ACCIONISTAS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,5,'MOVIMIENTO DE  PRIMA EN LA COLOCACIÓN DE APORTES Y DONACIONES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,6,'INCREMENTOS O DISMINUCIONES POR FUSIONES O ESCISIONES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,7,'REVALUACIÓN DE ACTIVOS','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,8,'CAPITALIZACIÓN DE PARTIDAS PATRIMONIALES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,9,'REDENCIÓN DE ACCIONES DE INVERSIÓN O REDUCCIÓN DE CAPITAL','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,10,'UTILIDAD (PÉRDIDA) NETA DEL EJERCICIO','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                listaQuerys.Add("INSERT INTO datahierarchy  values (171,11,'OTROS INCREMENTOS O DISMINUCIONES DE LAS PARTIDAS PATRIMONIALES','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387',NULL ,NULL ,NULL,NULL )");
                                break;
                            #endregion

                            #region Actualizar 200
                            case 200:
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD COLUMN \"i_Flag\" integer;");
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD COLUMN \"i_Orden\" integer;");
                                listaQuerys.Add("ALTER TABLE flujoefectivoconceptos ADD COLUMN \"d_Importe\" numeric(20,2);");
                                listaQuerys.Add("delete from flujoefectivoconceptos;");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de venta de bienes o servicios e ingresos operacionales', 0, 1, 0, 1, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de regalías, honorarios, comisiones y otros', 0, 1, 0, 2, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de intereses y dividendos recibidos', 0, 1, 0, 3, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros cobros de efectivo relativos a la actividad', 0, 1, 0, 4, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de emisión de acciones o nuevos aportes', 0, 3, 0, 20, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Ganancia por pasivos monetarios no corrientes', 0, 5, 0, 42, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pago a proveedores de bienes y servicios', 0, 1, 0, 5, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pago de remuneraciones y beneficios sociales', 0, 1, 0, 6, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pago de tributos', 0, 1, 0, 7, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pago de intereses y rendimientos', 0, 1, 0, 8, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros pagos de efectivo relativos a la actividad', 0, 1, 0, 9, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Operación', 1, -1, 0, 10, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de venta de valores e inversiones permanentes', 0, 2, 0, 11, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de venta de inmuebles, maquinaria y equipo', 0, 2, 0, 12, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de venta de activos intangibles', 0, 2, 0, 13, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros cobros de efectivo relativos a la actividad', 0, 2, 0, 14, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pagos por compra de valores e inversiones permanentes', 0, 2, 0, 15, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pagos por compra de inmuebles, maquinaria y equipo', 0, 2, 0, 16, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pagos por compra de activos intangibles', 0, 2, 0, 17, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros pagos de efectivo relativos a la actividad', 0, 2, 0, 18, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Inversión', 2, -1, 0, 19, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cobranza de recursos obtenidos por emisión de valores u otras obligaciones de largo plazo', 0, 3, 0, 21, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) Neto de efectivo y Equivalente de Efectivo', -1, 0, 1, 27, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Saldo Efectivo y Equivalente de Efectivo al Inicio del Ejercicio', 0, 4, 1, 28, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Saldo Efectivo y Equivalente de Efectivo al Finalizar el Ejercicio', NULL, NULL, 2, 29, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Utilidad (Pérdida) neta del Ejercicio', 0, 5, 0, 30, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Ajustes a la Utilidad (Pérdida) del Ejercicio', 0, 5, 0, 31, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Depreciación y amortización del período', 0, 5, 0, 32, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Provisión Beneficios Sociales', 0, 5, 0, 33, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Provisiones Diversas', 0, 5, 0, 34, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pérdida en venta de inmuebles, maquinaria y Equipo', 0, 5, 0, 35, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pérdida en venta de valores e inversiones permanentes', 0, 5, 0, 36, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pérdida por activos monetarios no corrientes', 0, 5, 0, 37, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros', 0, 5, 0, 38, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Ajustes a la Utilidad (Pérdida) del Ejercicio', 0, 5, 0, 39, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Utilidad en venta de inmuebles, maquinaria y equipo', 0, 5, 0, 40, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Utilidad en venta de valores e inversiones permanentes', 0, 5, 0, 41, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Cargos y Abonos por cambios netos en el Activo y Pasivo', 0, 5, 0, 43, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'(Aumento) Disminución de Cuentas por Cobrar Comerciales', 0, 5, 0, 44, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'(Aumento) Disminución de Cuentas por Cobrar Vinculadas', 0, 5, 0, 45, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'(Aumento) Disminución de Otras Cuentas por Cobrar', 0, 5, 0, 46, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'(Aumento) Disminución en Existencias', 0, 5, 0, 47, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'(Aumento) Disminución en Gastos Pagados por Anticipado', 0, 5, 0, 48, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) de Cuentas por Pagar Comerciales', 0, 5, 0, 49, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) de Cuentas por Pagar Vinculadas', 0, 5, 0, 50, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) de Otras Cuentas por Pagar', 0, 5, 0, 51, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de la Actividad de Operación', 5, 0, 0, 52, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pagos de amortización o cancelación de valores u otras obligaciones de largo plazo', 0, 3, 0, 23, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Pago de dividendos y otras distribuciones', 0, 3, 0, 24, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros pagos de efectivo relativos a la actividad', 0, 3, 0, 25, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Otros cobros de efectivo relativos a la actividad', 0, 3, 0, 22, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'Aumento (Disminución) del Efectivo y Equivalente de Efectivo Provenientes de Actividades de Financiamiento', 3, -1, 0, 26, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'- ACTIVIDADES DE INVERSIÓN', 100, NULL, NULL, 10, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'- ACTIVIDADES DE OPERACIÓN', 100, NULL, NULL, 0, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'- ACTIVIDADES DE FINANCIAMIENTO', 100, NULL, NULL, 20, NULL);");

                                listaQuerys.Add("INSERT INTO public.flujoefectivoconceptos (\"v_Descripcion\", \"i_EsSumatoria\", \"i_IdTipoAccion\", \"i_Flag\", \"i_Orden\", \"d_Importe\") " +
                                "VALUES (E'- CONCILIACIÓN', 100, NULL, NULL, 30, NULL);");
                                break;
                            #endregion

                            #region Actualiazacion 201
                            case 201:
                                listaQuerys.Add("ALTER TABLE configuracionbalances DROP COLUMN \"i_IdTipoElemento\"");
                                listaQuerys.Add("ALTER TABLE configuracionbalances DROP COLUMN \"i_TipoOperacion\"");
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD COLUMN \"i_TipoNota\" integer");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (0, 172,'NOTAS - ESTADO SITUACIÓN FINANCIERA','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,1,'NOTA 1','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (172,2,'NOTA 2','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,3,'NOTA 3','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,4,'NOTA 4','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (172,5,'NOTA 5','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,6,'NOTA 6','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,7,'NOTA 7','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,8,'NOTA 8','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,9,'NOTA 9','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (172,10,'NOTA 10','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (172,11,'NOTA 11','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (172,12,'NOTA 12','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,13,'NOTA 13','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387' )");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,14,'NOTA 14','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (172,15,'NOTA 15','','',NULL,0,0,0,1, '2015-12-03 14:45:50.0092387')");

                                break;
                            #endregion

                            #region Actualizacion 202
                            case 202:
                                listaQuerys.Add("ALTER TABLE pedido ALTER \"v_DireccionClienteTemporal\" TYPE  character varying(200);");
                                break;
                            #endregion

                            #region Actualizacion 203
                            case 203:
                                listaQuerys.Add("ALTER TABLE cobranza ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE pago ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE compra ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE pedido ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE movimiento ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE letras ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE diario ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE tesoreria ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE documentoretencion ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE cliente ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE documento ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                listaQuerys.Add("ALTER TABLE asientocontable ADD COLUMN \"v_MotivoEliminacion\" character varying(200);");
                                break;
                            #endregion

                            #region Actualizacion 204
                            case 204:
                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"b_Foto\" bytea");
                                break;

                            #endregion

                            #region Actualizacion 205
                            case 205:
                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"i_IdTipoDocumento\" integer");
                                listaQuerys.Add("update activofijo set " + "\"" + "i_IdTipoDocumento" + "\"" + " = 1;");
                                break;
                            #endregion

                            #region Actualizacion 206
                            case 206:

                                listaQuerys.Add("CREATE TABLE activofijoanexo" +
                                              "(\"i_IdActivoFijoAnexo\" serial NOT NULL," +
                                              "\"v_IdActivoFijo\" character varying(16)," +
                                              "\"i_IdTipoDocumento\" integer," +
                                              "\"v_NroDocumento\" character varying(15)," +
                                              "\"t_FechaEmision\" timestamp(6) without time zone," +
                                              "\"b_Foto\" bytea," +
                                               "\"v_Observaciones\" character varying(200)," +
                                               "\"v_UbicacionFoto\" character varying(200)," +
                                              "\"i_Eliminado\" integer," +
                                              "\"i_InsertaIdUsuario\" integer," +
                                              "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                              "\"i_ActualizaIdUsuario\" integer," +
                                              "\"t_ActualizaFecha\" timestamp(6) without time zone," +
                                              "CONSTRAINT \"fk_activofijoanexo_v_IdActivoFijo\" FOREIGN KEY (\"v_IdActivoFijo\")" +
                                                  "REFERENCES activofijo (\"v_IdActivoFijo\") MATCH SIMPLE " +
                                                  @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");

                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"v_UbicacionFoto\" character varying(200);");




                                break;

                            #endregion

                            #region Actualizacion 207
                            case 207:
                                listaQuerys.Add("ALTER TABLE guiaremision ADD COLUMN \"v_UbigueoLlegada\" character varying(10), ADD COLUMN \"v_UbigueoPartida\" character varying(10), ADD COLUMN \"d_TotalPeso\" decimal(18,4), ADD COLUMN \"i_EstadoSunat\" smallint, ADD COLUMN \"i_Modalidad\" smallint");
                                listaQuerys.Add("ALTER TABLE transportistachofer ADD \"i_IdTipoIdentificacion\" int, ADD COLUMN \"v_NroDocIdentificacion\" character varying(20)");
                                listaQuerys.Add("ALTER TABLE almacen ADD \"v_Ubigueo\" character varying(10)");
                                listaQuerys.Add(@"CREATE TABLE guiaremisionhomologacion(" +
                                                "\"i_Idhomologacion\" serial NOT NULL," +
                                                "\"v_IdGuiaRemision\" character varying(16)," +
                                                "\"b_FileXml\" bytea," +
                                                "CONSTRAINT \"PK_guiaremisionhomologacion\" PRIMARY KEY (\"i_Idhomologacion\")," +
                                                "CONSTRAINT \"Fk_remisionhomologacion_guiaremision\" FOREIGN KEY (\"v_IdGuiaRemision\")" +
                                                "REFERENCES guiaremision (\"v_IdGuiaRemision\") MATCH SIMPLE " +
                                                "ON UPDATE NO ACTION ON DELETE NO ACTION ) WITH ( OIDS=FALSE);");
                                listaQuerys.Add("ALTER TABLE guiaremisionhomologacion OWNER TO postgres;");
                                break;

                            #endregion

                            #region Actualizacion 208
                            case 208:

                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"i_IdSituacionActivoFijo\" integer");
                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"v_CodigoBarras\" character varying(20)");
                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"i_IdClaseActivoFijo\" integer");
                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"v_CodigoOriginal\" character varying(20)");
                                listaQuerys.Add("ALTER TABLE activofijo ADD COLUMN \"v_AnioFabricacion\" character varying(4)");
                                listaQuerys.Add("ALTER TABLE \"importacionDetalleProducto\" ALTER COLUMN \"v_NroFactura\" type character varying(30)");


                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (0, 173,'SITUACIÓN DEL ACTIVO FIJO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (173,1,'VIGENTE','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (173,2,'TOTALMENTE DEPRECIADO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (173,3,'VENDIDO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");


                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (0, 174,'CLASE DEL ACTIVO FIJO','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (174,1,'NORMAL','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (174,2,'COMPONENTE','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");


                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var Depreciacion = dbContext.datahierarchy.Where(l => l.i_GroupId == 109 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = dbContext.datahierarchy.Where(l => l.i_GroupId == 109).Max(l => l.i_ItemId);
                                    var Depreciacion240 = Depreciacion.Where(l => l.v_Value1.Contains("240")).ToList();
                                    if (!Depreciacion240.Any())
                                    {
                                        MaxItemId = MaxItemId + 1;
                                        listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (109," + MaxItemId + ",'240','5','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");

                                    }
                                }



                                break;
                            #endregion

                            #region Actualizacion 209
                            case 209:
                                listaQuerys.Add("ALTER TABLE activofijo ALTER COLUMN \"v_NumeroFactura\" type character varying(30)");
                                break;
                            #endregion

                            #region Actualizacion 210
                            case 210:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"SmtpHost\" character varying(80),ADD COLUMN \"SmtpPort\" integer,ADD COLUMN \"SmtpEmail\" character varying(80),ADD COLUMN \"SmtpPassword\" character varying(100), ADD COLUMN \"SmtpSsl\" boolean");
                                break;
                            #endregion

                            #region Actualizacion  211
                            case 211:
                                listaQuerys.Add("ALTER TABLE venta ALTER COLUMN \"v_NroBulto\" type character varying(150)");
                                break;
                            #endregion

                            #region Actualizacion 212
                            case 212:
                                listaQuerys.Add("ALTER TABLE cliente ALTER COLUMN \"v_PrimerNombre\" type character varying(150)");
                                break;
                            #endregion

                            #region Actualizacion 213
                            case 213:

                                listaQuerys.Add("ALTER TABLE cliente ADD COLUMN \"i_IdTipoAccionesSocio\" integer");
                                listaQuerys.Add("ALTER TABLE cliente ADD COLUMN \"i_NumeroAccionesSuscritas\"  numeric (20,2)");
                                listaQuerys.Add("ALTER TABLE cliente ADD COLUMN \"i_NumeroAccionesPagadas\"  numeric (20,2)");


                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (0, 175,'TIPO ACCIONES SOCIOS - BALANCE 50','','',NULL,0,0,0,1, '2017-07-19 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )   values (175,1,'NOMINATIVAS','','',NULL,0,0,0,1, '2017-08-03 14:45:50.0092387')");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\",\"i_ItemId\",\"v_Value1\",\"v_Value2\",\"v_Field\",\"i_ParentItemId\",\"i_Header\",\"i_Sort\",\"i_IsDeleted\",\"i_InsertUserId\",\"d_InsertDate\" )  values (175,2,'COMUNES','','',NULL,0,0,0,1, '2017-08-03 14:45:50.0092387')");


                                break;

                            #endregion

                            #region Actualizacion 214
                            case 214:



                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var RegimenLaboral = dbContext.datahierarchy.Where(l => l.i_GroupId == 38 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = RegimenLaboral.Max(l => l.i_ItemId);
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'HAMBURG – GERMANY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'JEDDAH – SAUDI ARABIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ALGER- ALGERIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ROSARIO – ARGENTINA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BUENOS AIRES – ARGENTINA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SIDNEY – AUSTRALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'MELBOURNE – AUSTRALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'POTI  – GEORGIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BAHRAIN – BAHRAIN','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BEIRUT – LEBANON','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ANTWERP – BELGICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'VARNA – BULGARIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'PRAIA– REP. DE CABO VERDE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'MINDELO– REP. DE CABO VERDE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'TORONTO – CANADA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'MONTREAL – CANADA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'VALPARAISO – CHILE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LIMASSOL – CYPRUS','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BARRANQUILLA – COLOMBIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'PUERTO CALDERA- COSTA RICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'PLOCE – CROACIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'PIRAEUS- GRECIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'THESSALONIKI- GRECIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ALEXANDRIA- EGYPT','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'JEBEL ALI – UNITED ARAB EMIRATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'VALENCIA – ESPAÑA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BARCELONA – ESPAÑA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ALGECIRAS  - ESPAÑA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'FOS SUR MER- FRANCIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LE HAVRE- FRANCIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ROTTERDAM – NETHERLANDS','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'HONG KONG- HONG KONG','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'NAVA SHEVA– INDIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'TILBURY – ENGLAND','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LONDON GATEWAY – ENGLAND','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'FELIXSTOWE – ENGLAND','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'HAIFA – ISRAEL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ASHDOD – ISRAEL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'VENECIA- ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'GENOVA- ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SALERNO ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'NAPLES- ITALIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'KINGSTON– JAMAICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'YOKOHAMA – JAPON','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");
                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'AL ‘AQABAH PORT– JORDANIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SEOUL – KOREA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SHUWAIKH – KUWAIT','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'KLAIPEDA- LITUANIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;

                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'KUALA LUMPUR- MALAYSIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                }

                                break;


                            #endregion

                            #region Actualizacion 215
                            case 215:

                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {
                                    var RegimenLaboral = dbContext.datahierarchy.Where(l => l.i_GroupId == 38 && l.i_IsDeleted == 0).ToList();
                                    int? MaxItemId = RegimenLaboral.Max(l => l.i_ItemId);

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'VERACRUZ – MEXICO','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");



                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'MANZANILLO – MEXICO','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BALBOA – PANAMA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'WARSZAWA – POLONIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'GDYNIA – POLONIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SINES/BOBADELA – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SINES – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LISBOA – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LEIXOES – PORTUGAL','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'HAMAD – QATAR','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'BREMERHAVEN -  GERMANY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'STARE MESTO -  REPUBLICA CHECA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'CONSTANZA PORT- RUMANIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ST. PETERSBURG – RUSIA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'DURBAN- SOUTH AFRICA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'KEELUNG – TAIWAN','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'KAOHSIUNG- TAIWAN','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'PORT OF SPAIN/TRINIDAD Y TOBAGO','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ISTANBUL – TURKEY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'IZMIR – TURKEY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'MERSIN – TURKEY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'ODESSA- UKRAINE','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'MONTEVIDEO – URUGUAY','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LONG BEACH – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LOS ANGELES – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'NEW YORK – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'HOUSTON – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'HOUSTON – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");


                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'SEATTLE – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'OAKLAND – UNITED STATES','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'LA GUAIRA/VENEZUELA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");

                                    MaxItemId = MaxItemId + 1;
                                    listaQuerys.Add("INSERT INTO datahierarchy    values    (38," + MaxItemId + ",'PUERTO CABELLO/VENEZUELA','','',NULL,0,0,0,1,'2017-08-04 14:45:50.0092387',NULL ,NULL )");



                                    listaQuerys.Add("ALTER TABLE  venta ADD  \"v_NroBultoIngles\" character varying(150)");
                                    listaQuerys.Add("ALTER TABLE venta ALTER COLUMN  \"v_Contenedor\" type character varying(150)");
                                }
                                break;

                            #endregion

                            #region Actualizacion 216
                            case 216:
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD COLUMN " + "\"" + "i_TipoDepreciacionActivoFijo" + "\"" + " integer");

                                listaQuerys.Add("update configuracionempresa set " + "\"" + "i_TipoDepreciacionActivoFijo" + "\"" + " = 1;");
                                break;
                            #endregion

                            #region Actualizacion 217
                            case 217:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER COLUMN \"v_Observaciones\" type character varying(500)");
                                break;
                            #endregion


                            #region Actualizacion 218
                            case 218:
                                listaQuerys.Add("ALTER TABLE ventadetalle ALTER \"v_Observaciones\" TYPE character varying(2000);");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_AfectaDetraccion\" integer;");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"d_TasaDetraccion\" numeric(10,2);");
                                listaQuerys.Add("ALTER TABLE diario ADD COLUMN \"i_AfectaDetraccion\" integer;");
                                listaQuerys.Add("ALTER TABLE diario ADD COLUMN \"d_TasaDetraccion\" numeric(10,2);");
                                listaQuerys.Add("ALTER TABLE venta ALTER \"v_SerieDocumento\" TYPE character varying(20);");
                                listaQuerys.Add("ALTER TABLE venta ALTER \"v_SerieDocumentoRef\" TYPE character varying(20);");
                                listaQuerys.Add("ALTER TABLE compra ALTER \"v_SerieDocumento\" TYPE character varying(20);");
                                listaQuerys.Add("ALTER TABLE compra ALTER \"v_SerieDocumentoRef\" TYPE character varying(20);");
                                listaQuerys.Add("ALTER TABLE establecimientodetalle ALTER \"v_Serie\" TYPE character varying(20);");
                                break;

                            #endregion

                            #region Actualizacion 219
                            case 219:
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD COLUMN \"t_FechaLiberacion\" timestamp(6) without time zone");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD COLUMN \"i_LiberacionUsuario\" integer");
                                break;
                            #endregion

                            #region Actualizacion 220
                            case 220:
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"v_IdAnexo\" character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 221
                            case 221:
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD COLUMN \"v_Mes\" character varying(2);");
                                listaQuerys.Add("ALTER TABLE configuracionbalances ADD COLUMN \"v_Periodo\" character varying(4);");
                                listaQuerys.Add("update  configuracionbalances  set  \"v_Mes\"='1',\"v_Periodo\"='2017' where \"v_TipoBalance\"='2';");
                                break;
                            #endregion

                            #region Actualizacion 223

                            case 223:



                                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                                {

                                    var ListaSituacionFinanciera = (from a in dbContext.configuracionbalances
                                                                    where a.v_TipoBalance == "2" && a.i_Eliminado == 0
                                                                    select a).ToList();
                                    int Mes = 2;
                                    int MesFin = DateTime.Now.Month;

                                    while (Mes <= MesFin)
                                    {
                                        foreach (var lsf in ListaSituacionFinanciera)
                                        {
                                            var inserta = lsf.t_InsertaFecha ?? DateTime.Now;
                                            listaQuerys.Add("INSERT INTO configuracionbalances (\"v_TipoBalance\", \"v_Codigo\",\"v_Nombre\",\"i_Eliminado\",\"v_NombreGrupo\",\"i_InsertaIdUsuario\",\"t_InsertaFecha\",\"v_Mes\",\"v_Periodo\") " +
                                                                                               "VALUES (" + lsf.v_TipoBalance + ",'" + lsf.v_Codigo + "','" + lsf.v_Nombre + "'," + lsf.i_Eliminado + ",'" + lsf.v_NombreGrupo + "'," + lsf.i_InsertaIdUsuario + ",'" + inserta.ToString("yyyy-MM-dd") + "' ,'" + Mes + "'," + "'2017');");


                                        }
                                        Mes = Mes + 1;
                                    }




                                }


                                break;
                            #endregion

                            #region Actualizacion 224
                            case 224:
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"d_PrecioMayorista\" decimal(18, 7)");
                                break;
                            #endregion

                            #region Actualizacion 225
                            case 225:
                                listaQuerys.Add("ALTER TABLE documento ADD COLUMN \"i_BancoDetraccion\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 226
                            case 226:
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (0, 176, E'VENTAS - PERFILES DETRACCION', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-08-31 11:34:02.229', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (176, 1, E'PERFIL 01', E'10', E'700', NULL, 0, 0, 0, 1, E'2017-08-31 11:34:31.872', 1, E'2017-08-31 11:35:04.637', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (176, 2, E'PERFIL 02', E'10', E'400', NULL, 0, 0, 0, 1, E'2017-08-31 11:34:39.843', 1, E'2017-08-31 11:35:13.576', NULL, NULL, E'');");

                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_IdPerfilDetraccion\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 227
                            case 227:

                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD \"i_PermiteEditarPedidoFacturado\" int;");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  \"b_LogoEmpresa\" bytea");
                                listaQuerys.Add("ALTER TABLE configuracionempresa ADD  \"b_FirmaDigitalEmpresa\" bytea");

                                break;
                            #endregion

                            #region Actualizacion 228
                            case 228:

                                listaQuerys.Add("ALTER TABLE producto ADD \"i_EsProductoPerecible\" int;");
                                listaQuerys.Add("ALTER TABLE pedido ADD  \"i_TieneGuia\" int");
                                break;
                            #endregion

                            #region Actualizacion  229
                            case 229:


                                listaQuerys.Add("ALTER TABLE producto DROP COLUMN \"i_EsProductoPerecible\";");
                                listaQuerys.Add("ALTER TABLE pedido DROP COLUMN \"i_TieneGuia\";");

                                break;
                            #endregion

                            #region Actualizacion  230
                            case 230:
                                listaQuerys.Add("CREATE TABLE planilladiasnolaborables " +
                                 "( " +
                                 "  \"i_IdDiaNoLaborable\" serial NOT NULL, " +
                                 "  \"t_DiaNoLaborable\" timestamp(6) without time zone, " +
                                 "  CONSTRAINT \"PK_planilladiasnolaborables\" PRIMARY KEY (\"i_IdDiaNoLaborable\") " +
                                 ") ");


                                listaQuerys.Add("CREATE TABLE planilladatoslaborables " +
                                "( " +
                                "  \"i_Id\" serial NOT NULL, " +
                                "  \"v_RangoSemana\" character varying(10), " +
                                "  \"v_RangoHoras\" character varying(10), " +
                                "  \"i_Vigente\" integer, " +
                                "  CONSTRAINT \"PK_planilladatoslaborables\" PRIMARY KEY (\"i_Id\") " +
                                ") ");


                                listaQuerys.Add("CREATE TABLE planillasemanasperiodo " +
                                "( " +
                                "  \"i_IdSemana\" serial NOT NULL, " +
                                "  \"i_NroSemana\" integer, " +
                                "  \"t_FechaInicio\" timestamp(6) without time zone, " +
                                "  \"t_FechaFin\" timestamp(6) without time zone, " +
                                "  CONSTRAINT \"PK_planillasemanasperiodo\" PRIMARY KEY (\"i_IdSemana\") " +
                                ") ");


                                listaQuerys.Add("CREATE TABLE planillaasistencia " +
                                "( " +
                                "  \"i_IdAsistencia\" serial NOT NULL, " +
                                "  \"v_IdTrabajador\" character varying(16), " +
                                "  \"i_IdSemana\" integer, " +
                                "  \"t_Fecha\" timestamp(6) without time zone, " +
                                "  \"t_Ingreso_I\" timestamp(6) without time zone, " +
                                "  \"t_Salida_I\" timestamp(6) without time zone, " +
                                "  \"t_Ingreso_II\" timestamp(6) without time zone, " +
                                "  \"t_Salida_II\" timestamp(6) without time zone, " +
                                "  \"d_HorasNormales\" numeric(20,3), " +
                                "  \"d_HorasExtras\" numeric(20,3), " +
                                "  CONSTRAINT \"PK_planillaasistencia\" PRIMARY KEY (\"i_IdAsistencia\"), " +
                                "  CONSTRAINT \"FK_planillasemanasperiodo\" FOREIGN KEY (\"i_IdSemana\") " +
                                "      REFERENCES planillasemanasperiodo (\"i_IdSemana\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION, " +
                                "  CONSTRAINT \"Fk_trabajador\" FOREIGN KEY (\"v_IdTrabajador\") " +
                                "      REFERENCES trabajador (\"v_IdTrabajador\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                ") ");


                                listaQuerys.Add("CREATE TABLE planillaturnos " +
                                "( " +
                                "  \"i_IdTurno\" serial NOT NULL, " +
                                "  \"v_Descripcion\" character varying(100), " +
                                "  \"i_IdSemana\" integer, " +
                                "  CONSTRAINT \"PK_planillaturnos\" PRIMARY KEY (\"i_IdTurno\"), " +
                                "  CONSTRAINT \"Fk_planillasemanasperiodo_planillaturnos\" FOREIGN KEY (\"i_IdSemana\") " +
                                "      REFERENCES planillasemanasperiodo (\"i_IdSemana\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                ") ");


                                listaQuerys.Add("CREATE TABLE planillaturnosdetalle " +
                                "( " +
                                "  \"i_IdTurnoDetalle\" serial NOT NULL, " +
                                "  \"i_IdTurno\" integer, " +
                                "  \"v_DiaLunes\" character varying(50), " +
                                "  \"v_DiaMartes\" character varying(50), " +
                                "  \"v_DiaMiercoles\" character varying(50), " +
                                "  \"v_DiaJueves\" character varying(50), " +
                                "  \"v_DiaViernes\" character varying(50), " +
                                "  \"v_DiaSabado\" character varying(50), " +
                                "  \"v_DiaDomingo\" character varying(50), " +
                                "  CONSTRAINT \"PK_planillaturnosdetalle\" PRIMARY KEY (\"i_IdTurnoDetalle\"), " +
                                "  CONSTRAINT \"FK_planillaturnos\" FOREIGN KEY (\"i_IdTurno\") " +
                                "      REFERENCES planillaturnos (\"i_IdTurno\") MATCH SIMPLE " +
                                "      ON UPDATE NO ACTION ON DELETE NO ACTION " +
                                ") ");

                                listaQuerys.Add("ALTER TABLE planilladiasnolaborables ADD \"v_Descripcion\" varchar(100);");
                                break;
                            #endregion

                            #region Actualizacion 231
                            case 231:
                                listaQuerys.Add("ALTER TABLE venta ADD \"i_IdCodigoDetraccion\" int;");
                                listaQuerys.Add("ALTER TABLE venta ADD \"i_IdTipoOperacionDetraccion\" int;");
                                listaQuerys.Add("ALTER TABLE diario ADD \"v_CodigoDetraccion\" character varying(5);");
                                listaQuerys.Add("ALTER TABLE diario ADD \"i_IdTipoOperacionDetraccion\" int;");

                                listaQuerys.Add("ALTER TABLE datahierarchy ALTER COLUMN \"v_Value1\" type character varying(400)");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               "VALUES (0, 177, 'TIPOS OPERACION DETRACCION','', NULL, NULL, NULL, 0, 0, 1,E'2017-09-12 11:34:02.229', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (177, 1, E'VENTA DE BIENES MUEBLES O INMUEBLES, PRESTACIÓN DE SERVICIOS O CONTRATOS DE CONSTRUCCIÓN GRAVADOS CON EL IGV','','', NULL, 0, 0, 0, 1, E'2017-09-12 11:34:02.229', 1, E'2017-08-31 11:35:04.637', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (177, 2, E'RETIRO DE BIENES GRAVADOS CON EL IGV', '', '', NULL, 0, 0, 0, 1, E'2017-09-12 11:34:02.229', 1, E'2017-09-12 11:34:02.229', NULL, NULL, E'');");


                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               "VALUES (177, 3, E'TRASLADO DE BIENES FUERA DEL CENTRO DE PRODUCCIÓN, ASÍ COMO DESDE CUALQUIER ZONA GEOGRÁFICA QUE GOCE DE BENEFICIOS TRIBUTARIOS HACIA EL RESTO DEL PAÍS, CUANDO DICHO TRASLADO NO SE ORIGINE EN UNA OPERACIÓN DE VENTA','','', NULL, 0, 0, 0, 1, E'2017-09-12 11:34:02.229', 1, E'2017-08-31 11:35:04.637', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                                "VALUES (177, 4, E'VENTA DE BIENES GRAVADA CON EL IGV REALIZADA A TRAVÉS DE LA BOLSA DE PRODUCTOS', '', '', NULL, 0, 0, 0, 1, E'2017-09-12 11:34:02.229', 1, E'2017-09-12 11:34:02.229', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\") " +
                               "VALUES (177, 5, E'VENTA DE BIENES EXONERADA DEL IGV', '', '', NULL, 0, 0, 0, 1, E'2017-09-12 11:34:02.229', 1, E'2017-09-12 11:34:02.229', NULL, NULL, E'');");




                                break;
                            #endregion

                            #region Actualizacion 232
                            case 232:
                                listaQuerys.Add("ALTER TABLE diario DROP  COLUMN \"v_CodigoDetraccion\";");
                                listaQuerys.Add("ALTER TABLE diario ADD \"i_IdCodigoDetraccion\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 233
                            case 233:
                                listaQuerys.Add("ALTER TABLE nbs_ordentrabajo ADD \"v_IdResponsable\" varchar(16);");
                                break;
                            #endregion

                            #region Actualizacion 234
                            case 234:
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaLunes\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaMartes\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaMiercoles\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaJueves\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaViernes\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaSabado\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle DROP COLUMN \"v_DiaDomingo\";");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"t_IngresoI\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"t_SalidaI\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"t_IngresoII\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"t_SalidaII\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"d_HorasSemanales\" numeric(15,3);");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"i_IdDia\" integer;");
                                listaQuerys.Add("ALTER TABLE planillaturnosdetalle ADD COLUMN \"i_Asiste\" integer;");
                                break;
                            #endregion

                            #region Actualizacion  235
                            case 235:
                                listaQuerys.Add("ALTER TABLE areaslaboratrabajador ADD COLUMN \"i_IdTurno\" integer;");
                                break;
                            #endregion

                            #region Actualizacion  236
                            case 236:
                                listaQuerys.Add("ALTER TABLE planillaasistencia DROP COLUMN \"d_HorasExtras\";");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"d_HorasExtras_25\" numeric(15,2);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"d_HorasExtras_35\" numeric(15,2);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"i_IdEstado\" integer;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"i_MinutosTardanza\" integer;");
                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (0, 178, E'PLANILLA - ESTADO ASISTENCIAS', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-09-18 11:15:53.415', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (178, 1, E'ASISTENCIA', E'A', E'', NULL, 0, 0, 0, 1, E'2017-09-18 11:16:55.396', 1, E'2017-09-18 11:17:25.212', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (178, 2, E'FALTA JUSTIFICADA', E'F', E'', NULL, 0, 0, 0, 1, E'2017-09-18 11:17:00.941', 1, E'2017-09-18 11:17:28.894', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (178, 3, E'FALTA INJUSTIFICADA', E'I', E'', NULL, 0, 0, 0, 1, E'2017-09-18 11:17:34.925', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (178, 4, E'SUBSIDIO', E'S', E'', NULL, 0, 0, 0, 1, E'2017-09-18 11:17:56.092', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (178, 5, E'VACACIONES', E'V', E'', NULL, 0, 0, 0, 1, E'2017-09-18 11:18:00.844', NULL, NULL, NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (178, 6, E'DESCANSO', E'D', E'', NULL, 0, 0, 0, 1, E'2017-09-18 11:18:00.844', NULL, NULL, NULL, NULL, E'');");
                                break;
                            #endregion

                            #region Actualizacion 237
                            case 237:

                                listaQuerys.Add("ALTER TABLE diario ADD \"t_FechaVencimiento\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE diario ADD \"t_FechaEmision\" timestamp(6) without time zone;");
                                break;
                            #endregion

                            #region Actualizacion 238
                            case 238:
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"d_HorasNormales\" TYPE integer;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"d_HorasExtras_25\" TYPE integer;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"d_HorasExtras_35\" TYPE integer;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"t_Ingreso_I_Turno\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"t_Ingreso_II_Turno\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"t_Salida_I_Turno\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"t_Salida_II_Turno\" timestamp(6) without time zone;");
                                break;
                            #endregion

                            #region Actualizacion  239
                            case 239:
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"d_HorasNormales\" TYPE numeric(15,6);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"d_HorasExtras_25\" TYPE numeric(15,6);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"d_HorasExtras_35\" TYPE numeric(15,6);");
                                listaQuerys.Add("ALTER TABLE planillaasistencia ALTER \"i_MinutosTardanza\" TYPE numeric(15,6);");
                                break;
                            #endregion

                            #region Actualizacion 240
                            case 240:
                                listaQuerys.Add("ALTER TABLE configuracionfacturacion ADD COLUMN \"v_FeEndpoint\" character varying(200),ADD COLUMN  \"v_FePassword\" character varying(100);");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_Publish\" smallint;");
                                break;

                            #endregion

                            #region Actualizacion 241
                            case 241:
                                listaQuerys.Add("ALTER TABLE ordendecompra ADD COLUMN \"i_IdTipoOrdenCompra\" integer;");



                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                               "VALUES (0, 179, E'TIPOS - ORDEN DE COMPRA', E'', NULL, NULL, NULL, 0, 0, 1, E'2017-09-22 11:15:53.415', NULL, NULL, NULL, NULL, NULL);");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (179, 1, E'NACIONAL', E'', E'', NULL, 0, 0, 0, 1, E'2017-09-22 11:16:55.396', 1, E'2017-09-22 11:17:25.212', NULL, NULL, E'');");

                                listaQuerys.Add("INSERT INTO datahierarchy (\"i_GroupId\", \"i_ItemId\", \"v_Value1\", \"v_Value2\", \"v_Field\", \"i_ParentItemId\", \"i_Header\", \"i_Sort\", \"i_IsDeleted\", \"i_InsertUserId\", \"d_InsertDate\", \"i_UpdateUserId\", \"d_UpdateDate\", \"i_SyncStatusId\", \"i_RecordStatusId\", \"v_Value4\")" +
                                "VALUES (179, 2, E'IMPORTACIÓN', E'', E'', NULL, 0, 0, 0, 1, E'2017-09-22 11:17:00.941', 1, E'2017-09-22 11:17:28.894', NULL, NULL, E'');");

                                listaQuerys.Add("update ordendecompra set " + "\"" + "i_IdTipoOrdenCompra" + "\"" + " = 1;");

                                break;
                            #endregion

                            #region Actualizacion 242
                            case 242:
                                listaQuerys.Add("ALTER TABLE importacion ADD COLUMN \"i_IdTipoDocRerefencia\" integer;");
                                listaQuerys.Add("ALTER TABLE importacion ADD COLUMN \"v_NumeroDocRerefencia\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE importacion ADD COLUMN \"v_IdDocumentoReferencia\" character varying(16);");
                                break;
                            #endregion

                            #region Actualizacion 243
                            case 243:
                                listaQuerys.Add("ALTER TABLE planillaasistencia ADD COLUMN \"i_HorasExtrasAutorizadas\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 244
                            case 244:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"t_FechaCaducidad\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"t_FechaFabricacion\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"v_NroLote\" character varying(30);");

                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"t_FechaCaducidad\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"t_FechaFabricacion\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE compradetalle ADD COLUMN \"v_NroLote\" character varying(30);");

                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"t_FechaCaducidad\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"t_FechaFabricacion\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE ventadetalle ADD COLUMN \"v_NroLote\" character varying(30);");

                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_SolicitarNroSerie\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_SolicitarNroLote\" integer;");


                                break;
                            #endregion

                            #region Actualizacion 245
                            case 245:
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD COLUMN \"t_FechaCaducidad\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE productoalmacen ADD COLUMN \"v_NroLote\" character varying(30);");
                                break;
                            #endregion

                            #region Actualizacion 246
                            case 246:
                                listaQuerys.Add("ALTER TABLE planillaconceptos ADD COLUMN \"v_Siglas\" character varying(10);");
                                listaQuerys.Add("ALTER TABLE planillaconceptos ADD COLUMN \"v_Formula\" character varying(1000);");
                                listaQuerys.Add("ALTER TABLE planillaconceptos ADD COLUMN \"i_IdTipo\" integer;");
                                break;
                            #endregion

                            #region Actualizacion 247
                            case 247:
                                Utils.Windows.RenombrarCodigosConceptos();
                                break;
                            #endregion

                            #region Actualizacion 248

                            case 248:
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD COLUMN \"v_NroLote\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD  COLUMN \"v_NroPedido\" character varying(20);");
                                listaQuerys.Add("ALTER TABLE pedidodetalle ADD COLUMN \"t_FechaCaducidad\" timestamp(6) without time zone;");

                                listaQuerys.Add("ALTER TABLE separacionproducto ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE separacionproducto ADD COLUMN \"v_NroLote\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE separacionproducto ADD  COLUMN \"v_NroPedido\" character varying(20);");


                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ADD COLUMN \"v_NroSerie\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ADD COLUMN \"v_NroLote\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE guiaremisioncompradetalle ADD  COLUMN \"t_FechaCaducidad\" timestamp(6) without time zone;");
                                listaQuerys.Add("ALTER TABLE vendedor ADD COLUMN \"i_EsActivo\" integer;");
                                listaQuerys.Add("update vendedor set " + "\"" + "i_EsActivo" + "\"" + " = 1;");

                                break;
                            #endregion

                            #region Actualizacion 249
                            case 249:
                                //**Sin Acción
                                break;
                            #endregion

                            #region Actualizacion 250
                            case 250:
                                listaQuerys.Add("ALTER TABLE movimientodetalle ADD COLUMN \"v_NroOrdenProduccion\" character varying(30);");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_SolicitaOrdenProduccion\" integer;");

                                listaQuerys.Add("CREATE TABLE ordenproduccion" +
                                             "(\"i_IdOrdenProduccion\" serial NOT NULL," +
                                             "\"v_IdProductoDetalle\" character varying(16)," +
                                             "\"v_Mes\" character varying(2)," +
                                             "\"v_Correlativo\" character varying(8)," +
                                             "\"v_Periodo\" character varying(4)," +
                                             "\"t_FechaRegistro\" timestamp(6) without time zone," +
                                             "\"v_Observacion\" character varying(200)," +
                                             "\"t_FechaInicio\" timestamp(6) without time zone," +
                                             "\"t_FechaTermino\" timestamp(6) without time zone," +
                                             "\"d_Cantidad\" decimal(16, 4)," +
                                             "\"d_CantidadUnidadMedida\" decimal(16, 4)," +
                                             "\"i_Eliminado\" integer," +
                                             "\"i_InsertaIdUsuario\" integer, " +
                                             "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                             "\"i_ActualizaIdUsuario\" integer," +
                                             "\"t_ActualizaFecha\" timestamp(6) without time zone," +
                                             "CONSTRAINT ordenproduccion_pkey PRIMARY KEY (\"i_IdOrdenProduccion\")," +
                                             "CONSTRAINT \"fk_productodetalle_v_IdProductoDetalle\" FOREIGN KEY (\"v_IdProductoDetalle\")" +
                                                 "REFERENCES productodetalle (\"v_IdProductoDetalle\") MATCH SIMPLE " +
                                                 @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");




                                break;
                            #endregion

                            #region Actualizacion 251
                            case 251:
                                listaQuerys.Add("CREATE TABLE ventaimportaciondataconfig " +
                                        "( " +
                                        "  \"i_Id\" serial NOT NULL, " +
                                        "  \"v_CtaVenta\" character varying(20), " +
                                        "  \"v_CtaEfectivo\" character varying(20), " +
                                        "  \"v_CtaVisa\" character varying(20), " +
                                        "  \"v_CtaMastercard\" character varying(20), " +
                                        "  \"v_CtaAmericanExpress\" character varying(20), " +
                                        "  CONSTRAINT \"Pk\" PRIMARY KEY (\"i_Id\") " +
                                        ") ");
                                break;
                            #endregion

                            #region Actualizacion 252
                            case 252:
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD COLUMN \"i_IdDocumentoEfectivo\" integer;");
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD COLUMN \"i_IdDocumentoVisa\" integer;");
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD COLUMN \"i_IdDocumentoMastercard\" integer;");
                                listaQuerys.Add("ALTER TABLE ventaimportaciondataconfig ADD COLUMN \"i_IdDocumentoAmericanExpress\" integer;");
                                break;
                            #endregion

                            #region Actualización 253:
                            case 253:

                                listaQuerys.Add("ALTER TABLE pedido ALTER \"v_Glosa\" TYPE character varying(200);");
                                break;
                            #endregion


                            #region Actualizacion 254
                            case 254:
                                listaQuerys.Add("ALTER TABLE asientocontable ALTER \"v_CodigoSituacionFinaciera\" TYPE character varying(200);");
                                listaQuerys.Add("ALTER TABLE producto RENAME COLUMN \"i_SolicitarNroSerie\" TO \"i_SolicitarNroSerieIngreso\"");
                                listaQuerys.Add("ALTER TABLE producto RENAME COLUMN \"i_SolicitarNroLote\" TO \"i_SolicitarNroLoteIngreso\"");
                                listaQuerys.Add("ALTER TABLE producto RENAME COLUMN \"i_SolicitaOrdenProduccion\" TO \"i_SolicitaOrdenProduccionIngreso\"");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_SolicitarNroSerieSalida\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_SolicitarNroLoteSalida\" integer;");
                                listaQuerys.Add("ALTER TABLE producto ADD COLUMN \"i_SolicitaOrdenProduccionSalida\" integer;");

                                break;
                            #endregion

                            #region Actualizacion 255
                            case 255:



                                listaQuerys.Add("CREATE TABLE ordenproducciondocumentos" +
                                            "(\"i_IdOrdenProduccionDocumentos\" serial NOT NULL," +
                                             "\"i_IdOrdenProduccion\" integer," +
                                             "\"i_IdTipoDocumento\" integer," +
                                            "\"v_SerieDocumento\" character varying(4)," +
                                            "\"v_CorrelativoDocumento\" character varying(8)," +
                                            "\"i_Eliminado\" integer," +
                                            "\"i_InsertaIdUsuario\" integer, " +
                                            "\"t_InsertaFecha\" timestamp(6) without time zone," +
                                            "\"i_ActualizaIdUsuario\" integer," +
                                            "\"t_ActualizaFecha\" timestamp(6) without time zone," +
                                            "CONSTRAINT ordenproducciondocumentos_pkey PRIMARY KEY (\"i_IdOrdenProduccionDocumentos\")," +
                                            "CONSTRAINT \"fk_ordenproduccion_i_IdOrdenProduccion\" FOREIGN KEY (\"i_IdOrdenProduccion\")" +
                                                "REFERENCES ordenproduccion (\"i_IdOrdenProduccion\") MATCH SIMPLE " +
                                                @"ON UPDATE RESTRICT ON DELETE RESTRICT
											)
											WITH (
											  OIDS=FALSE
											);");

                                break;
                            #endregion

                            #region Actualizacion 256
                            case 256:
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"v_IdDocAnticipo\" character(16)");
                                listaQuerys.Add("ALTER TABLE venta ADD COLUMN \"i_EsAnticipo\" integer");
                                break;
                            #endregion
                        }

                        break;

                    #endregion
                }

                ExecuteQueries(listaQuerys);

                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                #region Manejo de excepcion
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DBConfig.ActualizaEsquemaBD()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = "No se pudo actualizar el esquema de la Base de Datos: " + ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                #endregion
            }
        }

        /// <summary>
        /// Execute the queries in current DbContext.
        /// </summary>
        /// <param name="queries">The queries.</param>
        private static void ExecuteQueries(IEnumerable<string> queries)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var cnx = (dbContext.Connection as System.Data.EntityClient.EntityConnection)
                    .StoreConnection;
                cnx.Open();
                var trans = cnx.BeginTransaction();
                try
                {
                    var cmd = cnx.CreateCommand();
                    cmd.Transaction = trans;
                    foreach (var query in queries)
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}
