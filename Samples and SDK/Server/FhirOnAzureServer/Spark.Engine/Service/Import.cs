#region Information

// Solution:  Spark
// Spark.Engine
// File:  Import.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:00 PM

#endregion

namespace FhirOnAzure.Service
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Xml.Linq;
    using Core;
    using Engine.Auxiliary;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Model;
    using ResourceVisitor = Engine.Auxiliary.ResourceVisitor;

    /// <summary>
    ///     Import can map id's and references in incoming entries to id's and references that are local to the FhirOnAzure
    ///     Server.
    /// </summary>
    internal class Import
    {
        private readonly List<Entry> entries;
        private readonly IGenerator generator;
        private readonly ILocalhost localhost;
        private readonly Mapper<string, IKey> mapper;

        public Import(ILocalhost localhost, IGenerator generator)
        {
            this.localhost = localhost;
            this.generator = generator;
            mapper = new Mapper<string, IKey>();
            entries = new List<Entry>();
        }

        public void Add(Entry interaction)
        {
            if (interaction != null && interaction.State == EntryState.Undefined)
                entries.Add(interaction);
        }

        public void AddMappings(Mapper<string, IKey> mappings)
        {
            mapper.Merge(mappings);
        }

        public void Add(IEnumerable<Entry> interactions)
        {
            foreach (var interaction in interactions)
                Add(interaction);
        }

        public void Internalize()
        {
            InternalizeKeys();
            InternalizeReferences();
            InternalizeState();
        }

        private void InternalizeState()
        {
            foreach (var interaction in entries.Transferable())
                interaction.State = EntryState.Internal;
        }

        private void InternalizeKeys()
        {
            foreach (var interaction in entries.Transferable())
                InternalizeKey(interaction);
        }

        private void InternalizeReferences()
        {
            foreach (var entry in entries.Transferable())
                InternalizeReferences(entry.Resource);
        }

        private IKey Remap(Resource resource)
        {
            var newKey = generator.NextKey(resource).WithoutBase();
            AddKeyToInternalMapping(resource.ExtractKey(), newKey);
            return newKey;
        }

        private IKey RemapHistoryOnly(IKey key)
        {
            IKey newKey = generator.NextHistoryKey(key).WithoutBase();
            AddKeyToInternalMapping(key, newKey);
            return newKey;
        }

        private void AddKeyToInternalMapping(IKey localKey, IKey generatedKey)
        {
            if (localhost.GetKeyKind(localKey) == KeyKind.Temporary)
                mapper.Remap(localKey.ResourceId, generatedKey.WithoutVersion());
            else
                mapper.Remap(localKey.ToString(), generatedKey.WithoutVersion());
        }

        private void InternalizeKey(Entry entry)
        {
            var key = entry.Key;

            switch (localhost.GetKeyKind(key))
            {
                case KeyKind.Foreign:
                {
                    entry.Key = Remap(entry.Resource);
                    return;
                }
                case KeyKind.Temporary:
                {
                    entry.Key = Remap(entry.Resource);
                    return;
                }
                case KeyKind.Local:
                case KeyKind.Internal:
                {
                    if (entry.Method == Bundle.HTTPVerb.PUT || entry.Method == Bundle.HTTPVerb.DELETE)
                        entry.Key = RemapHistoryOnly(key);
                    else if (entry.Method == Bundle.HTTPVerb.POST)
                        entry.Key = Remap(entry.Resource);
                    return;
                }
                default:
                {
                    // switch can never get here.
                    throw Error.Internal("Unexpected key for resource: " + entry.Key);
                }
            }
        }

        private void InternalizeReferences(Resource resource)
        {
            Visitor action = (element, name) =>
            {
                if (element == null) return;

                if (element is ResourceReference)
                {
                    var reference = (ResourceReference) element;
                    reference.Url = InternalizeReference(reference.Url);
                }
                else if (element is FhirUri)
                {
                    var uri = (FhirUri) element;
                    uri.Value = InternalizeReference(uri.Value);
                    //((FhirUri)element).Value = LocalizeReference(new Uri(((FhirUri)element).Value, UriKind.RelativeOrAbsolute)).ToString();
                }
                else if (element is Narrative)
                {
                    var n = (Narrative) element;
                    n.Div = FixXhtmlDiv(n.Div);
                }
            };

            Type[] types = {typeof(ResourceReference), typeof(FhirUri), typeof(Narrative)};

            ResourceVisitor.VisitByType(resource, action, types);
        }

        private IKey InternalizeReference(IKey localkey)
        {
            var triage = localhost.GetKeyKind(localkey);
            if (triage == KeyKind.Foreign) throw new ArgumentException("Cannot internalize foreign reference");

            if (triage == KeyKind.Temporary)
                return GetReplacement(localkey);
            if (triage == KeyKind.Local)
                return localkey.WithoutBase();
            return localkey;
        }

        private IKey GetReplacement(IKey localkey)
        {
            var replacement = localkey;
            //CCR: To check if this is still needed. Since we don't store the version in the mapper, do we ever need to replace the key multiple times? 
            while (mapper.Exists(replacement.ResourceId))
            {
                var triage = localhost.GetKeyKind(localkey);
                if (triage == KeyKind.Temporary)
                    replacement = mapper.TryGet(replacement.ResourceId);
                else
                    replacement = mapper.TryGet(replacement.ToString());
            }

            if (replacement != null)
                return replacement;
            throw Error.Create(HttpStatusCode.Conflict,
                "This reference does not point to a resource in the server or the current transaction: {0}", localkey);
        }

        private Uri InternalizeReference(Uri uri)
        {
            if (uri == null) return uri;

            // If it is a reference to another contained resource do not internalize.
            // BALLOT: this seems very... ad hoc. 
            if (uri.HasFragment()) return uri;

            if (uri.IsTemporaryUri() || localhost.IsBaseOf(uri))
            {
                IKey key = localhost.UriToKey(uri);
                return InternalizeReference(key).ToUri();
            }
            return uri;
        }

        private string InternalizeReference(string uristring)
        {
            if (string.IsNullOrWhiteSpace(uristring)) return uristring;

            var uri = new Uri(uristring, UriKind.RelativeOrAbsolute);
            return InternalizeReference(uri).ToString();
        }

        private string FixXhtmlDiv(string div)
        {
            try
            {
                var xdoc = XDocument.Parse(div);
                xdoc.VisitAttributes("img", "src", n => n.Value = InternalizeReference(n.Value));
                xdoc.VisitAttributes("a", "href", n => n.Value = InternalizeReference(n.Value));
                return xdoc.ToString();
            }
            catch
            {
                // illegal xml, don't bother, just return the argument
                // todo: should we really allow illegal xml ? /mh
                return div;
            }
        }
    }
}