using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservasAPI.Controllers;
using SistemaReservasAPI.Data;
using SistemaReservasAPI.Models;
using Xunit;

namespace SistemaReservasAPI.Tests
{
    public class ReservaControllerTests
    {
        private ReservaDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ReservaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ReservaDbContext(options);
        }

        [Fact]
        public async Task GetReservas_ReturnsAllReservas()
        {
            // Arrange
            var context = GetDbContext();
            context.Reservas.Add(new Reserva { Id = 1, Cliente = "Juan", SalonId = "salon1", Fecha = DateTime.Today, HoraInicio = "10:00", HoraFin = "11:00" });
            context.Reservas.Add(new Reserva { Id = 2, Cliente = "Ana", SalonId = "salon2", Fecha = DateTime.Today, HoraInicio = "12:00", HoraFin = "13:00" });
            await context.SaveChangesAsync();

            var controller = new ReservaController(context);

            // Act
            var result = await controller.GetReservas();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reservas = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<object>>(okResult.Value);

            // Assert
            Assert.Equal(2, reservas.Count());
        }

        [Fact]
        public async Task GetReservasPorFecha_FiltersByFecha()
        {
            // Arrange
            var context = GetDbContext();
            var today = DateTime.Today;
            context.Reservas.Add(new Reserva { Id = 1, Cliente = "Juan", SalonId = "salon1", Fecha = today, HoraInicio = "10:00", HoraFin = "11:00" });
            context.Reservas.Add(new Reserva { Id = 2, Cliente = "Ana", SalonId = "salon2", Fecha = today.AddDays(1), HoraInicio = "12:00", HoraFin = "13:00" });
            await context.SaveChangesAsync();

            var controller = new ReservaController(context);

            // Act
            var result = await controller.GetReservasPorFecha(today);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reservas = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<object>>(okResult.Value);

            // Assert
            Assert.Single(reservas);
        }

        [Fact]
        public async Task CrearReserva_CreatesNewReserva()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new ReservaController(context);
            var reserva = new Reserva { Cliente = "Pedro", SalonId = "salon1", Fecha = DateTime.Today, HoraInicio = "14:00", HoraFin = "15:00" };

            // Act
            var result = await controller.CrearReserva(reserva);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdReserva = Assert.IsType<Reserva>(createdResult.Value);

            // Assert
            Assert.Equal(reserva.Cliente, createdReserva.Cliente);
            Assert.Equal(reserva.SalonId, createdReserva.SalonId);
            Assert.Single(context.Reservas); // Verifica que se guardó en el contexto
        }

        [Fact]
        public async Task CrearReserva_ReturnsBadRequest_WhenConflictingHour()
        {
            // Arrange
            var context = GetDbContext();
            var existing = new Reserva { Cliente = "Juan", SalonId = "salon1", Fecha = DateTime.Today, HoraInicio = "10:00", HoraFin = "11:00" };
            context.Reservas.Add(existing);
            await context.SaveChangesAsync();

            var controller = new ReservaController(context);
            var newReserva = new Reserva { Cliente = "Ana", SalonId = "salon1", Fecha = DateTime.Today, HoraInicio = "10:00", HoraFin = "11:00" };

            // Act
            var result = await controller.CrearReserva(newReserva);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Ya existe una reserva", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CrearReserva_ReturnsBadRequest_WhenClienteAlreadyReserved()
        {
            // Arrange
            var context = GetDbContext();
            var existing = new Reserva { Cliente = "Juan", SalonId = "salon1", Fecha = DateTime.Today, HoraInicio = "10:00", HoraFin = "11:00" };
            context.Reservas.Add(existing);
            await context.SaveChangesAsync();

            var controller = new ReservaController(context);
            var newReserva = new Reserva { Cliente = "Juan", SalonId = "salon2", Fecha = DateTime.Today, HoraInicio = "12:00", HoraFin = "13:00" };

            // Act
            var result = await controller.CrearReserva(newReserva);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("El cliente ya tiene una reserva", badRequest.Value.ToString());
        }
    }
}
