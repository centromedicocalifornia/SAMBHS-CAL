using System;
using System.Threading;
using System.Windows.Forms;

namespace SAMBHS.Common.Resource
{
    public class ConexionPerdidaClase : IDisposable
    {
        #region Field
        private Form _splash;
        #endregion

        #region Construct
        public ConexionPerdidaClase()
        {
            var t = new Thread(WorkerThread) {IsBackground = true};
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _splash.Invoke(new MethodInvoker(StopThread));
        }
        #endregion

        #region Method
        private void StopThread()
        {
            _splash.Close();
        }

        private void WorkerThread()
        {
            //_splash = new ConexionPerdida
            //{
            //    StartPosition = FormStartPosition.CenterScreen,
            //    TopMost = true
            //}; // Substitute this with your own
            ////mSplash.Location = mLocation;
            //Application.Run(_splash);
        }
        #endregion
    }
}
