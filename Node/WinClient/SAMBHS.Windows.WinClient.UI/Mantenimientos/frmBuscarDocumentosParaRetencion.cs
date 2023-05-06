using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Tesoreria.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmBuscarDocumentosParaRetencion : Form
    {
        public delegate void SeleccionTerminada(IEnumerable<ConsultaComprasRetencion> lista);
        public event SeleccionTerminada SeleccionTerminadaEvent;
        private readonly OperationResult _objOperationResult = new OperationResult();

        public FrmBuscarDocumentosParaRetencion(string idProveedor, string nombreProveedor)
        {
            InitializeComponent();
            var ds = DocumentoRetencionBL.ObtenerDocumentosParaRetencionPorRuc(ref _objOperationResult,
                idProveedor);
            if (_objOperationResult.Success == 1)
            {
                grdData.DataSource = ds;
                foreach (var row in grdData.Rows)  
                    row.Cells["_check"].Value = false;
                Text = string.Format("Compras afectas a retencion de: {0}", nombreProveedor);
            }
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void frmBuscarDocumentosParaRetencion_Load(object sender, EventArgs e)
        {

        }

        protected virtual void OnSeleccionTerminadaEvent(IEnumerable<ConsultaComprasRetencion> lista)
        {
            var handler = SeleccionTerminadaEvent;
            if (handler != null) handler(lista);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void btnTerminar_Click(object sender, EventArgs e)
        {
            var filasSeleccionadas = grdData.Rows.Where(p => bool.Parse(p.Cells["_check"].Value.ToString())).ToList();
            if (SeleccionTerminadaEvent != null) SeleccionTerminadaEvent(filasSeleccionadas.Select(f => (ConsultaComprasRetencion)f.ListObject).ToList());
            Close(); 
        }
    }
}
