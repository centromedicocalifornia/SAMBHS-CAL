using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace SAMBHS.Common.Resource
{
    public class GlobalFormColors : IDisposable
    {
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        #region Azul
        public System.Drawing.Color FormColor = System.Drawing.Color.FromArgb(255,255,255);
        public System.Drawing.Color BannerColor = System.Drawing.Color.FromArgb(240,240,240);

        #endregion

        #region Andres Template
        //public System.Drawing.Color FormColor = System.Drawing.Color.FromArgb(231, 231, 231); //VS 2013 Azul
        //public System.Drawing.Color BannerColor = System.Drawing.Color.FromArgb(61, 133, 198); // VS 2013 Azul 
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
            }
            disposed = true;
        }
    }
}
