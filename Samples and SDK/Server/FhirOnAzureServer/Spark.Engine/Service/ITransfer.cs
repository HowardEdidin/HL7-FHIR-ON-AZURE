using System.Collections.Generic;
using FhirOnAzure.Engine.Core;

namespace FhirOnAzure.Service
{
    public interface ITransfer
    {
        void Externalize(IEnumerable<Entry> interactions);
        void Externalize(Entry interaction);
        void Internalize(IEnumerable<Entry> interactions, Mapper<string, IKey> mapper);
        void Internalize(Entry entry);
    }
}