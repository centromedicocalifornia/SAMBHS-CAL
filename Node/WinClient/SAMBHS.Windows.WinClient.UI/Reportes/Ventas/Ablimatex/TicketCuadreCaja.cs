﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
using Par = System.Collections.Generic.KeyValuePair<float, string>;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas.Ablimatex
{
    public class TicketCuadreCaja : TicketPrinter
    {
        #region Fields
        private List<ReporteCuadredeCaja> _repTicket;
        private DateTime _init;
        private DateTime _end;
        private int _idAlmacen;
        private string _caja;
        private string _depositado;
        private string _nroOp;
        private int _idEstb;
        #endregion

        #region Construct

        public TicketCuadreCaja(DateTime init, DateTime end, int estab, int idAlmacen, string caja, string depositado, string nroOperacion)
        {
            _init = init;
            _end = end;
            _idAlmacen = idAlmacen;
            _caja = caja;
            _depositado = depositado;
            _nroOp = nroOperacion;
            Family = "Arial";
            _idEstb = estab;
        }
        #endregion

        #region Methods

        public override void Print()
        {
            _repTicket = new VentaBL().ReporteCuadreCaja(_idEstb, _init, _end, _idAlmacen, Globals.ClientSession.i_RoleId, Globals.ClientSession.i_SystemUserId);
            if (_repTicket == null || _repTicket.Count == 0) return;
            using (var pdoc = new PrintDocument())
            {
                pdoc.PrintPage += pdoc_PrintPage;

                try
                {
                    using (var dl = new PrintDialog {ShowNetwork = true})
                    {
                        dl.Document = pdoc;
                        if(dl.ShowDialog() == DialogResult.OK)
                            pdoc.Print();                        
                    }

                }
                catch (Exception er)
                {
                    UltraMessageBox.Show(er.Message, "Error en Impresion", Icono: MessageBoxIcon.Error);
                }
            }
        }

        private void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var width = 250 * 0.985f;
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            x = 0;
            x2 = width - x;
            currentY = e.PageBounds.Top;
            SetFont(new Font(Family, 11, FontStyle.Bold), graph);
            SetLine("CUADRE DE CAJA", StringAlignment.Center);
            currentY += 5;
            SetLine(string.Format("DEL {0:dd/MM/yyyy} AL {1:dd/MM/yyyy}", _init, _end), StringAlignment.Center);
            SetRelative(width,
                new Par(0, "DOCUMENTO"),
                new Par(0.57f, "PAGO"),
                new Par(0.70f, "TOTAL"));
            Separator();
            SetFont(new Font(Family, 10, FontStyle.Bold), graph);
            foreach (var tckt in _repTicket)
            {
                SetForPercent(width,
                    new Par(0.57f, tckt.Documento),
                    new Par(0.16f, tckt.CondicionPago),
                    new Par(0.27f, tckt.TotalS.ToString("0.00")));
            }
            Separator();
            SetRelative(width, new Par(0, "DINERO EN CAJA"), new Par(0.60f, _caja));
            SetRelative(width, new Par(0, "DINERO DEPOSITADO"), new Par(0.60f, _depositado));
            SetRelative(width, new Par(0, "NUMERO OPERACION"), new Par(0.60f, _nroOp));
            Separator();
            //var objOperationResult = new OperationResult();
            //var ds = new DatahierarchyBL().GetDataHierarchiesForCombo(ref objOperationResult, 46, null, UserConfig.Default.MostrarSoloFormasPagoAlmacenActual)
            //         .Select(r => r.Value1)
            //         .Distinct();

            var gps = _repTicket.GroupBy(r => r.CondicionPago).ToArray();
            foreach (var obj in gps)
            {
                //if (forma == "MIXTO") continue;
                //var obj = gps.FirstOrDefault(r => r.Key == forma);
                var monto = obj.Sum(r => r.TotalS);
                SetRelative(width,
                   new KeyValuePair<float, string>(0f, obj.Key),
                   new KeyValuePair<float, string>(0.6f, monto.ToString("0.00"))); 
            }
            Separator();
            SetFont(new Font(Family, 11, FontStyle.Bold), graph);
            SetRelative(width,
                   new KeyValuePair<float, string>(0f, "TOTAL GENERAL"),
                   new KeyValuePair<float, string>(0.6f, _repTicket.Sum(r => r.TotalS).ToString("0.00")));
            font.Dispose();
        }

        private void Separator()
        {
            currentY += 3;
            graph.DrawLine(Pens.Black, x, currentY, x2, currentY);
            currentY += 3;
        }
        #endregion

    }
}
