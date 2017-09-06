
// Solution:  Spark
// Spark.Engine
// File:  HtmlFhirFormatter.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:04 PM




// using FhirOnAzure.Service;

namespace FhirOnAzure.Formatters
{
    using Core;
    using Engine;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using Task = System.Threading.Tasks.Task;

    public class HtmlFhirFormatter : FhirMediaTypeFormatter
    {
        public HtmlFhirFormatter() => SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

        async Task WriteHtmlOutputAsync(Type type, object value, Stream writeStream)
        {
            var writer = new StreamWriter(writeStream, Encoding.UTF8);
            await writer.WriteLineAsync("<html>").ConfigureAwait(false);
            await writer.WriteLineAsync("<head>").ConfigureAwait(false);
            await writer.WriteLineAsync("  <link href=\"/Content/fhir-html.css\" rel=\"stylesheet\"></link>").ConfigureAwait(false);
            await writer.WriteLineAsync("</head>").ConfigureAwait(false);
            await writer.WriteLineAsync("<body>").ConfigureAwait(false);
            if (type == typeof(OperationOutcome))
            {
                var oo = (OperationOutcome)value;

                if (oo.Text != null)
                    await writer.WriteAsync(oo.Text.Div).ConfigureAwait(false);
            }
            else if (type == typeof(Resource))
            {
                if (value is Bundle)
                {
                    var resource = (Bundle)value;

                    if (resource.SelfLink != null)
                    {
                        writer.WriteLine("Searching: {0}<br/>", resource.SelfLink.OriginalString);

                        // Hl7.Fhir.Model.Parameters query = FhirParser.ParseQueryFromUriParameters(collection, parameters);

                        var ps = resource.SelfLink.ParseQueryString();
                        if (ps.AllKeys.Contains(FhirParameter.SORT))
                            writer.WriteLine("    Sort by: {0}<br/>", ps[FhirParameter.SORT]);
                        if (ps.AllKeys.Contains(FhirParameter.SUMMARY))
                            await writer.WriteLineAsync("    Summary only<br/>").ConfigureAwait(false);
                        if (ps.AllKeys.Contains(FhirParameter.COUNT))
                            writer.WriteLine("    Count: {0}<br/>", ps[FhirParameter.COUNT]);
                        if (ps.AllKeys.Contains(FhirParameter.SNAPSHOT_INDEX))
                            writer.WriteLine("    From RowNum: {0}<br/>", ps[FhirParameter.SNAPSHOT_INDEX]);
                        if (ps.AllKeys.Contains(FhirParameter.SINCE))
                            writer.WriteLine("    Since: {0}<br/>", ps[FhirParameter.SINCE]);


                        foreach (var item in ps.AllKeys.Where(k => !k.StartsWith("_")))
                            if (ModelInfo.SearchParameters.Exists(s => s.Name == item))
                                writer.WriteLine("    {0}: {1}<br/>", item, ps[item]);
                            else
                                writer.WriteLine("    <i>{0}: {1} (excluded)</i><br/>", item, ps[item]);
                    }

                    if (resource.FirstLink != null)
                        writer.WriteLine("First Link: {0}<br/>", resource.FirstLink.OriginalString);
                    if (resource.PreviousLink != null)
                        writer.WriteLine("Previous Link: {0}<br/>", resource.PreviousLink.OriginalString);
                    if (resource.NextLink != null)
                        writer.WriteLine("Next Link: {0}<br/>", resource.NextLink.OriginalString);
                    if (resource.LastLink != null)
                        writer.WriteLine("Last Link: {0}<br/>", resource.LastLink.OriginalString);

                    // Write the other Bundle Header data
                    writer.WriteLine(
                        "<span style=\"word-wrap: break-word; display:block;\">Type: {0}, {1} of {2}</span>",
                        resource.Type, resource.Entry.Count, resource.Total);

                    foreach (var item in resource.Entry)
                    {
                        //IKey key = item.ExtractKey();

                        await writer.WriteLineAsync("<div class=\"item-tile\">").ConfigureAwait(false);
                        if (item.IsDeleted())
                        {
                            if (item.Request != null)
                            {
                                var id = item.Request.Url;
                                writer.WriteLine("<span style=\"word-wrap: break-word; display:block;\">{0}</span>",
                                    id);
                            }

                            //if (item.Deleted.Instant.HasValue)
                            //    writer.WriteLine(String.Format("<i>Deleted: {0}</i><br/>", item.Deleted.Instant.Value.ToString()));

                            await writer.WriteLineAsync("<hr/>").ConfigureAwait(false);
                            await writer.WriteLineAsync("<b>DELETED</b><br/>").ConfigureAwait(false);
                        }
                        else if (item.Resource != null)
                        {
                            var key = item.Resource.ExtractKey();
                            var visualurl = key.WithoutBase().ToUriString();
                            var realurl = key.ToUriString() + "?_format=html";

                            writer.WriteLine("<a style=\"word-wrap: break-word; display:block;\" href=\"{0}\">{1}</a>",
                                realurl, visualurl);
                            if (item.Resource.Meta != null && item.Resource.Meta.LastUpdated.HasValue)
                                writer.WriteLine("<i>Modified: {0}</i><br/>", item.Resource.Meta.LastUpdated.Value);
                            await writer.WriteLineAsync("<hr/>").ConfigureAwait(false);

                            if (item.Resource is DomainResource)
                                if ((item.Resource as DomainResource).Text != null &&
                                    !string.IsNullOrEmpty((item.Resource as DomainResource).Text.Div))
                                    await writer.WriteAsync((item.Resource as DomainResource).Text.Div).ConfigureAwait(false);
                                else
                                    writer.WriteLine("Blank Text: {0}<br/>", item.Resource.ExtractKey().ToUriString());
                            else
                                await writer.WriteLineAsync("This is not a domain resource").ConfigureAwait(false);
                        }
                        await writer.WriteLineAsync("</div>").ConfigureAwait(false);
                    }
                }
                else
                {
                    var resource = (DomainResource)value;
                    var org = resource.ResourceBase + "/" + resource.ResourceType + "/" + resource.Id;
                    // TODO: This is probably a bug in the service (Id is null can throw ResourceIdentity == null
                    // reference ResourceIdentity : org = resource.ResourceIdentity().OriginalString;
                    writer.WriteLine("Retrieved: {0}<hr/>", org);

                    var text = resource.Text != null ? resource.Text.Div : null;
                    await writer.WriteAsync(text).ConfigureAwait(false);
                    await writer.WriteLineAsync("<hr/>").ConfigureAwait(false);

                    var summary = RequestMessage.RequestSummary();
                    var xml = FhirSerializer.SerializeResourceToXml(resource, summary);
                    var xmlDoc = new XPathDocument(new StringReader(xml));

                    // And we also need an output writer
                    TextWriter output = new StringWriter(new StringBuilder());

                    // Now for a little magic
                    // Create XML Reader with style-sheet
                    var stylesheetReader = XmlReader.Create(new StringReader(Resources.RenderXMLasHTML));

                    var xslTransform = new XslCompiledTransform();
                    xslTransform.Load(stylesheetReader);
                    xslTransform.Transform(xmlDoc, null, output);

                    await writer.WriteLineAsync(output.ToString()).ConfigureAwait(false);
                }
            }

            await writer.WriteLineAsync("</body>").ConfigureAwait(false);
            await writer.WriteLineAsync("</html>").ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger) => Task.Factory.StartNew<object>(() =>
                                               {
                                                   try
                                                   {
                                                       throw new NotSupportedException(
                                   $"Cannot read unsupported type {type.Name} from body");
                                                   }
                                                   catch (FormatException exc)
                                                   {
                                                       throw Error.BadRequest("Body parsing failed: " + exc.Message);
                                                   }
                                               });

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers,
            MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("text/html");
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext) => Task.Factory.StartNew(() => { WriteHtmlOutputAsync(type, value, writeStream).Wait(); });
    }
}