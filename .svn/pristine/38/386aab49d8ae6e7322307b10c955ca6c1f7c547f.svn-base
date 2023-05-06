using System;
using System.Drawing;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI
{
    public sealed partial class SplashScreen : Form
    {

        #region Constructor

        public SplashScreen()
        {
            DoubleBuffered = true;

            // The splash screen should not take focus.
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.StandardClick, false);

            // Required for Windows Form Designer support
            InitializeComponent();
            InitializeUi();
        }

        #endregion //Constructor

        #region Private Methods

        #region InitializeUI
        private void InitializeUi()
        {
            LocalizeStrings();

            HookEvents();
        }
        #endregion InitializeUI

        #region HookEvents
        private void HookEvents()
        {
            frmLogin.InitializationStatusChanged += Application_InitializationStatusChanged;
            frmLogin.InitializationComplete += Application_InitializationComplete;
        }
        #endregion HookEvents

        #region UnHookEvents
        private void UnHookEvents()
        {
            frmLogin.InitializationStatusChanged -= new InitializationStatusChangedEventHandler(this.Application_InitializationStatusChanged);
            frmLogin.InitializationComplete -= new EventHandler(this.Application_InitializationComplete);
        }
        #endregion UnHookEvents

        #region LocalizeStrings
        private void LocalizeStrings()
        {
            //this.lblAppName.Text = AboutControl.ApplicationName;
            //this.lblVersion.Text = string.Format(" v {0}", Infragistics.Shared.AssemblyVersion.MajorMinor);
            //this.lblStatus.Text = string.Format(rm.GetString("Application_Starting"), Properties.Resources.Title.ToUpper());
        }
        #endregion //LocalizeStrings

        #endregion Private Methods

        #region Event Handlers

        #region Application_InitializationStatusChanged

        private void Application_InitializationStatusChanged(object sender, InitializationStatusChangedEventArgs e)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new MethodInvoker(() => lblStatus.Text = e.Status));
            else
                lblStatus.Text = e.Status;
        }

        #endregion // Application_InitializationStatusChanged

        #region Application_InitializationComplete
        private void Application_InitializationComplete(object sender, EventArgs e)
        {
            if (InvokeRequired)
                try
                {
                    Invoke(new MethodInvoker(Close));
                }
                catch { }
            else
                Close();
        }
        #endregion //Application_InitializationComplete

        #endregion Event Handlers

        #region Base class overrides

        #region SetVisibleCore
        protected override void SetVisibleCore(bool visible)
        {
            if (visible)
            {
                // The Application.Run call will force the visible property to 
                // true but that will cause the splash screen to be activated
                // thereby deactivating other windows before the app has fully
                // loaded. In an effort to prevent this, we'll use apis to show
                // the splash screen without activating it. Note, since we are 
                // showing the form, we have to do the centering and so I removed
                // the FormStartPosition property setting.
                //
                var topMost = true;
                Rectangle formRect = new Rectangle(Point.Empty, Size);
                Rectangle screenRect = Infragistics.Win.Utilities.ScreenFromPoint(Cursor.Position).Bounds;
                Infragistics.Win.DrawUtility.AdjustHAlign(Infragistics.Win.HAlign.Center, ref formRect, screenRect);
                Infragistics.Win.DrawUtility.AdjustVAlign(Infragistics.Win.VAlign.Middle, ref formRect, screenRect);
                Point location = formRect.Location;
                var insertAfter = topMost ? NativeWindowMethods.HWND_TOPMOST : IntPtr.Zero;
                NativeWindowMethods.SetWindowPos(Handle, insertAfter, location.X, location.Y, 0, 0, NativeWindowMethods.SWP_NOACTIVATE | NativeWindowMethods.SWP_NOSIZE);
                NativeWindowMethods.ShowWindow(Handle, NativeWindowMethods.SW_SHOWNA);
                base.SetVisibleCore(true);
            }

            base.SetVisibleCore(visible);
        }
        #endregion //SetVisibleCore

        #region WndProc
        protected override void WndProc(ref System.Windows.Forms.Message message)
        {
            // We don't want the splash screen to be clickable and if possible
            // prevent its activation.
            switch (message.Msg)
            {
                case NativeWindowMethods.WM_MOUSEACTIVATE:
                    message.Result = (IntPtr)NativeWindowMethods.MA_NOACTIVATE;
                    break;
                case NativeWindowMethods.WM_NCHITTEST:
                    message.Result = (IntPtr)NativeWindowMethods.HTTRANSPARENT;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine(message.ToString(), DateTime.Now.ToString("hh:mm:ss:ffffff"));
                    base.WndProc(ref message);
                    break;
            }
        }
        #endregion //WndProc

        #region OnLoad
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            frmLogin.SplashLoadedEvent.Set();
        }
        #endregion //OnLoad

        #endregion //Base class overrides

        #region Event-related

        #region Delegates

        /// <summary>
        /// Event handler for the <c>SplashScreen</c>
        /// </summary>
        internal delegate void InitializationStatusChangedEventHandler(object sender, InitializationStatusChangedEventArgs e);

        #endregion Delegates

        #region Event Args

        #region InitializationStatusChangedEventArgs
        internal class InitializationStatusChangedEventArgs : EventArgs
        {
            private readonly bool _showProgressBar;
            private readonly int _percentComplete;
            private readonly string _status;

            public InitializationStatusChangedEventArgs(string status) : this(status, false, 0) { }

            public InitializationStatusChangedEventArgs(string status, bool showProgressBar, int percentComplete)
            {
                _status = status;
                _showProgressBar = showProgressBar;
                _percentComplete = percentComplete;
            }

            public string Status
            {
                get { return _status; }
            }

            public bool ShowProgressBar
            {
                get { return _showProgressBar; }
            }

            public int PercentComplete
            {
                get { return _percentComplete; }
            }
        }
        #endregion InitializationStatusChangedEventArgs

        #endregion Event Args	

        #endregion //Event-related
    }
}
