namespace DemoApplication.Areas.Client.ViewModels.Home.Index
{
    public class BookListItemViewModel
    {
        public BookListItemViewModel(int id, string title, string author, decimal price, string mainImgUrl, string hoverImgUrl)
        {
            Id = id;
            Title = title;
            Author = author;
            Price = price;
            MainImgUrl = mainImgUrl;
            HoverImgUrl = hoverImgUrl;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string MainImgUrl { get; set; }
        public string HoverImgUrl { get; set; }
        public BookListItemViewModel()
        {

        }
    }
}
