using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

/// <summary>
/// API controller for service management operations.
/// 
/// Handles service discovery, retrieval, creation, updates, and deletion.
/// Anonymous users can browse and search services.
/// Administrative operations (create, update, delete) require AdminPolicy authorization.
/// </summary>
[ApiController]
[Authorize(Policy = "AdminPolicy")]
[Route("api/[controller]")]
public class ServicesController(IServicesService servicesService) : ControllerBase
{
    /// <summary>
    /// Retrieve a paginated list of services with optional filtering.
    /// 
    /// Anonymous users can call this endpoint to browse services.
    /// Supports filtering by category and price range, with pagination.
    /// </summary>
    /// <param name="category">Optional category filter.</param>
    /// <param name="page">Page number for pagination (1-based, default 1).</param>
    /// <param name="pageSize">Number of items per page (default 10).</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <returns>PaginatedServicesDto containing services and pagination metadata.</returns>
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

    /// <summary>
    /// Retrieve detailed information about a specific service.
    /// 
    /// Anonymous users can call this endpoint to view service details.
    /// </summary>
    /// <param name="id">The service ID to retrieve.</param>
    /// <returns>
    /// Returns 200 OK with ServiceResponseDto if found.
    /// Returns 404 Not Found if service does not exist.
    /// </returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponseDto>> GetService(int id)
    {
        var service = await servicesService.GetServiceById(id);
        if (service == null) return NotFound();
        
        return Ok(service);
    }

    /// <summary>
    /// Retrieve all available service categories.
    /// 
    /// Anonymous users can call this endpoint to discover available categories.
    /// Returns a sorted list of unique category names.
    /// </summary>
    /// <returns>An enumerable collection of category names.</returns>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await servicesService.GetCategories();
        return Ok(categories);
    }

    /// <summary>
    /// Create a new service (admin only).
    /// 
    /// Requires AdminPolicy authorization. Service properties are validated
    /// according to the ServiceDto validation rules.
    /// </summary>
    /// <param name="serviceDto">The service details.</param>
    /// <returns>
    /// Returns 201 Created with the new ServiceResponseDto and Location header.
    /// Returns 400 Bad Request if ModelState is invalid.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> CreateService(ServiceDto serviceDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var newService = await servicesService.CreateService(serviceDto);
        return CreatedAtAction(nameof(GetService), new { id = newService.Id }, newService);
    }

    /// <summary>
    /// Update an existing service (admin only).
    /// 
    /// Requires AdminPolicy authorization. Updates only the provided fields
    /// while preserving unchanged values.
    /// </summary>
    /// <param name="id">The ID of the service to update.</param>
    /// <param name="serviceDto">The updated service details.</param>
    /// <returns>
    /// Returns 200 OK with updated ServiceResponseDto.
    /// Returns 400 Bad Request if ModelState is invalid.
    /// Returns 404 Not Found if service does not exist.
    /// </returns>
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

    /// <summary>
    /// Delete a service (admin only).
    /// 
    /// Requires AdminPolicy authorization. Permanently removes the service
    /// from the system. Existing orders referencing this service are preserved.
    /// </summary>
    /// <param name="id">The ID of the service to delete.</param>
    /// <returns>
    /// Returns 204 No Content if deletion successful.
    /// Returns 404 Not Found if service does not exist.
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var result = await servicesService.DeleteService(id);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}