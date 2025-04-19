using Microsoft.AspNetCore.Mvc;
using Luugiaphat.Data;
using Luugiaphat.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Luugiaphat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            // Lấy tất cả đơn hàng từ cơ sở dữ liệu
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            // Lấy đơn hàng theo ID từ cơ sở dữ liệu
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy đơn hàng
            }

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<Order>> Post(Order order)
        {
            // Thêm đơn hàng vào DbContext
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = order.ID }, order); // Trả về đơn hàng vừa tạo
        }

        // PUT: api/order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Order order)
        {
            if (id != order.ID)
            {
                return BadRequest(); // Trả về lỗi 400 nếu ID không khớp
            }

            _context.Entry(order).State = EntityState.Modified; // Đánh dấu đơn hàng là đã thay đổi

            try
            {
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound(); // Trả về lỗi 404 nếu không tìm thấy đơn hàng
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về mã 204 khi cập nhật thành công
        }

        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id); // Tìm đơn hàng theo ID

            if (order == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy đơn hàng
            }

            _context.Orders.Remove(order); // Xóa đơn hàng khỏi DbContext
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent(); // Trả về mã 204 khi xóa thành công
        }

        // Kiểm tra sự tồn tại của đơn hàng
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id); // Kiểm tra đơn hàng có tồn tại hay không
        }
    }
}
