namespace RequestLib
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
