using Microsoft.AspNetCore.Mvc;
using Luugiaphat.Model;
using Luugiaphat.Data;
using Microsoft.EntityFrameworkCore;  // Đảm bảo bạn sử dụng namespace AppDbContext

namespace Luugiaphat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            // Lấy tất cả sản phẩm từ cơ sở dữ liệu
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            // Lấy sản phẩm theo ID từ cơ sở dữ liệu
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            _context.Products.Add(product);  // Thêm sản phẩm vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(Get), new { id = product.ID }, product);  // Trả về thông tin sản phẩm vừa tạo
        }

        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();  // Trả về lỗi 400 nếu ID không khớp
            }

            _context.Entry(product).State = EntityState.Modified;  // Đánh dấu sản phẩm là đã thay đổi
            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();  // Trả về lỗi 404 nếu sản phẩm không tồn tại
                }
                else
                {
                    throw;
                }
            }

            return NoContent();  // Trả về mã 204 khi cập nhật thành công
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);  // Tìm sản phẩm theo ID

            if (product == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            _context.Products.Remove(product);  // Xóa sản phẩm khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();  // Trả về mã 204 khi xóa thành công
        }

        // Kiểm tra sự tồn tại của sản phẩm
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);  // Kiểm tra sản phẩm có tồn tại hay không
        }
    }
}
