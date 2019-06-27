using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoECommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DemoECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        //instance of DbContext
        private readonly FlixOneStoreContext _context;

        //Constructor
        public CustomersController(FlixOneStoreContext context)
        {
            _context = context;
        }


        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/{id}
        //telling the runtime what the basic authentication is
        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Customers>> GetCustomers([FromRoute]Guid id)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //
            var ident = User.Identity as ClaimsIdentity;
            var currentLoggedInUserId = ident.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if(currentLoggedInUserId != id.ToString())
            {
                //not authorized
                return BadRequest("You are not authorized");
            }

            var customers = await _context
                .Customers
                .SingleOrDefaultAsync(
                    m => m.Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return Ok(customers);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomers(Guid id, Customers customers)
        {
            if (id != customers.Id)
            {
                return BadRequest();
            }

            _context.Entry(customers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customers>> PostCustomers([FromBody]Customers customers)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Unique mail id check
            if(_context.Customers.Any(x => x.Email == customers.Email))
            {
                ModelState.AddModelError("email", "User with mail id already exists!");
                return BadRequest(ModelState);
            }

            //original code generation
            //operation to add customers if not filtered by unique constraint
            _context.Customers.Add(customers);

            //save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomersExists(customers.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomers", new { id = customers.Id }, customers);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customers>> DeleteCustomers(Guid id)
        {
            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();

            return customers;
        }

        private bool CustomersExists(Guid id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
