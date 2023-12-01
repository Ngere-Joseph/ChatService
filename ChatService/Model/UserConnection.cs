using System;

namespace ChatService.Model
{
    public class UserConnection
    {
        public Guid User { get; set; }

        public Guid Room { get; set; }

        public Guid StudentId { get; set; }

        public Guid CourseId { get; set; }

        public string EmojiUrl { get; set; } = string.Empty;

        public string ImgUrl { get; set; } = string.Empty;
    }
}