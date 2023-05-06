using System;
using System.Deployment.Application;
using System.Threading.Tasks;
using Infragistics.Win.Misc;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI
{
    public class WorkerUpdate
    {
        #region Properties
        private Task _objTaskTestUpdate;
        private readonly Func<Delegate, object> _invoke;
        private bool _isRun;
        private readonly object _lock = new object(); 
        #endregion

        #region Events
        public event EventHandler UpdateAvailable;
        private void OnUpdateAvailable()
        {
            if (UpdateAvailable != null)
                UpdateAvailable(this, new EventArgs());
        }
        #endregion

        public WorkerUpdate(Func<Delegate, object> pfuncInvoke)
        {
            _isRun = false;
            this._invoke = pfuncInvoke;
        }
        /// <summary>
        /// Inicia la Tarea de Busqueda de Actualizacion
        /// </summary>
        public void Start()
        {
            if (_objTaskTestUpdate != null && (_objTaskTestUpdate.Status == TaskStatus.Running)) return;
            _objTaskTestUpdate = new Task(TestUpdate_Work, TaskCreationOptions.LongRunning);
            _isRun = true;
            _objTaskTestUpdate.Start();
        }

        /// <summary>
        /// Para el proceso de Testear Actualizacion Disponible.
        /// </summary>
        public void Stop()
        {
            lock (_lock)
                _isRun = false;
        }

        private void TestUpdate_Work()
        {
            try
            {
                while (_isRun)
                {
                    var t = SleepAsyncB((int)TimeSpan.FromMinutes(4).TotalMilliseconds);
                    t.Wait();
                    //Thread.Sleep(TimeSpan.FromMinutes(4));
                    if (!ApplicationDeployment.IsNetworkDeployed) break;
                    var ad = ApplicationDeployment.CurrentDeployment;

                    var info = ad.CheckForDetailedUpdate();

                    if (!info.UpdateAvailable) continue;
                    //si encuentra una actualizacion
                    lock (_lock)
                    _isRun = false;
                    CrearNotificacion(info.AvailableVersion.ToString());
                }
            }
            catch
            {
                // ignored
            }
        }

        private Task SleepAsyncB(int millisecondsTimeout)
        {
            TaskCompletionSource<bool> tcs = null;
            var t = new System.Threading.Timer(delegate { tcs.TrySetResult(true); }, null, -1, -1);
            tcs = new TaskCompletionSource<bool>(t);
            t.Change(millisecondsTimeout, -1);
            return tcs.Task;
        }

        private void CrearNotificacion(string pstrVersion)
        {
            var desktopAlert = new UltraDesktopAlert
            {
                AnimationStyleShow = AnimationStyle.Scroll,
                AnimationScrollDirectionShow = AnimationScrollDirection.RightToLeft,
                AnimationSpeed = AnimationSpeed.Slow
            };
            var alerta = new UltraDesktopAlertShowWindowInfo("Actualizacion Disponible", "Descargar")
            {
                FooterText = "Nueva Versión : " + pstrVersion,
                PinButtonVisible = true,
                Pinned = true
            };

            desktopAlert.DesktopAlertLinkClicked += delegate
            {
                desktopAlert.Dispose();
                OnUpdateAvailable();
            };
            _invoke(new MethodInvoker(() => desktopAlert.Show(alerta)));
        }
    }
}
