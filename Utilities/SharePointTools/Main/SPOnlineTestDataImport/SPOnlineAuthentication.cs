using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Principal;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.Xml;

using Microsoft.SharePoint.Client;

namespace ES1OnlineTestDataImport
{
    class SPOnlineAuthentication:BaseAuthentication
    {
        private User _currentUser;
        private string _siteURL;
        MsOnlineClaimsHelper.MsOnlineClaimsHelper claimsHelper;
        public SPOnlineAuthentication(User targetUser, string siteURL)
            : base(siteURL)
        {
            _currentUser = targetUser;
            this._siteURL = siteURL;
        }

        public override void Authenticate()
        {
            claimsHelper = new MsOnlineClaimsHelper.MsOnlineClaimsHelper(_currentUser.UserName, _currentUser.Password, this._siteURL);
            this.clientContext.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(clientContext_ExecutingWebRequest);
        }

        void clientContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            e.WebRequestExecutor.WebRequest.CookieContainer = claimsHelper.CookieContainer;
        }        
    }
}
