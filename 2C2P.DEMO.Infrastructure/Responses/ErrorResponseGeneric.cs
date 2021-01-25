using _2C2P.DEMO.Infrastructure.Responses.Errors;
using System.Collections.Generic;

namespace _2C2P.DEMO.Infrastructure.Responses
{
    public class ErrorResponse<T> : Response<T>
    {
        public ErrorResponse(Error error) : base(new List<Error> { error })
        {

        }

        public ErrorResponse(List<Error> errors) : base(errors)
        {

        }
    }
}
