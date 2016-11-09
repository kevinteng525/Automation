using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Model;

namespace SaberAgent.WindowsFormApp
{
    public abstract class SaberTestExecutionHandler
    {
        protected AutomationJob Job2Run;
        protected string RootResultPath;
        
        public abstract void RunTestCase(TestCaseExecution execution);
    }
}
