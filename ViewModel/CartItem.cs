namespace ThuongMaiDienTu.ViewModel
{
	public class CartItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Image {  get; set; }
		public double Price { get; set; }
		public int Quantity { get; set; }
		public double Discout { get; set; }
		public double Total =>(Price-(Discout*Price))*Quantity;
	}
}
