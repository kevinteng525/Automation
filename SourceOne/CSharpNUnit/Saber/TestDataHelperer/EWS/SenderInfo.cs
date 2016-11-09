using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using Microsoft.Exchange.WebServices.Data;

using Saber.TestEnvironment;

namespace Saber.TestData.EWS
{
    public interface IUserData
    {
        ExchangeVersion Version { get; }
        string EmailAddress { get; }
        String Password { get; }
        Uri AutodiscoverUrl { get; set; }
    }
    public class SenderInfo : IUserData
    {
        public SenderInfo(string email, string pwd)
        {
            EmailAddress = email;
            Password = pwd;
            Version = GetExchangeVersion();
            AutodiscoverUrl = new Uri("https://" + TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.ExchangeServer).First().FullName + @"/ews/exchange.asmx");
        }
        public String EmailAddress
        {
            get;
            private set;
        }

        public String Password
        {
            get;
            private set;
        }

        public Uri AutodiscoverUrl
        {
            get;
            set;
        }
        public ExchangeVersion Version
        {
            get;
            private set;
        }
        public ExchangeVersion GetExchangeVersion()
        {
            ExchangeVersion exServerVersion = ExchangeVersion.Exchange2007_SP1;
            List<S1ComponentHost> exchangeServers = TestEnvironmentHelper.GetHostsWithS1ComponentType(S1HostType.ExchangeServer);
            if (exchangeServers.Count > 0)
            {
                S1ExchangeServerConfig config = exchangeServers[0].GetConfigOfType(S1HostType.ExchangeServer) as S1ExchangeServerConfig;
                switch (config.ExchangeVersion)
                {
                    case @"Exchange2007_SP1":
                        exServerVersion = ExchangeVersion.Exchange2007_SP1;
                        break;
                    case @"Exchange2010":
                        exServerVersion = ExchangeVersion.Exchange2010;
                        break;
                    case @"Exchange2010_SP1":
                        exServerVersion = ExchangeVersion.Exchange2010_SP1;
                        break;
                    case @"Exchange2010_SP2":
                        exServerVersion = ExchangeVersion.Exchange2010_SP2;
                        break;
                    case @"Exchange2013":
                        exServerVersion = ExchangeVersion.Exchange2013;
                        break;
                    default:
                        exServerVersion = ExchangeVersion.Exchange2007_SP1;
                        break;
                }
                return exServerVersion;
            }
            else
            {
                //TODO
                throw new Exception();
            }
        }
    }
}
