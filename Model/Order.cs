namespace Luugiaphat.Model
{
    public class Order
    {
        public int ID { get; set; }              // Mã đơn hàng
        public string CustomerName { get; set; } // Tên khách hàng
        public string ShippingAddress { get; set; } // Địa chỉ giao hàng
        public DateTime OrderDate { get; set; }    // Ngày đặt hàng
        public double TotalAmount { get; set; }     // Tổng giá trị của đơn hàng
    }
}

