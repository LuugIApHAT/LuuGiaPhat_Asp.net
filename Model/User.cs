namespace Luugiaphat.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        //Thêm cột mới
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
