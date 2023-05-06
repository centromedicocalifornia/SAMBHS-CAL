using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using System.Windows.Forms;

namespace SAMBHS.Common.Resource.Components
{
    [DefaultProperty("Grid")]
    [ToolboxItemFilter("System.Windows.Forms")]
    [System.Drawing.ToolboxBitmap(typeof(DataGridView))]
    public partial class GridAccion : Component
    {
        #region Fields
        private UltraGrid _grid;
        private IButtonControl _AccepButton;
        #endregion

        #region Public Event
        [Description("Invoca la Edicion de una Fila")]
        public event EventHandler EditRowFired;
        [Description("Invoca la Eliminacion de una Fila")]
        public event EventHandler DeleteRowFired;
        #endregion

        #region Property
        [Category("Data")]
        [Description("Grilla a Controlar")]
        public UltraGrid Grid
        {
            get { return _grid; }
            set
            {
                if (_grid != null)
                {
                    _grid.GotFocus -= _grid_GotFocus;
                    _grid.LostFocus -= _grid_LostFocus;
                    _grid.KeyDown -= _grid_KeyDown;
                    _grid.InitializeLayout -= _grid_InitializeLayout;
                    _grid.DoubleClickRow -= _grid_DoubleClickRow;
                    _AccepButton = null;
                }
                _grid = value;
                if (value != null)
                {
                    _grid.InitializeLayout += _grid_InitializeLayout;
                    _grid.DoubleClickRow += _grid_DoubleClickRow;
                    _grid.KeyDown += _grid_KeyDown;
                }
            }
        }
        #endregion

        #region Events Manager
        void _grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (EditRowFired != null)
                EditRowFired(sender, e);
        }

        void _grid_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.E) || e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                if (EditRowFired != null)
                    EditRowFired(sender, e);
            }
            else if ((e.Control && e.KeyCode == Keys.X) || e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                if (DeleteRowFired != null)
                    DeleteRowFired(sender, e);
            }
        }

        void _grid_LostFocus(object sender, EventArgs e)
        {
            _grid.FindForm().AcceptButton = _AccepButton;
        }

        void _grid_GotFocus(object sender, EventArgs e)
        {
            _grid.FindForm().AcceptButton = null;
        }

        void _grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            var form = _grid.FindForm();
            _AccepButton = form.AcceptButton;
            if (_AccepButton != null)
            {
                _grid.GotFocus += _grid_GotFocus;
                _grid.LostFocus += _grid_LostFocus;
            }
            _grid.InitializeLayout -= _grid_InitializeLayout;
        }
        #endregion

        #region Construct
        public GridAccion()
        {
            InitializeComponent();
        }

        public GridAccion(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion
    }
}
