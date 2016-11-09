using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ES1Common.Exceptions
{
    /// <summary>
    /// Exceptions in Automation Framework
    /// </summary>
    public class FrameworkException : AutomationException
    {
        public FrameworkException(string module, string message)
            : base(module, message)
        {
        }

        public FrameworkException(string module, string message, Exception innerException)
            : base(module, message, innerException)
        {
        }
    }
}
