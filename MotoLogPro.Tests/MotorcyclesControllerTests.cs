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
    // TEST 2: POST lascia passare l'eccezione al Middleware in caso di duplicato
    // ----------------------------------------------------------------
    [Fact]
    public async Task PostMotorcycle_ThrowsDbUpdateException_ForMiddlewareToCatch_WhenVinAlreadyExists()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();
        var dto = new CreateMotorcycleDto { Vin = "VIN-DUPLICATO" };

        // Simuliamo l'eccezione lanciata dal DB
        mockService.Setup(s => s.CreateAsync("user-1", dto))
                   .ThrowsAsync(new DbUpdateException("Vincolo di unicità violato"));

        var controller = CreateController(mockService.Object);

        // ACT & ASSERT
        // Essendo un test unitario, il Middleware non è in esecuzione.
        // Verifichiamo quindi che il Controller si comporti correttamente lasciando "uscire" l'eccezione.
        await Assert.ThrowsAsync<DbUpdateException>(() => controller.PostMotorcycle(dto));
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
                   .ReturnsAsync(true);

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
                   .ReturnsAsync(false);

        var controller = CreateController(mockService.Object);

        // ACT
        var result = await controller.DeleteMotorcycle(999);

        // ASSERT
        Assert.IsType<NotFoundResult>(result);
    }

    // ----------------------------------------------------------------
    // TEST 5: Qualsiasi endpoint restituisce 401 Unauthorized se manca l'ID
    // ----------------------------------------------------------------
    [Fact]
    public async Task GetMotorcycles_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // ARRANGE
        var mockService = new Mock<IMotorcycleService>();
        var controller = CreateController(mockService.Object, string.Empty);

        // ACT
        var result = await controller.GetMotorcycles();

        // ASSERT
        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}