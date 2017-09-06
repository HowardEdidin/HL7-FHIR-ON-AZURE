

The **HL7V2Parser** contains two classes.

- Parser
- DocumentExtensions

#### Parser ####

```c#

/// <summary>
/// Parses the specified HL7.
/// </summary>
/// <param name="hl7">The HL7.</param>
/// <returns>XDocument string</returns>
/// <exception cref="System.ArgumentNullException">hl7</exception>
public XDocument Parse(string hl7)
{
    if (hl7 == null)
        throw new ArgumentNullException(nameof(hl7));

    using (var reader = new StringReader(hl7))
    {
        return Parse(reader);
    }
}
        
```

#### DocumentExtensions ####

```C#
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

```

```C#

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

 ```
