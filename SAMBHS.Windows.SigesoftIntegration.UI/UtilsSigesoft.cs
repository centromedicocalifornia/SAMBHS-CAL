using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dapper;
using System.Text.RegularExpressions;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
  public  class UtilsSigesoft
    {
        public static Boolean email_bien_escrito(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static Image BytesArrayToImageOficce(byte[] pBytes, PictureBox pb)
        {
            System.Drawing.Image newImage = null;
            if (pBytes == null) return null;
            //            
            var ms = new MemoryStream(pBytes);
            Bitmap bm = null;
            try
            {
                bm = new Bitmap(ms);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return bm;
        }

        public static DataTable ConvertToDatatable<T>(List<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static byte[] ResizeUploadedImage(Stream streamToResize)
        {
            byte[] resizedImage;
            using (Image orginalImage = Image.FromStream(streamToResize))
            {
                ImageFormat orginalImageFormat = orginalImage.RawFormat;
                int orginalImageWidth = orginalImage.Width;
                int orginalImageHeight = orginalImage.Height;
                int resizedImageWidth = 383; // Type here the width you want
                int resizedImageHeight = Convert.ToInt32(resizedImageWidth * orginalImageHeight / orginalImageWidth);
                using (Bitmap bitmapResized = new Bitmap(orginalImage, resizedImageWidth, resizedImageHeight))
                {
                    using (MemoryStream streamResized = new MemoryStream())
                    {
                        bitmapResized.Save(streamResized, orginalImageFormat);
                        resizedImage = streamResized.ToArray();
                    }
                }
            }

            return resizedImage;
        }

        public static Image BytesArrayToImage(byte[] pBytes, PictureBox pb)
        {
            System.Drawing.Image newImage = null;
            if (pBytes == null) return null;
            //            
            MemoryStream ms = new MemoryStream(pBytes);
            Bitmap bm = null;
            try
            {
                bm = new Bitmap(ms);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return bm;
        }

        public static byte[] byteArrayToByteArrayImageJpg(byte[] byteArrayIn)
        {
            Image imgOriginal = byteArrayToImage(byteArrayIn);

            using (var ms = new MemoryStream())
            {
                imgOriginal.Save(ms, ImageFormat.Jpeg);
                byte[] jpgImage = ms.ToArray();
                return jpgImage;
            }
        }

        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            // TODO: Pasar a estático
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] ImageToByteArrayImageJpg1(System.Drawing.Image imageIn)
        {
            //MemoryStream ms = new MemoryStream();
            //imageIn.Save(ms, ImageFormat.Jpeg);
            //return ms.ToArray();

            using (var ms = new MemoryStream())
            {
                Bitmap bmp = new Bitmap(imageIn);
                bmp.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] ImageToByteArrayImageJpg(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public static AgendaBl.Secuential GetNextSecuentialId(int tableId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query =
                    "update secuential set i_SecuentialId = (select i_SecuentialId from secuential where i_NodeId = 9 and  i_TableId =" +
                    tableId + " ) + 1 where i_NodeId = 9 and  i_TableId = " + tableId +
                    " select i_NodeId as NodeId ,i_TableId as TableId ,i_SecuentialId as SecuentialId from secuential where i_NodeId = 9 and  i_TableId =" +
                    tableId;
                return cnx.Query<AgendaBl.Secuential>(query).FirstOrDefault();
            }
        }

        public static string GetNewId(int pintNodeId, int pintSequential, string pstrPrefix)
        {
            return string.Format("N{0:000}-{1}{2:000000000}", pintNodeId, pstrPrefix, pintSequential);
        }

    }
}
