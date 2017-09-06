#region Information

// Solution:  Spark
// FhirOnAzure
// File:  UnityLogExtension.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:40 PM

#endregion

namespace FhirOnAzure
{
    using System.Diagnostics;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    public class UnityLogExtension : UnityContainerExtension, IBuilderStrategy
    {
        void IBuilderStrategy.PostBuildUp(IBuilderContext context)
        {
            //Type type = context.Existing == null ? context.BuildKey.Type : context.Existing.GetType();
            //Debug.WriteLine("Builded up: " + type.Name);
        }

        void IBuilderStrategy.PostTearDown(IBuilderContext context)
        {
        }

        void IBuilderStrategy.PreBuildUp(IBuilderContext context)
        {
            var type = context.Existing?.GetType() ?? context.BuildKey.Type;
            Debug.WriteLine("Building up: " + type.Name);
        }

        void IBuilderStrategy.PreTearDown(IBuilderContext context)
        {
        }

        protected override void Initialize()
        {
            Debug.WriteLine("UnityLogExtension initializing");
            Context.Strategies.Add(this, UnityBuildStage.PreCreation);
        }
    }
}