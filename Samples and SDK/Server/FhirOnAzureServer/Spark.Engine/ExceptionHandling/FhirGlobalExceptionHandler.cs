#region Information

// Solution:  Spark
// Spark.Engine
// File:  FhirGlobalExceptionHandler.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:51 PM

#endregion

namespace FhirOnAzure.Engine.ExceptionHandling
{
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Results;

    public class FhirGlobalExceptionHandler : ExceptionHandler
    {
        private readonly IExceptionResponseMessageFactory _exceptionResponseMessageFactory;

        public FhirGlobalExceptionHandler(IExceptionResponseMessageFactory exceptionResponseMessageFactory)
        {
            _exceptionResponseMessageFactory = exceptionResponseMessageFactory;
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            var responseMessage = _exceptionResponseMessageFactory.GetResponseMessage(context.Exception,
                context.Request);
            context.Result = new ResponseMessageResult(responseMessage);
        }
    }
}