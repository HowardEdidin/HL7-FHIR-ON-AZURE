#region Information

// Solution:  Spark
// FhirOnAzure
// File:  Startup.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:45 PM

#endregion

namespace FhirOnAzure
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}