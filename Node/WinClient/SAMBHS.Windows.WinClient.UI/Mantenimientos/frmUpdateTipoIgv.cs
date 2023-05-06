using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmUpdateTipoIgv : Form
    {
        #region Fields
        private CancellationTokenSource _source;
        private CancellationToken _token;
        private Task _task;
        #endregion

        #region Construct
        public frmUpdateTipoIgv(string arg)
        {
            InitializeComponent();
        }
        #endregion

        #region Events UI
        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (btnExecute.Text.Equals("Cancelar"))
            {
                if (!_source.IsCancellationRequested)
                    _source.Cancel(true);
                if (_task.Status != TaskStatus.Running)
                {
                    btnExecute.Text = "Actualizar";
                    bar.Value = 0;
                }
                else
                {
                    btnExecute.Enabled = false;
                    lblStatus.Text = "Cancelando...";
                }
            }
            else
            {
                Action<object> _update;
                if (Globals.TipoMotor == TipoMotorBD.MSSQLServer)
                {
                    btnExecute.Enabled = false;
                    _update = UpdateSqlServer;
                }
                else
                {
                    btnExecute.Text = "Cancelar";
                    _update = UpdatePostgres;
                }
                _source = new CancellationTokenSource();
                _token = _source.Token;
                lblStatus.Text = "Starting...";
                _task = System.Threading.Tasks.Task.Factory.StartNew(_update, GetStringConection(Globals.TipoMotor), _token);
            }
        }

        private void btnUpdatePedido_Click(object sender, EventArgs e)
        {
            if (btnUpdatePedido.Text.Equals("Cancelar"))
            {
                if (!_source.IsCancellationRequested)
                    _source.Cancel(true);
                if (_task.Status != TaskStatus.Running)
                {
                    btnUpdatePedido.Text = "Actualizar";
                    bar.Value = 0;
                }
                else
                {
                    btnUpdatePedido.Enabled = false;
                    lblStatus.Text = "Cancelando...";
                }
            }
            else
            {
                Action<object> _update;
                if (Globals.TipoMotor == TipoMotorBD.MSSQLServer)
                {
                    btnUpdatePedido.Enabled = false;
                    _update = UpdateSqlServerPedido;
                }
                else
                {
                    btnUpdatePedido.Text = "Cancelar";
                    _update = UpdatePostgresPedido;
                }
                _source = new CancellationTokenSource();
                _token = _source.Token;
                lblStatus.Text = "Starting...";
                _task = System.Threading.Tasks.Task.Factory.StartNew(_update, GetStringConection(Globals.TipoMotor), _token);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the string conection.
        /// </summary>
        /// <param name="typeDb">The type database.</param>
        /// <returns>System.String.</returns>
        private string GetStringConection(TipoMotorBD typeDb)
        {
            var server = UserConfig.Default.csServidor;
            var db = Globals.ClientSession.v_RucEmpresa;
            var user = UserConfig.Default.csUsuario;
            var password = Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");

            if (typeDb == TipoMotorBD.MSSQLServer)
            {
                var con = new System.Data.SqlClient.SqlConnectionStringBuilder();
                con.InitialCatalog = db;
                con.DataSource = server;
                con.UserID = user;
                con.Password = password;
                con.IntegratedSecurity = false;
                con.PersistSecurityInfo = true;
                con.MultipleActiveResultSets = true;
                con.ConnectTimeout = 0;
                return con.ConnectionString;
            }
            else
            {
                var con = new Devart.Data.PostgreSql.PgSqlConnectionStringBuilder();
                con.Database = db;
                con.Host = server;
                con.UserId = user;
                con.Password = password;
                con.DefaultCommandTimeout = 0;
                con.Schema = "public";
                return con.ConnectionString;
            }
        }

        private void UpdateSqlServerPedido(object strcon)
        {
            try
            {
                using (var con = new System.Data.SqlClient.SqlConnection((string)strcon))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    Invoke(new MethodInvoker(() => bar.Value = 100));
                    cmd.CommandText = @"
                    DECLARE @idPedido VARCHAR(16);
                    DECLARE @idIgv INT;
                    DECLARE cPedidos CURSOR FOR SELECT v_IdPedido, i_AfectoIgv FROM dbo.pedido WHERE i_Eliminado = 0;
                    OPEN cPedidos;
                    FETCH NEXT FROM cPedidos INTO @idPedido, @idIgv;
                    WHILE @@FETCH_STATUS = 0
                      BEGIN
                        IF @idIgv = 0
                          BEGIN
                            SET @idIgv = 2;
                            UPDATE dbo.pedido SET i_IdTipoOperacion = 2 WHERE v_IdPedido = @idPedido
                          END
                        UPDATE dbo.pedidodetalle SET i_IdTipoOperacion = @idIgv WHERE v_IdPedido = @idPedido AND i_Eliminado = 0 AND (v_IdProductoDetalle IS NULL OR v_IdProductoDetalle <> 'N002-PE000000000');
                        FETCH NEXT FROM cPedidos INTO @idPedido, @idIgv;
                      END
                    CLOSE cPedidos;
                    DEALLOCATE cPedidos;";
                    _token.ThrowIfCancellationRequested();
                    cmd.ExecuteNonQuery();
                }
                Invoke(new MethodInvoker(() => lblStatus.Text = "¡FINALIZADO!"));
            }
            catch (Exception er)
            {
                Invoke(new MethodInvoker(() => lblStatus.Text = "Cancelado: " + er.Message));
            }
            finally
            {
                Invoke(new MethodInvoker(() =>
                {
                    bar.Value = 0;
                    btnUpdatePedido.Enabled = true;
                }));
            }
        }

        private void UpdatePostgresPedido(object strcon)
        {
            //Devart.Data.PostgreSql.PgSqlTransaction trans = null;
            try
            {
                using (var con = new Devart.Data.PostgreSql.PgSqlConnection((string)strcon))
                {
                    con.Open();
                    //trans = con.BeginTransaction();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "DROP FUNCTION IF EXISTS operPedidos(int)";
                    cmd.ExecuteNonQuery();
                    //cmd.Transaction = trans;
                    cmd.CommandText = "SELECT COUNT(*) FROM pedido WHERE \"i_Eliminado\" = 0";
                    var count = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd.CommandText = @"
                    CREATE FUNCTION operPedidos(int) RETURNS VOID AS $$
                    DECLARE
                      p RECORD;
                      tipo INTEGER;
                    BEGIN  " + 
                      " FOR p IN SELECT  \"v_IdPedido\", \"i_AfectoIgv\" FROM pedido WHERE \"i_Eliminado\" = 0 LIMIT 1000 OFFSET $1 " +
                      "  LOOP " +
                      "  tipo := p.\"i_AfectoIgv\"; " +
                      "  IF p.\"i_AfectoIgv\" = 0 THEN " +
                          " tipo := 2; " +
                          " UPDATE pedido SET \"i_IdTipoOperacion\" = 2 WHERE \"v_IdPedido\" = p.\"v_IdPedido\"; " +
                        " END IF; " +
                        " UPDATE pedidodetalle SET \"i_IdTipoOperacion\" = tipo WHERE \"v_IdPedido\" = p.\"v_IdPedido\" AND \"i_Eliminado\" = 0  AND (\"v_IdProductoDetalle\" IS NULL OR \"v_IdProductoDetalle\" != 'N002-PE000000000'); " +
                      @" END LOOP;
                     END;
                    $$ LANGUAGE plpgsql;";
                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
                    for (int i = 0; i < count; i += 1000)
                    {
                        t.Start();
                        cmd.CommandText = string.Format("SELECT operPedidos({0})", i);
                        cmd.ExecuteNonQuery();
                        t.Stop();
                        _token.ThrowIfCancellationRequested();
                        if (count < 1000) continue;
                        Invoke(new MethodInvoker(() =>
                        {
                            lblStatus.Text = "Hora de Fin : " + DateTime.Now.AddMilliseconds(((count - i) / 1000) * t.ElapsedMilliseconds).ToString("hh:mm:ss tt");
                            bar.Value = i * 100 / count;
                        }));
                        t.Reset();
                    }
                    cmd.CommandText = "DROP FUNCTION IF EXISTS operPedidos(int)";
                    cmd.ExecuteNonQuery();
                    Invoke(new MethodInvoker(() => lblStatus.Text = "¡Finalizado!"));
                    //trans.Commit();
                }
            }
            catch (Exception er)
            {
                string message = "Cancelado ! ";
                if (!(er is System.OperationCanceledException))
                    message += er.Message;
                Invoke(new MethodInvoker(() => lblStatus.Text = message));
                //if(trans != null)
                //    trans.Rollback();
            }
            finally
            {
                Invoke(new MethodInvoker(() =>
                {
                    btnUpdatePedido.Text = "Actualizar";
                    btnUpdatePedido.Enabled = true;
                    bar.Value = 0;
                }));
            }
        }

        private void UpdateSqlServer(object strcon)
        {
            try
            {
                using (var con = new System.Data.SqlClient.SqlConnection((string)strcon))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    Invoke(new MethodInvoker(()=> bar.Value = 100));
                    cmd.CommandText = @"
                    DECLARE @idVenta VARCHAR(16);
                    DECLARE @idTipoV INT;
                    DECLARE @TipoV INT;
                    DECLARE @idTipo INT;
                    DECLARE @idTipoa INT;
                    DECLARE @idDetalle NCHAR(16);

                    DECLARE cventas CURSOR FOR SELECT v_IdVenta,i_IdTipoOperacion, i_IdTipoVenta FROM dbo.venta WHERE i_Eliminado = 0 AND i_IdEstado = 1;
                    OPEN cventas;
                    FETCH NEXT FROM cventas INTO @idVenta, @idTipoV, @TipoV;
                    WHILE @@FETCH_STATUS = 0  
                        BEGIN
                        IF @TipoV = 5 
	                    BEGIN
		                    UPDATE venta SET i_IdTipoOperacion = 4 WHERE v_IdVenta = @idVenta;
                            UPDATE ventadetalle SET i_IdTipoOperacion = 4 WHERE v_IdVenta = @idVenta;
		                    FETCH NEXT FROM cventas INTO @idVenta, @idTipoV, @TipoV;
                            CONTINUE;
	                    END
                        DECLARE cdetalle CURSOR FOR SELECT v_IdVentaDetalle,i_IdTipoOperacion, i_IdTipoOperacionAnexo FROM dbo.ventadetalle WHERE v_IdVenta = @idVenta AND i_Eliminado = 0 AND (v_IdProductoDetalle IS NULL OR v_IdProductoDetalle <> 'N002-PE000000000');
                        OPEN cdetalle;
                        FETCH NEXT FROM cdetalle INTO @idDetalle, @idTipo, @idTipoa;
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                        IF @idTipo IS NULL OR @idTipo = 0
                        BEGIN
                            SET @idTipo = @idTipoV;
                            UPDATE dbo.ventadetalle SET i_IdTipoOperacion = @idTipo WHERE v_IdVentaDetalle = @idDetalle;
                        END
                        ELSE IF @idTipo = 2 AND @idTipoa = 1
                        BEGIN
                            SET @idTipo = 3;
                            UPDATE dbo.ventadetalle SET i_IdTipoOperacion = @idTipo WHERE v_IdVentaDetalle = @idDetalle;
                        END
                        FETCH NEXT FROM cdetalle INTO @idDetalle, @idTipo, @idTipoa;
                        END
                        CLOSE cdetalle;
                        DEALLOCATE cdetalle;
  
                        IF (SELECT COUNT(DISTINCT i_IdTipoOperacion) FROM dbo.ventadetalle WHERE v_IdVenta = @idVenta AND i_Eliminado = 0 AND (v_IdProductoDetalle IS NULL OR v_IdProductoDetalle <> 'N002-PE000000000')) > 1
                            SET @idTipo = 5;

                        IF  @idTipoV IS NULL OR @idTipo != @idTipoV
                        UPDATE dbo.venta SET i_IdTipoOperacion = @idTipo WHERE v_IdVenta = @idVenta;
                        FETCH NEXT FROM cventas INTO @idVenta, @idTipoV, @TipoV;
                        END
                        CLOSE cventas;
                        DEALLOCATE cventas;";
                    _token.ThrowIfCancellationRequested();
                    cmd.ExecuteNonQuery();
                }
                Invoke(new MethodInvoker(() => lblStatus.Text = "¡FINALIZADO!"));
            }
            catch (Exception er)
            {
                Invoke(new MethodInvoker(() =>  lblStatus.Text = "Cancelado: " + er.Message));
            }
            finally
            {
                Invoke(new MethodInvoker(() => {
                    bar.Value = 0;
                    btnExecute.Enabled = true; }));
            }
        }

        private void UpdatePostgres(object strcon)
        {
            //Devart.Data.PostgreSql.PgSqlTransaction trans = null;
            try
            {
                using (var con = new Devart.Data.PostgreSql.PgSqlConnection((string)strcon))
                {
                    con.Open();
                    //trans = con.BeginTransaction();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "DROP FUNCTION IF EXISTS operacionesTipo(int)";
                    cmd.ExecuteNonQuery();
                    //cmd.Transaction = trans;
                    cmd.CommandText = "SELECT COUNT(*) FROM venta WHERE \"i_Eliminado\" = 0 AND \"i_IdEstado\" = 1";
                    var count = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd.CommandText = @"
                    CREATE FUNCTION operacionesTipo(int) RETURNS VOID AS $$
                    DECLARE
                     v  record;
                     d  record;
                     tipo1 INTEGER;
                    BEGIN
                       FOR v IN " +
                            " SELECT \"v_IdVenta\",\"i_IdTipoOperacion\",\"i_IdTipoVenta\" FROM venta WHERE \"i_Eliminado\" = 0 AND \"i_IdEstado\" = 1 LIMIT 1000 OFFSET $1 " +
                        " LOOP " +
                        " IF v.\"i_IdTipoVenta\" = 5 THEN" +
                        " UPDATE venta SET \"i_IdTipoOperacion\" = 4 WHERE \"v_IdVenta\" = v.\"v_IdVenta\";" +
                        " UPDATE ventadetalle SET \"i_IdTipoOperacion\" = 4 WHERE \"v_IdVenta\" = v.\"v_IdVenta\";" +
                        " CONTINUE;" +
                        " END IF;" +
                        " FOR d IN SELECT \"v_IdVentaDetalle\", \"i_IdTipoOperacion\", \"i_IdTipoOperacionAnexo\" FROM ventadetalle WHERE \"v_IdVenta\" = v.\"v_IdVenta\" AND \"i_Eliminado\" = 0 AND (\"v_IdProductoDetalle\" IS NULL OR \"v_IdProductoDetalle\" != 'N002-PE000000000')" +
                          " LOOP IF d.\"i_IdTipoOperacion\" IS NULL OR d.\"i_IdTipoOperacion\" = 0 THEN " +
                              " UPDATE ventadetalle SET \"i_IdTipoOperacion\" = v.\"i_IdTipoOperacion\" WHERE \"v_IdVentaDetalle\" = d.\"v_IdVentaDetalle\"; " +
                            " ELSIF d.\"i_IdTipoOperacion\" = 2 AND d.\"i_IdTipoOperacionAnexo\" = 1 THEN " +
                            " UPDATE ventadetalle SET \"i_IdTipoOperacion\" = 3 WHERE \"v_IdVentaDetalle\" = d.\"v_IdVentaDetalle\";"+
                            " END IF; " +
                          " END LOOP;" +
                          " IF (SELECT COUNT(DISTINCT \"i_IdTipoOperacion\") FROM ventadetalle WHERE \"v_IdVenta\" = v.\"v_IdVenta\" AND \"i_Eliminado\" = 0 AND (\"v_IdProductoDetalle\" IS NULL OR \"v_IdProductoDetalle\" != 'N002-PE000000000')) = 1 THEN " +
                            " SELECT \"i_IdTipoOperacion\" INTO tipo1 FROM ventadetalle WHERE \"v_IdVenta\" = v.\"v_IdVenta\" AND \"i_Eliminado\" = 0 AND (\"v_IdProductoDetalle\" IS NULL OR \"v_IdProductoDetalle\" != 'N002-PE000000000') LIMIT 1;" +
                          " ELSE  tipo1 := 5; END IF; " +
                          " IF v.\"i_IdTipoOperacion\" IS NULL OR tipo1 <> v.\"i_IdTipoOperacion\"	THEN  "+
	                    " UPDATE venta SET \"i_IdTipoOperacion\" = tipo1 WHERE \"v_IdVenta\" = v.\"v_IdVenta\";"+
                          " END IF;" +
                       " END LOOP;" +
                    " END; $$ LANGUAGE plpgsql;";
                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
                    for (int i = 0; i < count; i += 1000)
                    {
                        t.Start();
                        cmd.CommandText = string.Format("SELECT operacionesTipo({0})", i);
                        cmd.ExecuteNonQuery();
                        t.Stop();
                        _token.ThrowIfCancellationRequested();
                        if (count < 1000) continue;
                        Invoke(new MethodInvoker(() => {
                            lblStatus.Text = "Hora de Fin : " + DateTime.Now.AddMilliseconds(((count - i) / 1000) * t.ElapsedMilliseconds).ToString("hh:mm:ss tt");
                            bar.Value = i * 100 / count;
                        }));                   
                        t.Reset();
                    }
                    cmd.CommandText = "DROP FUNCTION IF EXISTS operacionesTipo(int)";                 
                    cmd.ExecuteNonQuery();
                    Invoke(new MethodInvoker(() => lblStatus.Text = "¡Finalizado!"));
                    //trans.Commit();
                }
            }
            catch(Exception er){
                string message = "Cancelado ! ";
                if (!(er is System.OperationCanceledException))
                    message += er.Message;
                Invoke(new MethodInvoker(() => lblStatus.Text = message));
                //if(trans != null)
                //    trans.Rollback();
            }
            finally{
                Invoke(new MethodInvoker(() => {
                    btnExecute.Text = "Actualizar";
                    btnExecute.Enabled = true;
                    bar.Value = 0;
                }));
            }
        }
        #endregion

    }
}
