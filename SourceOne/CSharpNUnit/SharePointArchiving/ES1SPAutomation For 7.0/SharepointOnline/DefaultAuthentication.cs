using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Net;

namespace SharepointOnline
{
    class DefaultAuthentication : BaseAuthentication
    {
        private User _currentUser;
        private string _siteURL;

        public DefaultAuthentication(User targetUser, string siteURL):base(siteURL)
        {
            _currentUser = targetUser;
            _siteURL = siteURL;
        }

        public override void Authenticate()
        {
            clientContext.AuthenticationMode = ClientAuthenticationMode.Default;
            string[] userNames = _currentUser.UserName.Split(new char[]{'\\'});
            NetworkCredential credential = new NetworkCredential(userNames[1], _currentUser.Password, userNames[0]);
            clientContext.Credentials = credential;

        }
    }
}
