using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class SessionModel
    {
        public int SessionID { get; set; }
        public List<int> Users { get; set; }
        //public QueueModel Queue { get; set; }
        //public PlayerModel Player { get; set; }
    }
}
