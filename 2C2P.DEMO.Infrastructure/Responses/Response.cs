using _2C2P.DEMO.Infrastructure.Responses.Errors;
using System.Collections.Generic;

namespace _2C2P.DEMO.Infrastructure.Responses
{
    public abstract class Response
    {
        protected Response()
        {
            Errors = new List<Error>();
        }

        protected Response(List<Error> errors)
        {
            Errors = errors ?? new List<Error>();
        }

        public string Message { get; } = "Backend Validation Failed";
        public List<Error> Errors { get; }
        public bool IsError => Errors.Count > 0;

        public object FormatErrors()
        {
            return new { Message, Errors };
        }
    }
}
