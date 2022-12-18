namespace DemoApplication.Areas.Admin.ViewModels.Author
{
    public class UpdateViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdateAt { get; set; }

        public UpdateViewModel(string firstName, string lastName, DateTime updateAt)
        {
            FirstName = firstName;
            LastName = lastName;
            UpdateAt = updateAt;
        }

        public UpdateViewModel()
        {
        }
    }
}
