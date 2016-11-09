using System;

namespace ES1Common.Exceptions
{
    /// <summary>
    /// Exception base class
    /// </summary>
    public abstract class AutomationException : ApplicationException
    {
        public AutomationException(string message)
            : base(message)
        {
        }

        public AutomationException(string module, string message)
            : base(string.Format("{0}:{1}", module, message))
        {
        }

        public AutomationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AutomationException(string module, string message, Exception innerException)
            : base(string.Format("{0}:{1}", module, message), innerException)
        {
        }
    }
}
