using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace SAMBHS.Common.Resource
{
    public class UtilXml
    {
        public static string ReadMessageInformation(string prmSection, string prmKey)
        {
            XElement xmlMessageInformation = XElement.Load(@"Message\MessageInformation.xml");
            string Information = (from c in xmlMessageInformation.Descendants(prmSection).Elements("InformationMessage")
                                  where c.Attribute("type").Value.Equals(prmKey)
                                  select c.Value).FirstOrDefault();
            return Information;
        }

        /// <summary>
        /// Lee la configuracion del sistema para una seccion y llave especificada
        /// </summary>
        /// <param name="prmSection"></param>
        /// <param name="prmKey"></param>
        /// <returns></returns>
        public static string ReadSystemConfiguration(string prmSection, string prmKey)
        {
            XElement xmlMessageInformation = XElement.Load(Constants.CONFIGURATION_FILE_PATH_SYSTEM);
            string Information = (from c in xmlMessageInformation.Descendants(prmSection).Elements("Information")
                                  where c.Attribute("type").Value.Equals(prmKey)
                                  select c.Value).FirstOrDefault();
            return Information;
        }

        public static void WriteSystemConfiguration(string prmSection, string prmKey,string pValue)
        {
            XElement xmlMessageInformation = XElement.Load(Constants.CONFIGURATION_FILE_PATH_SYSTEM);

            var Information = (from c in xmlMessageInformation.Descendants(prmSection).Elements("Information")
                                  where c.Attribute("type").Value.Equals(prmKey)
                                  select c).FirstOrDefault();

            if (Information != null)
            {
                Information.Value = pValue;
                xmlMessageInformation.Save(Constants.CONFIGURATION_FILE_PATH_SYSTEM);
            }
        }

        public static string SearchFileNameInList(string pUpdateCriteria, List<string> pFilesList)
        {
            return pFilesList.Find(
                s => (Path.GetFileName(s).StartsWith(pUpdateCriteria))
                );
        }

        #region Serialization

        public static void Serialize(object data, string file)
        {
            if (data == null) return;

            XmlSerializer serializer = new XmlSerializer(data.GetType());
            using (StreamWriter writer = new StreamWriter(file))
            {
                serializer.Serialize(writer, data);
            }
        }

        public static object DeSerialize(Type dataType, string file)
        {
            XmlSerializer serializer = new XmlSerializer(dataType);
            object dataToReturn;

            using (StreamReader reader = new StreamReader(file))
            {
                dataToReturn = serializer.Deserialize(reader);
            }

            return dataToReturn;
        }

        #endregion
    }
}
