using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TLSharp.Core;

namespace Odnogruppniki.Core
{
    public class SessionStoreFake : ISessionStore
    {
        public Session Load(string sessionUserId)
        {
            return null;
        }

        public void Save(Session session)
        {
        }
    }
}