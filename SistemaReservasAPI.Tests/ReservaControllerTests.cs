using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SistemaReservasAPI.Controllers;
using SistemaReservasAPI.Models;
using Xunit;
using System.Linq;

namespace SistemaReservasAPI.Tests
{
    public class ReservaControllerTests
    {
        private ReservaController GetControllerConDatosIniciales()
        {
            // Limpia lista estática para evitar datos cruzados entre tests
            var field = typeof(ReservaController).GetField("reservas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, new List<Reserva>());

            var idField = typeof(ReservaController).GetField("nextId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            idField.SetValue(null, 1);

            return new ReservaController();
        }

        [Fact]
        public void GetReservas_DebeRetornarListaVacia_AlInicio()
        {
            // Arrange
            var controller = GetControllerConDatosIniciales();

            // Act
            var result = controller.GetReservas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Empty(lista);
        }

        [Fact]
        public void CrearReserva_DebeCrearCorrectamente()
        {
            // Arrange
            var controller = GetControllerConDatosIniciales();
            var nuevaReserva = new Reserva
            {
                Cliente = "Juan Pérez",
                SalonId = "Salon1",
                Fecha = new DateTime(2025, 8, 14),
                HoraInicio = "10:00",
                HoraFin = "11:00"
            };

            // Act
            var result = controller.CrearReserva(nuevaReserva);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var reservaCreada = Assert.IsType<Reserva>(createdResult.Value);
            Assert.Equal(1, reservaCreada.Id);
            Assert.Equal("Juan Pérez", reservaCreada.Cliente);
        }

        [Fact]
        public void CrearReserva_DebeFallarSiHorarioOcupado()
        {
            // Arrange
            var controller = GetControllerConDatosIniciales();
            var reserva1 = new Reserva
            {
                Cliente = "Cliente1",
                SalonId = "Salon1",
                Fecha = new DateTime(2025, 8, 14),
                HoraInicio = "10:00",
                HoraFin = "11:00"
            };
            controller.CrearReserva(reserva1);

            var reserva2 = new Reserva
            {
                Cliente = "Cliente2",
                SalonId = "Salon1",
                Fecha = new DateTime(2025, 8, 14),
                HoraInicio = "10:00",
                HoraFin = "11:00"
            };

            // Act
            var result = controller.CrearReserva(reserva2);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var mensaje = badRequest.Value.GetType().GetProperty("message")?.GetValue(badRequest.Value, null);
            Assert.Equal("Ya existe una reserva en ese día y horario", mensaje);
        }

        [Fact]
        public void GetReservasPorFecha_DebeFiltrarCorrectamente()
        {
            // Arrange
            var controller = GetControllerConDatosIniciales();
            var reserva1 = new Reserva
            {
                Cliente = "Cliente1",
                SalonId = "Salon1",
                Fecha = new DateTime(2025, 8, 14),
                HoraInicio = "10:00",
                HoraFin = "11:00"
            };
            controller.CrearReserva(reserva1);

            // Act
            var result = controller.GetReservasPorFecha(new DateTime(2025, 8, 14));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(lista);
        }
    }
}
