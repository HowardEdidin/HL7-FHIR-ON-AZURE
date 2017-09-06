#region Information

// Solution:  Spark
// Spark.Engine
// File:  SearchResults.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using FhirOnAzure.Search;
    using Hl7.Fhir.Model;

    public class SearchResults : List<string>
    {
        private readonly OperationOutcome outcome;


        // todo: I think OperationOutcome logic should be on a higher level or at least not SearchResults specific -mh
        public SearchResults()
        {
            UsedCriteria = new List<Criterium>();
            MatchCount = 0;
            outcome = new OperationOutcome();
            outcome.Issue = new List<OperationOutcome.IssueComponent>();
        }

        public List<Criterium> UsedCriteria { get; set; }
        public int MatchCount { get; set; }

        public OperationOutcome Outcome => outcome.Issue.Any() ? outcome : null;

        public bool HasErrors
        {
            get
            {
                return Outcome != null && Outcome.Issue.Any(i => i.Severity <= OperationOutcome.IssueSeverity.Error);
            }
        }

        public bool HasIssues => Outcome != null && Outcome.Issue.Any();

        public string UsedParameters
        {
            get
            {
                var used = UsedCriteria.Select(c => c.ToString()).ToArray();
                return string.Join("&", used);
            }
        }

        public void AddIssue(string errorMessage,
            OperationOutcome.IssueSeverity severity = OperationOutcome.IssueSeverity.Error)
        {
            var newIssue = new OperationOutcome.IssueComponent {Diagnostics = errorMessage, Severity = severity};
            outcome.Issue.Add(newIssue);
        }
    }

    //public static class UriListExtentions
    //{
    //public static bool SameAs(this ResourceIdentity a, ResourceIdentity b)
    //{
    //    if (a.ResourceType == b.ResourceType && a.Id == b.Id)
    //    {
    //        if (a.VersionId == b.VersionId || a.VersionId == null || b.VersionId == null)
    //            return true;
    //    }
    //    return false;
    //}
    //public static bool Has(this SearchResults list, Uri uri)
    //{
    //    foreach (Uri item in list)
    //    {
    //        //if (item.ToString() == uri.ToString())
    //        ResourceIdentity a = new ResourceIdentity(item);
    //        ResourceIdentity b = new ResourceIdentity(uri);
    //        if (a.SameAs(b))
    //            return true;

    //    }
    //    return false;
    //}
    //public static bool Has(this SearchResults list, string s)
    //{
    //    //var uri = new Uri(s, UriKind.Relative);
    //    //var uri = new Uri(s, UriKind.RelativeOrAbsolute);
    //    return list.Contains(s);
    //}
    //}
}