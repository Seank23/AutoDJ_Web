using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Data
{
    public class VideoDataModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(50)]
        public string VideoId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Channel { get; set; }

        [Required]
        [MaxLength(12)]
        public string PublishedDate { get; set; }

        [Required]
        [MaxLength(12)]
        public string Duration { get; set; }

        [Required]
        [MaxLength(50)]
        public string Thumbnail { get; set; }

        [Required]
        public int Rating { get; set; }
    }
}
