namespace Luugiaphat.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // Các thuộc tính mới thêm vào
        public string FirstName { get; set; }     // Tên người dùng
        public string LastName { get; set; }      // Họ người dùng
        public string PhoneNumber { get; set; }   // Số điện thoại
        public DateTime DateOfBirth { get; set; } // Ngày sinh
        public bool IsActive { get; set; }        // Trạng thái tài khoản (hoạt động hay không)
    }
}
