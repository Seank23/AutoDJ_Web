using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public static class QueueModel
    {
        private static int id = -1;
        public static List<QueueItemModel> Queue { get; set; } = new List<QueueItemModel>();

        public static int NextId()
        {
            id++;
            return id;
        }

        public static void OrderQueue()
        {
            Queue = Queue.OrderByDescending(item => item.Rating).ToList();
        }

        public static List<int> GetOrderList()
        {
            List<int> orderList = new List<int>();

            for(int i = 0; i < Queue.Count; i++)
                orderList.Add(Queue[i].Id);

            return orderList;
        }
    }
}
