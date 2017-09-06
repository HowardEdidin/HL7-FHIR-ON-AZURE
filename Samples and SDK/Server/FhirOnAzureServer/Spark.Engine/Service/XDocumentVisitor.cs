#region Information

// Solution:  Spark
// Spark.Engine
// File:  XDocumentVisitor.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:57 PM

#endregion

namespace FhirOnAzure.Core
{
    using System;
    using System.Xml.Linq;
    using Engine.Core;

    public static class XDocumentExtensions
    {
        public static void VisitAttributes(this XDocument document, string tagname, string attrName,
            Action<XAttribute> action)
        {
            var nodes = document.Descendants(Namespaces.XHtml + tagname).Attributes(attrName);
            foreach (var node in nodes)
                action(node);
        }
    }
}