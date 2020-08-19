using System.Collections.Generic;
using System.Linq;
using CursoAPI.Data;
using CursoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CursoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class QuotesController : ControllerBase
    {
        private readonly QuoteDbContext _quoteDbContext;
        public QuotesController(QuoteDbContext quoteDbContext)
        {
            _quoteDbContext = quoteDbContext;

        }
        [HttpGet]
        [ResponseCache(Duration = 60,Location=ResponseCacheLocation.Any)]
        public IActionResult Get(string sort)
        {
            IQueryable<Quote> quotes;
            switch (sort)
            {
                case "desc":
                    quotes = _quoteDbContext.Quotes.OrderByDescending(q => q.CriadoEm);
                    break;
                case "asc":
                    quotes = _quoteDbContext.Quotes.OrderBy(q => q.CriadoEm);
                    break;
                default:
                    quotes = _quoteDbContext.Quotes;
                    break;
            }
            return Ok(quotes);
        }
        //para retornar essa pagina digitar
        //https://localhost:5001/api/Quotes/PagingQuote
        //ou
        //https://localhost:5001/api/Quotes/PagingQuote?pageNumber=1&pageSize=5 ou outros numeros quaisquer
        //Essa função server para quando se tem muitos dados ele trazer menos, por exemplo em uma base com 13 dados, em cada pageNumber vai trazer no maximo 5 linhas dessas 13


        [HttpGet("[action]")]
        //pageNumber é o numero da pagina que irá retornar
        //pageSize são quantos dados ira retornar na pagina
        public IActionResult PagingQuote(int? pageNumber, int? pageSize)
        {
            var quote = _quoteDbContext.Quotes;
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;

            return Ok(quote.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult SearchQuote(string type)
        {
            var quotes = _quoteDbContext.Quotes.Where(q => q.Type.StartsWith(type));
            return Ok(quotes);
        }
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var quote = _quoteDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return NotFound("Não foi encontrado");
            }
            return Ok(quote);
        }
        // api/quotes/Test/1
        /*
        Assim é feito para retornar um dado em alguma outra pagina.
        [HttpGet("[action]/{id}")]
        public int Test(int id)
        {
            return id;
        }*/
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            _quoteDbContext.Quotes.Add(quote);
            _quoteDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            var entity = _quoteDbContext.Quotes.Find(id);
            if (entity == null)
            {
                return NotFound("Nenhuma informação retornou sobre esse id...");
            }
            else
            {
                entity.Title = quote.Title;
                entity.Author = quote.Author;
                entity.Description = quote.Description;
                entity.Type = quote.Type;
                entity.CriadoEm = quote.CriadoEm;
                _quoteDbContext.SaveChanges();
                return Ok("Atualização feita com sucesso...");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var quote = _quoteDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return NotFound("Id " + id + " não encontrado");
            }
            else
            {
                _quoteDbContext.Quotes.Remove(quote);
                _quoteDbContext.SaveChanges();
                return Ok("Quote com id " + id + " deletado");
            }
        }
    }
}