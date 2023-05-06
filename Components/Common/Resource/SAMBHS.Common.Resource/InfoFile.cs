using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SAMBHS.Common.Resource
{
    public class InfoFile
    {
        XmlDocument _xmlDocument;

        public InfoFile(string pFileName)
        {
            //Abrir el archivo en la forma de documento
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(pFileName);
        }

        public string GetInformation(string pHeader, string pRowField, string pNameField, string pValueField)
        {
            XmlElement rootNode = _xmlDocument.DocumentElement;
            XmlElement headerNode = (XmlElement)rootNode.SelectSingleNode("./" + pHeader);
            XmlNodeList rowNodesArray = headerNode.GetElementsByTagName(pRowField);

            string infoValue = null;

            foreach (XmlNode nodo in rowNodesArray)
            {

                if (nodo.Attributes[0].Value == pNameField)
                {
                    if (nodo.Attributes[pValueField] != null)
                    {
                        infoValue = nodo.Attributes[pValueField].Value;
                    }
                    break;
                }
            }

            return infoValue;
        }

        public XmlNodeList GetChilds(string pHeader, string pRowField)
        {
            XmlElement rootNode = _xmlDocument.DocumentElement;
            XmlElement headerNode = (XmlElement)rootNode.SelectSingleNode("./" + pHeader);
            XmlNodeList rowNodesArray = headerNode.GetElementsByTagName(pRowField);

            return rowNodesArray;
        }

        public static void SaveItemInXML(XmlWriter pWriter, string pStartElement, NameValue[] pItems)
        {
            pWriter.WriteStartElement(pStartElement);
            foreach (NameValue item in pItems)
            {
                pWriter.WriteAttributeString(item.Name, item.Value);
            }
            pWriter.WriteEndElement();
        }

    }

    public class NameValue
    {
        public string Name;
        public string Value;

        public NameValue(string pName, string pValue)
        {
            Name = pName;
            Value = pValue;
        }
    }
}
