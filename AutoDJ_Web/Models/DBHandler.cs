using AutoDJ_Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class DBHandler
    {
        private const int sessionId = 26296860;
        protected ApplicationDbContext mContext;

        public DBHandler(ApplicationDbContext context)
        {
            mContext = context;
        }

        public void WriteVideo(QueueItemModel video)
        {
            mContext.Database.EnsureCreated();
            mContext.Video.Add(new VideoDataModel
            {
                SessionId = sessionId,
                ItemId = video.Id,
                VideoId = video.Video.VideoId,
                Name = video.Video.Name,
                Channel = video.Video.Channel,
                PublishedDate = video.Video.PublishedDate,
                Duration = video.Video.Duration,
                Thumbnail = video.Video.Thumbnail,
                Rating = video.Rating
            });
            mContext.SaveChanges();
        }

        public void UpdateRating(QueueItemModel video)
        {
            var element = mContext.Video.Where(e => e.SessionId == sessionId).Where(e => e.ItemId == video.Id).FirstOrDefault();
            element.Rating = video.Rating;
            mContext.SaveChanges();
        }

        public void DeleteVideo(QueueItemModel video)
        {
            var element = mContext.Video.Where(e => e.SessionId == sessionId).Where(e => e.ItemId == video.Id).FirstOrDefault();
            mContext.Video.Remove(element);
            mContext.SaveChanges();
        }

        public void ClearSession()
        {
            mContext.Video.RemoveRange(mContext.Video.Where(e => e.SessionId == sessionId));
            mContext.SaveChanges();
        }
    }
}
