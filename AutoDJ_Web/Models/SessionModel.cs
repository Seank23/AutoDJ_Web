using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class SessionModel
    {
        public string SessionID { get; set; }
        public List<string> Users { get; set; }
        //public QueueModel Queue { get; set; }
        //public PlayerModel Player { get; set; }

        public SessionModel(string id)
        {
            SessionID = id;
            Users = new List<string>();
        }
    }
}
