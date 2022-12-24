using Microsoft.Build.Framework;

namespace DemoApplication.Areas.Admin.ViewModels.Author
{
    public class AddViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AddViewModel(string firstName, string lastName, DateTime createdAt, DateTime updatedAt)
        {
            FirstName = firstName;
            LastName = lastName;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public AddViewModel()
        {
        }
    }
}
