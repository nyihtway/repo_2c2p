namespace _2C2P.DEMO.Infrastructure.Responses
{
    public class OkResponse<T> : Response<T>
    {
        public OkResponse(T data) : base(data)
        {
        }
    }
}
