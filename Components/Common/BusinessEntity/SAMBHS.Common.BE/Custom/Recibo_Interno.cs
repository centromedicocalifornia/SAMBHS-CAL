using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using SAMBHS.Common.BE;
using System.IO;
using SAMBHS.Common.BE.Custom;
using System.Data.SqlClient;

namespace NetPdf
{
    public class Recibo_Interno2
    {
        private static void RunFile(string filePDF)
        {
            Process proceso = Process.Start(filePDF);
            proceso.WaitForExit();
            proceso.Close();
        }

        public static void CreateRecibo(string filePDF, string DatosPaciente, string calendarId, DateTime fechaNacimiento, string serie_correlativo, List<Recibo_San_Lorenzo> datosList)
        {
            Document document = new Document(PageSize.A7, 5f, 5f, 0f, 0f);
            document.SetPageSize(iTextSharp.text.PageSize.A7);

            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePDF, FileMode.Create));
            pdfPage page = new pdfPage();
            writer.PageEvent = page;
            document.Open();

            #region Declaration Tables
            var subTitleBackGroundColor = new BaseColor(System.Drawing.Color.Gray);
            string include = string.Empty;
            List<PdfPCell> cells = null;
            float[] columnWidths = null;
            string[] columnValues = null;
            string[] columnHeaders = null;
            PdfPTable header2 = new PdfPTable(6);
            header2.HorizontalAlignment = Element.ALIGN_CENTER;
            header2.WidthPercentage = 100;
            float[] widths1 = new float[] { 16.6f, 18.6f, 16.6f, 16.6f, 16.6f, 16.6f };
            header2.SetWidths(widths1);
            PdfPTable companyData = new PdfPTable(6);
            companyData.HorizontalAlignment = Element.ALIGN_CENTER;
            companyData.WidthPercentage = 100;
            float[] widthscolumnsCompanyData = new float[] { 16.6f, 16.6f, 16.6f, 16.6f, 16.6f, 16.6f };
            companyData.SetWidths(widthscolumnsCompanyData);
            PdfPTable filiationWorker = new PdfPTable(4);
            PdfPTable table = null;
            PdfPCell cell = null;
            document.Add(new Paragraph("\r\n"));
            #endregion

