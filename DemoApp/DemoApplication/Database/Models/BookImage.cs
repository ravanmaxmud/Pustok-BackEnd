using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
    public class BookImage : BaseEntity, IAuditable
    {
        public string? ImageName { get; set; }
        public string? ImageNameFileSystem { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
