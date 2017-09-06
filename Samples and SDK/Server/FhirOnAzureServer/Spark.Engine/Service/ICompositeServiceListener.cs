using FhirOnAzure.Engine.Core;
using FhirOnAzure.Service;

namespace FhirOnAzure.Engine.Service
{
    public interface ICompositeServiceListener : IServiceListener
    {
        void Add(IServiceListener listener);
        void Clear();
        void Inform(Entry interaction);
    }
}