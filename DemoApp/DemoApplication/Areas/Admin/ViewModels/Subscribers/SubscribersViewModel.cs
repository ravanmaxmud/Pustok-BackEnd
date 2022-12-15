namespace DemoApplication.Areas.Admin.ViewModels.Subscribers
{
    public class SubscribersViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public SubscribersViewModel(int id,string email)
        {
            Id=id;
            Email=email;
        }
    }
}
