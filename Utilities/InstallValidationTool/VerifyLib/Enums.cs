namespace VerifyLib
{
    public enum VerifyType
    {
        NotDefined,
        Registry,
        File,
        Version,
        Uninstall,
        Database,
        COM,
        DCOM,
        WinService,
        Installation,
        EventLog,
        GACAssembly,
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
