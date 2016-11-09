using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequestLib.Requests;

namespace RequestLib.Agents
{
    public abstract class Agent
    {
        protected Request Request { get; private set; }
    }
}
