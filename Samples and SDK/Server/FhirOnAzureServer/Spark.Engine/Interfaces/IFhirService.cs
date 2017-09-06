#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirService.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Core.Interfaces
{
    /*
    public enum FhirEvent { Create, Read, Update, Delete }

    public interface FhirSubscription
    {
        public void FhirService(Uri endpoint); // Constructor
        public Response Read(Key key);
        public Response VRead(Key key);
        public Response Create(Key key, Resource resource);
        public Bundle Search(string collection, IEnumerable<Tuple<string, string>> parameters, int pageSize, string sortby);
        public Response Update(Key key, Resource resource);
        public Response Upsert(Key key, Resource resource);
        public Response Delete(Key key);
        public Bundle Transaction(Bundle bundle);
        public Bundle History(DateTimeOffset? since, string sortby);
        public Bundle History(string collection, DateTimeOffset? since, string sortby);
        public Response History(Key key, DateTimeOffset? since, string sortby);
        public Bundle Mailbox(Bundle bundle, Binary body);
        public OperationOutcome Validate(Resource resource);
        public Response Validate(Key key, Resource resource);
    }

    public interface IFhirService
    {
        public void FhirService(Uri endpoint); // Constructor
        public Response Read(Key key);
        public Response VRead(Key key);
        public Response Create(Key key, Resource resource);
        public Bundle Search(string collection, IEnumerable<Tuple<string, string>> parameters, int pageSize, string sortby);
        public Response Update(Key key, Resource resource);
        public Response Upsert(Key key, Resource resource);
        public Response Delete(Key key);
        public Bundle Transaction(Bundle bundle);
        public Bundle History(DateTimeOffset? since, string sortby);
        public Bundle History(string collection, DateTimeOffset? since, string sortby);
        public Response History(Key key, DateTimeOffset? since, string sortby);
        public Bundle Mailbox(Bundle bundle, Binary body);
        public OperationOutcome Validate(Resource resource);
        public Response Validate(Key key, Resource resource);
        public CapabilityStatement CapabilityStatement();
        public Bundle GetSnapshot(string snapshotkey, int index, int count);
    }
    */
}