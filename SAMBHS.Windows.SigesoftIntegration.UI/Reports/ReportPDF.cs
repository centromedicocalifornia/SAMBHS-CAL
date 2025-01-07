using iTextSharp.text;
using iTextSharp.text.pdf;
using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NetPdf
{
    public class ReportPDF
    {
        #region Test

        public static void CreateTest(string filePDF)
        {
            // step 1: creation of a document-object
            Document document = new Document();
            //Document document = new Document(new Rectangle(500f, 300f), 10, 10, 10, 10);
            //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            document.SetPageSize(iTextSharp.text.PageSize.A4);
            //Document document = new Document(PageSize.A4, 0, 0, 20, 20);

            try
            {
                // step 2: we create a writer that listens to the document
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePDF, FileMode.Create));

                //create an instance of your PDFpage class. This is the class we generated above.
                pdfPage page = new pdfPage();
               
                // step 3: we open the document
                document.Open();
                // step 4: we Add content to the document
                // we define some fonts

                #region Fonts

                Font fontTitle1 = FontFactory.GetFont("Calibri", 14, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                Font fontTitle2 = FontFactory.GetFont("Calibri", 12, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                Font fontTitleTable = FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                Font fontTitleTableNegro = FontFactory.GetFont("Calibri", 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                Font fontSubTitle = FontFactory.GetFont("Calibri", 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                Font fontSubTitleNegroNegrita = FontFactory.GetFont("Calibri", 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));

                Font fontColumnValue = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                Font fontColumnValueBold = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                Font fontColumnValueApendice = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                Font fontColumnValue1 = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));


                #endregion

                #region Title

                Paragraph cTitle = new Paragraph("Examen Clínico", fontTitle1);
                Paragraph cTitle2 = new Paragraph("Historia Clínica: ", fontTitle2);
                cTitle.Alignment = Element.ALIGN_CENTER;
                cTitle2.Alignment = Element.ALIGN_CENTER;

                document.Add(cTitle);
                document.Add(cTitle2);

                #endregion

                // step 5: we close the document
                document.Close();
                writer.Close();
                writer.Dispose();
                RunFile(filePDF);

            }
            catch (DocumentException)
            {
                throw;
            }
            catch (IOException)
            {
                throw;
            }

        }

         #endregion



        public static void CreateCuadreCaja(string filePDF, string inicio, string fin, int systemUserId, int tipo, int cuadreGeneral)
        {
           
            
                #region Declaration Tables
                // step 1: creation of a document-object
                Document document = new Document();
                //Document document = new Document(new Rectangle(500f, 300f), 10, 10, 10, 10);
                //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                document.SetPageSize(iTextSharp.text.PageSize.A4);
                //Document document = new Document(PageSize.A4, 0, 0, 20, 20);

               
                    // step 2: we create a writer that listens to the document
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePDF, FileMode.Create));

                    //create an instance of your PDFpage class. This is the class we generated above.
                    pdfPage page = new pdfPage();
                    writer.PageEvent = page;
                    // step 3: we open the document
                    document.Open();
                    // step 4: we Add content to the document
                    // we define some fonts
                var subTitleBackGroundColor = new BaseColor(System.Drawing.Color.Gray);
                string include = string.Empty;
                List<PdfPCell> cells = null;
                float[] columnWidths = null;
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
                document.Add(new Paragraph("\r\n"));
                var tamaño_celda = 14f;
                var tamaño_celda2 = 70f;
            

                #endregion

                 #region Fonts

                 Font fontTitle1 = FontFactory.GetFont("Calibri", 9, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                 Font fontTitle2 = FontFactory.GetFont("Calibri", 9, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                 Font fontTitleTable = FontFactory.GetFont("Calibri", 7, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                 Font fontTitleTable1 = FontFactory.GetFont("Calibri", 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));

                 Font fontTitleTableNegro = FontFactory.GetFont("Calibri", 5, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                 Font fontSubTitle = FontFactory.GetFont("Calibri", 5, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                 Font fontSubTitleNegroNegrita = FontFactory.GetFont("Calibri", 5, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));

                 Font fontColumnValue = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                 Font fontColumnValueBold = FontFactory.GetFont("Calibri", 5, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                 Font fontColumnValueApendice = FontFactory.GetFont("Calibri", 5, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                 Font fontColumnValue1 = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                 Font fontColumnValueBold_1 = FontFactory.GetFont("Calibri", 6, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                 #endregion

                 #region Conexion SAM
                 ConexionSAM conectasam = new ConexionSAM();
                 conectasam.opensam();
                 #endregion

                 #region Fecha
                 string[] fechaInicio = inicio.Split(' ');
                 string[] fecha1 = fechaInicio[0].Split('/');
                 string anioinicio_i = fecha1[2];
                 string mesinicio_i = fecha1[1];
                 string diainicio_i = fecha1[0];

                 string[] fechaFin = fin.Split(' ');
                 string[] fecha2 = fechaFin[0].Split('/');
                 string anioinicio_f = fecha2[2];
                 string mesinicio_f = fecha2[1];
                 string diainicio_f = fecha2[0];

                 DateTime localDate = DateTime.Now;
                 #endregion

                 #region Query Tablas
                 var cadena = "SELECT V.v_SerieDocumento as SERIE, V.v_CorrelativoDocumento as CORRELATIVO, VD.v_DescripcionProducto as CONCEPTO, " +
                              "CL.v_RazonSocial as CLIENTE,CL.v_PrimerNombre as Nombre1,	CL.v_SegundoNombre as Nombre2,	CL.v_ApePaterno as Paterno, " +
                              "CL.v_ApeMaterno as Materno,VD.d_Cantidad as CANTIDAD,VD.d_Precio as PRECIO_UNITARIO, VD.d_Valor as COSTO, " +
                              "VD.d_Igv as IGV, VD.d_PrecioVenta as TOTAL, DH.v_Value1 as CONDICION, DH2.v_Value1 as TIPO , SU.v_UserName as USUARIO, V.i_ClienteEsAgente as TIPO_VENTA " +
                              "From venta V " +
                              "Inner Join ventadetalle VD " +
                              "ON V.v_IdVenta=VD.v_IdVenta " +
                              "Inner Join cliente CL " +
                              "ON V.v_IdCliente = CL.v_IdCliente " +
                              "Left Join cobranzadetalle CD " +
                              "ON CD.v_IdVenta = V.v_IdVenta " +
                              "Left Join datahierarchy DH " +
                              "ON DH.i_GroupId=41 and DH.i_ItemId=V.i_IdCondicionPago " +
                              "Left Join datahierarchy DH2 " +
                              "ON DH2.i_GroupId=46 and DH2.i_ItemId=CD.i_IdFormaPago " +
                              "Inner Join systemuser SU ON SU.i_SystemUserId=V.i_InsertaIdUsuario " +
                              "WHERE ((Year(V.t_InsertaFecha)>=" + anioinicio_i + " and Month(V.t_InsertaFecha)>=" + mesinicio_i +
                              " and Day(V.t_InsertaFecha)>=" + diainicio_i + ") and (Year(V.t_InsertaFecha)<=" + anioinicio_f + " and Month(V.t_InsertaFecha)<=" + mesinicio_f +
                              " and Day(V.t_InsertaFecha)<=" + diainicio_f + ") ) and V.i_Eliminado= 0 and V.i_ClienteEsAgente is not null and V.i_InsertaIdUsuario=" + systemUserId + " order by V.i_ClienteEsAgente, V.v_SerieDocumento";
            
                var cadenaSA = "SELECT V.v_SerieDocumento as SERIE, V.v_CorrelativoDocumento as CORRELATIVO, VD.v_DescripcionProducto as CONCEPTO, " +
                                   "CL.v_RazonSocial as CLIENTE,CL.v_PrimerNombre as Nombre1,	CL.v_SegundoNombre as Nombre2,	CL.v_ApePaterno as Paterno, " +
                                   "CL.v_ApeMaterno as Materno,VD.d_Cantidad as CANTIDAD,VD.d_Precio as PRECIO_UNITARIO, VD.d_Valor as COSTO, " +
                                   "VD.d_Igv as IGV, VD.d_PrecioVenta as TOTAL, DH.v_Value1 as CONDICION, DH2.v_Value1 as TIPO , SU.v_UserName as USUARIO, V.i_ClienteEsAgente as TIPO_VENTA " +
                                   "From venta V " +
                                   "Inner Join ventadetalle VD " +
                                   "ON V.v_IdVenta=VD.v_IdVenta " +
                                   "Inner Join cliente CL " +
                                   "ON V.v_IdCliente = CL.v_IdCliente " +
                                   "Left Join cobranzadetalle CD " +
                                   "ON CD.v_IdVenta = V.v_IdVenta " +
                                   "Left Join datahierarchy DH " +
                                   "ON DH.i_GroupId=41 and DH.i_ItemId=V.i_IdCondicionPago " +
                                   "Left Join datahierarchy DH2 " +
                                   "ON DH2.i_GroupId=46 and DH2.i_ItemId=CD.i_IdFormaPago " +
                                   "Inner Join systemuser SU ON SU.i_SystemUserId=V.i_InsertaIdUsuario " +
                                   "WHERE ((Year(V.t_InsertaFecha)>=" + anioinicio_i + " and Month(V.t_InsertaFecha)>=" + mesinicio_i +
                                   " and Day(V.t_InsertaFecha)>=" + diainicio_i + ") and (Year(V.t_InsertaFecha)<=" + anioinicio_f + " and Month(V.t_InsertaFecha)<=" + mesinicio_f +
                                   " and Day(V.t_InsertaFecha)<=" + diainicio_f + ") ) and V.i_Eliminado= 0 and V.i_ClienteEsAgente is not null and V.i_ClienteEsAgente != 3 and V.i_InsertaIdUsuario != 2036 order by V.i_ClienteEsAgente, V.v_SerieDocumento";

                var cadenaFarmacia = "SELECT V.v_SerieDocumento as SERIE, V.v_CorrelativoDocumento as CORRELATIVO, VD.v_DescripcionProducto as CONCEPTO, " +
                               "CL.v_RazonSocial as CLIENTE,CL.v_PrimerNombre as Nombre1,	CL.v_SegundoNombre as Nombre2,	CL.v_ApePaterno as Paterno, " +
                               "CL.v_ApeMaterno as Materno,VD.d_Cantidad as CANTIDAD,VD.d_Precio as PRECIO_UNITARIO, VD.d_Valor as COSTO, " +
                               "VD.d_Igv as IGV, VD.d_PrecioVenta as TOTAL, DH.v_Value1 as CONDICION, DH2.v_Value1 as TIPO , SU.v_UserName as USUARIO, V.i_ClienteEsAgente as TIPO_VENTA " +
                               "From venta V " +
                               "Inner Join ventadetalle VD " +
                               "ON V.v_IdVenta=VD.v_IdVenta " +
                               "Inner Join cliente CL " +
                               "ON V.v_IdCliente = CL.v_IdCliente " +
                               "Left Join cobranzadetalle CD " +
                               "ON CD.v_IdVenta = V.v_IdVenta " +
                               "Left Join datahierarchy DH " +
                               "ON DH.i_GroupId=41 and DH.i_ItemId=V.i_IdCondicionPago " +
                               "Left Join datahierarchy DH2 " +
                               "ON DH2.i_GroupId=46 and DH2.i_ItemId=CD.i_IdFormaPago " +
                               "Inner Join systemuser SU ON SU.i_SystemUserId=V.i_InsertaIdUsuario " +
                               "WHERE ((Year(V.t_InsertaFecha)>=" + anioinicio_i + " and Month(V.t_InsertaFecha)>=" + mesinicio_i +
                               " and Day(V.t_InsertaFecha)>=" + diainicio_i + ") and (Year(V.t_InsertaFecha)<=" + anioinicio_f + " and Month(V.t_InsertaFecha)<=" + mesinicio_f +
                               " and Day(V.t_InsertaFecha)<=" + diainicio_f + ") ) and V.i_Eliminado= 0 and V.i_ClienteEsAgente is not null and (V.i_ClienteEsAgente = 3 or V.i_ClienteEsAgente = 4) order by V.i_ClienteEsAgente, V.v_SerieDocumento";

                 #endregion

                 #region Title
                 var rutaImg = GetApplicationConfigValue("Logo");
                 var imageLogo = new PdfPCell(HandlingItextSharp.GetImageLogo(rutaImg.ToString(), null, null, 120, 50)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER };
                 SqlCommand comandou = null;
                 if (systemUserId == 1 || systemUserId == 2037)//|| systemUserId == 3045  || systemUserId == 4049)
                     comandou = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                 else
                     comandou = new SqlCommand(cadena, connection: conectasam.conectarsam);

                 SqlDataReader lectoru = comandou.ExecuteReader();
                 var usuario = "";
                 while (lectoru.Read())
                 {
                     if (systemUserId == 1 )
                         usuario = "ADMINISTRADOR DEL SISTEMA";
                     else if (systemUserId == 10055)
                         usuario = "DAFNIS ANYELA CORTEZ BARANDIARAN";
                     else if (systemUserId == 10056)
                         usuario = "EDELIZ CONSUELO MORALES SAUCEDO";
                     else if (systemUserId == 9053)
                         usuario = "MAICOL ARTEAGA SANCHEZ";
                   
                 }
                 cells = new List<PdfPCell>()
                 {
                     new PdfPCell(imageLogo){Rowspan=1, HorizontalAlignment = PdfPCell.ALIGN_CENTER,  MinimumHeight = tamaño_celda2,  VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE},
                     new PdfPCell(new Phrase("CUADRE DE CAJA ", fontTitleTable)) {Rowspan = 1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda2},    
                     new PdfPCell(new Phrase("usuario: "+usuario+"\r\n "+"\r\n  Cuadre de la Fecha: "+diainicio_i+" - "+mesinicio_i+" - "+anioinicio_i+"\r\n "+"\r\n AL: "+diainicio_f+" - "+mesinicio_f+" - "+anioinicio_f + "\r\n "+"\r\n Fecha y hora de Impresión: "+localDate, fontColumnValueBold)) {Rowspan = 1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda2},    
                 };
                 columnWidths = new float[] { 30f, 40f, 30f };
                 filiationWorker = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTableNegro, null);
                 document.Add(filiationWorker);
                 lectoru.Close();
                 #endregion

                 #region Reporte
                  if (conectasam.conectarsam != null)
                     {
                        #region EGRESOS
                         SqlCommand comando = null;
                         if (cuadreGeneral == 1)
                         {
                             systemUserId = 1;
                         }
                         //if ((systemUserId == 1 || systemUserId == 3045 || systemUserId == 2037 || systemUserId == 4049) && tipo == 0)
                         if ((systemUserId == 1 || systemUserId == 2037) && tipo == 0)
                             comando = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                         //else if ((systemUserId == 1 || systemUserId == 3045 || systemUserId == 2037 || systemUserId == 4049) && tipo == 1)
                         else if ((systemUserId == 1 || systemUserId == 2037) && tipo == 1)
                             comando = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                         else if (systemUserId == 2036 )
                             comando = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                         else
                             comando = new SqlCommand(cadena, connection: conectasam.conectarsam);

                         SqlDataReader lector = comando.ExecuteReader();
                         
                         cells = new List<PdfPCell>()
                         {         
                             new PdfPCell(new Phrase("EGRESOS ", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =12, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase("Itm", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Documento", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Descripción", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cliente", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cantidad", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Precio Unitario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Total", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Venta", fontColumnValueBold)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             //new PdfPCell(new Phrase("IGV", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Condición", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Tipo", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Usuario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                         };
                         columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                         table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                         document.Add(table);
                         int count = 1; decimal total = 0;
                         while (lector.Read())
                         {

                             if (lector.GetValue(0).ToString() == "ECO" || lector.GetValue(0).ToString() == "ECA" || lector.GetValue(0).ToString() == "ECF" || lector.GetValue(0).ToString() == "ECT" || lector.GetValue(0).ToString() == "ECG" || lector.GetValue(0).ToString() == "ECR")
                             {
                                 decimal eco_a_1_1 = decimal.Round(decimal.Parse(lector.GetValue(9).ToString()), 2);
                                 decimal eco_a_2_1 = decimal.Round(decimal.Parse(lector.GetValue(12).ToString()), 2);
                                 cells = new List<PdfPCell>()
                                 { 
                                     new PdfPCell(new Phrase(count.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(lector.GetValue(0).ToString()+" - "+lector.GetValue(1).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(lector.GetValue(2).ToString().Split('-')[0], fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(lector.GetValue(3).ToString()+lector.GetValue(4).ToString()+" "+lector.GetValue(5).ToString()+" "+lector.GetValue(6).ToString()+" "+lector.GetValue(7).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(double.Parse(lector.GetValue(8).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(eco_a_1_1.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(eco_a_2_1.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(lector.GetValue(16).ToString() == "1"?"OCUP":lector.GetValue(16).ToString()=="2"?"ASIS":lector.GetValue(16).ToString()=="3"?"FAR":lector.GetValue(16).ToString()=="4"?"S-FAR":lector.GetValue(16).ToString()=="5"?"S-PAC":lector.GetValue(16).ToString()=="6"?"S-FAC":lector.GetValue(16).ToString() == "7"?"MTC":lector.GetValue(16).ToString()  == "8"?"GIN":lector.GetValue(16).ToString() == "9"?"RTF":lector.GetValue(16).ToString(), fontColumnValue)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     //new PdfPCell(new Phrase("- - -", fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                     new PdfPCell(new Phrase(lector.GetValue(13).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                                     new PdfPCell(new Phrase(lector.GetValue(14).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                                     new PdfPCell(new Phrase(lector.GetValue(15).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 };
                                 count++; total =decimal.Round(decimal.Parse(lector.GetValue(12).ToString()) * -1 + total , 2);
                                 columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                                 table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                                 document.Add(table);
                             }


                         }
                         cells = new List<PdfPCell>()
                         {         
                             new PdfPCell(new Phrase(" TOTAL EGRESOS:", fontColumnValueBold)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase(total.ToString(), fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase(" ", fontColumnValueBold)){ Colspan =5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                         };
                         columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                         table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                         document.Add(table);
                         

                         #endregion

                        #region INGRESOS CONTADO EFECTIVO
                         cells = new List<PdfPCell>()
                         {         
                             new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =12, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase("Itm", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Documento", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Descripción", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cliente", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cantidad", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Precio Unitario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Total", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Venta", fontColumnValueBold)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             //new PdfPCell(new Phrase("IGV", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Condición", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Tipo", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Usuario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                         };
                         columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                         table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                         document.Add(table);
                         lector.Close();
                     }
            
                     SqlCommand comando1 = null;
                     if ((systemUserId == 1 || systemUserId == 2037) && tipo == 0)
                         comando1 = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                     else if ((systemUserId == 1 || systemUserId == 2037) && tipo == 1)
                         comando1 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else if (systemUserId == 2036)
                         comando1 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else
                         comando1 = new SqlCommand(cadena, connection: conectasam.conectarsam);
                     SqlDataReader lector1 = comando1.ExecuteReader();
                     int count1 = 1; decimal total1 = 0;
                     while (lector1.Read())
                     {
                         if (lector1.GetValue(0).ToString() != "ECO" && lector1.GetValue(0).ToString() != "ECA" && lector1.GetValue(0).ToString() != "ECT" && lector1.GetValue(0).ToString() != "ECG" && lector1.GetValue(0).ToString() != "ECR" && lector1.GetValue(0).ToString() != "ECF" && lector1.GetValue(0).ToString() != "TFM" && lector1.GetValue(0).ToString() != "THM" && lector1.GetValue(13).ToString() == "CONTADO" && lector1.GetValue(14).ToString() == "EFECTIVO SOLES")
                             //|| lector1.GetValue(0).ToString() == "ICO" || lector1.GetValue(0).ToString() == "ICA" || lector1.GetValue(0).ToString() == "ICF"
                         {
                             decimal eco_a_1_2 = decimal.Round(decimal.Parse(lector1.GetValue(9).ToString()),2);
                             decimal eco_a_2_2 = decimal.Round(decimal.Parse(lector1.GetValue(12).ToString()), 2);

                             cells = new List<PdfPCell>()
                            { 
                             new PdfPCell(new Phrase(count1.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector1.GetValue(0).ToString()+" - "+lector1.GetValue(1).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector1.GetValue(2).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector1.GetValue(3).ToString()+lector1.GetValue(4).ToString()+" "+lector1.GetValue(5).ToString()+" "+lector1.GetValue(6).ToString()+" "+lector1.GetValue(7).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(double.Parse(lector1.GetValue(8).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(eco_a_1_2.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(eco_a_2_2.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector1.GetValue(16).ToString() == "1"?"OCUP":lector1.GetValue(16).ToString()=="2"?"ASIS":lector1.GetValue(16).ToString()=="3"?"FAR":lector1.GetValue(16).ToString()=="4"?"S-FAR":lector1.GetValue(16).ToString()=="5"?"S-PAC":lector1.GetValue(16).ToString()=="6"?"S-FAC":lector1.GetValue(16).ToString()=="7"?"MTC":lector1.GetValue(16).ToString().ToString()=="8"?"GIN":lector1.GetValue(16).ToString().ToString()=="9"?"RTF":lector1.GetValue(16).ToString().ToString(), fontColumnValue)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             //new PdfPCell(new Phrase(double.Parse(lector1.GetValue(11).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector1.GetValue(13).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                             new PdfPCell(new Phrase(lector1.GetValue(14).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                             new PdfPCell(new Phrase(lector1.GetValue(15).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                         };
                             count1++; total1 = decimal.Round(decimal.Parse(lector1.GetValue(12).ToString()) + total1, 2);
                             columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                             table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                             document.Add(table);
                         }

                     }
                     cells = new List<PdfPCell>()
                        {         
                        new PdfPCell(new Phrase(" TOTAL CONTADO EFECTIVO:", fontColumnValueBold)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        new PdfPCell(new Phrase(total1.ToString(), fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        new PdfPCell(new Phrase(" ", fontColumnValueBold)){ Colspan =5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                        };
                     columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                     table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                     document.Add(table);


                        #endregion

                        #region TOTAL A ENTREGAR
                     lector1.Close();
                     SqlCommand comando4 = null;
                     if ((systemUserId == 1 || systemUserId == 2037) && tipo == 0)
                         comando4 = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                     else if ((systemUserId == 1 || systemUserId == 2037 ) && tipo == 1)
                         comando4 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else if (systemUserId == 2036 )
                         comando4 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else
                         comando4 = new SqlCommand(cadena, connection: conectasam.conectarsam);
                     SqlDataReader lector4 = comando4.ExecuteReader();
                     decimal egreso = 0;
                     decimal ingreso = 0;
                     decimal entregar;
                     while (lector4.Read())
                     {
                         if (lector4.GetValue(0).ToString() == "ECO" || lector4.GetValue(0).ToString() == "ECA" || lector4.GetValue(0).ToString() == "ECF" || lector4.GetValue(0).ToString() == "ECT" || lector4.GetValue(0).ToString() == "ECG" || lector4.GetValue(0).ToString() == "ECR")
                         {
                             egreso = decimal.Round(decimal.Parse(lector4.GetValue(12).ToString()) + egreso , 2);
                         }
                         else if (lector4.GetValue(0).ToString() != "ECO" && lector4.GetValue(0).ToString() != "ECA" && lector4.GetValue(0).ToString() != "ECT" && lector4.GetValue(0).ToString() != "ECF" && lector4.GetValue(0).ToString() != "ECG" && lector4.GetValue(0).ToString() != "ECR" && lector4.GetValue(0).ToString() != "TFM" && lector4.GetValue(0).ToString() != "THM" && lector4.GetValue(13).ToString() == "CONTADO" && lector4.GetValue(14).ToString() == "EFECTIVO SOLES")
                         {
                             ingreso = decimal.Round(decimal.Parse(lector4.GetValue(12).ToString()) + ingreso ,2);
                         }
                     }

                     entregar = decimal.Round(ingreso - egreso,2);
                     cells = new List<PdfPCell>()
                     {         
                         new PdfPCell(new Phrase("VENTA EN EFECTIVO ", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =12, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                         new PdfPCell(new Phrase(" TOTAL A ENTREGAR: "+ingreso+" - "+egreso+" = ", fontTitleTable)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                         new PdfPCell(new Phrase(entregar.ToString(), fontTitleTable)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                         new PdfPCell(new Phrase(" ", fontColumnValueBold)){ Colspan =8, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                         //new PdfPCell(new Phrase("  ", fontTitleTable)){ Colspan =12, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                     };
                     columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                     table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                     document.Add(table);
                    #endregion

                        #region INGRESOS CREDITO
                     cells = new List<PdfPCell>()
                        {         
                             new PdfPCell(new Phrase("INGRESOS CRÉDITO", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =12, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Itm", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Documento", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Descripción", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cliente", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cantidad", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Precio Unitario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Total", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Venta", fontColumnValueBold)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             //new PdfPCell(new Phrase("IGV", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Condición", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Tipo", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Usuario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                        };
                     columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                     table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                     document.Add(table);
                     lector4.Close();

                     SqlCommand comando2 = null;
                     if ((systemUserId == 1 || systemUserId == 2037) && tipo == 0)
                         comando2 = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                     else if ((systemUserId == 1 || systemUserId == 2037)&& tipo == 1)
                         comando2 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else if (systemUserId == 2036 )
                         comando2 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else
                         comando2 = new SqlCommand(cadena, connection: conectasam.conectarsam);
                     SqlDataReader lector2 = comando1.ExecuteReader();
                     int count2 = 1; decimal total2 = 0;
                     while (lector2.Read())
                     {
                         if (lector2.GetValue(0).ToString() != "ECO" && lector2.GetValue(0).ToString() != "ECT" && lector2.GetValue(0).ToString() != "ECG" && lector2.GetValue(0).ToString() != "ECR" && lector2.GetValue(0).ToString() != "ECA" && lector2.GetValue(0).ToString() != "ECF" && lector2.GetValue(0).ToString() != "TFM" && lector2.GetValue(0).ToString() != "THM" && lector2.GetValue(13).ToString() == "CREDITO")
                         {
                             decimal eco_a_1_3 = decimal.Round(decimal.Parse(lector2.GetValue(9).ToString()), 2);
                             decimal eco_a_2_3 = decimal.Round(decimal.Parse(lector2.GetValue(12).ToString()), 2);

                             cells = new List<PdfPCell>()
                            { 
                            new PdfPCell(new Phrase(count1.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector2.GetValue(0).ToString()+" - "+lector2.GetValue(1).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector2.GetValue(2).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector2.GetValue(3).ToString()+lector2.GetValue(4).ToString()+" "+lector2.GetValue(5).ToString()+" "+lector2.GetValue(6).ToString()+" "+lector2.GetValue(7).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(double.Parse(lector2.GetValue(8).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(eco_a_1_3.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(eco_a_2_3.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector2.GetValue(16).ToString() == "1"?"OCUP":lector2.GetValue(16).ToString()=="2"?"ASIS":lector2.GetValue(16).ToString()=="3"?"FAR":lector2.GetValue(16).ToString()=="4"?"S-FAR":lector2.GetValue(16).ToString()=="5"?"S-PAC":lector2.GetValue(16).ToString()=="6"?"S-FAC":lector2.GetValue(16).ToString()=="7"?"MTC":lector2.GetValue(16).ToString()=="8"?"GIN":lector2.GetValue(16).ToString()=="9"?"RTF":lector2.GetValue(16).ToString(), fontColumnValue)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             //new PdfPCell(new Phrase(double.Parse(lector2.GetValue(11).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector2.GetValue(13).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                             new PdfPCell(new Phrase(lector2.GetValue(14).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                             new PdfPCell(new Phrase(lector2.GetValue(15).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                         };
                             count2++; total2 = decimal.Round(decimal.Parse(lector2.GetValue(12).ToString()) + total2 , 2);
                             columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                             table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                             document.Add(table);
                         }

                     }
                     cells = new List<PdfPCell>()
                        {         
                        new PdfPCell(new Phrase(" TOTAL CRÉDITO:", fontColumnValueBold)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        new PdfPCell(new Phrase(total2.ToString(), fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        new PdfPCell(new Phrase(" ", fontColumnValueBold)){ Colspan =5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                        };
                     columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                     table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                     document.Add(table);
                    #endregion

                        #region INGRESOS NO EFECTIVO
                     cells = new List<PdfPCell>()
                        {         
                             new PdfPCell(new Phrase("INGRESOS NO EFECTIVO", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =12, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Itm", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Documento", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Descripción", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cliente", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Cantidad", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Precio Unitario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Total", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Venta", fontColumnValueBold)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             //new PdfPCell(new Phrase("IGV", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Condición", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Tipo", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("Usuario", fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                        };
                     columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                     table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                     document.Add(table);
                     lector2.Close();
                     SqlCommand comando3 = null;
                     if ((systemUserId == 1 || systemUserId == 2037  )&& tipo == 0)
                         comando3 = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                     else if ((systemUserId == 1  || systemUserId == 2037) && tipo == 1)
                         comando3 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else if (systemUserId == 2036 )
                         comando3 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                     else
                         comando3 = new SqlCommand(cadena, connection: conectasam.conectarsam);
                     SqlDataReader lector3 = comando3.ExecuteReader();
                     int count3 = 1; decimal total3 = 0;
                     while (lector3.Read())
                     {
                         if ((lector3.GetValue(0).ToString() != "ECO" && lector3.GetValue(0).ToString() != "ECT" && lector3.GetValue(0).ToString() != "ECG" && lector3.GetValue(0).ToString() != "ECR" && lector3.GetValue(0).ToString() != "ECA" && lector3.GetValue(0).ToString() != "ECF" && lector3.GetValue(0).ToString() != "TFM" && lector3.GetValue(0).ToString() != "THM" && lector3.GetValue(13).ToString() == "CONTADO" && lector3.GetValue(14).ToString() != "EFECTIVO SOLES") || (lector3.GetValue(13).ToString() == "CHEQUE" || lector3.GetValue(13).ToString() == "DEPOSITO"))
                         {
                             decimal eco_a_1_4 = decimal.Round(decimal.Parse(lector3.GetValue(9).ToString()), 2);
                             decimal eco_a_2_4 = decimal.Round(decimal.Parse(lector3.GetValue(12).ToString()), 2);

                             cells = new List<PdfPCell>()
                            { 
                            new PdfPCell(new Phrase(count3.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector3.GetValue(0).ToString()+" - "+lector3.GetValue(1).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector3.GetValue(2).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector3.GetValue(3).ToString()+lector3.GetValue(4).ToString()+" "+lector3.GetValue(5).ToString()+" "+lector3.GetValue(6).ToString()+" "+lector3.GetValue(7).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(double.Parse(lector3.GetValue(8).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(eco_a_1_4.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(eco_a_2_4.ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector3.GetValue(16).ToString() == "1"?"OCUP":lector3.GetValue(16).ToString()=="2"?"ASIS":lector3.GetValue(16).ToString()=="3"?"FAR":lector3.GetValue(16).ToString()=="4"?"S-FAR":lector3.GetValue(16).ToString()=="5"?"S-PAC":lector3.GetValue(16).ToString()=="6"?"S-FAC":lector3.GetValue(16).ToString()=="7"?"MTC":lector3.GetValue(16).ToString()=="8"?"GIN":lector3.GetValue(16).ToString()=="9"?"RTF":lector3.GetValue(16).ToString(), fontColumnValue)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             //new PdfPCell(new Phrase(double.Parse(lector3.GetValue(11).ToString()).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             new PdfPCell(new Phrase(lector3.GetValue(13).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                             new PdfPCell(new Phrase(lector3.GetValue(14).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE}, 
                             new PdfPCell(new Phrase(lector3.GetValue(15).ToString(), fontColumnValue)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                         };
                             count3++; total3 = decimal.Round(decimal.Parse(lector3.GetValue(12).ToString()) + total3 , 2);
                             columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                             table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                             document.Add(table);
                         }

                     }
                     cells = new List<PdfPCell>()
                        {         
                        new PdfPCell(new Phrase(" TOTAL NO EFECTIVO:", fontColumnValueBold)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        new PdfPCell(new Phrase(total3.ToString(), fontColumnValueBold)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        new PdfPCell(new Phrase(" ", fontColumnValueBold)){ Colspan =5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                        
                        };
                     columnWidths = new float[] { 3f, 10f, 20f, 20f, 5f, 5f, 5f, 5f, 3f, 7f, 10f, 7f };
                     table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                     document.Add(table);
                     lector3.Close();
                    #endregion

                        #region RESUMEN
                     
                     
                     SqlCommand comando5 = null;
                     if (systemUserId != 2036 && tipo == 0 )
                     {
                         if (systemUserId == 1 || systemUserId == 2037)
                             comando5 = new SqlCommand(cadenaSA, connection: conectasam.conectarsam);
                         else
                             comando5 = new SqlCommand(cadena, connection: conectasam.conectarsam);
                         SqlDataReader lector5 = comando5.ExecuteReader();
                         decimal totalEgresos = 0,
                             totalIngresoEfectivo = 0,
                             totalCredito = 0,
                             totalIngresoNoEfectivo = 0,
                             totalOcupacional = 0,
                             totalAsistencial = 0,
                             totalEgresoOcupacional = 0,
                             totalEgresoAsistencial = 0,
                             totalEgresoMTC = 0,
                             totalEgresoGinecologia = 0,
                             totalEgresoRehabilitacion = 0,
                             totalIngresoEfectivoAsistencial = 0,
                             totalIngresoEfectivoOcupacional = 0,
                             totalIngresoEfectivoMTC = 0,
                             totalIngresoEfectivoGinecologia = 0,
                             totalIngresoEfectivoRehabilitacion = 0,

                             totalIngresoNoEfectivoAsistencial_1 = 0,
                             totalIngresoNoEfectivoOcupacional_2 = 0,
                             totalIngresoNoEfectivoAsistencial_3 = 0,
                             totalIngresoNoEfectivoOcupacional_4 = 0,
                             totalIngresoNoEfectivoMTC_5 = 0,
                             totalIngresoNoEfectivoMTC_6 = 0,
                             totalIngresoNoEfectivo_visaGineccologia = 0,
                             totalIngresoNoEfectivo_visarehabilitacion = 0,
                             totalIngresoNoEfectivo_depositoGinecologia = 0,
                             totalIngresoNoEfectivo_depositorehabilitacion = 0,
                             totalCreditoOcupacional = 0,
                             totalCreditoMTC = 0,
                             totalCreditoGinecologia = 0,
                             totalCreditoRehabilitacion = 0,
                             totalCreditoAsistencial = 0;


                         while (lector5.Read())
                         {
                             //if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA" || lector5.GetValue(0).ToString() == "ECF")
                             if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA" || lector5.GetValue(0).ToString() == "ECF" || lector5.GetValue(0).ToString() == "ECT" || lector5.GetValue(0).ToString() == "ECG" || lector5.GetValue(0).ToString() == "ECR")
                             {
                                 totalEgresos = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalEgresos, 2);
                             }
                             //if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA")
                             //{
                             //    totalEgresos = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresos, 2);
                             //}
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES")
                             {
                                 totalIngresoEfectivo = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivo, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM"  && lector5.GetValue(13).ToString() == "CREDITO")
                             {
                                 totalCredito = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCredito, 2);
                             }
                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") || (lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO"))
                             {
                                 totalIngresoNoEfectivo = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo, 2);
                             }

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(16).ToString() == "1")
                             {
                                 totalOcupacional = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalOcupacional, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM"  && ( lector5.GetValue(16).ToString() == "2" ||  lector5.GetValue(16).ToString() == "5" ||  lector5.GetValue(16).ToString() == "6"))
                             {
                                 totalAsistencial = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalAsistencial, 2);
                             }

                             if (lector5.GetValue(0).ToString() == "ECO")
                             {
                                 totalEgresoOcupacional = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresoOcupacional, 2);
                             }

                             if (lector5.GetValue(0).ToString() == "ECA")
                             {
                                 totalEgresoAsistencial = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresoAsistencial, 2);
                             }

                             if (lector5.GetValue(0).ToString() == "ECT")
                             {
                                 totalEgresoMTC = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresoMTC, 2);
                             }

                             if (lector5.GetValue(0).ToString() == "ECG")
                             {
                                 totalEgresoGinecologia = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresoGinecologia, 2);
                             }

                             if (lector5.GetValue(0).ToString() == "ECR")
                             {
                                 totalEgresoRehabilitacion = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresoRehabilitacion, 2);
                             }

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES" && lector5.GetValue(16).ToString() == "1" || lector5.GetValue(0).ToString() == "ICO")
                             {
                                 totalIngresoEfectivoOcupacional = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivoOcupacional, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES" && (lector5.GetValue(16).ToString() == "2" || lector5.GetValue(16).ToString() == "5" || lector5.GetValue(16).ToString() == "6"))
                             //|| lector5.GetValue(0).ToString() == "ICA"
                             {
                                 totalIngresoEfectivoAsistencial = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivoAsistencial, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES" && lector5.GetValue(16).ToString() == "7")
                             {
                                 totalIngresoEfectivoMTC = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivoMTC, 2);
                             }

                             //if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES" && lector5.GetValue(16).ToString() == "7")
                             //{
                             //    totalIngresoEfectivoMTC = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivoMTC, 2);
                             //}

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES" && lector5.GetValue(16).ToString() == "8")
                             {
                                 totalIngresoEfectivoGinecologia = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivoGinecologia, 2);
                             }

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES" && lector5.GetValue(16).ToString() == "9")
                             {
                                 totalIngresoEfectivoRehabilitacion = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivoRehabilitacion, 2);
                             }

                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && (lector5.GetValue(16).ToString() == "2" || lector5.GetValue(16).ToString() == "5" || lector5.GetValue(16).ToString() == "6"))
                             {
                                 totalIngresoNoEfectivoAsistencial_1 = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoAsistencial_1, 2);
                             }
                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && lector5.GetValue(16).ToString() == "1")
                             {
                                 totalIngresoNoEfectivoOcupacional_2 = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoOcupacional_2, 2);
                             }
                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && lector5.GetValue(16).ToString() == "7")
                             {
                                 totalIngresoNoEfectivoMTC_5 = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoMTC_5, 2);
                             }
                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && lector5.GetValue(16).ToString() == "8")
                             {
                                 totalIngresoNoEfectivo_visaGineccologia = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo_visaGineccologia, 2);
                             }
                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && lector5.GetValue(16).ToString() == "9")
                             {
                                 totalIngresoNoEfectivo_visarehabilitacion = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo_visarehabilitacion, 2);
                             }
                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && (lector5.GetValue(16).ToString() == "2" || lector5.GetValue(16).ToString() == "5" || lector5.GetValue(16).ToString() == "6"))
                             {
                                 totalIngresoNoEfectivoAsistencial_3 = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoAsistencial_3, 2);
                             }
                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && lector5.GetValue(16).ToString() == "1")
                             {
                                 totalIngresoNoEfectivoOcupacional_4 = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoOcupacional_4, 2);
                             }
                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && lector5.GetValue(16).ToString() == "7")
                             {
                                 totalIngresoNoEfectivoMTC_6 = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoMTC_6, 2);
                             }

                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && lector5.GetValue(16).ToString() == "8")
                             {
                                 totalIngresoNoEfectivo_depositoGinecologia = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo_depositoGinecologia, 2);
                             }

                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && lector5.GetValue(16).ToString() == "9")
                             {
                                 totalIngresoNoEfectivo_depositorehabilitacion = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo_depositorehabilitacion, 2);
                             }

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CREDITO" && lector5.GetValue(16).ToString() == "1")
                             {
                                 totalCreditoOcupacional = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCreditoOcupacional, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CREDITO" && lector5.GetValue(16).ToString() == "7")
                             {
                                 totalCreditoMTC = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCreditoMTC, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CREDITO" && lector5.GetValue(16).ToString() == "8")
                             {
                                 totalCreditoGinecologia = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCreditoGinecologia, 2);
                             }

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CREDITO" && lector5.GetValue(16).ToString() == "9")
                             {
                                 totalCreditoRehabilitacion = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCreditoRehabilitacion, 2);
                             }

                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECT" && lector5.GetValue(0).ToString() != "ECG" && lector5.GetValue(0).ToString() != "ECR" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CREDITO" && (lector5.GetValue(16).ToString() == "2" || lector5.GetValue(16).ToString() == "5" || lector5.GetValue(16).ToString() == "6"))
                             {
                                 totalCreditoAsistencial = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCreditoAsistencial, 2);
                             }

                         }

                             cells = new List<PdfPCell>()
                            {         
                                 new PdfPCell(new Phrase("RESUMEN DE CAJA", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                                 new PdfPCell(new Phrase("OCUPACIONAL", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                                 new PdfPCell(new Phrase("ASISTENCIAL", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalEgresoOcupacional.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalEgresoAsistencial.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivoOcupacional.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivoAsistencial.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalCreditoOcupacional.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalCreditoAsistencial.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoOcupacional_2.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoAsistencial_1.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoOcupacional_4.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoAsistencial_3.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("TOTAL ENTREGAR   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoOcupacional + totalEgresoOcupacional).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("TOTAL ENTREGAR   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoAsistencial + totalEgresoAsistencial).ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("VENTA TOTAL      S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoOcupacional + totalEgresoOcupacional + totalCreditoOcupacional + totalIngresoNoEfectivoOcupacional_2 + totalIngresoNoEfectivoOcupacional_4).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("VENTA TOTAL      S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoAsistencial + totalEgresoAsistencial + totalCreditoAsistencial + totalIngresoNoEfectivoAsistencial_1 + totalIngresoNoEfectivoAsistencial_3).ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("GINECOLOGÍA", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                                 new PdfPCell(new Phrase("REHABILITACIÓN", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalEgresoGinecologia.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalEgresoRehabilitacion.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivoGinecologia.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivoRehabilitacion.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalCreditoGinecologia.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalCreditoRehabilitacion.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivo_visaGineccologia.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivo_visarehabilitacion.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivo_depositoGinecologia.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivo_depositorehabilitacion.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("TOTAL ENTREGAR   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoGinecologia + totalEgresoGinecologia).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("TOTAL ENTREGAR   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoRehabilitacion + totalEgresoRehabilitacion).ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    


                                 new PdfPCell(new Phrase("VENTA TOTAL      S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoGinecologia + totalEgresoGinecologia + totalCreditoGinecologia + totalIngresoNoEfectivo_visaGineccologia + totalIngresoNoEfectivo_depositoGinecologia).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("VENTA TOTAL      S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoRehabilitacion + totalEgresoRehabilitacion + totalCreditoRehabilitacion + totalIngresoNoEfectivo_visarehabilitacion + totalIngresoNoEfectivo_depositorehabilitacion).ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    




                                 new PdfPCell(new Phrase("MTC", fontColumnValueBold_1)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalEgresoMTC.ToString(), fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                             
                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivoMTC.ToString(), fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                
                                 new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalCreditoMTC.ToString(), fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                               
                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoMTC_5.ToString(), fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                
                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoMTC_6.ToString(), fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                               
                                 new PdfPCell(new Phrase("TOTAL ENTREGAR   S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoMTC + totalEgresoMTC).ToString() , fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                               
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("---------------------------", fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                
                                 new PdfPCell(new Phrase("VENTA TOTAL      S/. ", fontColumnValueBold_1)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivoMTC + totalEgresoMTC + totalCreditoMTC + totalIngresoNoEfectivoMTC_5 + totalIngresoNoEfectivoMTC_6).ToString() , fontColumnValueBold_1)){ Colspan =2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    



                                 new PdfPCell(new Phrase("TOTAL EFECTIVO (5 cajas)      S/.  " + (totalIngresoEfectivoOcupacional + totalEgresoOcupacional + totalIngresoEfectivoAsistencial + totalEgresoAsistencial + totalIngresoEfectivoMTC + totalEgresoMTC + totalIngresoEfectivoGinecologia + totalEgresoGinecologia + totalIngresoEfectivoRehabilitacion + totalEgresoRehabilitacion).ToString(), fontTitleTable1)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("TOTAL EFECTIVO (ASISTENCIAL: A + G + R)      S/.  " + ( totalIngresoEfectivoAsistencial + totalEgresoAsistencial  + totalIngresoEfectivoGinecologia + totalEgresoGinecologia + totalIngresoEfectivoRehabilitacion + totalEgresoRehabilitacion).ToString(), fontTitleTable1)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =6, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    

                            };
                             columnWidths = new float[] { 25f, 15f, 10f, 25f, 15f, 10f };
                             table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                             document.Add(table);

                     }
                     else if (systemUserId != 2036 && tipo == 1 )
                     {
                         comando5 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                         SqlDataReader lector5 = comando5.ExecuteReader();
                         decimal totalEgresos = 0,
                             totalIngresoEfectivo = 0,
                             totalCredito = 0,
                             totalIngresoNoEfectivo = 0,
                             totalIngresoNoEfectivoVISA = 0;

                         while (lector5.Read())
                         {

                             if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA" || lector5.GetValue(0).ToString() == "ECF")
                             {
                                 totalEgresos = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalEgresos, 2);
                             }
                             //if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA")
                             //{
                             //    totalEgresos = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresos, 2);
                             //}
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(0).ToString() != "ECF" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES")
                             {
                                 totalIngresoEfectivo = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivo, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(0).ToString() != "ECF" && lector5.GetValue(13).ToString() == "CREDITO")
                             {
                                 totalCredito = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCredito, 2);
                             }
                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && (lector5.GetValue(16).ToString() == "3" || lector5.GetValue(16).ToString() == "4"))
                             {
                                 totalIngresoNoEfectivo = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo, 2);
                             }


                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "ECF" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && (lector5.GetValue(16).ToString() == "3" || lector5.GetValue(16).ToString() == "4"))
                             {
                                 totalIngresoNoEfectivoVISA = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoVISA, 2);
                             }

                         }
                         //entregar = decimal.Round(totalIngresoEfectivo - totalEgresos, 2);

                         //total = decimal.Round(totalIngresoEfectivo - totalEgresos + totalIngresoNoEfectivoVISA + totalIngresoNoEfectivo, 2);
                         decimal caja_chica = 203.5m;
                         cells = new List<PdfPCell>()
                            {         
                                 new PdfPCell(new Phrase("RESUMEN DE CAJA FARMACIA", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                                 
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("- " + totalEgresos.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivo.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                
                                 //new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 //new PdfPCell(new Phrase(totalCredito.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 //new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoVISA.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivo.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("*** CAJA CHICHA MENSUAL   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((caja_chica ).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("TOTAL EFECTIVO S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivo - totalEgresos - caja_chica).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("TOTAL VENTAS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivo - totalEgresos + totalIngresoNoEfectivoVISA + totalIngresoNoEfectivo - caja_chica).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    

     
                            };
                         columnWidths = new float[] { 45f, 25f, 30f };
                         table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                         document.Add(table);
                     }
                     else
                     {
                         comando5 = new SqlCommand(cadenaFarmacia, connection: conectasam.conectarsam);
                         SqlDataReader lector5 = comando5.ExecuteReader();
                         decimal totalEgresos = 0,
                             totalIngresoEfectivo = 0,
                             totalCredito = 0,
                             totalIngresoNoEfectivo = 0,
                             totalIngresoNoEfectivoVISA = 0;

                         while (lector5.Read())
                         {

                             if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA" || lector5.GetValue(0).ToString() == "ECF")
                             {
                                 totalEgresos = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalEgresos, 2);
                             }
                             //if (lector5.GetValue(0).ToString() == "ECO" || lector5.GetValue(0).ToString() == "ECA")
                             //{
                             //    totalEgresos = decimal.Round((decimal.Parse(lector5.GetValue(12).ToString()) * -1) + totalEgresos, 2);
                             //}
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(0).ToString() != "ECF" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() == "EFECTIVO SOLES")
                             {
                                 totalIngresoEfectivo = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoEfectivo, 2);
                             }
                             if (lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(0).ToString() != "ECF" && lector5.GetValue(13).ToString() == "CREDITO")
                             {
                                 totalCredito = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalCredito, 2);
                             }
                             if ((lector5.GetValue(13).ToString() == "CHEQUE" || lector5.GetValue(13).ToString() == "DEPOSITO") && (lector5.GetValue(16).ToString() == "3" || lector5.GetValue(16).ToString() == "4"))
                             {
                                 totalIngresoNoEfectivo = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivo, 2);
                             }


                             if ((lector5.GetValue(0).ToString() != "ECO" && lector5.GetValue(0).ToString() != "ECA" && lector5.GetValue(0).ToString() != "ECF" && lector5.GetValue(0).ToString() != "TFM" && lector5.GetValue(0).ToString() != "THM" && lector5.GetValue(13).ToString() == "CONTADO" && lector5.GetValue(14).ToString() != "EFECTIVO SOLES") && (lector5.GetValue(16).ToString() == "3" || lector5.GetValue(16).ToString() == "4"))
                             {
                                 totalIngresoNoEfectivoVISA = decimal.Round(decimal.Parse(lector5.GetValue(12).ToString()) + totalIngresoNoEfectivoVISA, 2);
                             }

                         }
                         //entregar = decimal.Round(totalIngresoEfectivo - totalEgresos, 2);

                         //total = decimal.Round(totalIngresoEfectivo - totalEgresos + totalIngresoNoEfectivoVISA + totalIngresoNoEfectivo, 2);
                         decimal caja_chica = 203.5m;
                         cells = new List<PdfPCell>()
                            {         
                                 new PdfPCell(new Phrase("RESUMEN DE CAJA FARMACIA", fontTitleTable)){ BackgroundColor=BaseColor.LIGHT_GRAY, Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                                 
                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.BLACK},    

                                 new PdfPCell(new Phrase("EGRESOS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("- " + totalEgresos.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("INGRESOS CONTADO EFECTIVO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoEfectivo.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                
                                 //new PdfPCell(new Phrase("INGRESOS CRÉDITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 //new PdfPCell(new Phrase(totalCredito.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 //new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("INGRESOS VISA   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivoVISA.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("INGRESOS DEPOSITO   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase(totalIngresoNoEfectivo.ToString(), fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("*** CAJA CHICHA MENSUAL   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((caja_chica ).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("TOTAL EFECTIVO S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivo - totalEgresos - caja_chica).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 
                                 new PdfPCell(new Phrase("TOTAL VENTAS   S/. ", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase((totalIngresoEfectivo - totalEgresos + totalIngresoNoEfectivoVISA + totalIngresoNoEfectivo - caja_chica).ToString() , fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.WHITE,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    
                                 new PdfPCell(new Phrase("", fontColumnValueBold_1)){ Colspan =1, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda, UseVariableBorders=true, BorderColorLeft=BaseColor.WHITE,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.WHITE, BorderColorTop=BaseColor.WHITE},    

                                 new PdfPCell(new Phrase("", fontColumnValueBold)){ Colspan =3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = 7f, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.WHITE},    

     
                            };
                         columnWidths = new float[] { 45f, 25f, 30f };
                         table = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTable);
                         document.Add(table);
                     }


                     #endregion

                        #region SE RECIBIÓ CONFORME
                     cells = new List<PdfPCell>()
                         {
                             new PdfPCell(new Phrase("FIRMA RECIBÍ CONFORME", fontColumnValueBold)){ Colspan =1, Rowspan =5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda2, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                             new PdfPCell(new Phrase("FIRMA USUARIO DE CAJA", fontColumnValueBold)){ Colspan =1,Rowspan =5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, MinimumHeight = tamaño_celda2, UseVariableBorders=true, BorderColorLeft=BaseColor.BLACK,  BorderColorRight=BaseColor.BLACK,  BorderColorBottom=BaseColor.BLACK, BorderColorTop=BaseColor.BLACK},    
                         };
                         columnWidths = new float[] { 50f, 50f };
                         filiationWorker = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, null, fontTitleTableNegro, null);
                         document.Add(filiationWorker);
                       #endregion
                 
                
                 // step 5: we close the document

                 document.Close();
                 writer.Close();
                 writer.Dispose();
                 RunFile(filePDF);

               

                 

                 #endregion
                
                
         }

        private static object GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }



         #region Utils

         private static void RunFile(string filePDF)
         {
             Process proceso = Process.Start(filePDF);
             proceso.WaitForExit();
             proceso.Close();

         }

         #endregion

    }
}
