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
            try
            {
                // Lấy tất cả các category từ cơ sở dữ liệu
                var categories = await _context.Categories.ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tải danh mục", error = ex.Message });
            }
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            try
            {
                // Lấy category theo ID từ cơ sở dữ liệu
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound(new { message = "Danh mục không tìm thấy" });  // Trả về lỗi 404 nếu không tìm thấy category
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tải danh mục", error = ex.Message });
            }
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<Category>> Post(Category category)
        {
            try
            {
                // Kiểm tra xem category đã tồn tại chưa
                if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
                {
                    return Conflict(new { message = "Danh mục với tên này đã tồn tại" });
                }

                // Thêm category vào DbContext
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

                return CreatedAtAction(nameof(Get), new { id = category.Id }, category);  // Trả về thông tin category vừa tạo
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thêm danh mục", error = ex.Message });
            }
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest(new { message = "ID trong URL không khớp với ID danh mục" });
            }

            try
            {
                // Kiểm tra sự tồn tại của category trong cơ sở dữ liệu
                var existingCategory = await _context.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new { message = "Danh mục không tồn tại" });  // Trả về lỗi 404 nếu không tìm thấy category
                }

                // Cập nhật các trường cần thay đổi
                existingCategory.Name = category.Name;

                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

                return NoContent();  // Trả về mã 204 khi cập nhật thành công
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(new { message = "Danh mục không tồn tại" });  // Trả về lỗi 404 nếu không tìm thấy category
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật danh mục", error = ex.Message });
            }
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);  // Tìm category theo ID

                if (category == null)
                {
                    return NotFound(new { message = "Danh mục không tồn tại" });  // Trả về lỗi 404 nếu không tìm thấy category
                }

                _context.Categories.Remove(category);  // Xóa category khỏi DbContext
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

                return NoContent();  // Trả về mã 204 khi xóa thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa danh mục", error = ex.Message });
            }
        }

        // Kiểm tra sự tồn tại của category
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);  // Kiểm tra category có tồn tại hay không
        }
    }
}
