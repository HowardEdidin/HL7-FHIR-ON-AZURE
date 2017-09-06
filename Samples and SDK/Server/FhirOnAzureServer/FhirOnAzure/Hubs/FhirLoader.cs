#region Information

// Solution:  Spark
// FhirOnAzure
// File:  FhirLoader.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:39 PM

#endregion

namespace FhirOnAzure.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Hl7.Fhir.Utility;

    internal static class FhirLoader
    {
        private static Resource ParseResource(string data)
        {
            if (!SerializationUtil.ProbeIsJson(data) || !SerializationUtil.ProbeIsXml(data))
                throw new FormatException("Data is neither Json nor Xml");
            if (SerializationUtil.ProbeIsJson(data))
            {
                var fhirJsonParser = new FhirJsonParser();
                return fhirJsonParser.Parse<Resource>(data);
            }
            if (SerializationUtil.ProbeIsXml(data))
            {
                var fhirXmlParser = new FhirXmlParser();
                return fhirXmlParser.Parse<Resource>(data);
            }
            return null;
        }

        public static IEnumerable<Resource> ExtractResourcesFromZip(this byte[] buffer)
        {
            return buffer.ExtractZipEntries()
                .SelectMany(ImportData);
        }

        public static IEnumerable<string> ExtractZipEntries(this byte[] buffer)
        {
            using (Stream stream = new MemoryStream(buffer))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    var reader = new StreamReader(entry.Open());
                    var data = reader.ReadToEnd();
                    yield return data;
                }
            }
        }

        public static IEnumerable<Resource> ImportData(string data)
        {
            var resource = ParseResource(data);
            if (!(resource is Bundle)) return new[] {resource};
            var bundle = resource as Bundle;
            return bundle.GetResources();
        }

        public static IEnumerable<Resource> ImportFile(string filename)
        {
            var data = File.ReadAllText(filename);
            return ImportData(data);
        }

        public static IEnumerable<Resource> ImportZip(string filename)
        {
            return File.ReadAllBytes(filename)
                .ExtractZipEntries().SelectMany(ImportData);
        }
    }
}