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

        /// <summary>
        /// Create new showcase
        /// </summary>
        /// <param name="showcaseDTO"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get all items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Showcase>>> Get([FromQuery] bool active = true)
        {
            return await _context
                .Showcases
                .Where(x=>x.RemovedAt.HasValue == !active)
                .Include(x=>x.Products)
                .ToListAsync();
        }

        /// <summary>
        /// Get one entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<Showcase>>> Get([FromRoute]long id)
        {
            var showcase = await _context
                .Showcases
                .Where(x => x.RemovedAt.HasValue == false)
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (showcase == null)
                return BadRequest("Showcase does not exist");

            return Ok(showcase);
        }
        
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="showcaseDTO"></param>
        /// <returns></returns>
        [HttpPut] 
        public async Task<ActionResult<Showcase>> Update([FromBody]ShowcaseDTO showcaseDTO)
        {
            if (showcaseDTO == null)
                return BadRequest();

            if (_context.Showcases.Any(x => x.Id == showcaseDTO.Id) == false)
                return NotFound("Showcase does not exist");

            _context.Update(showcaseDTO);

            await _context.SaveChangesAsync();
            return Ok(showcaseDTO);
        }

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Showcase>> Delete(long id)
        {
            var showcase = await _context
                .Showcases
                .Include(x=>x.Products)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (showcase == null)
                return NotFound("Showcase does not exist");

            if (showcase.Products.Count > 0)
                return BadRequest("Can't remove. Showcase contain products.");

            showcase.RemovedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(showcase);
        }

        /// <summary>
        /// Place product into showcase
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/place/{productId:int}")]
        public async Task<ActionResult<Showcase>> Place(long id, long productId, [FromQuery]int count = 1)
        {
            var showcase = _context
                .Showcases
                .Include(x => x.Products)
                .FirstOrDefault(x => x.Id == id);

            if (showcase == null)
                return BadRequest("Showcase does not exist");

            var product = _context.Products.FirstOrDefault(x => x.Id == productId);

            if (product == null)
                return BadRequest("Product does not exist");

            if (showcase.Products != null && showcase.Products.Any(x => x.Id == product.Id))
                return BadRequest("Product already exist in showcase");

            if (count <= 0)
                return BadRequest("The Count should be positive number");

            if (showcase.Capacity + product.Capacity * count > showcase.Capacity)
                return BadRequest("There is no free space on the showcase");

            showcase.Products.Add(product);

            _context.Update(showcase);

            await _context.SaveChangesAsync();

            return Ok(showcase);
        }

        /// <summary>
        /// Take out product from showcase
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/takeout/{productId:int}")]
        public async Task<ActionResult<Showcase>> TakeOut(long id, long productId)
        {
            var showcase = _context
                .Showcases
                .Include(x => x.Products)
                .Where(x=>x.Products.Any(p=>p.Id == productId))                
                .FirstOrDefault(x => x.Id == id);

            if (showcase == null)
                return BadRequest("Showcase with this product does not exist");

            var product = _context.Products.FirstOrDefault(x => x.Id == productId);

            if (product == null)
                return BadRequest("Product does not exist");

            showcase.Products.Remove(product);

            _context.Update(showcase);

            await _context.SaveChangesAsync();

            return Ok(showcase);
        }

    }
}
