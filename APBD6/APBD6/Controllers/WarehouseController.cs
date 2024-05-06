using APBD6.Models.DTOs;
using APBD6.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APBD6.Controllers;

[Controller]
[Route("api/warehouses")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _repository;

    public WarehouseController(IWarehouseRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost]
    public async Task<IActionResult> FullfillOrder(ClientRequest clientRequest)
    {
        Console.WriteLine(clientRequest.IdProduct+ " XD");
        var serveRequest = await _repository.ServeRequest(clientRequest);
        switch (serveRequest)
        {
            case -1:
                return NotFound("Product not found");
            case -2:
                return NotFound("Warehouse not found");
            case -3:
                return NotFound("no such order");
            case -4:
                return BadRequest("Already served order");
            case -5:
                return StatusCode(StatusCodes.Status500InternalServerError);
            default:
                return StatusCode(StatusCodes.Status201Created); // Obsługa innych przypadków
        }
    }
}