using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EvernoteSDK;
using EvernoteSDK.Advanced;
using Evernote.EDAM.Type;

namespace EvernoteReadOnlyTags
{
    class EvernoteConnection
    {
        private static ENSession session;

        public static void Create()
        {
            const string DEV_TOKEN = @"S=s1:U=90a30:E=153e3c3aab4:C=14c8c127c60:P=1cd:A=en-devtoken:V=2:H=d8d13e0d201c83db041872b2f5a9f691";

            string url = System.Configuration.ConfigurationManager.AppSettings["URL"];

            ENSessionAdvanced.SetSharedSessionDeveloperToken(DEV_TOKEN, url);
            if (ENSession.SharedSession.IsAuthenticated == false)
            {
                ENSession.SharedSession.AuthenticateToEvernote();
            }

            session = ENSession.SharedSession;

            EvernoteReadOnlyTagsException.Assert(ENSession.SharedSession.IsAuthenticated, "Authentication failed");
        }


        public static ENSession CurrentSession
        {
            get
            {
                EvernoteReadOnlyTagsException.Assert(ENSession.SharedSession.IsAuthenticated, "Not connected");
                return session;
            }
        }
    }
}
