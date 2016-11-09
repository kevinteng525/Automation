using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ES1Common.Exceptions;

namespace ES1Common.Exceptions
{
    public class EnvironmentException : AutomationException
    {
        public EnvironmentException(string module, string message)
            : base(module, message)
        {
        }

        public EnvironmentException(string module, string message, Exception innerException)
            : base(module, message, innerException)
        {
        }
    }
}
