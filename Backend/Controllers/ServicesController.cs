using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Authorize(Policy = "AdminPolicy")]
[Route("api/[controller]")]
public class ServicesController(IServicesService servicesService) : ControllerBase
{
    // GET: api/services
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedServicesDto>> GetServices(
        [FromQuery] string? category, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        var paginatedServices = await servicesService.GetServices(category, page, pageSize, minPrice, maxPrice);
        return Ok(paginatedServices);
    }
    
    // GET: api/services/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponseDto>> GetService(int id)
    {
        var service = await servicesService.GetServiceById(id);
        if (service == null) return NotFound();
        
        return Ok(service);
    }
    
    // GET: api/services/categories
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await servicesService.GetCategories();
        return Ok(categories);
    }
    
    // POST: api/services
    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> CreateService(ServiceDto serviceDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var newService = await servicesService.CreateService(serviceDto);
        return CreatedAtAction(nameof(GetService), new { id = newService.Id }, newService);
    }
    
    // PUT: api/services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, ServiceDto serviceDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var updatedService = await servicesService.UpdateService(id, serviceDto);
        if (updatedService == null)
            return NotFound();
        
        return Ok(updatedService);
    }
    
    // DELETE: api/services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var result = await servicesService.DeleteService(id);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}