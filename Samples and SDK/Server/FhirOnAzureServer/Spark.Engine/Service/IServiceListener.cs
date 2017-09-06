using System;
using FhirOnAzure.Engine.Core;

namespace FhirOnAzure.Service
{
    public interface IServiceListener
    {
        void Inform(Uri location, Entry interaction);
    }

}
