using System;

namespace GraphieDb.Core.Exceptions
{
    public class GraphieDbException : Exception
    {
        public GraphieDbException(string message): base(message)
        {
        }
    }
}
