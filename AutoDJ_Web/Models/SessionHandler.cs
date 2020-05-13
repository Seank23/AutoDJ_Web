using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public static class SessionHandler
    {
        public static int CreateSessionId()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        public static string CreateUserId()
        {
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
