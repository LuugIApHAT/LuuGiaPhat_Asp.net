using Microsoft.AspNetCore.Mvc;
using Luugiaphat.Model;  // Đảm bảo bạn sử dụng đúng namespace của model Category
using Luugiaphat.Data;   // Đảm bảo bạn sử dụng đúng namespace của AppDbContext
using Microsoft.EntityFrameworkCore;  // Để làm việc với DbContext

namespace Luugiaphat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            // Lấy tất cả các category từ cơ sở dữ liệu
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            // Lấy category theo ID từ cơ sở dữ liệu
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy category
            }

            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<Category>> Post(Category category)
        {
            // Kiểm tra xem category đã tồn tại chưa (nếu có thể)
            if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
            {
                return Conflict(new { message = "Category with this name already exists." });
            }

            _context.Categories.Add(category);  // Thêm category vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);  // Trả về thông tin category vừa tạo
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("ID in the URL does not match the Category ID.");
            }

            // Kiểm tra sự tồn tại của category trong cơ sở dữ liệu
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy category
            }

            // Cập nhật các trường cần thay đổi
            existingCategory.Name = category.Name;

            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();  // Trả về lỗi 404 nếu không tìm thấy category
                }
                else
                {
                    throw;  // Ném lỗi nếu có vấn đề khác
                }
            }

            return NoContent();  // Trả về mã 204 khi cập nhật thành công
        }


        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);  // Tìm category theo ID

            if (category == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy category
            }

            _context.Categories.Remove(category);  // Xóa category khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();  // Trả về mã 204 khi xóa thành công
        }

        // Kiểm tra sự tồn tại của category
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);  // Kiểm tra category có tồn tại hay không
        }
    }
}
