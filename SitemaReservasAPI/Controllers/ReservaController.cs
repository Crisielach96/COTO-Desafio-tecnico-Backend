using Microsoft.AspNetCore.Mvc;
using SistemaReservasAPI.Models;

namespace SistemaReservasAPI.Controllers;

[ApiController]
[Route("api/")]
public class ReservaController : ControllerBase
{
    private static List<Reserva> reservas = new List<Reserva>();
    private static int nextId = 1;

    // Obtengo todas las reservas
    [HttpGet]
    [Route("reserva/all")]
    public ActionResult<IEnumerable<Reserva>> GetReservas()
    {
        var reservasFormateadas = reservas.Select(r => new
        {
            r.Id,
            r.Cliente,
            r.SalonId,
            Fecha = r.Fecha.ToString("dd-MM-yyyy"),
            r.HoraInicio,
            r.HoraFin
        });

        return Ok(reservasFormateadas);
    }

    // Obtengo todas las reservas
    [HttpGet]
    [Route("reserva")]
    public ActionResult<IEnumerable<Reserva>> GetReservasPorFecha([FromQuery] DateTime? fecha)
    {

        var query = reservas.AsQueryable();

        if (fecha.HasValue)
        {
            query = query.Where(r => r.Fecha.Date == fecha.Value.Date);
        }

        var reservasFormateadas = query.Select(r => new
        {
            r.Id,
            r.Cliente,
            r.SalonId,
            Fecha = r.Fecha.ToString("dd-MM-yyyy"),
            r.HoraInicio,
            r.HoraFin
        });

        return Ok(reservasFormateadas);
    }

    // Creo una reserva
    [HttpPost]
    [Route("reserva")]
    public ActionResult<Reserva> CrearReserva([FromBody] Reserva reserva)
    {
        if (reservas.Any(r => r.Fecha == reserva.Fecha && r.HoraInicio == reserva.HoraInicio && r.SalonId == reserva.SalonId))
        {
            return BadRequest(new { message = "Ya existe una reserva en ese día y horario" });
        }

        if (reservas.Any(r => r.Fecha == reserva.Fecha && r.Cliente == reserva.Cliente))
        {
            return BadRequest(new { message = "El cliente ya tiene una reserva ese día" });
        }

        reserva.Id = nextId++;
        reserva.Fecha = DateTime.ParseExact(reserva.Fecha.ToString("dd-MM-yyyy"), "dd-MM-yyyy", null);
        reservas.Add(reserva);
        return CreatedAtAction(nameof(GetReservas), new { id = reserva.Id }, reserva);
    }
}
