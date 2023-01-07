namespace DemoApplication.Areas.Admin.ViewModels.Role
{
    public class RoleViewModel
    {
        public RoleViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

    }
}
