using Microsoft.AspNetCore.Mvc;
using Luugiaphat.Data;
using Luugiaphat.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luugiaphat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orderdetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> Get()
        {
            // Lấy tất cả chi tiết đơn hàng từ cơ sở dữ liệu
            var orderDetails = await _context.OrderDetails.ToListAsync();
            return Ok(orderDetails);
        }

        // GET: api/orderdetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> Get(int id)
        {
            // Lấy chi tiết đơn hàng theo ID từ cơ sở dữ liệu
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy chi tiết đơn hàng
            }

            return Ok(orderDetail);
        }

        // POST: api/orderdetail
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> Post(OrderDetail orderDetail)
        {
            // Thêm chi tiết đơn hàng vào DbContext
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = orderDetail.OrderDetailID }, orderDetail);  // Trả về chi tiết đơn hàng vừa tạo
        }

        // PUT: api/orderdetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return BadRequest();  // Trả về lỗi 400 nếu ID không khớp
            }

            _context.Entry(orderDetail).State = EntityState.Modified;  // Đánh dấu chi tiết đơn hàng là đã thay đổi

            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                {
                    return NotFound();  // Trả về lỗi 404 nếu không tìm thấy chi tiết đơn hàng
                }
                else
                {
                    throw;
                }
            }

            return NoContent();  // Trả về mã 204 khi cập nhật thành công
        }

        // DELETE: api/orderdetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);  // Tìm chi tiết đơn hàng theo ID

            if (orderDetail == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy chi tiết đơn hàng
            }

            _context.OrderDetails.Remove(orderDetail);  // Xóa chi tiết đơn hàng khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();  // Trả về mã 204 khi xóa thành công
        }

        // Kiểm tra sự tồn tại của chi tiết đơn hàng
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);  // Kiểm tra chi tiết đơn hàng có tồn tại hay không
        }
    }
}
