using AzureTables.Entities;
using AzureTables.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureTables.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly TableStorageService tableStorage;

        public BookController(TableStorageService tableStorage)
        {
            this.tableStorage = tableStorage;
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookEntity book)
        {
            await tableStorage.AddBook(book);
            return Ok($"Book <{book.Title}> has been added successfully!");
        }
    }
}
