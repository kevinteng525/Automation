using ES1Common.Logs;

namespace ATFExchangeCentre.Agents
{
    public abstract class Agent
    {
        protected static AutomationLog Log = new AutomationLog("Agent");

        public abstract void StartAgent();

        public abstract void StopAgent();
    }
}
