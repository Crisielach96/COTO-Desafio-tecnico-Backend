using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaReservasAPI.Data;
using SistemaReservasAPI.Models;

namespace SistemaReservasAPI.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ReservaController : ControllerBase
    {
        private readonly ReservaDbContext _context;

        public ReservaController(ReservaDbContext context)
        {
            _context = context;
        }

        // GET: api/reserva/all
        [HttpGet("reserva/all")]
        public async Task<ActionResult<IEnumerable<object>>> GetReservas()
        {
            var reservasFormateadas = await _context.Reservas
                .Select(r => new
                {
                    r.Id,
                    r.Cliente,
                    r.SalonId,
                    Fecha = r.Fecha.ToString("dd-MM-yyyy"),
                    r.HoraInicio,
                    r.HoraFin
                })
                .ToListAsync();

            return Ok(reservasFormateadas);
        }

        // GET: api/reserva?fecha=2025-08-14
        [HttpGet("reserva")]
        public async Task<ActionResult<IEnumerable<object>>> GetReservasPorFecha([FromQuery] DateTime? fecha)
        {
            var query = _context.Reservas.AsQueryable();

            if (fecha.HasValue)
            {
                query = query.Where(r => r.Fecha.Date == fecha.Value.Date);
            }

            var reservasFormateadas = await query
                .Select(r => new
                {
                    r.Id,
                    r.Cliente,
                    r.SalonId,
                    Fecha = r.Fecha.ToString("dd-MM-yyyy"),
                    r.HoraInicio,
                    r.HoraFin
                })
                .ToListAsync();

            return Ok(reservasFormateadas);
        }

        // POST: api/reserva
        [HttpPost("reserva")]
        public async Task<ActionResult<Reserva>> CrearReserva([FromBody] Reserva reserva)
        {
            // Validar conflicto de horario exacto
            if (await _context.Reservas.AnyAsync(r =>
                r.Fecha.Date == reserva.Fecha.Date &&
                r.SalonId == reserva.SalonId &&
                r.HoraInicio == reserva.HoraInicio))
            {
                return BadRequest(new { message = "Ya existe una reserva en ese día y horario" });
            }

            // Validar cliente ya reservado en la misma fecha
            if (await _context.Reservas.AnyAsync(r =>
                r.Fecha.Date == reserva.Fecha.Date &&
                r.Cliente == reserva.Cliente))
            {
                return BadRequest(new { message = "El cliente ya tiene una reserva ese día" });
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservas), new { id = reserva.Id }, reserva);
        }
    }
}
