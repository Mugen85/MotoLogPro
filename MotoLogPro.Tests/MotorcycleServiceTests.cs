using Microsoft.EntityFrameworkCore;
using MotoLogPro.Domain.Entities;
using MotoLogPro.Infrastructure.Data;
using MotoLogPro.Infrastructure.Services;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Tests;

public class MotorcycleServiceTests
{
    // Questo metodo crea un DbContext fresco con un DB in memoria
    // Ogni test riceve il suo database vuoto e isolato
    private ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    // Crea un utente di test da inserire nel DB
    private ApplicationUser CreateUser(string id = "user-1") => new()
    {
        Id = id,
        UserName = "test@test.com",
        Email = "test@test.com",
        FullName = "Test User"
    };

    // ----------------------------------------------------------------
    // TEST 1: GetByUserAsync ritorna solo le moto dell'utente corretto
    // ----------------------------------------------------------------
    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyMotorcyclesOfUser()
    {
        // ARRANGE — preparo il database con dati di test
        using var context = CreateContext("test-get-1");

        var user1 = CreateUser("user-1");
        var user2 = CreateUser("user-2");
        context.Users.AddRange(user1, user2);
        context.Motorcycles.AddRange(
            new Motorcycle { Brand = "Yamaha", Model = "R1", Year = 2020, Vin = "VIN001", UserId = "user-1" },
            new Motorcycle { Brand = "Honda", Model = "CBR", Year = 2021, Vin = "VIN002", UserId = "user-1" },
            new Motorcycle { Brand = "Ducati", Model = "V4", Year = 2022, Vin = "VIN003", UserId = "user-2" }
        );
        await context.SaveChangesAsync();

        var service = new MotorcycleService(context);

        // ACT — chiedo le moto di user-1
        var result = await service.GetByUserAsync("user-1");

        // ASSERT — mi aspetto solo 2 moto (non quella di user-2)
        Assert.Equal(2, result.Count());
        Assert.All(result, m => Assert.NotEqual("Ducati", m.Brand));
    }

    // ----------------------------------------------------------------
    // TEST 2: GetByUserAsync ritorna lista vuota se l'utente non ha moto
    // ----------------------------------------------------------------
    [Fact]
    public async Task GetByUserAsync_ReturnsEmpty_WhenUserHasNoMotorcycles()
    {
        // ARRANGE
        using var context = CreateContext("test-get-2");
        var service = new MotorcycleService(context);

        // ACT
        var result = await service.GetByUserAsync("utente-inesistente");

        // ASSERT
        Assert.Empty(result);
    }

    // ----------------------------------------------------------------
    // TEST 3: CreateAsync salva correttamente e ritorna il DTO
    // ----------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_SavesMotorcycleAndReturnsDto()
    {
        // ARRANGE
        using var context = CreateContext("test-create-1");
        var service = new MotorcycleService(context);

        var dto = new CreateMotorcycleDto
        {
            Brand = "Kawasaki",
            Model = "ZX10R",
            Year = 2023,
            Vin = "VIN999"
        };

        // ACT
        var result = await service.CreateAsync("user-1", dto);

        // ASSERT — il DTO ritornato ha i dati corretti
        Assert.Equal("Kawasaki", result.Brand);
        Assert.Equal("ZX10R", result.Model);
        Assert.Equal(2023, result.Year);
        Assert.True(result.Id > 0); // l'ID è stato generato dal DB

        // Verifico anche che sia stato salvato davvero nel DB
        var saved = await context.Motorcycles.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("user-1", saved.UserId);
    }

    // ----------------------------------------------------------------
    // TEST 4: UpdateAsync modifica i dati correttamente
    // ----------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_UpdatesMotorcycle_WhenOwnerMatches()
    {
        // ARRANGE
        using var context = CreateContext("test-update-1");
        context.Motorcycles.Add(
            new Motorcycle { Id = 1, Brand = "Honda", Model = "CBR", Year = 2019, Vin = "VIN100", UserId = "user-1" }
        );
        await context.SaveChangesAsync();

        var service = new MotorcycleService(context);
        var updateDto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = "CBR1000RR", // modello aggiornato
            Year = 2023,
            Vin = "VIN100"
        };

        // ACT
        var result = await service.UpdateAsync("user-1", 1, updateDto);

        // ASSERT
        Assert.True(result);
        var updated = await context.Motorcycles.FindAsync(1);
        Assert.Equal("CBR1000RR", updated!.Model);
        Assert.Equal(2023, updated.Year);
    }

    // ----------------------------------------------------------------
    // TEST 5: UpdateAsync ritorna false se la moto appartiene ad altro utente
    // ----------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenUserDoesNotOwnMotorcycle()
    {
        // ARRANGE
        using var context = CreateContext("test-update-2");
        context.Motorcycles.Add(
            new Motorcycle { Id = 1, Brand = "BMW", Model = "S1000RR", Year = 2021, Vin = "VIN200", UserId = "user-1" }
        );
        await context.SaveChangesAsync();

        var service = new MotorcycleService(context);

        // ACT — user-2 prova a modificare la moto di user-1
        var result = await service.UpdateAsync("user-2", 1, new CreateMotorcycleDto());

        // ASSERT — deve fallire silenziosamente
        Assert.False(result);
    }

    // ----------------------------------------------------------------
    // TEST 6: DeleteAsync rimuove la moto se appartiene all'utente
    // ----------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_RemovesMotorcycle_WhenOwnerMatches()
    {
        // ARRANGE
        using var context = CreateContext("test-delete-1");
        context.Motorcycles.Add(
            new Motorcycle { Id = 1, Brand = "Suzuki", Model = "GSX", Year = 2020, Vin = "VIN300", UserId = "user-1" }
        );
        await context.SaveChangesAsync();

        var service = new MotorcycleService(context);

        // ACT
        var result = await service.DeleteAsync("user-1", 1);

        // ASSERT
        Assert.True(result);
        Assert.Equal(0, await context.Motorcycles.CountAsync());
    }

    // ----------------------------------------------------------------
    // TEST 7: DeleteAsync ritorna false se la moto non esiste
    // ----------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenMotorcycleNotFound()
    {
        // ARRANGE
        using var context = CreateContext("test-delete-2");
        var service = new MotorcycleService(context);

        // ACT — provo a cancellare una moto con ID che non esiste
        var result = await service.DeleteAsync("user-1", 999);

        // ASSERT
        Assert.False(result);
    }
}