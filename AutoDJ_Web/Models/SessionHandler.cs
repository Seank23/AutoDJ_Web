using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public static class SessionHandler
    {
        public static List<SessionModel> Sessions { get; set; }
        private static List<string> SessionIdList { get; set; }
        private static Dictionary<string, DateTime> UserIdDict { get; set; }
        private static Timer pruneTimer;

        static SessionHandler()
        {
            Sessions = new List<SessionModel>();
            SessionIdList = new List<string>();
            UserIdDict = new Dictionary<string, DateTime>();

            pruneTimer = new Timer(300000); // 5 mins
            pruneTimer.Elapsed += PruneUsers;
            pruneTimer.Enabled = true;
            pruneTimer.Start();
        }

        private static void PruneUsers(object sender, ElapsedEventArgs e)
        {
            foreach(string user in UserIdDict.Keys)
            {
                if (UserIdDict[user] < DateTime.Now - TimeSpan.FromMinutes(30))
                {
                    LeaveSession(GetUsersSession(user), user);
                    UserIdDict.Remove(user);
                }
            }
        }

        private static string CreateSessionId()
        {
            Random rnd = new Random();
            string sessionId = "";
            while(SessionIdList.Contains(sessionId) || sessionId == "")
                sessionId = rnd.Next(100000, 999999).ToString();
            SessionIdList.Add(sessionId);
            return sessionId;
        }

        private static string CreateUserId()
        {
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string userId = "";
            while (UserIdDict.Keys.Contains(userId) || userId == "")
                userId = new string(Enumerable.Repeat(chars, 8).Select(s => s[rnd.Next(s.Length)]).ToArray());
            UserIdDict.Add(userId, DateTime.Now);
            return userId;
        }

        public static string CreateSession()
        {
            string sessionId = CreateSessionId();
            Sessions.Add(new SessionModel(sessionId));
            return sessionId;
        }

        public static string CreateAndAddUser(string sessionId)
        {
            string userId = CreateUserId();
            Sessions.Where(session => session.SessionID == sessionId).First().Users.Add(userId);
            return userId;
        }

        public static void LeaveSession(string sessionId, string userId)
        {
            if (GetUsersSession(userId) == sessionId)
            {
                Sessions.Where(session => session.SessionID == sessionId).First().Users.Remove(userId);
                UserIdDict.Remove(userId);

                if (Sessions.Where(session => session.SessionID == sessionId).First().Users.Count == 0)
                    DisposeSession(sessionId);
            }
        }

        public static void DisposeSession(string sessionId)
        {
            Sessions.Remove(Sessions.Where(session => session.SessionID == sessionId).First());
            SessionIdList.Remove(sessionId);
        }

        public static bool IsValidSession(string sessionId)
        {
            return SessionIdList.Contains(sessionId);
        }

        public static bool IsValidUser(string userId)
        {
            return UserIdDict.Keys.Contains(userId);
        }

        public static string GetUsersSession(string userId)
        {
            if (IsValidUser(userId))
                return Sessions.Where(session => session.Users.Contains(userId)).First().SessionID;
            else
                return "";
        }

        public static void Ping(string userId)
        {
            if(IsValidUser(userId))
                UserIdDict[userId] = DateTime.Now;
        } 
    }
}
