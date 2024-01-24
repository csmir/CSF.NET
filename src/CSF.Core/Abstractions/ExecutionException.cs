﻿namespace CSF
{
    public abstract class ExecutionException : Exception
    {
        public ExecutionException(string message)
            : base(message)
        {

        }

        public ExecutionException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
