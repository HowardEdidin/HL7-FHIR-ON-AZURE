#region Information

// Solution:  Spark
// FhirOnAzure
// File:  InitializerHub.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:43 PM

#endregion

namespace FhirOnAzure.Import
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;
    using Engine.Core;
    using Engine.Interfaces;
    using Hl7.Fhir.Model;
    using Microsoft.AspNet.SignalR;
    using Service;

    public class ImportProgressMessage
    {
        public string Message;
        public int Progress;
    }

    public class InitializerHub : Hub
    {
        private readonly IFhirIndex _fhirIndex;

        private readonly IFhirService _fhirService;
        private readonly IFhirStoreAdministration _fhirStoreAdministration;
        private const int LimitPerType = 0; //0 for no limit at all.
        private readonly ILocalhost _localhost;

        private int _progress;

        private int _resourceCount;

        private List<Resource> _resources;

        public InitializerHub(IFhirService fhirService, ILocalhost localhost,
            IFhirStoreAdministration fhirStoreAdministration, IFhirIndex fhirIndex)
        {
            _localhost = localhost;
            _fhirService = fhirService;
            _fhirStoreAdministration = fhirStoreAdministration;
            _fhirIndex = fhirIndex;
            _resources = null;
        }

        public List<Resource> GetExampleData()
        {
            var list = new List<Resource>();

            var data = Examples.ImportEmbeddedZip().ToBundle(_localhost.DefaultBase);

            if (data.Entry != null && data.Entry.Count() != 0)
                list.AddRange(from entry in data.Entry where entry.Resource != null select entry.Resource);
            return list;
        }

        private void Progress(string message, int progress)
        {
            _progress = progress;

            var msg = new ImportProgressMessage
            {
                Message = message,
                Progress = progress
            };

            Clients.Caller.sendMessage(msg);
        }

        private void Progress(string message)
        {
            Progress(message, _progress);
        }

        private ImportProgressMessage Message(string message, int idx)
        {
            var msg = new ImportProgressMessage
            {
                Message = message,
                Progress = 10 + (idx + 1) * 90 / _resourceCount
            };
            return msg;
        }

        public void LoadData()
        {
            var messages = new StringBuilder();
            messages.AppendLine("Import completed!");
            try
            {
                //cleans store and index
                Progress("Clearing the database...", 0);
                _fhirStoreAdministration.Clean();
                _fhirIndex.Clean();

                Progress("Loading examples data...", 5);
                _resources = GetExampleData();

                var resarray = _resources.ToArray();
                _resourceCount = resarray.Count();

                for (var x = 0; x <= _resourceCount - 1; x++)
                {
                    var res = resarray[x];
                    // Sending message:
                    var msg = Message("Importing " + res.ResourceType + " " + res.Id + "...", x);
                    Clients.Caller.sendMessage(msg);

                    try
                    {
                        //Thread.Sleep(1000);
                        var key = res.ExtractKey();

                        if (!string.IsNullOrEmpty(res.Id))
                            _fhirService.Put(key, res);
                        else
                            _fhirService.Create(key, res);
                    }
                    catch (Exception e)
                    {
                        // Sending message:
                        var msgError = Message("ERROR Importing " + res.ResourceType + " " + res.Id + "... ", x);
                        Clients.Caller.sendMessage(msg);
                        messages.AppendLine(msgError.Message + ": " + e.Message);
                    }
                }

                Progress(messages.ToString(), 100);
            }
            catch (Exception e)
            {
                Progress("Error: " + e.Message);
            }
        }
    }
}