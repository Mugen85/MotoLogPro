using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MotoLogPro.API.Controllers;
using MotoLogPro.Domain.Interfaces;
using MotoLogPro.Shared.DTOs;
using System.Security.Claims;

namespace MotoLogPro.Tests;

public class MotorcyclesControllerTests
{
    // Metodo Helper per creare un Controller con un utente "loggato" finto
    private static MotorcyclesController CreateController(IMotorcycleService mockService, string userId = "user-1")
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId)
        ], "mock-auth"));

        var controller = new MotorcyclesController(mockService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            }
        };

        return controller;
    }

    // ----------------------------------------------------------------
    // TEST 1: POST restituisce 201 Created quando ha successo
    // ----------------------------------------------------------------
    [Fact]
    public async Task PostMotorcycle_ReturnsCreatedAtAction_OnSuccess()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();
        var dto = new CreateMotorcycleDto { Brand = "Aprilia", Model = "RSV4" };
        var createdVehicle = new VehicleDto { Id = 1, Brand = "Aprilia", Model = "RSV4" };

        // Diciamo al servizio finto: "Quando chiamano CreateAsync, restituisci createdVehicle"
        mockService.Setup(s => s.CreateAsync("user-1", dto))
                   .ReturnsAsync(createdVehicle);

        var controller = CreateController(mockService.Object);

        // ACT
        var result = await controller.PostMotorcycle(dto);

        // ASSERT
        var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<VehicleDto>(actionResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    // ----------------------------------------------------------------
    // TEST 2: POST restituisce 409 Conflict se il VIN esiste già
    // ----------------------------------------------------------------
    [Fact]
    public async Task PostMotorcycle_ReturnsConflict_WhenVinAlreadyExists()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();
        var dto = new CreateMotorcycleDto { Vin = "VIN-DUPLICATO" };

        // Simuliamo l'eccezione lanciata da Entity Framework per il vincolo di unicità
        mockService.Setup(s => s.CreateAsync("user-1", dto))
                   .ThrowsAsync(new DbUpdateException("Vincolo di unicità violato"));

        var controller = CreateController(mockService.Object);

        // ACT
        var result = await controller.PostMotorcycle(dto);

        // ASSERT
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal("Il VIN inserito è già presente nel sistema.", conflictResult.Value);
    }

    // ----------------------------------------------------------------
    // TEST 3: PUT restituisce 204 NoContent quando ha successo
    // ----------------------------------------------------------------
    [Fact]
    public async Task PutMotorcycle_ReturnsNoContent_OnSuccess()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();
        var dto = new CreateMotorcycleDto { Brand = "Yamaha" };

        mockService.Setup(s => s.UpdateAsync("user-1", 1, dto))
                   .ReturnsAsync(true); // Il servizio dice "tutto ok, aggiornato"

        var controller = CreateController(mockService.Object);

        // ACT
        var result = await controller.PutMotorcycle(1, dto);

        // ASSERT
        Assert.IsType<NoContentResult>(result);
    }

    // ----------------------------------------------------------------
    // TEST 4: DELETE restituisce 404 NotFound se la moto non esiste
    // ----------------------------------------------------------------
    [Fact]
    public async Task DeleteMotorcycle_ReturnsNotFound_WhenMotorcycleDoesNotExist()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();

        mockService.Setup(s => s.DeleteAsync("user-1", 999))
                   .ReturnsAsync(false); // Il servizio dice "non ho trovato nulla da cancellare"

        var controller = CreateController(mockService.Object);

        // ACT
        var result = await controller.DeleteMotorcycle(999);

        // ASSERT
        Assert.IsType<NotFoundResult>(result);
    }

    // ----------------------------------------------------------------
    // TEST 5: Qualsiasi endpoint restituisce 401 Unauthorized se manca l'ID nel Token
    // ----------------------------------------------------------------
    [Fact]
    public async Task GetMotorcycles_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();

        // Creiamo un controller senza "user-1" (stringa vuota = utente non riconosciuto)
        var controller = CreateController(mockService.Object, string.Empty);

        // ACT
        var result = await controller.GetMotorcycles();

        // ASSERT
        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}