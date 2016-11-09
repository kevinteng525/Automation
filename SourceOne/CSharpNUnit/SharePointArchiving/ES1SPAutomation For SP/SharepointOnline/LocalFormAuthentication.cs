using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;

namespace SharepointOnline
{
    class LocalFormAuthentication : BaseAuthentication
    {
        private User _currentUser;

        public LocalFormAuthentication(User targetUser, string siteURL):base(siteURL)
        {
            _currentUser = targetUser;
        }

        public override void Authenticate()
        {
            clientContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
            clientContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(_currentUser.UserName, _currentUser.Password);
        }
    }
}
