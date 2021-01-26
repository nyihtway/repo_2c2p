using _2C2P.DEMO.Infrastructure.Responses.Errors;
using System.Collections.Generic;

namespace _2C2P.DEMO.Infrastructure.Responses
{
    public abstract class Response<T> : Response
    {
        protected Response(T data)
        {
            Data = data;
        }

        protected Response(List<Error> errors) : base(errors)
        {
        }

        public T Data { get; private set; }
    }
}
