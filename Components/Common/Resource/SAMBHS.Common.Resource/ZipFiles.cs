using System.Collections.Generic;
using Ionic.Zip;

namespace SAMBHS.Common.Resource
{
    public class ZipFiles
    {
        public static void CompressFile(string pFileName, List<string> pFilesToCompress)
        {
            // Generating the ZIP File
            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                foreach (var item in pFilesToCompress)
                {
                    zip.AddFile(item, @"\");
                }

                zip.Save(pFileName);
            }
        }

        public static List<string> DecompressFile(string pCompressedFile, string pTargetFolder)
        {
            var unzippedFilePaths = new List<string>();
            using (var zipFile = ZipFile.Read(pCompressedFile))
            {
                foreach (var entry in zipFile)
                {
                    entry.Extract(pTargetFolder, ExtractExistingFileAction.OverwriteSilently);
                    unzippedFilePaths.Add(System.IO.Path.Combine(pTargetFolder, entry.FileName));
                }
            }
            return unzippedFilePaths;
        }


    }
}
