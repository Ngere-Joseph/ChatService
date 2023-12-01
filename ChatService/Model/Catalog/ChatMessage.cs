using ChatService.Common;
using System.Security.Principal;
using System;
using ChatService.Contracts;

namespace ChatService.Model.Catalog
{
    public class ChatMessage : AuditableEntity, IAggregateRoot, IEntity
    {
        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserId { get; set; }

        public string CourseId { get; set; }

        public string EmojiUrl { get; set; } = string.Empty;

        public string ImgUrl { get; set; } = string.Empty;

        public bool IsOnline { get; set; }
    }
}
