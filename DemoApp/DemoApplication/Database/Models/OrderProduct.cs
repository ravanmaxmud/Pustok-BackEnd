using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
    public class OrderProduct : BaseEntity ,IAuditable
    {
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public string OrderId { get; set; }
        public Order Order { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
