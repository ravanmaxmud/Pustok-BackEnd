using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
    public class Addres : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Title { get; set; }
        public string PhoneNum { get; set; }

    }
}
