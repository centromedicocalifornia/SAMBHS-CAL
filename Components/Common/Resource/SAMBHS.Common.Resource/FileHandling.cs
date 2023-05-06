using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace SAMBHS.Common.Resource
{
    public class FileHandling
    {
        public static void DeleteFiles(List<string> pFilesToDelete)
        {
            foreach (var item in pFilesToDelete)
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception)
                {
                }
            }
        }

        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buffer = null;

            // Open file for reading
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // attach filestream to binary reader
            var binaryReader = new BinaryReader(fileStream);

            // get total byte length of the file
            long totalBytes = new System.IO.FileInfo(fileName).Length;

            // read entire file into buffer
            buffer = binaryReader.ReadBytes((Int32)totalBytes);

            // close file reader
            fileStream.Close();
            binaryReader.Close();

            return buffer;
        }

        public static void ByteArrayToFile(string pFileName, byte[] pByteArray)
        {
            // Open file for reading
            var fileStream = new FileStream(pFileName, FileMode.Create, FileAccess.Write);

            // Writes a block of bytes to this stream using data from a byte array.
            fileStream.Write(pByteArray, 0, pByteArray.Length);

            // close file stream
            fileStream.Close();
        }

        public static byte[] GetBytesOfFile(string pFileName)
        {
            var fs = new FileStream(pFileName, FileMode.Open, FileAccess.Read);
            var myData = new byte[fs.Length];
            fs.Read(myData, 0, (int)fs.Length);
            fs.Close();
            return myData;
        }

        public static string GetDataFromUtf8TextFile(string pTextFile)
        {
            // Reading contents using FileStream
            //using (FileStream stream = File.OpenRead(pTextFile))
            //{
            //    // reading data
            //    StringBuilder strb = new StringBuilder();
            //    byte[] b = new byte[stream.Length];
            //    UTF8Encoding temp = new UTF8Encoding(true);

            //    while (stream.Read(b, 0, b.Length) > 0)
            //    {
            //        strb.Append(temp.GetString(b, 0, b.Length));
            //    }

            //    return strb.ToString();
            //}

            //Creating a new stream-reader and opening the file.
            TextReader trs = new StreamReader(pTextFile);

            //Reading all the text of the file.
            string data = trs.ReadToEnd();

            //Close the file.
            trs.Close();

            return data;
        }

        public static string GetFileSize(long bytes)
        {
            var ingles = new CultureInfo("en-US");

            if (bytes >= 1073741824)
            {
                var size = Decimal.Divide(bytes, 1073741824);
                return String.Format(ingles, "{0:##.##} GB", size);
            }
            else if (bytes >= 1048576)
            {
                var size = Decimal.Divide(bytes, 1048576);
                return String.Format(ingles, "{0:##.##} MB", size);
            }
            else if (bytes >= 1024)
            {
                var size = Decimal.Divide(bytes, 1024);
                return String.Format(ingles, "{0:##.##} KB", size);
            }
            else if (bytes > 0 & bytes < 1024)
            {
                var size = bytes;
                return String.Format(ingles, "{0:##.##} Bytes", size);
            }
            else
            {
                return "0 Bytes";
            }
        }

        public static string GetFileSize(string pFile)
        {
            var f = new FileInfo(pFile);
            return GetFileSize(f.Length);
        }

        public static string GetFileSizeInMegabytes(string pFile)
        {
            if (!File.Exists(pFile)) return string.Empty;
           
            var f = new FileInfo(pFile);
            var ingles = new CultureInfo("en-US");
            var size = ((float)f.Length) / 1048576;
            return String.Format(ingles, "{0:0.###}", size);
        }

        public static double GetFileSizeInMegabytesAsDouble(string pFile)
        {
            var f = new FileInfo(pFile);
            var size = ((double)f.Length) / 1048576;
            return size;
        }
    }
}
