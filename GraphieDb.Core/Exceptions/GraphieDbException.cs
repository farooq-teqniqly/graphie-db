using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphieDb.Core.Exceptions
{
    public class GraphieDbException : Exception
    {
        public GraphieDbException(string message): base(message)
        {
        }
    }
}
