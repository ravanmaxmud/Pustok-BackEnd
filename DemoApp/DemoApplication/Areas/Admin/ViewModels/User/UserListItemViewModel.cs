namespace DemoApplication.Areas.Admin.ViewModels.User
{
    public class UserListItemViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }


        public UserListItemViewModel(int id ,string email, string firstName, string lastName, string role)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }
    }
}
