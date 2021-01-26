namespace _2C2P.DEMO.Infrastructure.Responses.Errors
{
    public abstract class Error
    {
        protected Error(string message, string code)
        {
            Message = message;
            Code = code;
        }

        public string Code { get; private set; }
        public string Message { get; private set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