            #region Fonts
            Font fontTitle1 = FontFactory.GetFont("Calibri Light", 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontTitle2 = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
            Font fontTitleTable = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontTitleTableNegro = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontSubTitle = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
            Font fontSubTitleNegroNegrita = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));

            Font fontColumnValue = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
            Font fontColumnValueBold = FontFactory.GetFont("Calibri Light", 7, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontColumnValueApendice = FontFactory.GetFont("Calibri Light", 5, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontColumnValue1 = FontFactory.GetFont("Calibri Light", 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontColumnValueBold1 = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));

            Font fontColumnValueBoldWhite = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
            Font fontColumnValueBoldRed = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Red));
            Font fontColumnValueBold_1 = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
            Font fontColumnValue_1 = FontFactory.GetFont("Calibri Light", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
            #endregion

            #region Consultas de datos

            string[] serie = serie_correlativo.Split('|');
            string[] coorelativos = serie[0].Trim().Split('-');
            coorelativos[0] = coorelativos[0].Trim();
            coorelativos[1] = coorelativos[1].Trim();
            string DatosEmpresa = ObtenerDatosEmpresa();
            string moneda;
            if (serie[1] == "SOLES") { moneda = "S/."; }
            else { moneda = "$"; }
            string[] empresa = DatosEmpresa.Split('|');
            //string DatosPaciente = ObtenerDatosPaciente(serviceId);
            string[] Paciente = DatosPaciente.Split('|');
            //var servcomp = ObtenerServiceCOmponent(serviceId);
            var venta = Obtenerventas(coorelativos[0], coorelativos[1]);
            string edad = fechaNacimiento.Date == DateTime.Now.Date ? "- - -" : ObtenerEdad(fechaNacimiento).ToString();
            //DateTime? servicedate = datosList[0].t_FechaRegistro == null ? DateTime.Now : datosList[0].t_FechaRegistro;
            //if (servicedate != null)
            //{
            //    string FechaServ = servicedate.ToShortDateString();
            //}

            string usuario = ObtenerUsuario(coorelativos);
            string medico = "- - -";
            if (calendarId.Split('|').Count() >= 2)
            {
                medico = calendarId.Split('|')[1];
            }

            #endregion

            #region Cabecera

            cells = new List<PdfPCell>();
            var tamaño_celda = 15f;
            var logo = Image.GetInstance("C:/Banner/banner5.png");
            logo.ScalePercent(19);
            logo.SetAbsolutePosition(6, 267);
            logo.Alignment = Image.ALIGN_BASELINE;
            document.Add(logo);
            //var sl = Image.GetInstance("C:/Banner/banner6.png");
            //sl.ScalePercent(25);
            //sl.SetAbsolutePosition(75, 115);
            //sl.Alignment = Image.UNDERLYING;
            //document.Add(sl);
            int i = Paciente.Length - 1;
            var cellsTit = new List<PdfPCell>()
                { 
                    new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =  3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                    new PdfPCell(new Phrase(empresa[0], fontTitle1)) {Colspan =  7, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase(empresa[1], fontColumnValueBold)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("RUC "+empresa[2], fontColumnValueBold)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    
                    new PdfPCell(new Phrase("", fontColumnValueBold)) {Colspan = 10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("RECIBO N°: "+ serie[0].ToString(), fontColumnValue1)) {Colspan = 10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    
                    new PdfPCell(new Phrase("CLIENTE: "+Paciente[1], fontColumnValue)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("DNI: "+Paciente[0], fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase(fechaNacimiento.Date ==  DateTime.Now.Date ? "FECHA NAC.: - - -"  : "FECHA NAC.: " + fechaNacimiento.ToShortDateString(), fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("EDAD: "+edad, fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("HC: "+Paciente[0], fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("Medico: "+medico, fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("DIRECCION: "+Paciente[2], fontColumnValue)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                    
                    new PdfPCell(new Phrase("[CANT.] ", fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("DESCRIPCIÓN ", fontColumnValue)) {Colspan =  5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("P/U", fontColumnValue)) {Colspan =  1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase("TOTAL", fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                     
                };
            columnWidths = new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f };
            table = HandlingItextSharp.GenerateTableFromCells(cellsTit, columnWidths, null, fontTitleTable);
            document.Add(table);


            #endregion

            #region Ventas
            double acumuladorIGV = 0f;
            double acumuladorTotal = 0f;
            int count = 1;
            foreach (var dato in venta)
            {
                cellsTit = new List<PdfPCell>()
                { 
                    new PdfPCell(new Phrase("[ "+dato.cantidad+" ] ", fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase(dato.v_DescripcionProducto, fontColumnValue)) {Colspan =  5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase(dato.d_Precio.ToString("N2"), fontColumnValue)) {Colspan =  1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    new PdfPCell(new Phrase(dato.d_ValorVenta.ToString("N2"), fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                    
                };
                columnWidths = new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f };
                table = HandlingItextSharp.GenerateTableFromCells(cellsTit, columnWidths, null, fontTitleTable);
                document.Add(table);
                acumuladorIGV = acumuladorIGV + (dato.d_Igv);
                acumuladorTotal = acumuladorTotal + dato.d_ValorVenta;
                count++;
            }

            string cantidad = new Conversion().enletras((Math.Round(acumuladorTotal, 2) + Math.Round(acumuladorIGV, 2)).ToString());
            cellsTit = new List<PdfPCell>()
            { 
                new PdfPCell(new Phrase("", fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},
                new PdfPCell(new Phrase("GRAVADA", fontColumnValue)) {Colspan =  5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},
                new PdfPCell(new Phrase(moneda, fontColumnValue)) {Colspan =  1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},
                new PdfPCell(new Phrase((acumuladorTotal).ToString("N2"), fontColumnValue)) {Colspan = 2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},

                new PdfPCell(new Phrase("", fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase("IGV", fontColumnValue)) {Colspan =  5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase(moneda, fontColumnValue)) {Colspan =  1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase((acumuladorIGV).ToString("N2"), fontColumnValue)) {Colspan = 2 , HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},

                new PdfPCell(new Phrase("", fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase("TOTAL", fontColumnValue)) {Colspan =  5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase(moneda, fontColumnValue)) {Colspan =  1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase((acumuladorTotal + acumuladorIGV).ToString("N2"), fontColumnValue)) {Colspan =  2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                
                new PdfPCell(new Phrase("SON : " + cantidad +" "+serie[1], fontColumnValue)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},
                
                new PdfPCell(new Phrase("FECHA DE ATENCIÓN: "+datosList[0].t_FechaRegistro, fontColumnValueApendice)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase("FECHA DE IMPRESIÓN: "+DateTime.Now.ToShortDateString(), fontColumnValueApendice)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},
                new PdfPCell(new Phrase("USUARIO: "+usuario, fontColumnValueApendice)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},

                //new PdfPCell(new Phrase("AVISO: "+"JULIO", fontColumnValueApendice)) {Colspan =  10, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,  UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},

    
                
            };
            columnWidths = new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f };
            table = HandlingItextSharp.GenerateTableFromCells(cellsTit, columnWidths, null, fontTitleTable);
            document.Add(table);

            #endregion

            document.Close();
            writer.Close();
            writer.Dispose();
            RunFile(filePDF);
        }

        private static List<VentasDto> Obtenerventas(string v_SerieDocumento, string v_CorrelativoDocumento)
        {
            ConexionSAM2 ConexionSigesoft = new ConexionSAM2();
            ConexionSigesoft.opensam();
            var cadena =
                "select VTD.v_DescripcionProducto, VTD.d_ValorVenta, VTD.d_Igv, VTD.d_Precio, Convert(int,VTD.d_Cantidad) from venta VT " +
                "inner join ventadetalle VTD on VT.v_IdVenta=VTD.v_IdVenta " +
                "where v_SerieDocumento='" + v_SerieDocumento + "' and v_CorrelativoDocumento='" + v_CorrelativoDocumento + "'";
            var comando = new SqlCommand(cadena, connection: ConexionSigesoft.conectarsam);
            var lector = comando.ExecuteReader();
            List<VentasDto> List = new List<VentasDto>();
            while (lector.Read())
            {
                VentasDto obj = new VentasDto();
                obj.v_DescripcionProducto = lector.GetValue(0).ToString();
                obj.d_ValorVenta = float.Parse(lector.GetValue(1).ToString());
                obj.d_Igv = float.Parse(lector.GetValue(2).ToString());
                obj.d_Precio = float.Parse(lector.GetValue(3).ToString());
                obj.cantidad = Convert.ToInt32(lector.GetValue(4).ToString());
                List.Add(obj);
            }
            lector.Close();
            ConexionSigesoft.closesam();
            return List;
        }

        private static string ObtenerUsuario(string[] coorelativos)
        {
            ConexionSAM2 ConexionSAM = new ConexionSAM2();
            ConexionSAM.opensam();
            var cadena =
                "select PP.v_FirstLastName + ' ' + PP.v_SecondLastName + ', ' + PP.v_FirstName from venta VT " +
                "inner join systemuser SU on VT.i_InsertaIdUsuario=SU.i_SystemUserId " +
                "inner join [TIS_INTEGRADO].[dbo].[person] PP on SU.i_PersonId=PP.i_PersonId " +
                "where VT.v_CorrelativoDocumento='" + coorelativos[1] + "' and VT.v_SerieDocumento='" + coorelativos[0] + "'";
            var comando = new SqlCommand(cadena, connection: ConexionSAM.conectarsam);
            var lector = comando.ExecuteReader();
            string nombre = "";
            while (lector.Read())
            {
                nombre = lector.GetValue(0).ToString();
            }
            lector.Close();
            ConexionSAM.closesam();
            return nombre;
        }

        private static DateTime ObtenerFechaService(string serviceId)
        {
            ConexionSigesoft ConexionSigesoft = new ConexionSigesoft();
            ConexionSigesoft.opensigesoft();
            var cadena = "select d_ServiceDate from service where v_ServiceId='" + serviceId + "'";
            var comando = new SqlCommand(cadena, connection: ConexionSigesoft.conectarsigesoft);
            var lector = comando.ExecuteReader();
            DateTime fecha = default(DateTime);
            while (lector.Read())
            {
                fecha = Convert.ToDateTime(lector.GetValue(0).ToString());
            }
            lector.Close();
            ConexionSigesoft.closesigesoft();
            return fecha;
        }

        private static int ObtenerEdad(DateTime fechaNacimiento)
        {
            int edad = ((TimeSpan)(DateTime.Now - fechaNacimiento)).Days;
            edad = int.Parse((edad / 365).ToString());
            return edad;
        }

        private static List<ServiceComponent> ObtenerServiceCOmponent(string serviceId)
        {
            ConexionSigesoft ConexionSigesoft = new ConexionSigesoft();
            ConexionSigesoft.opensigesoft();
            var cadena =
                "select SC.v_ComponentId, CP.v_Name, PR.v_Name, SC.r_Price, SC.i_MedicoTratanteId, SC.i_IsFact from service SR " +
                "inner join protocol PR on SR.v_ProtocolId=PR.v_ProtocolId " +
                "inner join servicecomponent SC on SR.v_ServiceId=SC.v_ServiceId " +
                "inner join component CP on SC.v_ComponentId=CP.v_ComponentId " +
                "where SR.v_ServiceId='" + serviceId + "' and SC.r_Price>0";
            var comando = new SqlCommand(cadena, connection: ConexionSigesoft.conectarsigesoft);
            var lector = comando.ExecuteReader();
            List<ServiceComponent> List = new List<ServiceComponent>();
            while (lector.Read())
            {
                ServiceComponent obj = new ServiceComponent();
                obj.v_ComponentId = lector.GetValue(0).ToString();
                obj.v_Name = lector.GetValue(1).ToString();
                obj.v_Protocol = lector.GetValue(2).ToString();
                obj.r_Price = float.Parse(lector.GetValue(3).ToString());
                obj.i_MedicoTratanteId = int.Parse(lector.GetValue(4).ToString());
                if (string.IsNullOrEmpty(lector.GetValue(5).ToString()))
                {
                    obj.i_IsFact = 0;
                }
                else obj.i_IsFact = int.Parse(lector.GetValue(5).ToString());
                List.Add(obj);
            }
            lector.Close();
            ConexionSigesoft.closesigesoft();
            return List;
        }

        private static string ObtenerDatosPaciente(string serviceId)
        {
            ConexionSigesoft ConexionSigesoft = new ConexionSigesoft();
            ConexionSigesoft.opensigesoft();
            var cadena =
                "select PP.v_DocNumber, PP.v_FirstLastName + ' ' + PP.v_SecondLastName + ', ' + PP.v_FirstName, PP.v_AdressLocation " +
                "from service SR inner join person PP on SR.v_PersonId=PP.v_PersonId where v_ServiceId='" + serviceId + "'";
            var comando = new SqlCommand(cadena, connection: ConexionSigesoft.conectarsigesoft);
            var lector = comando.ExecuteReader();
            string Datos = "";
            while (lector.Read())
            {
                Datos = lector.GetValue(0).ToString() + "|" + lector.GetValue(1).ToString() + "|" + lector.GetValue(2).ToString();
                if (lector.GetValue(2).ToString() == "")
                {
                    Datos = Datos + "- - - ";
                }
            }
            lector.Close();
            ConexionSigesoft.closesigesoft();
            return Datos;
        }

        private static string ObtenerDatosEmpresa()
        {
            ConexionSigesoft ConexionSigesoft = new ConexionSigesoft();
            ConexionSigesoft.opensigesoft();
            var cadena = "select v_Name, v_Address, v_IdentificationNumber from organization where v_OrganizationId='N009-OO000000052'";
            var comando = new SqlCommand(cadena, connection: ConexionSigesoft.conectarsigesoft);
            var lector = comando.ExecuteReader();
            string Datos = "";
            while (lector.Read())
            {
                Datos = lector.GetValue(0).ToString() + "|" + lector.GetValue(1).ToString() + "|" + lector.GetValue(2).ToString();
            }
            lector.Close();
            ConexionSigesoft.closesigesoft();
            return Datos;
        }
    }


    public class Conversion
    {
        public string enletras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;

            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));
            if (decimales > 0)
            {
                dec = " CON " + decimales.ToString() + " / 100";
            }
            else
            {
                dec = " CON 00 / 100";
            }

            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }

        private string toText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {
                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            return Num2Text;

        }

    }
}
