#region Information

// Solution:  HL7V2Connector
// HL7V2Parser
// File:  DocumentExtensions.cs
// 
// Created: 09/06/2017 : 5:58 PM
// 
// Modified By: Howard Edidin
// Modified:  09/06/2017 : 5:58 PM

#endregion

namespace HL7V2Parser
{
    using System.Xml;
    using System.Xml.Linq;

    public static class DocumentExtensions
    {
        /// <summary>
        /// To the XML document.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            var xDeclaration = xDocument.Declaration;
            if (xDeclaration == null) return xmlDocument;
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration(
                xDeclaration.Version,
                xDeclaration.Encoding,
                xDeclaration.Standalone);

            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.FirstChild);


            return xmlDocument;
        }


        /// <summary>
        /// To the x document.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}