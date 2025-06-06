﻿using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Windows.WinClient.UI.Reportes.Compras;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.WinClient.UI.Reportes.Ventas;
using SAMBHS.Windows.WinClient.UI.Reportes.Ventas.Sigesoft;

namespace SAMBHS.Windows.WinClient.UI.Reportes
{
    public class ReportesUtils
    {
        /// <summary>
        /// Devuelve el reporte deacuerdo a la empresa.
        /// </summary>
        /// <param name="RUC"></param>
        /// <param name="_TiposReportes"></param>
        /// <returns></returns>
        public static ReportDocument DevolverReporte(string RUC, TiposReportes _TiposReportes, string Serie = "")
        {
            try
            {
               
                ReportDocument Reporte = null;


                switch (_TiposReportes)
                {

                    case TiposReportes.Factura:
                        Reporte = new crDocumentoFacturaSM();
                        break;
                    case TiposReportes.Boleta:
                        Reporte = new crDocumentoBoletaSM();
                        break;
                    case TiposReportes.NotaCredito:
                        Reporte = new crDocumentoNotaCreditoSM();
                        break;
                    case TiposReportes.NotaIngreso:
                        Reporte = new crDocumentoNotaIngresoSM();
                        break;
                    case TiposReportes.Pedido:
                        Reporte = new crDocumentoPedidoManguifajas();
                        break;
                    case TiposReportes.OrdenCompra:
                        Reporte = new crDocumentoOrdenCompra();
                        break;
                    case TiposReportes.GuiaRemisionVenta:
                        Reporte = new crDocumentoGuiaRemisionMangueras();
                        //Reporte = new crGuiaRemisionElectronica();

                        break;
                }

                return Reporte;

            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }
    }

    public enum TiposReportes
    {
        Factura = 1,
        Boleta = 2,
        Pedido = 3,
        NotaCredito = 4,
        NotaDebidto = 5,
        Ticket = 12,
        TicketRuc = 10,
        OrdenCompra = 11,
        GuiaRemisionVenta = 9,
        LetraCobrar = 411,
        OtroFormatoGuia = 15,
        Proforma = 439,
        Irpe = 438,
        PreFactura = 0,
        DocumentoRetencion = 500,
        NotaIngreso = 320
    }
    public enum Idioma
    {
        Ingles = 1,
        Espaniol = 2,
    }
}
