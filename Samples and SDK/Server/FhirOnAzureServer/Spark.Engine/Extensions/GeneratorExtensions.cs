using Hl7.Fhir.Model;
using FhirOnAzure.Core;

namespace FhirOnAzure.Engine.Extensions
{
    public static class Format
    {
        public static string RESOURCEID = "azure{0}";
        public static string VERSIONID = "azure{0}";
    }

    public static class GeneratorExtensions
    {
        public static string NextResourceId(this IGenerator generator, Resource resource)
        {
            return generator.NextResourceId(resource);
        }

        public static string NextVersionId(this IGenerator generator, Resource resource)
        {

            return generator.NextVersionId(resource.TypeName);
        }

    }


}
