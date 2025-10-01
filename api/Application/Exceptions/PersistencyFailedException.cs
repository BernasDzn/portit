namespace Api.Application.Exceptions
{
    public class PersistencyFailedException : Exception
    {
        public PersistencyFailedException()
        {
        }

        public PersistencyFailedException(string message)
            : base(message)
        {
        }

        public PersistencyFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}