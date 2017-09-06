#region Information

// Solution:  Spark
// Spark.Engine
// File:  SearchParamType.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Engine.Search.Model
{
    using System.Collections.Generic;

    public abstract class SearchParamType
    {
        public abstract Hl7.Fhir.Model.SearchParamType SupportsType { get; }

        public abstract bool ModifierIsAllowed(ActualModifier modifier);
    }

    public class SearchParamTypeNumber : SearchParamType
    {
        protected static List<Modifier> allowedModifiers = new List<Modifier> {Modifier.NONE, Modifier.MISSING};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Number;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeDate : SearchParamType
    {
        protected static List<Modifier> allowedModifiers = new List<Modifier> {Modifier.NONE, Modifier.MISSING};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Date;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeString : SearchParamType
    {
        protected static List<Modifier> allowedModifiers =
            new List<Modifier> {Modifier.NONE, Modifier.MISSING, Modifier.EXACT, Modifier.CONTAINS};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.String;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeToken : SearchParamType
    {
        protected static List<Modifier> allowedModifiers =
            new List<Modifier>
            {
                Modifier.NONE,
                Modifier.MISSING,
                Modifier.TEXT,
                Modifier.IN,
                Modifier.BELOW,
                Modifier.ABOVE,
                Modifier.NOT_IN
            };

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Token;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeReference : SearchParamType
    {
        protected static List<Modifier> allowedModifiers =
            new List<Modifier> {Modifier.NONE, Modifier.MISSING, Modifier.TYPE};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Reference;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeComposite : SearchParamType
    {
        protected static List<Modifier> allowedModifiers = new List<Modifier> {Modifier.NONE};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Composite;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeQuantity : SearchParamType
    {
        protected static List<Modifier> allowedModifiers = new List<Modifier> {Modifier.NONE, Modifier.MISSING};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Quantity;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }

    public class SearchParamTypeUri : SearchParamType
    {
        protected static List<Modifier> allowedModifiers =
            new List<Modifier> {Modifier.NONE, Modifier.MISSING, Modifier.BELOW, Modifier.ABOVE};

        public override Hl7.Fhir.Model.SearchParamType SupportsType => Hl7.Fhir.Model.SearchParamType.Uri;

        public override bool ModifierIsAllowed(ActualModifier modifier)
        {
            return allowedModifiers.Contains(modifier.Modifier);
        }
    }
}