#region Information

// Solution:  Spark
// Spark.Engine
// File:  ServiceListener.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:59 PM

#endregion

namespace FhirOnAzure.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Core;
    using Engine.Service;

    public class ServiceListener : IServiceListener, ICompositeServiceListener
    {
        private readonly List<IServiceListener> listeners;
        private readonly ILocalhost localhost;

        public ServiceListener(ILocalhost localhost, IServiceListener[] listeners = null)
        {
            this.localhost = localhost;
            this.listeners = new List<IServiceListener>(listeners.AsEnumerable());
        }

        public void Add(IServiceListener listener)
        {
            listeners.Add(listener);
        }

        public void Clear()
        {
            listeners.Clear();
        }

        public void Inform(Entry interaction)
        {
            // todo: what we want is not to send localhost to the listener, but to add the Resource.Base. But that is not an option in the current infrastructure.
            // It would modify interaction.Resource, while 
            foreach (var listener in listeners)
            {
                var location = localhost.GetAbsoluteUri(interaction.Key);
                Inform(listener, location, interaction);
            }
        }

        public void Inform(Uri location, Entry entry)
        {
            foreach (var listener in listeners)
                Inform(listener, location, entry);
        }

        private void Inform(IServiceListener listener, Uri location, Entry entry)
        {
            listener.Inform(location, entry);
        }
    }
}