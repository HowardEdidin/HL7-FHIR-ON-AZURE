using System.Net.Http.Headers;

namespace FhirOnAzure.Engine.Extensions
{
    public static class ETag
    {
        public static EntityTagHeaderValue Create(string value)
        {
            string tag = "\"" + value + "\"";
            return new EntityTagHeaderValue(tag, true);
        }
    }
}
