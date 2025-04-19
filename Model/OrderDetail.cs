namespace Luugiaphat.Model
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }    // Mã chi tiết đơn hàng
        public int OrderID { get; set; }          // Mã đơn hàng (liên kết với bảng Order)
        public int ProductID { get; set; }        // Mã sản phẩm (liên kết với bảng Product)
        public int Quantity { get; set; }         // Số lượng sản phẩm
        public double Price { get; set; }         // Giá của sản phẩm
        public double TotalPrice => Quantity * Price;  // Tổng giá trị của sản phẩm (Số lượng * Giá)
    }
}
