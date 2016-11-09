using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATFExchangeCentre
{
    public enum AgentType
    {
        SocketServer,
        WebService,
    }

    public enum VerifyResult
    {
        Pass,
        Failed,
        Warning,
        Skip,
        Unknow,
    }

    public enum ResultView
    {
        All,
        PassOnly,
        FailOnly,
    }
}
