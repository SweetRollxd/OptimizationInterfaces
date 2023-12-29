namespace OptimizationInterfaces
{
    public class UnsupportedFuncationalException : Exception
    {
        public UnsupportedFuncationalException()
        {
        }

        public UnsupportedFuncationalException(string message)
            : base(message)
        {
        }

        public UnsupportedFuncationalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}