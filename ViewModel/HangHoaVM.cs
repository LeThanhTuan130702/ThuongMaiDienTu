namespace ThuongMaiDienTu.ViewModel
{
    public class HangHoaVM
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Image {  get; set; }
        public double Price { get; set; }
        public double Discout { get; set; }
        public int? CateID { get; set; }
    }
    public class HangHoaDetailVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public double Discout { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Information { get; set; }
        public int Review {  get; set; }
        public int QuantityInStock { get; set; }
        public int? CateId { get; set; }   

    }
}

