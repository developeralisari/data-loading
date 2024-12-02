using data_loading.Data;
using data_loading.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace data_loading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Author (Eager Loading)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            // Eager Loading: İlgili verilerin (Books) önceden yüklenmesini sağlar.
            // Include(a => a.Books) kullanılarak her Author ile ilişkili Books verisi birlikte yüklenir.
            return await _context.Authors.Include(a => a.Books).ToListAsync();
            // Eager Loading burada kullanılıyor çünkü Authors ve ilişkili Books verilerini aynı anda almak istiyoruz.
        }

        // GET: api/Author/5 (Eager Loading)
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            // Eager Loading: Author ile ilişkili Books verilerini hemen yükler.
            // Yani, veritabanından sadece Author değil, aynı zamanda o yazarın kitapları da çekilecektir.
            var author = await _context.Authors.Include(a => a.Books)
                                                .FirstOrDefaultAsync(a => a.AuthorId == id);

            if (author == null)
            {
                return NotFound();
            }

            // Burada yine Eager Loading kullanılıyor çünkü Books ile ilişkili veriler de yüklenmiş oluyor.
            return author;
        }

        // GET: api/Author/5 (Lazy Loading - Kullanımı için Navigation Property'nin Virtual olması gerekir)
        [HttpGet("lazy/{id}")]
        public async Task<ActionResult<Author>> GetAuthorLazy(int id)
        {
            // Lazy Loading: Lazy Loading etkinleştirilmişse, Author'ın Books koleksiyonu başlangıçta yüklenmez.
            // Bu koleksiyon yalnızca erişildiğinde veritabanından yüklenir.
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            // Lazy Loading burada kullanılıyor, çünkü Books koleksiyonu yalnızca erişildiğinde yüklenir.
            // Bu, yalnızca Books koleksiyonuna erişildiğinde veri çekilir.
            var books = author.Books;  // Bu satırda Lazy Loading tetiklenir

            return author;
        }

        // GET: api/Author/5 (Explicit Loading)
        [HttpGet("explicit/{id}")]
        public async Task<ActionResult<Author>> GetAuthorExplicit(int id)
        {
            // Explicit Loading: Author nesnesi ilk başta yüklenir, ancak ilişkili Books koleksiyonu daha sonra yüklenir.
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == id);

            if (author == null)
            {
                return NotFound();
            }

            // Explicit Loading: Books koleksiyonu explicit olarak yüklenir.
            await _context.Entry(author).Collection(a => a.Books).LoadAsync();

            // Explicit Loading burada kullanılıyor, çünkü Books verileri yalnızca `LoadAsync()` ile yükleniyor.
            return author;
        }

        // POST: api/Author
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, author);
        }

        // GET: api/Author/Create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            // Veritabanında tabloları oluşturmak için EnsureCreated kullanılır.
            bool databaseCreated = await _context.Database.EnsureCreatedAsync();

            if (databaseCreated)
            {
                // Örnek veriler ekleniyor.
                var author1 = new Author { Name = "J.K. Rowling", Books = new List<Book> { new Book { Title = "Harry Potter and the Sorcerer's Stone" } } };
                var author2 = new Author { Name = "George R.R. Martin", Books = new List<Book> { new Book { Title = "A Game of Thrones" } } };

                _context.Authors.AddRange(author1, author2);
                await _context.SaveChangesAsync();

                return Ok("Database and initial seed data created successfully.");
            }

            return Ok("Database already exists. No changes were made.");
        }
    }
}
