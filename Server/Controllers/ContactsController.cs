using BlazorApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RestService.Controllers;

[Route("api/contacts")]
[ApiController]
public class CustomerProfilesController : ControllerBase
{
    private readonly BlazorAppContext _context;

    private readonly ILogger<CustomerProfilesController> _logger;

    public CustomerProfilesController(BlazorAppContext context, ILogger<CustomerProfilesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetCustomerProfiles()
    {
        return await _context.Contacts.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetCustomerProfile(string id)
    {
        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null)
        {
            return NotFound();
        }

        return contact;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomerProfile(string id, Contact contact)
    {
        if (id != contact.Id)
        {
            return BadRequest();
        }

        _context.Entry(contact).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CustomerProfileExists(id))
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


    [HttpPost]
    public async Task<ActionResult<Contact>> PostCustomerProfile(Contact customerProfile)
    {
        _context.Contacts.Add(customerProfile);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCustomerProfile", new { id = customerProfile.Id }, customerProfile);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomerProfile(string id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
        {
            return NotFound();
        }

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CustomerProfileExists(string id)
    {
        return _context.Contacts.Any(e => e.Id == id);
    }
}