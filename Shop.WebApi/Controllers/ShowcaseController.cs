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
    public class ShowcaseController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ShowcaseController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Showcase>> Create([FromBody] ShowcaseDTO showcaseDTO)
        {
            if (showcaseDTO == null)
                return BadRequest();

            var showcase = new Showcase
            {
                Name = showcaseDTO.Name,
                MaxCapacity = showcaseDTO.MaxCapacity,
                CreatedAt = DateTime.Now
            };

            _context.Showcases.Add(showcase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = showcase.Id }, showcase);
        }

        [HttpPost("{id:int}/place/{productId:int}")]
        public async Task<ActionResult<Showcase>> Place(long id, long productId)
        {
            var showcase = _context
                .Showcases
                .Include("Products")
                .FirstOrDefault(x => x.Id == id);

            if (showcase == null)
                return BadRequest("Showcase does not exist");

            var product = _context.Products.FirstOrDefault(x => x.Id == productId); 
            
            if (product == null)
                return BadRequest("Product does not exist");

            if (showcase.Products != null && showcase.Products.Any(x => x.Id == product.Id))
                return BadRequest("Product already exist in showcase");

            _context.Update(showcase);

            showcase.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok(showcase);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Showcase>>> Get()
        {
            return await _context
                .Showcases
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<Showcase>>> Get(long id)
        {
            var showcase = await _context.Showcases.FirstOrDefaultAsync(x => x.Id == id);

            if (showcase == null)
                return BadRequest("Showcase does not exist");

            return Ok(showcase);
        }

        [HttpPut]
        public async Task<ActionResult<Showcase>> Update(ShowcaseDTO showcaseDTO)
        {
            if (showcaseDTO == null)
                return BadRequest();

            if (_context.Showcases.Any(x => x.Id == showcaseDTO.Id) == false)
                return NotFound("Showcase does not exist");

            _context.Update(showcaseDTO);

            await _context.SaveChangesAsync();
            return Ok(showcaseDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Showcase>> Delete(long id)
        {
            var showcase = await _context.Showcases.FirstOrDefaultAsync(x => x.Id == id);

            if (showcase == null)
                return NotFound("Showcase does not exist");

            _context.Showcases.Remove(showcase);
            await _context.SaveChangesAsync();
            return Ok(showcase);
        }

        [HttpPost("{count}")]
        public async Task<ActionResult> Seed(int count)
        {
            if (count < 1)
                return BadRequest();

            if (_context.Showcases.Any() == false)
            {
                for (int i = 0; i < count; i++)
                {
                    await _context.AddAsync(new Showcase()
                    {
                        Name = "Витрина " + (i + 1),
                        CreatedAt = DateTime.Now,
                        MaxCapacity = 1 + i
                    });
                }

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
