
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs.Requests;
using UserService.DTOs.Responses;
using UserService.Models;
using UserService.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/addresses")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    // Chuyển Address Model thành AddressResponseDto
    private static AddressResponseDto ToResponseDto(Address address)
    {
        return new AddressResponseDto
        {
            AddressId = address.AddressId,
            UserId = address.UserId,
            ReceiverName = address.ReceiverName,
            Phone = address.Phone,
            Province = address.Province,
            District = address.District,
            Ward = address.Ward,
            DetailAddress = address.DetailAddress,
            IsDefault = address.IsDefault
        };
    }

    // GET: api/addresses/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetAddressesByUserId(Guid userId)
    {
        try
        {
            var addresses = await _addressService
                .GetAddressesByUserIdAsync(userId);

            return Ok(addresses.Select(ToResponseDto));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // GET: api/addresses/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAddressById(Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);

        if (address == null)
        {
            return NotFound(new
            {
                message = "Address not found."
            });
        }

        return Ok(ToResponseDto(address));
    }

    // POST: api/addresses
    [HttpPost]
    public async Task<IActionResult> CreateAddress(
        [FromBody] CreateAddressRequestDto request)
    {
        try
        {
            var address = await _addressService.CreateAddressAsync(request);

            return CreatedAtAction(
                nameof(GetAddressById),
                new { id = address.AddressId },
                ToResponseDto(address)
            );
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PUT: api/addresses/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] UpdateAddressRequestDto request)
    {
        var address = await _addressService.UpdateAddressAsync(id, request);

        if (address == null)
        {
            return NotFound(new
            {
                message = "Address not found."
            });
        }

        return Ok(ToResponseDto(address));
    }

    // PATCH: api/addresses/{id}/default
    [HttpPatch("{id:guid}/default")]
    public async Task<IActionResult> SetDefaultAddress(Guid id)
    {
        var result = await _addressService.SetDefaultAddressAsync(id);

        if (!result)
        {
            return NotFound(new
            {
                message = "Address not found."
            });
        }

        return Ok(new
        {
            message = "Default address updated successfully."
        });
    }

    // DELETE: api/addresses/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var result = await _addressService.DeleteAddressAsync(id);

        if (!result)
        {
            return NotFound(new
            {
                message = "Address not found."
            });
        }

        return NoContent();
    }
}
