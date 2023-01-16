namespace DemoApplication.Areas.Client.ViewModels.Order
{
	public class OrderViewModel
	{
		public int UserId { get; set; }
		public decimal SumTotal { get; set; }
		public List<ListItemViewModel>? ProductList { get; set; }

		public class ListItemViewModel
		{
			public ListItemViewModel(int id, string? title, int quantity, decimal price, decimal total)
			{
				Id = id;
				Title = title;
				Quantity = quantity;
				Price = price;
				Total = total;
			}

			public int Id { get; set; }
			public string? Title { get; set; }
			public int Quantity { get; set; }
			public decimal Price { get; set; }
			public decimal Total { get; set; }
		}
	}
}
