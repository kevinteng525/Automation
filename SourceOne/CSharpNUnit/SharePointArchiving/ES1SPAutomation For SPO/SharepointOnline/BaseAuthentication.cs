using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;

namespace SharepointOnline
{
    abstract class BaseAuthentication:IAuthentication, IDisposable
    {
        public ClientContext clientContext;
        public abstract void Authenticate();
        public BaseAuthentication(string siteURL)
        {
            clientContext = new ClientContext(siteURL);
        }
        public void Close()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            if (null != clientContext)
                clientContext.Dispose();
        }
    }
}
