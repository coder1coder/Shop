using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.WebApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProductController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody]Product product)
        {
            if (product == null)
                return BadRequest();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            return await _context
                .Products
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<Product>>> Get(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return BadRequest();

            return Ok(product);
        }

        [HttpPut]
        public async Task<ActionResult<Product>> Update(Product product)
        {
            if (product == null)
                return BadRequest();

            if (_context.Products.Any(x => x.Id == product.Id) == false)
                return NotFound();

            _context.Update(product);

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            
            if (product == null)
                return NotFound();
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

    }
}
