using Microsoft.Build.Framework;

namespace DemoApplication.Areas.Admin.ViewModels.Author
{
    public class AddViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public AddViewModel(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public AddViewModel()
        {
        }
    }
}
