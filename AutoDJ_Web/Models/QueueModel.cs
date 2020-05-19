using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class QueueModel
    {
        private int id = -1;
        public List<QueueItemModel> Queue { get; set; } = new List<QueueItemModel>();

        public int NextId()
        {
            id++;
            return id;
        }

        public void OrderQueue()
        {
            Queue = Queue.OrderByDescending(item => item.Rating).ToList();
        }

        public List<int> GetOrderList()
        {
            List<int> orderList = new List<int>();

            for(int i = 0; i < Queue.Count; i++)
                orderList.Add(Queue[i].Id);

            return orderList;
        }

        public void PopFromQueue()
        {
            Queue.RemoveAt(0);
        }

        public string[] GetDurationList()
        {
            string[] durations = new string[Queue.Count];

            for(int i = 0; i < Queue.Count; i++)
                durations[i] = Queue[i].Video.Duration;

            return durations;
        }

        public static int GetDurationSeconds(string[] songDurations)
        {
            int duration = 0;
            foreach(string item in songDurations)
            {
                string[] minsecs = item.Split(":");
                duration += (int.Parse(minsecs[0]) * 60) + int.Parse(minsecs[1]);
            }
            return duration;
        }
    }
}
