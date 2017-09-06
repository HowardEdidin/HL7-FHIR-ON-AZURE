#region Information

// Solution:  Spark
// Spark.Engine
// File:  Interaction.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using Hl7.Fhir.Model;

    public enum EntryState
    {
        Internal,
        Undefined,
        External
    }

    public class Entry
    {
        private IKey _key;
        private DateTimeOffset? _when;

        protected Entry(Bundle.HTTPVerb method, IKey key, DateTimeOffset? when, Resource resource)
        {
            if (resource != null)
                key.ApplyTo(resource);
            else
                Key = key;
            Resource = resource;
            Method = method;
            When = when ?? DateTimeOffset.Now;
            State = EntryState.Undefined;
        }

        public IKey Key
        {
            get
            {
                if (Resource != null)
                    return Resource.ExtractKey();
                return _key;
            }
            set
            {
                if (Resource != null)
                    value.ApplyTo(Resource);
                else
                    _key = value;
            }
        }

        public Resource Resource { get; set; }

        public Bundle.HTTPVerb Method { get; set; }

        // API: HttpVerb should not be in Bundle.
        public DateTimeOffset? When
        {
            get
            {
                if (Resource != null && Resource.Meta != null)
                    return Resource.Meta.LastUpdated;
                return _when;
            }
            set
            {
                if (Resource != null)
                {
                    if (Resource.Meta == null) Resource.Meta = new Meta();
                    Resource.Meta.LastUpdated = value;
                }
                else
                {
                    _when = value;
                }
            }
        }

        public EntryState State { get; set; }

        public bool IsDelete
        {
            get => Method == Bundle.HTTPVerb.DELETE;
            set
            {
                Method = Bundle.HTTPVerb.DELETE;
                Resource = null;
            }
        }

        public bool IsPresent => Method != Bundle.HTTPVerb.DELETE;


        public static Entry Create(Bundle.HTTPVerb method, Resource resource)
        {
            return new Entry(method, null, null, resource);
        }

        public static Entry Create(Bundle.HTTPVerb method, IKey key, Resource resource)
        {
            return new Entry(method, key, null, resource);
        }

        public static Entry Create(Bundle.HTTPVerb method, IKey key, DateTimeOffset when)
        {
            return new Entry(method, key, when, null);
        }

        /// <summary>
        ///     Creates a deleted entry
        /// </summary>
        public static Entry DELETE(IKey key, DateTimeOffset? when)
        {
            return Create(Bundle.HTTPVerb.DELETE, key, DateTimeOffset.UtcNow);
        }

        public static Entry POST(IKey key, Resource resource)
        {
            return Create(Bundle.HTTPVerb.POST, key, resource);
        }

        public static Entry POST(Resource resource)
        {
            return Create(Bundle.HTTPVerb.POST, resource);
        }

        public static Entry PUT(IKey key, Resource resource)
        {
            return Create(Bundle.HTTPVerb.PUT, key, resource);
        }

        //public static Interaction GET(IKey key)
        //{
        //    return new Interaction(Bundle.HTTPVerb.GET, key, null, null);
        //}

        public override string ToString()
        {
            return string.Format("{0} {1}", Method, Key);
        }
    }
}